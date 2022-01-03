using DAL.Models;
using DAL.Models.Manager;
using DAL.Models.Manager.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static DAL.Models.Manager.PermissionDataModel;

namespace ShopStore.Models.Interface
{
    public interface IManager
    {
        /// <summary>
        /// 取得菜單
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<MenuModel>> GetMenu(int userid);

        /// <summary>
        /// 新增菜單
        /// </summary>
        /// <param name="menuModel"></param>
        /// <returns></returns>
        public bool AddMenu(MenuModel menuModel);

        /// <summary>
        /// 新增子菜單 && 變更狀態
        /// </summary>
        /// <param name="menuModel"></param>
        /// <returns></returns>
        Task<bool> AddSubMenu(MenuViewModel model);

        /// <summary>
        /// 取訂單列表
        /// </summary>
        /// <returns></returns>
        public List<OrderManageViewModel> GetOrderList();

        /// <summary>
        /// 刪除訂單
        /// </summary>
        /// <param name="ordernum"></param>
        /// <returns></returns>
        public bool RemoveOrder(string ordernum);

        /// <summary>
        /// 更新訂單
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        public bool UpdateOrder(List<Order> orders);

        /// <summary>
        /// 新增後台帳號
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddUser(UserManageModel model);

        /// <summary>
        /// 取得所有後台使用者
        /// </summary>
        /// <returns></returns>
        public List<UserManageViewModels> GetUsers();

        /// <summary>
        /// 取得後台使用者
        /// </summary>
        /// <returns></returns>
        public List<UserPermission> GetUserPermissionsByID(int userId);

        /// <summary>
        /// 更新使用者權限
        /// </summary>
        /// <param name="permissionData"></param>
        /// <returns></returns>
        public bool UpdatePermissionsByID(PermissionData permissionData);

        /// <summary>
        /// 刪除使用者
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool RemoveUserByID(string userId);

        /// <summary>
        /// 取得會員列表
        /// </summary>
        /// <returns></returns>
        public List<MemberManagerViewModel> GetMemberList();

        /// <summary>
        /// 會員停權
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public bool SuspendByMemberId(int memberId, int isSuspend);

        /// <summary>
        /// 更新會員
        /// </summary>
        /// <param name="memberManageModel"></param>
        /// <returns></returns>
        public bool UpdateByMemberId(MemberManageModel memberManageModel);
    }
}
