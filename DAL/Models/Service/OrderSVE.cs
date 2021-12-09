using ShopStore.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ShopStore.Models.Interface;
using System.Transactions;
using NLog;

namespace ShopStore.Models.Service
{
    public class OrderSVE : IOrders
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        readonly private SqlConnection _connection;
        public OrderSVE(SqlConnection connection)
        {
            _connection = connection;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Any()
        {
            return true;
        }

        /// <summary>
        /// 取得訂單資料
        /// </summary>
        /// <param name="memberid">使用者ID</param>
        /// <returns></returns>
        public List<OrderViewModel> GetOrderList(string memberid)
        {            
            using var conn = _connection;
            string strSql = @"select * from t_orders where f_memberid = @f_memberid and f_isdel = 0

                              select c.f_Id, c.f_ordernum, c.f_productid, c.f_amount, p.f_name, p.f_price from t_orders o with (NOLOCK)
                              join t_orderDetails c with (NOLOCK)
                              join t_products p with (NOLOCK) on c.f_productid = p.f_id
                              on o.f_num = c.f_ordernum
                              where f_memberid = @f_memberid and o.f_isdel = 0";
            
            SqlMapper.GridReader result = conn.QueryMultiple(strSql, new { f_memberid = memberid });
            List<OrderModel> orderModels = result.Read<OrderModel>().ToList();            
            List<OrderItem> orderdetail = result.Read<OrderItem>().ToList();
            List<OrderViewModel> orderViewModels = (from a in orderModels
                                                    select new OrderViewModel
                                                    {
                                                        Num = a.f_num,
                                                        Date = a.f_date.ToString(),
                                                        Status = a.f_status.ToString(),
                                                        ShippingMethod = a.f_shippingMethod.ToString(),
                                                        TotalAmountOfMoney = a.f_total,
                                                        TotalAmountOfProducts = orderdetail.Where(b => b.f_ordernum == a.f_num).Sum(x => x.f_amount),
                                                        ListOfItem = orderdetail.Where(b => b.f_ordernum == a.f_num).Select(x => new ItemDetail
                                                        {
                                                            Id = x.f_productid,
                                                            Name = x.f_name,
                                                            Amount = x.f_amount,
                                                            Price = x.f_price,
                                                            AmountOfMoney = orderdetail.First(a => a.f_productid == x.f_productid).f_price * x.f_amount
                                                        }).ToList()
                                                    }).ToList();
            return orderViewModels;
        }

        /// <summary>
        /// 取消訂單
        /// </summary>
        /// <param name="ordernum">訂單編號</param>
        /// <returns></returns>
        public bool DelOrder(string ordernum)
        {
            using TransactionScope scope = new TransactionScope();
            using var conn = _connection;            
            try
            {
                //string strSql = @"update t_orders set f_isdel = 1 where f_num = @f_num";
                //conn.Execute(strSql, new { f_num = ordernum });

                //string strSql2 = @"update t_orderDetails set f_isdel = 1 where f_ordernum = @f_ordernum";
                //conn.Execute(strSql2, new { f_ordernum = ordernum });

                string strSql =
                    @"update t_orders set f_isdel = 1 where f_num = @f_num
                      update t_orderDetails set f_isdel = 1 where f_ordernum = @f_ordernum";
                conn.Execute(strSql, new { f_num = ordernum, f_ordernum = ordernum });
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Debug");
                return false;
            }
            scope.Complete();
            return true;
        }
    }
}
