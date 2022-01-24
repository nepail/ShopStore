#region 功能與歷史修改描述

/*
    描述:前台會員資料庫處理
    建立日期:2021-11-24

    描述:程式碼風格調整
    修改日期:2022-01-07

 */

#endregion

using DAL.ViewModels;
using Dapper;
using NLog;
using ShopStore.Models.Interface;
using ShopStore.ViewModels;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ShopStore.Models.Service
{
    public class MembersSVE : IMembers
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
        private readonly SqlConnection CONNECTION;

        public MembersSVE(SqlConnection connection)
        {
            CONNECTION = connection;
        }

        /// <summary>
        /// 確認重複Email
        /// </summary>
        /// <param name="f_mail"></param>
        /// <returns></returns>
        public async Task<bool> VerifyEmailAsync(string f_mail)
        {
            using var conn = CONNECTION;
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
            using var conn = CONNECTION;
            string strSql = @"select top 1 1 from t_members where f_account = @f_account";
            return await conn.ExecuteScalarAsync<bool>(strSql, new { f_account });
        }

        /// <summary>
        /// 登入查詢有無該User
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pcode"></param>
        /// <returns></returns>
        public MemberViewModel FindUser(string account, string pcode)
        {
            try
            {
                using var conn = CONNECTION;
                return conn.QueryFirstOrDefault<MemberViewModel>(@"pro_fr_getMember",
                                                                new { account, pcode, date = DateTime.Now },
                                                                commandType: System.Data.CommandType.StoredProcedure);                
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "Debug");
                return null;
            }
        }

        /// <summary>
        /// 取得該Member的資料
        /// </summary>
        /// <param name="memberId"></param>        
        /// <returns></returns>
        public UserProfileViewModel GetMemberProfile(int memberId)
        {
            try
            {
                using var conn = CONNECTION;
                return conn.QueryFirstOrDefault<UserProfileViewModel>(@"pro_fr_getMemberProfile",
                                                                          new { memberId },
                                                                          commandType: System.Data.CommandType.StoredProcedure);                
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "Debug");
                return null;
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

                //Dapper.SqlMapper.SetTypeMap()

                using var conn = CONNECTION;
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
                LOGGER.Debug(ex, "Debug");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 重置會員密碼
        /// </summary>
        /// <param name="code"></param>
        /// <param name="mail"></param>
        /// <returns></returns>
        public bool ResetMemberPcode(string pwd, string mail)
        {
            try
            {
                using var conn = CONNECTION;
                return conn.Execute("pro_bg_editMemberPwd", new { pwd, mail }, commandType: System.Data.CommandType.StoredProcedure) > 0;
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "Debug");
                return false;
            }
        }
    }
}
