using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopStore.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopStore.Controllers
{
    [AllowAnonymous]
    public class VerifyController : Controller
    {
        private readonly IMembers _members;

        public VerifyController(IMembers members)
        {
            _members = members;
        }
        /// <summary>
        /// 檢查 Email 是否重複
        /// </summary>
        /// <param name="f_mail"></param>
        /// <returns></returns>
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerifyEmail(string f_mail)
        {
            if (await _members.VerifyEmailAsync(f_mail))
            {
                return Json($"{f_mail} 已經使用過，請使用其他 Email 註冊");
            }
            return Json(true);
        }

        /// <summary>
        /// 檢查 Account 是否重複
        /// </summary>
        /// <param name="f_mail"></param>
        /// <returns></returns>
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerifyAccount(string f_account)
        {
            if (await _members.VerifyAccountAsync(f_account))
            {
                return Json($"{f_account} 已經使用過，請使用其他帳號註冊");
            }
            return Json(true);
        }
    }
}
