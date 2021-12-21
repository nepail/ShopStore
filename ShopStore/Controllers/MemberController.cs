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
        private readonly IMembers _members;
        private readonly IDistributedCache _cache;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MemberController(IMembers members, IDistributedCache cache, IHttpContextAccessor httpContextAccessor)
        {
            _members = members;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
        }


        #region 註冊

        /// <summary>
        /// 註冊頁面
        /// </summary>
        /// <returns></returns>        
        public IActionResult SignUp()
        {
            //test 測試資料
            MemberViewModel memberviewmodel = new MemberViewModel()
            {
                f_name = "admin",
                f_nickname = "菜頭",
                f_phone = "0908609268",
                f_mail = "linjim1101@gmail.com",
                f_account = "admin01",
                f_pcode = "admin01",
                f_address = "台中市西屯區市政路388號",
            };

            return View(memberviewmodel);
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
            if (_cache.GetString(memberName) == null)
            {
                //寄認證信
                string verifyCode = MailHelper.SendMail(memberName, mailAddress);
                //將認證碼寫入Redis
                var options = new DistributedCacheEntryOptions();
                options.SetAbsoluteExpiration(TimeSpan.FromMinutes(5)); //時間到即消失
                //options.SetSlidingExpiration(TimeSpan.FromMinutes(5)); //重新讀取後會重新計時
                _cache.SetString(memberName, verifyCode, options);

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
            string tempCode = _cache.GetString(model.f_name);

            if (code == tempCode)
            {
                try
                {
                    if (_members.AddNewMember(model))
                        return Json(new { success = true, message = "認證成功" });
                }
                catch (Exception ex)
                {
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
                logger.Debug(ex, $"{model.f_name} AddNewMember Error");
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

            return View();
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="request"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>        
        [HttpPost]
        public async Task<IActionResult> Login(string account, string pcode, string returnUrl)
        {
            var member = _members.FindUser(account, pcode);

            if (member == null)
            {
                ViewBag.errMsg = "無此會員";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim("Account", member.f_account),
                new Claim(ClaimTypes.Name, member.f_nickname), //暱稱                
                new Claim(ClaimTypes.NameIdentifier, member.f_id), //userId                
                new Claim(ClaimTypes.Role, member.f_groupid),
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
            _cache.SetString(account, userGuid, options);

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

        #endregion
    }
}
