using ShopStore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopStore.Models.Interface
{
    public interface IOrders
    {
        /// <summary>
        /// 取得訂單資料
        /// </summary>
        /// <param name="memberid">使用者ID</param>
        /// <returns></returns>
        public List<OrderViewModel> GetOrderList(string memberid);
        /// <summary>
        /// 取消訂單
        /// </summary>
        /// <param name="ordernum">訂單編號</param>
        /// <returns></returns>
        public bool DelOrder(string ordernum);
    }
}
