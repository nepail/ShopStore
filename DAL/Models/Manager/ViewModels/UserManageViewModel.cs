﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models.Manager.ViewModels
{
    public class UserManageViewModel
    {
        public DateTime f_createTime { get; set; }
        public DateTime f_updateTime { get; set; }


        public int ID { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        //public string CreateTime { get { return f_createTime.ToString("yyyy/MM/dd HH:mm:ss"); } set { } }
        //public string UpdateTime { get { return f_updateTime.ToString("yyyy/MM/dd HH:mm:ss"); } set { } }

        public string CreateTime { get; set; }
        public string UpdateTime { get; set; }

        public List<UserPermission> UserPermissions { get; set; }
    }

    public class UserManageViewModels
    {
        public int ID { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        //public string CreateTime { get { return f_createTime.ToString("yyyy/MM/dd HH:mm:ss"); } set { } }
        //public string UpdateTime { get { return f_updateTime.ToString("yyyy/MM/dd HH:mm:ss"); } set { } }

        public string CreateTime { get; set; }
        public string UpdateTime { get; set; }

        public List<UserPermission> UserPermissions { get; set; }
    }

    public class UserPermission
    {
        public int f_groupId { get; set; }

        public string MenuName { get; set; }
        public string PermissionCode { get; set; }
    }
}