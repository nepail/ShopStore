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

namespace ShopStore.Controllers
{
    public class ManagerController : Controller
    {
        private readonly IProducts _products;
        private readonly IManager _manager;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public ManagerController(IProducts products, IManager manager)
        {
            _products = products;
            _manager = manager;
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


        public async Task<IActionResult> Menu()
        {
            IEnumerable<MenuModel> model = await _manager.GetMenu(2);
            return PartialView("PartialView/Menu/_MenuPartial", model);
        }
    }
}
