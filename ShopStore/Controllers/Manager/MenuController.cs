using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using ShopStore.Models.Interface;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace ShopStore.Controllers.Manager
{
    public class MenuController : Controller
    {
        private readonly IManager _manager;
        public MenuController(IManager manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// 取得菜單
        /// </summary>
        /// <param name="menuModel"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetMenu(int userid)
        {
            //根據 user 的權限取得對應的菜單
            userid = 2;
            var result = _manager.GetMenu(userid);


            return Json(new { success = true, result = result });
        }

        /// <summary>
        /// 新增子菜單
        /// </summary>
        /// <param name="menuModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddSubMenu([FromBody] MenuViewModel model)
        {
            try
            {
                if(model != null)
                {
                    var result = await _manager.AddSubMenu(model);
                }
            }
            catch (Exception e)
            {

            }

            return Json(new { success = true, message = "執行成功" });
        }


        /// <summary>
        /// 新增菜單
        /// </summary>
        /// <param name="menuModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddMenu(MenuModel menuModel)
        {
            var result = _manager.AddMenu(menuModel);

            return Json(new { success = true, message = "111" });
        }
    }
}
