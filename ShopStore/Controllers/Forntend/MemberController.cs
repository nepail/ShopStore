﻿#region 功能與歷史修改描述

/*
    描述:前台會員相關
    建立日期:2021-11-29

    描述:程式碼風格調整
    修改日期:2022-01-10

 */

#endregion

using Microsoft.AspNetCore.Mvc;
using ShopStore.Models.Interface;
using ShopStore.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using ShopStore.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System;
using NLog;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace ShopStore.Controllers
{
    [AllowAnonymous]
    public class MemberController : Controller
    {
        private readonly IMembers MEMBERS;
        private readonly IDistributedCache REDIS;
        private static Logger LOGGER = LogManager.GetCurrentClassLogger();
        private readonly IHttpContextAccessor HTTPCONTEXTACCESSOR;        

        public MemberController(IMembers members, IDistributedCache redis, IHttpContextAccessor httpContextAccessor)
        {
            MEMBERS = members;
            REDIS = redis;
            HTTPCONTEXTACCESSOR = httpContextAccessor;
        }


        #region 註冊

        /// <summary>
        /// 註冊頁面
        /// </summary>
        /// <returns></returns>        
        public IActionResult SignUp()
        {
            //test 測試資料
            // MemberViewModel memberviewmodel = new MemberViewModel()
            // {
            //     f_name = "admin",
            //     f_nickname = "菜頭",
            //     f_phone = "0908609268",
            //     f_mail = "linjim1101@gmail.com",
            //     f_account = "admin01",
            //     f_pcode = "admin01",
            //     f_address = "台中市西屯區市政路388號",
            // };

            // return View("/Views/Frontend/Member/SignUp.cshtml", memberviewmodel);
            return View("/Views/Frontend/Member/SignUp.cshtml");
        }

        /// <summary>
        /// 發送信箱驗證碼
        /// </summary>
        /// <param name="memberName"></param>
        /// <param name="mailAddress"></param>
        /// <returns></returns>        
        private bool SendEmail(string memberName, string mailAddress)
        {
            //在Redis確認是否有重複的memberName
            if (REDIS.GetString(memberName) == null)
            {
                //寄認證信
                string verifyCode = MailHelper.SendMail(memberName, mailAddress);
                //將認證碼寫入Redis
                var options = new DistributedCacheEntryOptions();
                options.SetAbsoluteExpiration(TimeSpan.FromMinutes(5)); //時間到即消失
                //options.SetSlidingExpiration(TimeSpan.FromMinutes(5)); //重新讀取後會重新計時
                REDIS.SetString(memberName, verifyCode, options);

                return true;
            }

            return false;
        }

        /// <summary>
        /// 驗證信箱認證碼 => 成功後資料庫新增資料
        /// </summary>
        /// <param name="memberName"></param>
        /// <param name="code"></param>
        /// <returns>回傳JSON</returns>
        [HttpPost]
        public IActionResult CheckEmailCode(string code, MemberViewModel model)
        {
            string tempCode = REDIS.GetString(model.f_name);

            if (code == tempCode)
            {
                try
                {
                    if (MEMBERS.AddNewMember(model))
                        return Json(new { success = true, message = "認證成功" });
                }
                catch (Exception ex)
                {
                    LOGGER.Debug(ex, $"CheckEmailCode Error");
                    return Json(new { success = false, message = "系統錯誤" });
                }
            }

            if (tempCode == null)
            {
                return Json(new { success = false, code = 2, message = "認證碼失效" });
            }

            return Json(new { success = false, code = 0, message = "驗證碼錯誤" });
        }

        /// <summary>
        /// 新增會員資料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddNewMember(MemberViewModel model)
        {
            //紀錄IP位址
            //string ipaddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            //var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;

            try
            {
                if (ModelState.IsValid)
                {
                    //寄送認證信件
                    if (!SendEmail(model.f_name, model.f_mail))
                    {
                        return Json(new { success = false, message = "短時間無法重複註冊" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "輸入的格式中含有違法字元" });
                }
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, $"{model.f_name} AddNewMember Error");
                return Json(new { success = false, message = $"註冊失敗，信件系統發生錯誤：{ex.ToString()}" });
            }

            return Json(new { success = true, message = "系統已寄發認證信件至您的信箱" });
        }
        #endregion

        #region 登入登出

        /// <summary>
        /// 登入頁面
        /// </summary>
        /// <returns>回傳登入頁面</returns>        
        public IActionResult Login()
        {
            ViewBag.account = "admin01";
            ViewBag.pwd = "admin01";

            return View("/Views/Frontend/Member/Login.cshtml");
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="request"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>        
        [HttpPost]
        public async Task<IActionResult> Login(string account, string pwd, string returnUrl)
        {
            MemberViewModel member = MEMBERS.FindUser(account, pwd);

            if (member == null)
            {
                ViewBag.errMsg = "無此會員";
                return View("/Views/Frontend/Home/LoginError.cshtml");
            }

            var claims = new List<Claim>
            {
                new Claim("Account", member.f_account),
                new Claim(ClaimTypes.Name, member.f_nickname), //暱稱                
                new Claim(ClaimTypes.NameIdentifier, member.f_id), //userId                
                //new Claim(ClaimTypes.Role, member.f_groupid),
                new Claim(ClaimTypes.Role, "Normal"),
            };



            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false
            };


            //呼叫登入管理員登入
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);



            //防止重複登入
            var userGuid = Guid.NewGuid().ToString();
            Response.Cookies.Append(account, userGuid);
            //設定Redis
            var options = new DistributedCacheEntryOptions();
            options.SetSlidingExpiration(TimeSpan.FromMinutes(30)); //重新讀取後會重新計時
            REDIS.SetString(account, userGuid, options);

            //Session 自動保存到Redis
            //HttpContext.Session.SetString("UserId", "Tester");


            //導回原址OR導回頁首
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns>重導回首頁</returns>
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// 忘記密碼
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ForgetPcode(string mail)
        {
            //確認email
            if (await MEMBERS.VerifyEmailAsync(mail))
            {
                string code = MailHelper.SendResetPcode("", mail);

                //將認證碼寫入Redis
                var options = new DistributedCacheEntryOptions();
                options.SetAbsoluteExpiration(TimeSpan.FromMinutes(1)); //時間到即消失                                                                        
                REDIS.SetString(mail, code, options);

                //送出信件
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, msg = "查無此信箱" });
            }
        }

        /// <summary>
        /// 驗證重置認證碼
        /// </summary>
        /// <param name="code"></param>
        /// <param name="mail"></param>
        /// <returns></returns>
        public IActionResult CheckCode(string code, string mail)
        {
            if (REDIS.GetString(mail) != null)
            {
                if (REDIS.GetString(mail) == code)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, msg = "認證碼錯誤" });
                }
            }

            return Json(new { success = false, msg = "認證碼失效" });
        }

        /// <summary>
        /// 重置密碼
        /// </summary>
        /// <param name="code"></param>
        /// <param name="mail"></param>
        /// <returns></returns>
        public IActionResult ResetPcode(string code, string mail)
        {
            return MEMBERS.ResetMemberPcode(code, mail) == true ? Json(new { success = true }) : Json(new { success = false });
        }

        #endregion
    }
}
