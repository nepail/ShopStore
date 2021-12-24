using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLog;
using ShopStore.Models;
using ShopStore.Models.Interface;
using ShopStore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShopStore.Controllers
{
    public class OrderController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IOrders _orders;


        public OrderController(IOrders orders)
        {
            _orders = orders;
        }

        /// <summary>
        /// 我的訂單
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            List<OrderViewModel> model;
            string userId;

            try
            {
                var user = User.Identity.Name;                
                userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                model = _orders.GetOrderList(userId);
                return View(model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return RedirectToAction("/Home/Error");
            }
        }

        /// <summary>
        /// 取消訂單
        /// </summary>
        /// <param name="ordernum"></param>
        /// <returns></returns>
        public IActionResult CancelOrder(string ordernum)
        {

            try
            {
                _orders.DelOrder(ordernum);
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Debug");
            }

            return Json(new { success = true, message = "successfull" });
        }
    }
}
