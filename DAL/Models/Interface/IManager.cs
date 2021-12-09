using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShopStore.Models.Interface
{
    public interface IManager
    {
        /// <summary>
        /// 取得菜單
        /// </summary>
        /// <returns></returns>
        Task <IEnumerable<MenuModel>> GetMenu(int userid);

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
        Task <bool> AddSubMenu(MenuViewModel model);

    }
}
