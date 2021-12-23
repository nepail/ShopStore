using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models.Manager
{
    public class UserManageModel
    {
        public string f_account { get; set; }
        public string f_pcode { get; set; }
        public string f_name { get; set; }
        public int f_groupId { get; set; }
        public DateTime f_createTime { get; set; } = DateTime.Now;
        public DateTime f_updateTime { get; set; } = DateTime.Now;
    }
}
