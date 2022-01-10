#region 功能與歷史修改描述

/*
    描述:後台相關功能
    建立日期:2021-12-03

    描述:程式碼風格調整
    修改日期:2022-01-10

 */

#endregion

using DAL.Models;
using DAL.Models.Manager;
using DAL.Models.Manager.ViewModels;
using DAL.Models.Manager.ViewModels.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Distributed;
using NLog;
using ShopStore.Models;
using ShopStore.Models.Interface;
using ShopStore.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static DAL.Models.Manager.PermissionDataModel;

namespace ShopStore.Controllers
{
    [Authorize(AuthenticationSchemes = "manager")]
    public class ManagerController : Controller
    {
        private readonly IProducts PRODUCTS;
        private readonly IManager MANAGER;
        private readonly IDistributedCache REDIS;
        private readonly IWebHostEnvironment WEBHOSTENVIRONMENT;        
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
        public ManagerController
        (
            IProducts products,
            IManager manager,
            IWebHostEnvironment webHostEnvironment,
            IDistributedCache redis
        )
        {
            PRODUCTS = products;
            MANAGER = manager;
            WEBHOSTENVIRONMENT = webHostEnvironment;
            REDIS = redis;            
        }

        /// <summary>
        /// 後台登入頁面
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 後台登入
        /// </summary>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login(UserLoginViewModel userLogin)
        {

            UserManageViewModels user = MANAGER.GetUser(userLogin);

            if (user == null)
            {
                return Json(new { success = false });
            }

            var claims = new List<Claim>
            {
                new Claim("Account", user.Account),
                new Claim(ClaimTypes.Name, user.Name), //暱稱                
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()), //userId                
                new Claim(ClaimTypes.Role, user.GroupId.ToString()),
            };

            //防止重複登入
            string userGuid = Guid.NewGuid().ToString();
            Response.Cookies.Append(user.Account, userGuid);
            var options = new DistributedCacheEntryOptions();
            options.SetSlidingExpiration(TimeSpan.FromMinutes(5)); //重新讀取後會重新計時
            REDIS.SetString(user.Account, userGuid, options);

            //呼叫登入管理員登入
            HttpContext.SignInAsync(
                "manager",
                new ClaimsPrincipal(new ClaimsIdentity(claims, "manager")),
                new AuthenticationProperties { IsPersistent = false });


            return Json(new { success = true, username = user.Name });
        }

