﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopStore.Models
{
    public class OrderModel
    {
        public OrderModel()
        {
            Orderlist = new List<OrderItem>();
            f_isdel = 0;
            f_ispaid = 0;
        }

        public string f_num { get; set; }
        public string f_memberid { get; set; }
        public DateTime f_date { get; set; }
        public int f_status { get; set; }
        public int f_shippingMethod{ get; set; }
        public int f_total { get; set; }
        public int f_ispaid { get; set; }
        public int f_isdel { get; set; }
        public List<OrderItem> Orderlist { get; set; }
        public OrderItem Orderlists { get; set; }
    }

    public class OrderItem
    {
        //public string f_id { get; set; }
        public string f_ordernum { get; set; }
        public string f_productid { get; set; }
        public int f_amount { get; set; }
        public string f_name { get; set; }
        public int f_price { get; set; }
        public int f_isdel { get; set; }

        public static implicit operator List<object>(OrderItem v)
        {
            throw new NotImplementedException();
        }
    }
}