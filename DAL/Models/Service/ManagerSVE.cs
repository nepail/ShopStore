using DAL.Models;
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
            } catch (Exception ex)
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
            catch(Exception ex)
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
                if(model.SubItems != null && model.SubItems.Count > 0)
                {
                    var result = await conn.ExecuteAsync("pro_shopStore_Manager_addSubMenu", model.SubItems, commandType: System.Data.CommandType.StoredProcedure);
                    //string sqlStr = "";
                    //var result = await conn.ExecuteAsync(sqlStr, model.SubItems);
                }
                if(model.MenuSubModels != null && model.MenuSubModels.Count > 0)
                {
                    var result = await conn.ExecuteAsync("pro_shopStore_Manager_updateSubMenu", model.MenuSubModels, commandType: System.Data.CommandType.StoredProcedure);
                    //string sqlStr = @"	UPDATE t_manager_menusub 
                    //                 SET	f_isopen = @f_isopen
                    //                 WHERE f_id = @f_id";
                    //var result = await conn.ExecuteAsync(sqlStr, model.MenuSubModels);
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
    }
}