        /// <summary>
        /// 後台登出
        /// </summary>
        /// <returns>重導回首頁</returns>
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("manager");
            return View("Index");
        }

        public async Task<IActionResult> Home()
        {

            int userId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            //回傳菜單列表
            IEnumerable<MenuModel> menuModels = await MANAGER.GetMenu(userId);           
            return View(menuModels);
        }        

        #region 產品管理

        /// <summary>
        /// Manager 新增商品
        /// </summary>
        /// <returns></returns>        
        public IActionResult AddNewProducts()
        {
            try
            {
                var productList = PRODUCTS.GetCatgoryId().ToList();
                ProductsViewModel productsViewModels = new ProductsViewModel();
                productsViewModels.SelectListItems.AddRange(from a in productList
                                                            select new SelectListItem
                                                            {
                                                                Value = a.f_id.ToString(),
                                                                Text = a.f_name,
                                                            });
                return PartialView("PartialView/Product/_AddNewProductPartial", productsViewModels);
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "Debug");
            }
            return BadRequest();
        }

        /// <summary>
        /// Manager 商品庫存管理
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult ProductList(int type)
        {
            return PartialView("PartialView/Product/_ProductManagePartial");
        }

        /// <summary>
        /// 更新產品
        /// </summary>
        /// <returns></returns>        
        public async Task<IActionResult> EditProductById(ProductsViewModel model)
        {
            try
            {
                if (model != null && ModelState.IsValid)
                {
                    if (model.ProductPic != null)
                    {
                        UploadedFile(model.ProductPic, model.f_pId);
                    }

                    if (model.f_content != null)
                    {
                        EditProductContent(model.f_content, model.f_pId);
                    }

                    bool result = await PRODUCTS.EditProductById(model);

                    if (result)
                    {
                        return Json(new { success = true, message = "success" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "fail" });
                    }
                }

                return Json(new { success = false, message = "傳入data error" });
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "Debug");
                return Json(new { success = false, message = "server error" });
            }
        }

        /// <summary>
        /// 上傳圖片
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private async void UploadedFile(IFormFile file, string id)
        {
            try
            {
                if (file != null)
                {
                    string uploadsFolder = Path.Combine(WEBHOSTENVIRONMENT.WebRootPath, "images");
                    string filePath = Path.Combine(uploadsFolder, id + ".jpg");
                    await using var fileStream = new FileStream(filePath, FileMode.Create);
                    file.CopyTo(fileStream);
                }
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "UploadFile Error");
            }
        }

        /// <summary>
        /// 編輯文字檔
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private void EditProductContent(string contentText, string id)
        {
            string uploadFolder = Path.Combine(WEBHOSTENVIRONMENT.WebRootPath, "content");
            string filePath = Path.Combine(uploadFolder, id + ".txt");
            using StreamWriter file = new StreamWriter(filePath, false);
            file.Write(contentText);
        }

        /// <summary>
        /// 取得列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetCategoryList()
        {
            List<CategoriesViewModel> productList = PRODUCTS.GetCatgoryId().ToList();
            ProductsViewModel productsViewModels = new ProductsViewModel();
            productsViewModels.SelectListItems.AddRange(from a in productList
                                                        select new SelectListItem
                                                        {
                                                            Value = a.f_id.ToString(),
                                                            Text = a.f_name,
                                                        });

            return Json(new { success = true, item = productsViewModels });
        }

        #endregion

        #region 後台使用者管理

        /// <summary>
        /// Manager 會員查詢
        /// </summary>
        /// <returns></returns>
        public IActionResult UserQuery() => PartialView("PartialView/User/_UserQueryPartial");

        /// <summary>
        /// 新增用戶
        /// </summary>
        /// <param name="postData"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddUser(UserManageModel postData)
        {
            bool result = MANAGER.AddUser(postData);
            return Json(new { success = result });
        }

        /// <summary>
        /// 取得用戶
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetUsers()
        {
            List<UserManageViewModels> userManageViewModel = MANAGER.GetUsers();
            return Json(new { success = true, item = userManageViewModel });
        }

        /// <summary>
        /// 取得群組列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetUserGroup(string md5)
        {
            string sign = "ssddsds";

            if (md5 != sign)
            {
                var groupDic = new Dictionary<string, string>
                {
                    { "1", "Admin" },
                    { "2", "Normal" }
                };

                return Json(new { success = true, item = groupDic, sign });
            }

            return Json(new { success = false });
        }

        /// <summary>
        /// 取得用戶權限
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetUserPermissionsByID(int userId)
        {
            try
            {
                var groupList = MANAGER.GetUserPermissionsByID(userId);
                return Json(new { success = true, groupList });
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex);
                return Json(new { success = false });
            }
        }

        /// <summary>
        /// 更新用戶權限
        /// </summary>
        /// <param name="permissionData"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdatePermissionsByID(PermissionData permissionData)
        {
            try
            {
                MANAGER.UpdatePermissionsByID(permissionData);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex);
                return Json(new { success = false });
            }
        }

        /// <summary>
        /// 刪除使用者
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult RemoveUserByID(string userId)
        {
            try
            {
                MANAGER.RemoveUserByID(userId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex);
                return Json(new { success = false });
            }
        }

        /// <summary>
        /// Manager 等級設定
        /// </summary>
        /// <returns></returns>
        public IActionResult MemberLevelSetting() => PartialView("PartialView/Member/_MemberLevelSettingPartial");

        /// <summary>
        /// Manager 權限設定
        /// </summary>
        /// <returns></returns>
        public IActionResult MemberPermissionSetting() => PartialView("PartialView/Member/_MemberPermissionSettingPartial");

        #endregion

        #region 菜單相關

        public async Task<IActionResult> Menu()
        {
            IEnumerable<MenuModel> model = await MANAGER.GetMenu(2);
            return PartialView("PartialView/Menu/_MenuPartial", model);
        }

        #endregion

        #region 訂單管理

        /// <summary>
        /// 訂單管理
        /// </summary>
        /// <returns></returns>
        public IActionResult OrderManage() => PartialView("PartialView/Order/_OrderManagePartial");

        /// <summary>
        /// 取所有訂單
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetOrderList()
        {
            try
            {
                var item = MANAGER.GetOrderList();
                return Json(new { success = true, result = item });
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "GetOrderList");
                return Json(new { success = false });
            }
        }

        /// <summary>
        /// 刪除訂單
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult RemoveOrder(string id)
        {
            try
            {
                bool result = MANAGER.RemoveOrder(id);
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "RemoveOrder");
                return Json(new { success = false });
            }
        }

        /// <summary>
        /// 更新訂單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateOrder(List<Order> orders)
        {
            try
            {
                MANAGER.UpdateOrder(orders);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "UpdateOrder");
                return Json(new { success = false });
            }
        }

        /// <summary>
        /// 取所有狀態
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetOrderStatus()
        {
            OrderStatusModel result = MANAGER.GetOrderStatus();
            return Json(new { success = true, result });
        }

        #endregion

        #region 會員管理
        /// <summary>
        /// 回傳View
        /// </summary>
        /// <returns></returns>
        public IActionResult MemberManage()
        {
            return PartialView("PartialView/Member/_MemberManagePartial");
        }

        /// <summary>
        /// 取得會員列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetMemberList()
        {
            var result = MANAGER.GetMemberList();
            return result != null ? Json(new { success = true, result }) : Json(new { success = false });
        }


        /// <summary>
        /// 更新會員
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateMember(MemberManageModel data)
        {

            return MANAGER.UpdateByMemberId(data) ? Json(new { Success = true }) : Json(new { Success = false });
        }

        /// <summary>
        /// 會員停權
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult SuspendByMemberId(int memberId, int isSuspend) => MANAGER.SuspendByMemberId(memberId, isSuspend) ? Json(new { Success = true }) : Json(new { Success = false });

        #endregion
    }
}
