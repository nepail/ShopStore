using Dapper;
using NLog;
using ShopStore.Models.Interface;
using ShopStore.ViewModels;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ShopStore.Models.Service
{
    public class MembersSVE : IMembers
    {                
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly SqlConnection _connection;

        public MembersSVE(SqlConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// 確認重複Email
        /// </summary>
        /// <param name="f_mail"></param>
        /// <returns></returns>
        public async Task<bool> VerifyEmailAsync(string f_mail)
        {
            using var conn = _connection;
            string strSql = @"select top 1 1 from t_members where f_mail = @f_mail";
            return await conn.ExecuteScalarAsync<bool>(strSql, new { f_mail });
        }

        /// <summary>
        /// 確認重複帳號
        /// </summary>
        /// <param name="f_account"></param>
        /// <returns></returns>
        public async Task<bool> VerifyAccountAsync(string f_account)
        {
            using var conn = _connection;
            string strSql = @"select top 1 1 from t_members where f_account = @f_account";
            return await conn.ExecuteScalarAsync<bool>(strSql, new { f_account });
        }

        /// <summary>
        /// 登入查詢有無該User
        /// </summary>
        /// <param name="f_account"></param>
        /// <param name="f_pcode"></param>
        /// <returns></returns>
        public MemberViewModel FindUser(string f_account, string f_pcode)
        {
            //using var conn = _connection;            
            //return conn.QueryFirstOrDefault<MemberViewModel>("select * from t_members where f_account = @f_account and f_pwd = @f_pwd", new { f_account, f_pwd });

            try
            {
                using var conn = _connection;
                var result = conn.QueryFirstOrDefault<MemberViewModel>(@"pro_shopStore_getMember",
                                                                          new { f_account, f_pcode, f_date = DateTime.Now },
                                                                          commandType: System.Data.CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Debug");
                throw ex;
            }
        }

        /// <summary>
        /// 新增新的會員
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddNewMember(MemberViewModel model)
        {
            try
            {                
                using var conn = _connection;
                string strSql = @" INSERT INTO t_members(                                          
                                         f_name
                                        ,f_nickname
                                        ,f_account
                                        ,f_pcode                                        
                                        ,f_phone                                        
                                        ,f_mail                                        
                                        ,f_createTime
                                        ,f_address
                                        ,f_groupid
                                        ,f_cash
                                        ,f_isSuspend
                                        ) VALUES (                                                                                 
                                         @f_name
                                        ,@f_nickname
                                        ,@f_account
                                        ,@f_pcode                                        
                                        ,@f_phone                                        
                                        ,@f_mail                                        
                                        ,@f_createTime
                                        ,@f_address                                        
                                        ,@f_groupid
                                        ,@f_cash
                                        ,@f_isSuspend                                     
                                        );";
                conn.Execute(strSql, model);
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Debug");
                return false;
            }
            return true;
        }
    }
}
