﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShopStore.Models;
using System.Diagnostics;

namespace ShopStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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

        /// <summary>
        /// 授權拒絕
        /// </summary>
        /// <returns></returns>        
        //[AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View("/Views/Frontend/Home/AccessDenied.cshtml");
        }

        //[AllowAnonymous]
        [Authorize(Policy = "RequireAdministratorRole")]
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
