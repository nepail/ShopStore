using Dapper;
using NLog;
using ShopStore.Models.Interface;
using ShopStore.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace ShopStore.Models.Service
{
    public class CartSVE : ICart
    {        
        private readonly SqlConnection _connection;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public CartSVE(SqlConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// 取得已提交的購物車訂單
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public CartViewModel GetCartList(CartViewModel model)
        {                 
            try
            {                
                using var conn = _connection;
                string strSql = @"select * from orders";                
                return conn.Query<CartViewModel>(strSql).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Debug");                
            }            
            return null;
        }

        /// <summary>
        /// 取得產品資訊
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ProductsViewModel Single(string id)
        {            
            using var conn = _connection;
            string strSql = @"select * from ShoppingDB.dbo.t_products where f_id = @f_id";
            return conn.QuerySingle<ProductsViewModel>(strSql, new { f_id = id });
        }

        public List<ProductsViewModel> QueryMutiple(string[] list)
        {
            //var dynamicParams = new DynamicParameters();
            //foreach (var item in list)
            //{
            //    dynamicParams.Add("@f_id", item);
            //}

            List<string> lists = list.ToList();

            using var conn = _connection;
            string strSql = @"select * from t_products where f_id in @f_id";
            return (List<ProductsViewModel>)conn.Query<ProductsViewModel>(strSql, new { f_id = lists });
        }

        /// <summary>
        /// 將訂單寫入資料庫
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int InsertOrderItem(OrderModel model)
        {
            using TransactionScope scope = new TransactionScope();            
            using var conn = _connection;
            try
            {
                //計算此筆訂單的總金額

                //List<string> prolist = new List<string>();
                //foreach(var itemid in model.Orderlist)
                //{
                //    prolist.Add(itemid.f_productid);
                //}

                List<string> prolist = model.Orderlist.Select(itemid => itemid.f_productid).ToList();

                string strSql1 = @"select f_price from t_products where f_id in @f_ids";
                IEnumerable<ProductsViewModel> prolists = _connection.Query<ProductsViewModel>(strSql1, new { f_ids = prolist });
                
                model.f_total = prolists.Sum(x => x.f_price);

                //將訂單編號寫入明細
                foreach (OrderItem a in model.Orderlist)
                {
                    a.f_ordernum = model.f_num;
                }

                //寫入訂單主表
                string strSql = @"insert into t_orders(f_num
                                                      ,f_memberid
                                                      ,f_date
                                                      ,f_status
                                                      ,f_shippingmethod
                                                      ,f_total
                                                      ,f_ispaid
                                                      ,f_isdel
                                                      ) values (
                                                       @f_num
                                                      ,@f_memberid
                                                      ,@f_date
                                                      ,@f_status
                                                      ,@f_shippingmethod
                                                      ,@f_total
                                                      ,@f_ispaid
                                                      ,@f_isdel)";
                int result = conn.Execute(strSql, model);
                //將訂單明細寫入表
                string strDetailSql = 
                @"insert into t_orderDetails(f_ordernum, f_productid, f_amount, f_isdel) values (@f_ordernum, @f_productid, @f_amount, @f_isdel)";
                conn.Execute(strDetailSql, model.Orderlist);
                
                //交易完成
                scope.Complete();
                return result;
            }
            catch (Exception ex)
            {
                //交易失敗，RollBack
                logger.Debug(ex, "Debug");
                return 0;
            }
        }
    }
}
