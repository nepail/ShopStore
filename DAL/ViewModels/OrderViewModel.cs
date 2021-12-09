using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopStore.ViewModels
{
    public class OrderViewModel
    {

        private string _status;
        private string _shippingMethod;

        public OrderViewModel()
        {
            ListOfItem = new List<ItemDetail>();
        }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public string Num { get; set; }

        /// <summary>
        /// 訂單日期
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// 配送狀態
        /// </summary>
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value switch
                {
                    "1" => "待確認",
                    "2" => "處理中",
                    "3" => "已發貨",
                    _ => "狀態未知",
                };
            }
        }

        /// <summary>
        /// 配送方式
        /// </summary>
        public string ShippingMethod
        {
            get 
            {
                return _shippingMethod;
            }
            set 
            {
                _shippingMethod = value switch
                {
                    "1" => "郵寄",
                    "2" => "店到店",
                    "3" => "賣家直送",
                    _ => "狀態未知",
                };
            }
        }

        /// <summary>
        /// 訂單金額
        /// </summary>
        public int TotalAmountOfMoney { get; set; }
        
        /// <summary>
        /// 訂購總數量
        /// </summary>
        public int TotalAmountOfProducts { get; set; }

        ///// <summary>
        ///// 總數量
        ///// </summary>
        //public int TotalAmount { get; set; }

        /// <summary>
        /// 訂單明細
        /// </summary>
        public List<ItemDetail> ListOfItem { get; set; }

    }

    public class ItemDetail
    {
        public string Id { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 價格
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// 數量
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 小計
        /// </summary>
        public int AmountOfMoney { get; set; }
    }
}
