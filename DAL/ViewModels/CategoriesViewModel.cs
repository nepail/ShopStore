using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopStore.Models
{
    /// <summary>
    /// 類別 ViewModel
    /// </summary>
    public class CategoriesViewModel
    {
        /// <summary>
        /// 類別流水號
        /// </summary>
        public int f_id { get; set; }
        /// <summary>
        /// 類別代號
        /// </summary>
        public string f_code { get; set; }
        /// <summary>
        /// 類別名稱
        /// </summary>
        public string f_name { get; set; }
    }
}
