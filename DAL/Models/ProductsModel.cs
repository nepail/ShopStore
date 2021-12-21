using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShopStore.Models
{
    public class ProductsModel
    {
        /// <summary>
        /// 流水號
        /// </summary>        
        public int f_id { get; set; }

        /// <summary>
        /// 產品編號
        /// </summary>
        public string f_pId { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        [Display(Name = "名稱"), Required(ErrorMessage = "請輸入商品名稱")]
        public string f_name { get; set; }

        /// <summary>
        /// 售價
        /// </summary>
        [Display(Name = "售價"), Required(ErrorMessage = "請輸入商品售價"),]
        public int f_price { get; set; }

        /// <summary>
        /// 圖片名稱
        /// </summary>
        public string f_picName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "描述"), Required(ErrorMessage = "請輸入商品描述")]
        public string f_description { get; set; }

        /// <summary>
        /// 分類
        /// </summary>
        [Display(Name = "類型"), Required(ErrorMessage = "請輸入商品分類")]
        public int f_categoryId { get; set; }

        /// <summary>
        /// 庫存數量
        /// </summary>
        [Display(Name = "庫存數量"), Required(ErrorMessage = "請輸入庫存量")]
        public int f_stock { get; set; } = 1;

        /// <summary>
        /// 是否刪除
        /// </summary>
        [Display(Name = "是否刪除")]
        public int f_isDel { get; set; } = 0;

        /// <summary>
        /// 是否開放
        /// </summary>
        [Display(Name = "是否開放"), Required(ErrorMessage = "請選擇是否開放")]
        public int f_isOpen { get; set; } = 1;

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime f_createTime { get; set; }

    }
    public class ProductDetailsModel : ProductsModel
    {
        /// <summary>
        /// 細項描述
        /// </summary>
        [Display(Name = "細項描述")]
        public string f_content { get; set; }

        public DateTime f_updateTime { get; set; }
    }
}
