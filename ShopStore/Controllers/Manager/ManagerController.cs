using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShopStore.Models;
using ShopStore.Models.Interface;
using ShopStore.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using NLog;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using static ShopStore.ViewModels.ProductsViewModel;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace ShopStore.Controllers
{
    public class ManagerController : Controller
    {
        private readonly IProducts _products;
        private readonly IManager _manager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public ManagerController(IProducts products, IManager manager, IWebHostEnvironment webHostEnvironment)
        {
            _products = products;
            _manager = manager;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            //從session 取得 username
            var userid = 2;

            //回傳菜單列表
            IEnumerable<MenuModel> menuModels = await _manager.GetMenu(userid);

            #region 測試資料
            //List<MenuModel> menuModels = new List<MenuModel>()
            //{
            //    new MenuModel()
            //    {
            //        f_id = 1,
            //        f_name = "產品管理",
            //        f_icon = "bx-basket",
            //        MenuSubModels = new List<MenuSubModel>()
            //        {
            //            new MenuSubModel(){f_id = 1,f_menuid = 1,f_name="新增產品"},
            //            new MenuSubModel(){f_id = 2,f_menuid = 1,f_name="庫存管理"},
            //            new MenuSubModel(){f_id = 3,f_menuid = 1,f_name="測試管理"},
            //            new MenuSubModel(){f_id = 4,f_menuid = 1,f_name="你好哈囉"},
            //        },
            //        f_isdel = 0
            //    },
            //    new MenuModel()
            //    {
            //        f_id = 2,
            //        f_name = "帳號管理",
            //        f_icon = "bx-male",
            //        MenuSubModels=new List<MenuSubModel>()
            //        {
            //            new MenuSubModel(){f_id = 1,f_menuid = 2,f_name="帳號管理"},
            //            new MenuSubModel(){f_id = 2,f_menuid = 2,f_name="會員查詢"},
            //            new MenuSubModel(){f_id = 3,f_menuid = 2,f_name="等級設定"}
            //        },
            //        f_isdel = 0
            //    },
            //    new MenuModel()
            //    {
            //        f_id = 3,
            //        f_name = "訂單管理",
            //        f_icon = "bx-windows",
            //        MenuSubModels = new List<MenuSubModel>()
            //        {
            //            new MenuSubModel(){f_id = 1,f_menuid = 3, f_name="訂單管理"},
            //            new MenuSubModel(){f_id = 1,f_menuid = 3, f_name="訂單設定"}
            //        }
            //    },
            //    new MenuModel()
            //    {
            //        f_id = 4,
            //        f_name = "設定",
            //        f_icon = "bx-cog",
            //        MenuSubModels = new List<MenuSubModel>()
            //        {
            //            new MenuSubModel(){f_id = 1,f_menuid = 4, f_name="菜單管理"},
            //            new MenuSubModel(){f_id = 1,f_menuid = 4, f_name="設定"}
            //        }
            //    },
            //};
            #endregion

            return View(menuModels);
        }

        [HttpGet("[action]")]
        public IActionResult SampleData()
        {
            #region Index的測試資料
            var result = new List<PersonModel>();
            result.Add(new PersonModel
            {
                Age = 28,
                FirstName = "Tim",
                LastName = "Tsai"
            });
            result.Add(new PersonModel
            {
                Age = 21,
                FirstName = "Larsen",
                LastName = "Shaw"
            });
            result.Add(new PersonModel
            {
                Age = 89,
                FirstName = "Geneva",
                LastName = "Wilson"
            });
            result.Add(new PersonModel
            {
                Age = 28,
                FirstName = "Jami",
                LastName = "Carney"
            });
            #endregion
            return Ok(result);
        }

        [HttpPost("[action]")]
        public IActionResult PersonData([FromBody] PersonDataModel data)
        {
            return Ok();
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
                var productList = _products.GetCatgoryId().ToList();
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
                logger.Debug(ex, "Debug");
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
                    if(model.ProductPic != null)
                    {
                        UploadedFile(model.ProductPic, model.f_pId);
                    }

                    if(model.f_content != null)
                    {
                        EditProductContent(model.f_content, model.f_pId);
                    }
                                               
                    //if (model.f_picName != "")
                    //{


                    //}
                    //else
                    //{
                    //    return Json(new { success = false, message = "圖片上傳失敗" });
                    //}


                    bool result = await _products.EditProductById(model);

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
                logger.Debug(ex, "Debug");
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
            //string uniqueFileName = null;
            try
            {
                if (file != null)
                {                    
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    //uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    string filePath = Path.Combine(uploadsFolder, id + ".jpg");
                    await using var fileStream = new FileStream(filePath, FileMode.Create);
                    file.CopyTo(fileStream);
                }

                //return uniqueFileName;
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "UploadFile Error");
                //return "";
            }
        }

        /// <summary>
        /// 編輯文字檔
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private void EditProductContent(string contentText, string id)
        {
            string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "content");
            string filePath = Path.Combine(uploadFolder, id + ".txt");
            using StreamWriter file = new StreamWriter(filePath, false);
            file.Write(contentText);

            //return true;
        }

        /// <summary>
        /// 取得列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetCategoryList()
        {
            List<CategoriesViewModel> productList = _products.GetCatgoryId().ToList();
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

        #region 會員管理

        /// <summary>
        /// Manager 會員查詢
        /// </summary>
        /// <returns></returns>
        public IActionResult MemberQuery() => PartialView("PartialView/Member/_MemberQueryPartial");

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
            IEnumerable<MenuModel> model = await _manager.GetMenu(2);
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
            var item = _manager.GetOrderList();

            return Json(new { success = true, result = item });
        }

        /// <summary>
        /// 刪除訂單
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult RemoveOrder(string id)
        {
            bool result = _manager.RemoveOrder(id);

            return Json(new { success = result });
        }

        #endregion
    }
}
