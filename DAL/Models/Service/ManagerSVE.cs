using DAL.Models;
using DAL.Models.Manager;
using Dapper;
using NLog;
using ShopStore.Models.Interface;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace ShopStore.Models.Service
{
    public class ManagerSVE : IManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly SqlConnection _connection;

        public ManagerSVE(SqlConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// 取得菜單
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public async Task<IEnumerable<MenuModel>> GetMenu(int userid)
        {
            try
            {
                using var conn = _connection;
                //var result = await conn.QueryAsync<MenuModel>(@"pro_shopStore_getMenu");

                SqlMapper.GridReader result = await conn.QueryMultipleAsync(@"pro_shopStore_getMenu", new { userid = userid }, commandType: System.Data.CommandType.StoredProcedure);
                List<MenuModel> menuModels = result.Read<MenuModel>().ToList();
                List<MenuSubModel> menuSubModels = result.Read<MenuSubModel>().ToList();
                List<MenuModel> menu = (from a in menuModels
                                        select new MenuModel
                                        {
                                            f_id = a.f_id,
                                            f_icon = a.f_icon,
                                            f_name = a.f_name,
                                            f_level = a.f_level,
                                            f_isdel = a.f_isdel,
                                            f_issys = a.f_issys,
                                            MenuSubModels = menuSubModels.Where(x => x.f_menuid == a.f_id).ToList()

                                        }).ToList();


                return menu;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// 新增菜單
        /// </summary>
        /// <param name="menuModel"></param>
        /// <returns></returns>
        public bool AddMenu(MenuModel menuModel)
        {
            try
            {
                using var conn = _connection;
                var result = conn.Execute("pro_shopStore_Manager_addMenu", menuModel, commandType: System.Data.CommandType.StoredProcedure);
                return true;
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Debug");
                return false;
            }
        }


        /// <summary>
        /// 新增子菜單
        /// </summary>
        /// <param name="menuModel"></param>
        /// <returns></returns>
        public async Task<bool> AddSubMenu(MenuViewModel model)
        {
            //using TransactionScope scope = new TransactionScope();
            try
            {
                using var conn = _connection;
                if (model.MainMenuItems != null && model.MainMenuItems.Count > 0)
                {
                    var result = await conn.ExecuteAsync("pro_shopStore_Manager_addMainMenu", model.MainMenuItems, commandType: System.Data.CommandType.StoredProcedure);
                }

                if (model.SubItems != null && model.SubItems.Count > 0)
                {
                    var result = await conn.ExecuteAsync("pro_shopStore_Manager_addSubMenu", model.SubItems, commandType: System.Data.CommandType.StoredProcedure);
                }
                if (model.MenuSubModels != null && model.MenuSubModels.Count > 0)
                {
                    var result = await conn.ExecuteAsync("pro_shopStore_Manager_updateSubMenu", model.MenuSubModels, commandType: System.Data.CommandType.StoredProcedure);
                }

            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Debug");
                return false;
            }
            //scope.Complete();
            return true;
        }

        /// <summary>
        /// 取訂單資料
        /// </summary>
        /// <returns></returns>
        public List<OrderManageViewModel> GetOrderList()
        {
            try
            {
                using var conn = _connection;
                string sqlStr = @"pro_shopStore_Manager_getOrderList";

                //var result = conn.QueryMultiple(sqlStr);
                var result = conn.Query(sqlStr);
                List<OrderManageViewModel> model = (List<OrderManageViewModel>)conn.Query<OrderManageViewModel>(sqlStr);
                //List<OrderManageModel> orderManageModels = (List<OrderManageModel>)result.Read<OrderManageModel>();

                return model;
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Debug");
                return null;
            }
        }

        /// <summary>
        /// 刪除Order單
        /// isdel => 1, status => 4(已退貨)
        /// </summary>
        /// <returns></returns>
        public bool RemoveOrder(string ordernum)
        {
            try
            {
                using var conn = _connection;
                string sqlStr = @"pro_shopStore_Manager_removeOrderList";
                bool result = conn.Query<bool>(sqlStr, new { f_id = ordernum }, commandType:System.Data.CommandType.StoredProcedure).FirstOrDefault();
                return true;
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Debug");
                return false;
            }
        }
    }
}
