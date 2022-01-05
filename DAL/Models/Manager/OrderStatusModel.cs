using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models.Manager
{
    public class OrderStatusModel
    {

        public OrderStatusModel()
        {
            cartgoState = new Dictionary<string, StatusProp>();
            sipState = new Dictionary<string, StatusProp>();
        }
        public Dictionary<string, StatusProp> cartgoState { get; set; }
        public Dictionary<string, StatusProp> sipState { get; set; }



        public class StatusProp
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string Style { get; set; }
        }
    }
}
