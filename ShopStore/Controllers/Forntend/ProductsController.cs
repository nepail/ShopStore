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
            return View("/Views/Frontend/Products/Index.cshtml");
        }

        /// <summary>
        /// 取得商品列表
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IActionResult> ProductList(int type)
        {
            var isopen = 1;
            return View("/Views/Frontend/Products/ProductList.cshtml", await _products.GetProductsAsync(isopen));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<object> ProductLists(string md5string)
        {
            var isopen = 0;

            try
            {
                IEnumerable<ProductsViewModel> result = await _products.GetProductsAsync(isopen);

                foreach(var a in result)
                {
                    a.f_content = ReadProductContent(a.f_pId);
                    a.f_picName = a.f_pId + ".jpg";
                }

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
            return View("/Views/Frontend/Products/AddNewProduct.cshtml", productsViewModels);
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
                    request.f_pId = Guid.NewGuid().ToString();
                    request.f_picName = await UploadedFile(request);
                    request.f_content = request.f_picName;
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
                    //uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProductPic.FileName;
                    //uniqueFileName = Guid.NewGuid().ToString();
                    uniqueFileName = model.f_pId.ToString();
                    WriteProductContent(model.ContentText, uniqueFileName);

                    string filePath = Path.Combine(uploadsFolder, uniqueFileName + ".jpg");
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

        /// <summary>
        /// 寫入文字檔
        /// </summary>
        /// <param name="contentText"></param>
        /// <param name="uniqueFileName"></param>
        private void WriteProductContent(string contentText, string uniqueFileName)
        {
            string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "content");
            string filePath = Path.Combine(uploadFolder, uniqueFileName + ".txt");
            using StreamWriter file = new StreamWriter(filePath, false);
            file.Write(contentText);
        }

        /// <summary>
        /// 讀取文字檔
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private string ReadProductContent(string id)
        {
            string filePath = @$"{Path.Combine(_webHostEnvironment.WebRootPath, "content\\")}{id}.txt";
            string contentTxt = null;

            using StreamReader reader = new StreamReader(filePath);
            if (System.IO.File.Exists(filePath))
            {
                contentTxt = reader.ReadToEnd();
            }

            return contentTxt;
        }



        [HttpGet]        
        public IActionResult GetProductDetailById(string id)
        {
            try
            {
                //var result = await _products.GetProductDetailByIdAsync(id);

                ViewBag.Id = id;
                return PartialView("_ProductPartial");
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "GetProductDetailById Error");
            }

            return NotFound();
        }

        private static string Md5(string s)
        {
            using var md5 = MD5.Create();
            var result = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
            var strResult = BitConverter.ToString(result);
            return strResult.Replace("-", "").ToUpper();
        }
    }
}
