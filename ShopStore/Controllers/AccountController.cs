//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using ShopStore.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace ShopStore.Controllers
//{
//    public class AccountController : Controller
//    {
//        private readonly UserManager<SysUser> _userManager;
//        private readonly SignInManager<SysUser> _signInManager;
//        public AccountController(UserManager<SysUser> userManager, SignInManager<SysUser> signInManager)
//        {
//            _userManager = userManager;
//            _signInManager = signInManager;
//        }
//        public IActionResult Index()
//        {
//            return View();
//        }
//        public async Task<IActionResult> Login()
//        {
//            string userid = "test";
//            string pwd = "123ABCabc.";

//            var result = await _signInManager.PasswordSignInAsync(userid, pwd, false, false);
//            //var result = await _userManager.FindByIdAsync(userid);            
//            if (result.Succeeded)
//            {
                
//            }
//            else if (result.IsLockedOut)
//            {
                
//            }
//            else
//            {
//                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
//            }
//            return View();
//        }
//        public async Task<IActionResult> Register()
//        {

//            var user = new SysUser { UserID = "test", UserName = "測試" };
//            var result = await _userManager.CreateAsync(user, "123ABCabc.");
//            if (result.Succeeded)
//            {
//                //await _signInManager.SignInAsync(user, isPersistent: false);
//            }
//            else
//            {
//                //dosomething
//            }
//            return View();
//        }
//        public async Task<IActionResult> GetUser()
//        {
//            var res = await _userManager.GetUserAsync(HttpContext.User);
//            return View();
//        }
//        public async Task<IActionResult> Logout()
//        {
//            await _signInManager.SignOutAsync();
//            return View();
//        }
//    }
//}
