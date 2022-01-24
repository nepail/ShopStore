using System;

namespace DAL.Models.Manager
{
    public class UserManageModel
    {
        public string account { get; set; }
        public string pcode { get; set; }
        public string name { get; set; }
        public int groupId { get; set; }
        public DateTime createTime { get; set; } = DateTime.Now;
        public DateTime updateTime { get; set; } = DateTime.Now;
    }
}
