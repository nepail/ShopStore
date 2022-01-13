using System;

namespace ShopStore.Hubs.Models
{
    public class ConUserModel
    {
        public string ConnectionID { get; set; }
        public string UserName { get; set; }
        public DateTime OnlineTime { get; set; }
    }
}
