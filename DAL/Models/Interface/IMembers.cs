using DAL.ViewModels;
using ShopStore.ViewModels;
using System.Threading.Tasks;

namespace ShopStore.Models.Interface
{
    public interface IMembers
    {
        public bool AddNewMember(MemberViewModel model);
        Task<bool> VerifyEmailAsync(string f_mail);

        Task<bool> VerifyAccountAsync(string f_account);
        public MemberViewModel FindUser(string f_account, string f_pcode);

        /// <summary>
        /// 取得該Member的資料
        /// </summary>
        /// <param name="f_account"></param>
        /// <param name="f_pcode"></param>
        /// <returns></returns>
        public UserProfileViewModel GetMemberProfile(int memberId);
        /// <summary>
        /// 重置會員密碼
        /// </summary>
        /// <param name="code"></param>
        /// <param name="mail"></param>
        /// <returns></returns>
        public bool ResetMemberPcode(string code, string mail);
    }
}
