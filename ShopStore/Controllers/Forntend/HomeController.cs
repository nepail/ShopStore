using DAL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShopStore.Models;
using ShopStore.Models.Interface;
using System.Diagnostics;
using System.Security.Claims;

namespace ShopStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMembers _members;

        public HomeController(ILogger<HomeController> logger, IMembers members)
        {
            _logger = logger;
            _members = members;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View("/Views/Frontend/Home/Index.cshtml");
        }

        /// <summary>
        /// 帳號重複登入
        /// </summary>
        /// <returns></returns>
        //[AllowAnonymous]
        [Authorize(Roles = "Normal")]
        public IActionResult Error()
        {
            return View("/Views/Frontend/Home/Error.cshtml");
        }

        [Authorize(Roles = "Normal")]
        public IActionResult UserProfile()
        {
            //UserProfileViewModel userProfileViewModel = new UserProfileViewModel()
            //{
            //    NickName = "菜頭",
            //    GroupName = "LV 1",
            //    Cash = 100,
            //    RealName = "dfff",
            //    Email = "SSSSSS@gmail.com",
            //    Address = "台中市"
            //};
            
            var userId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            UserProfileViewModel userProfileViewModel = _members.GetMemberProfile(userId);

            return View("/Views/Frontend/Home/UserProfile.cshtml", userProfileViewModel);
        }


        /// <summary>
        /// 授權拒絕
        /// </summary>
        /// <returns></returns>        
        //[AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View("/Views/Frontend/Home/AccessDenied.cshtml");
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult WishList()
        {
            return View("/Views/Frontend/Home/WishList.cshtml");
        }
    }
}
