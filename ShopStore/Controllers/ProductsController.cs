using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NLog;
using ShopStore.Models.Interface;
using ShopStore.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using ShopStore.Models;

namespace ShopStore.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProducts _products;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ProductsController(IProducts products, IWebHostEnvironment webHostEnvironment)
        {
            _products = products;
            _webHostEnvironment = webHostEnvironment;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 取得商品列表
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IActionResult> ProductList(int type)
        {
            var isopen = 1;
            return View(await _products.GetProductsAsync(isopen));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<object> ProductLists(string md5string)
        {
            var isopen = 0;

            try
            {
                IEnumerable<ProductsViewModel> result = await _products.GetProductsAsync(isopen);
                string resultJson = JsonConvert.SerializeObject(result);
                string resiltJsonMd5 = Md5(resultJson);

                if (resiltJsonMd5.Equals(md5string))
                {
                    //md5相同 無須更新
                    return (new { success = false });
                }

                return (new { success = true, item = result, ajaxsign = resiltJsonMd5 });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 取得類別清單
        /// </summary>
        /// <returns></returns> 
        [Route("Products/AddProduct")]
        [Authorize(Roles = "admin")]
        public IActionResult AddNewProduct()
        {
            List<CategoriesViewModel> productList = _products.GetCatgoryId().ToList();
            ProductsViewModel productsViewModels = new ProductsViewModel();
            productsViewModels.SelectListItems.AddRange(from a in productList
                                                        select new SelectListItem
                                                        {
                                                            Value = a.f_id.ToString(),
                                                            Text = a.f_name,
                                                        });
            return View(productsViewModels);
        }

        [Route("Products/AddProducts")]
        //[Authorize(Roles = "admin")]
        public IActionResult AddNewProducts()
        {
            List<CategoriesViewModel> productList = _products.GetCatgoryId().ToList();
            ProductsViewModel productsViewModels = new ProductsViewModel();
            productsViewModels.SelectListItems.AddRange(from a in productList
                                                        select new SelectListItem
                                                        {
                                                            Value = a.f_id.ToString(),
                                                            Text = a.f_name,
                                                        });
            return PartialView("_ProductManagePartial", productsViewModels);
        }

        /// <summary>
        /// 新增商品資訊
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewProduct(ProductsViewModel request)
        {
            try
            {
                if (request != null && ModelState.IsValid)
                {
                    request.f_picPath = await UploadedFile(request);
                    if (!_products.AddProducts(request))
                    {
                        return Json(new { success = false, message = "新增商品錯誤" });
                    }
                    return Json(new { success = true, message = "新增商品成功" });
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Debug");
            }
            return Json(new { success = false, message = "新增商品失敗" });
        }

        /// <summary>
        /// 上傳圖片
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task<string> UploadedFile(ProductsViewModel model)
        {
            string uniqueFileName = null;
            try
            {
                if (model.ProductPic != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProductPic.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    await using var fileStream = new FileStream(filePath, FileMode.Create);
                    model.ProductPic.CopyTo(fileStream);
                }
                return uniqueFileName;
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "UploadFile Error");
                return "";
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductDetailById(string id)
        {
            try
            {
                var result = await _products.GetProductDetailByIdAsync(id);
                return PartialView("_ProductPartial", result);
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "GetProductDetailById Error");
            }

            return NotFound();
        }

        public static string Md5(string s)
        {
            using var md5 = MD5.Create();
            var result = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
            var strResult = BitConverter.ToString(result);
            return strResult.Replace("-", "").ToUpper();
        }
    }
}
