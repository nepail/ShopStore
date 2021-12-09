using ShopStore.ViewModels;
using System;
using System.Collections.Generic;

namespace ShopStore.Models.Interface
{
    public interface ICart
    {
        public ProductsViewModel Single(string id);        
        public int InsertOrderItem(OrderModel model);
    }
}
