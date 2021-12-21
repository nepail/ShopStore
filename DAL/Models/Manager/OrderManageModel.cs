using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models.Manager
{
    public class OrderManageModel
    {
        public string f_id { get; set; }
        public int f_memberid { get; set; }
        public string f_date { get; set; }
        public int f_status { get; set; }
        public int f_shippingmethod { get; set; }
        public int f_total { get; set; }
        public int f_ispaid { get; set; }
        public int f_isdel { get; set; }
    }
}
