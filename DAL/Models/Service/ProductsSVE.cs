using Dapper;
using NLog;
using ShopStore.Models.Interface;
using ShopStore.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ShopStore.Models.Service
{
    public class ProductsSVE : IProducts
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly SqlConnection _connection;
        public ProductsSVE(SqlConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// 取得商品列表
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ProductsViewModel>> GetProductsAsync()
        {
            try
            {
                using var conn = _connection;
                var result = await conn.QueryAsync<ProductsViewModel>(@"pro_shopStore_getProducts");
                return result;
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Debug");
                throw ex;
            }
        }

        /// <summary>
        /// 取得商品明細
        /// </summary>
        /// <returns></returns>
        public async Task<ProductDetailViewModel> GetProductDetailByIdAsync(string id)
        {
            try
            {
                using var conn = _connection;
                var Model = await conn.QuerySingleAsync<ProductDetailModel>(@"pro_shopStore_getProductByID",
                                                                          new { ID = new DbString { Value = id, Length = 36, IsAnsi = true, IsFixedLength = true } },
                                                                          commandType: System.Data.CommandType.StoredProcedure);


                ProductDetailViewModel result = new ProductDetailViewModel()
                {
                    Id = Model.f_id,
                    Name = Model.f_name,
                    Content = Model.f_content,
                    Type = Model.f_categoryId.ToString(),
                    Price = Model.f_price,
                    ImgPath = Model.f_picPath
                };

                return result;
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Debug");
            }

            return null;
        }

        /// <summary>
        /// 取得類別列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CategoriesViewModel> GetCatgoryId()
        {
            try
            {
                using var conn = _connection;
                var result = conn.Query<CategoriesViewModel>(@"SELECT 
                                                                 [f_id]
                                                                ,[f_code]
                                                                ,[f_name] FROM t_categories WITH(NOLOCK)");
                return result;
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Debug");
                throw ex;
            }
        }

        /// <summary>
        /// 新增商品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool AddProducts(ProductsViewModel model)
        {
            //using TransactionScope scope = new TransactionScope();
            try
            {
                using var conn = _connection;
                ProductDetailsModel productsModel = new ProductDetailsModel
                {
                    f_id = Guid.NewGuid().ToString(),
                    f_name = model.f_name,
                    f_price = model.f_price,
                    f_picPath = model.f_picPath,
                    f_description = model.f_description,
                    f_categoryId = model.f_categoryId,
                    f_stock = model.f_stock,
                    f_isdel = model.f_isdel,
                    f_isopen = model.f_isopen,
                    f_content = model.f_content,
                    f_updatetime = DateTime.Now,
                    f_createtime = DateTime.Now 
                };

                var result = conn.Execute("pro_shopStore_addProduct", productsModel, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Debug");
                return false;
            }
            //scope.Complete();
            return true;
        }

        /// <summary>
        /// 確認是否有此產品
        /// </summary>
        /// <returns></returns>
        public bool Any(string id)
        {
            using var conn = _connection;
            Guid guid = Guid.Parse(id);
            return conn.ExecuteScalar<bool>("select count(1) from t_products where f_id=@f_id", new { f_id = guid });
        }

    }
}
