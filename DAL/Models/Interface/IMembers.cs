using ShopStore.Models.Service;
using ShopStore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopStore.Models.Interface
{
    public interface IMembers
    {
        public bool AddNewMember(MemberViewModel model);
        Task<bool> VerifyEmailAsync(string f_mail);

        Task<bool> VerifyAccountAsync(string f_account);
        public MemberViewModel FindUser(string f_account, string f_pwd);
    }
}
