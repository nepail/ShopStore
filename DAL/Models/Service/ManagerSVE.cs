using DAL.Models;
using DAL.Models.Manager;
using DAL.Models.Manager.ViewModels;
using Dapper;
using NLog;
using ShopStore.Models.Interface;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using static DAL.Models.Manager.PermissionDataModel;

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

                var result = conn.Query(sqlStr);
                List<OrderManageViewModel> model = (List<OrderManageViewModel>)conn.Query<OrderManageViewModel>(sqlStr);

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
                bool result = conn.Query<bool>(sqlStr, new { f_id = ordernum }, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                return true;
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Debug");
                return false;
            }
        }

        /// <summary>
        /// 更新訂單
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        public bool UpdateOrder(List<Order> orders)
        {
            try
            {
                using var conn = _connection;
                string sqlStr = @"pro_shopStore_Manager_UpdateOrder";
                var result = conn.Execute(sqlStr, orders, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Debug");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 新增後台帳號
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddUser(UserManageModel model)
        {
            try
            {
                using var conn = _connection;
                return conn.Execute(@"pro_shopStore_Manager_AddUser", model, commandType: System.Data.CommandType.StoredProcedure) == 1;
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Debug");
                return false;
            }
        }

        /// <summary>
        /// 取得後台使用者
        /// </summary>
        /// <returns></returns>
        public List<UserManageViewModels> GetUsers()
        {
            try
            {
                using var conn = _connection;
                //List<UserManageViewModel> result = new List<UserManageViewModel>();
                //result = (List<UserManageViewModel>)conn.Query<UserManageViewModel>(@"pro_shopStore_Manager_getUsers", commandType: System.Data.CommandType.StoredProcedure);

                var result = conn.QueryMultiple(@"pro_shopStore_Manager_getUsers", commandType: System.Data.CommandType.StoredProcedure);

                //這段要修改
                List<UserManageViewModel> userManageViewModels = result.Read<UserManageViewModel>().ToList();
                //List<UserPermission> permissionList = result.Read<UserPermission>().ToList();

                List<UserManageViewModels> model = userManageViewModels.Select(x => new UserManageViewModels
                {
                    ID = x.ID,
                    Account = x.Account,
                    Name = x.Name,
                    GroupId = x.GroupId,
                    GroupName = x.GroupName,
                    CreateTime = x.f_createTime.ToString("yyyy/MM/dd HH:mm:ss"),
                    UpdateTime = x.f_updateTime.ToString("yyyy/MM/dd HH:mm:ss"),
                    //UserPermissions = permissionList.Where(s => s.f_groupId == x.GroupId).ToList()
                }).ToList();

                return model;
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Debug");
                return null;
            }
        }

        /// <summary>
        /// 取得用戶權限
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<UserPermission> GetUserPermissionsByID(int userId)
        {
            try
            {
                using var conn = _connection;
                var userPermission = conn.Query<UserPermission, UserPermissionDetail, UserPermission>
                    ("pro_shopStore_Manager_getUsersPermissions",
                    (o, c) =>
                    {
                        o.PermissionDetail = c;
                        return o;
                    },
                    new { userId }, splitOn: "MenuName", commandType: System.Data.CommandType.StoredProcedure).ToList();

                return userPermission;
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Debug");
                return null;
            }
        }

        /// <summary>
        /// 更新使用者權限
        /// </summary>
        /// <param name="permissionData"></param>
        /// <returns></returns>
        public bool UpdatePermissionsByID(PermissionData permissionData)
        {
            try
            {
                var updateTime = DateTime.Now;

                List<PermissionModel> permissionModels = permissionData.PermissionDetails.Select(x => new PermissionModel
                {
                    f_userId = permissionData.UserId,
                    f_menuSubId = x.MenuId,
                    f_permissionCode = x.PermissionsCode,
                    f_updateTime = DateTime.Now,
                    f_groupId = permissionData.GroupId,
                    UpdateType = 1
                }).ToList();


                if (permissionModels.Count == 0)
                {
                    permissionModels.Add(new PermissionModel()
                    {
                        f_userId = permissionData.UserId,
                        f_menuSubId = 0,
                        f_permissionCode = 0,
                        f_updateTime = DateTime.Now,
                        f_groupId = permissionData.GroupId,
                        UpdateType = 0
                    });
                }


                using var conn = _connection;
                var result = conn.Execute("pro_shopStore_Manager_UserPermissionsUpdate", permissionModels, commandType: System.Data.CommandType.StoredProcedure);

                return result > 0;
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Debug");
                return false;
            }
        }

        /// <summary>
        /// 刪除使用者
        /// </summary>
        /// <returns></returns>
        public bool RemoveUserByID(string userId)
        {
            try
            {
                using var conn = _connection;
                return conn.Execute("pro_shopStore_Manager_UserRemove",
                                    new { userId },
                                    commandType: System.Data.CommandType.StoredProcedure) == 1;
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "RemoveUserByID");
                return false;
            }
        }

        /// <summary>
        /// 取得會員列表
        /// </summary>
        /// <returns></returns>
        public List<MemberManagerViewModel> GetMemberList()
        {
            try
            {
                using var conn = _connection;
                return (List<MemberManagerViewModel>)conn.Query<MemberManagerViewModel>("pro_shopStore_Manager_Member_GetList",
                                    commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "GetMemberList");
                return null;
            }
        }

        /// <summary>
        /// 會員停權
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public bool SuspendByMemberId(int memberId, int isSuspend)
        {
            try
            {
                using var conn = _connection;
                return conn.Execute("pro_shopStore_Manager_Member_SuspendByID",
                    new { memberId, isSuspend },
                    commandType: System.Data.CommandType.StoredProcedure) == 1;
            }
            catch (Exception ex)
            {
                logger.Debug(ex, $"SuspendByID={memberId}");
                return false;
            }
        }

        /// <summary>
        /// 會員停權
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public bool UpdateByMemberId(MemberManageModel memberManageModel)
        {
            try
            {
                using var conn = _connection;
                return conn.Execute("pro_shopStore_Manager_Member_UpdateByID",
                    memberManageModel.MemberModel,
                    commandType: System.Data.CommandType.StoredProcedure) > 0;
            }
            catch (Exception ex)
            {
                logger.Debug(ex, $"SuspendByID={memberManageModel}");
                return false;
            }
        }
    }
}
