using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopStore.ViewModels
{
    /// <summary>
    /// 購物車 ViewModel
    /// </summary>
    public class CartViewModel
    {
        //public CartViewModel()
        //{
        //    listOfViewModel = new List<CartViewModel>();
        //}
        //public int f_Id { get; set; }
        //public int f_orderId { get; set; }
        //public int f_productId { get; set; }
        //public int f_quantity { get; set; }
        //public int f_unitPrice { get; set; }
        //public int f_total { get; set; }
        //public string ImgPath { get; set; }
        //public string ImgName { get; set; }

        //public List<CartViewModel> listOfViewModel;

        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }  //商品ID
        public int Amount { get; set; }     //數量
        public int SubTotal { get; set; }   //小計
    }
    public class CartItem : CartViewModel
    {
        public ProductsViewModel Product { get; set; } //商品內容
        //public string imageSrc { get; set; }
   
    }
}
