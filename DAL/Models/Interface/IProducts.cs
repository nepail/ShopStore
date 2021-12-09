﻿using ShopStore.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopStore.Models.Interface
{
    public interface IProducts
    {
        /// <summary>
        /// 取得商品列表
        /// </summary>
        /// <returns></returns>
        Task <IEnumerable<ProductsViewModel>> GetProductsAsync();

        /// <summary>
        /// 取得類別列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CategoriesViewModel> GetCatgoryId();

        /// <summary>
        /// 新增商品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool AddProducts(ProductsViewModel request);

        /// <summary>
        /// 確認是否有此產品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Any(string id);

        /// <summary>
        /// 取得商品明細
        /// </summary>
        /// <returns></returns>
        Task<ProductDetailViewModel> GetProductDetailByIdAsync(string id);
    }
}
