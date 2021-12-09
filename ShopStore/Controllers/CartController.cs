using Microsoft.AspNetCore.Mvc;
using NLog;
using ShopStore.Common;
using ShopStore.Models;
using ShopStore.Models.Interface;
using ShopStore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ShopStore.Controllers
{
    public class CartController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly ICart _cart;

        public CartController(ICart cart)
        {
            _cart = cart;
        }

        public IActionResult Index()
        {
            //向 Session 取得商品列表
            List<CartItem> CartItems = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");

            //計算商品總額
            if (CartItems != null)
            {
                ViewBag.Total = CartItems.Sum(m => m.SubTotal);
            }
            else
            {
                ViewBag.Total = 0;
            }

            return View(CartItems);
        }

        public IActionResult AddtoCart(string id)
        {
            var cartItem = _cart.Single(id);

            //取得商品資料
            CartItem item = new CartItem
            {
                Product = cartItem,
                Amount = 1,
                SubTotal = cartItem.f_price
            };

            //判斷 Session 內有無購物車
            if (SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart") == null)
            {
                //如果沒有已存在購物車: 建立新的購物車
                List<CartItem> cart = new List<CartItem>
                {
                    item
                };
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            else
            {
                //如果已存在購物車: 檢查有無相同的商品，有的話只調整數量
                List<CartItem> cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");

                int index = cart.FindIndex(m => m.Product.f_id.Equals(id));
                if (index != -1)
                {
                    cart[index].Amount += item.Amount;
                    cart[index].SubTotal += item.SubTotal;
                }
                else
                {
                    cart.Add(item);
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }

            return NoContent(); // HttpStatus 204: 請求成功但不更新畫面
        }

        /// <summary>
        /// 建立新的訂單
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateNewOrder([FromBody] List<OrderItem> data)
        {
            //清空購物車
            HttpContext.Session.Clear();
            
            OrderModel orderModel = new OrderModel()
            {
                f_num = DateTime.Now.ToString("yyyyMMddHHmmss"),
                f_memberid = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                f_date = DateTime.Now,
                f_status = 1,
                f_shippingMethod = 1,
                Orderlist = data
            };

            try
            {
                var result = _cart.InsertOrderItem(orderModel);
                return Json(new { success = true, message = $"成功新增 {result} 筆訂單" });                
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Debug");
                return Json(new { success = false, message = "訂單新增失敗" });
            }               
        }


        public IActionResult RemoveItem(string id)
        {
            //向 Session 取得商品列表
            List<CartItem> cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");

            //用FindIndex查詢目標在List裡的位置
            int index = cart.FindIndex(m => m.Product.f_id.Equals(id));
            cart.RemoveAt(index);

            if (cart.Count < 1)
            {
                SessionHelper.Remove(HttpContext.Session, "cart");
            }
            else
            {
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            
            return Json(new { success = true, message = $"刪除產品 {id} 成功" });
        }
    }
}
