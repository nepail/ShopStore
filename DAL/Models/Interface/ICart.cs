using ShopStore.ViewModels;
using System;
using System.Collections.Generic;

namespace ShopStore.Models.Interface
{
    public interface ICart
    {
        public ProductsViewModel Single(string id);        
        public int InsertOrderItem(OrderModel model);

        public List<ProductsViewModel> QueryMutiple(string[] list);

        public List<CartItem> CheckCartItem(List<CartItem> cartItems);
    }
}
