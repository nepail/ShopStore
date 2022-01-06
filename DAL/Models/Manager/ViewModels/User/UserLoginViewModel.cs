using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models.Manager.ViewModels.User
{
    public class UserLoginViewModel
    {
        public UserLoginViewModel()
        {

        }
        public string Account { get; set; }
        public string Pcode { get; set; }
    }
}
