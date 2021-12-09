//using Dapper;
//using Microsoft.AspNetCore.Identity;
//using ShopStore.Models;
//using System;
//using System.Collections.Generic;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Threading.Tasks;

//namespace ShopStore.Providers
//{
//    public class SysUserDal
//    {
//        private readonly SqlConnection _connection;
//        public SysUserDal(SqlConnection connection)
//        {
//            _connection = connection;
//        }        

//        public async Task<IdentityResult> CreateAsync(SysUser user)
//        {
//            string sql = "insert into sys_user (userid,username,password) values (@userid, @username, @password)";

//            int rows = await _connection.ExecuteAsync(sql, new { userid = user.UserID, username = user.UserName, password = user.PasswordHash });

//            if (rows > 0)
//            {
//                return IdentityResult.Success;
//            }
//            return IdentityResult.Failed(new IdentityError { Description = $"Could not insert user {user.UserID}." });
//        }

//        public async Task<IdentityResult> DeleteAsync(SysUser user)
//        {
//            string sql = "delete from sys_user where userid = @userid";
//            int rows = await _connection.ExecuteAsync(sql, new { user.UserID });

//            if (rows > 0)
//            {
//                return IdentityResult.Success;
//            }
//            return IdentityResult.Failed(new IdentityError { Description = $"Could not delete user {user.UserID}." });
//        }

//        public async Task<IdentityResult> UpdateAsync(SysUser user)
//        {
//            string sql = "update sys_user set password=@password, username=@username where userid=@userid";
//            int rows = await _connection.ExecuteAsync(sql, new { userid = user.UserID, password = user.PasswordHash });
//            if (rows > 0)
//            {
//                return IdentityResult.Success;
//            }
//            return IdentityResult.Failed(new IdentityError { Description = $"Could not update user {user.UserID}." });
//        }

//        public async Task<SysUser> FindByIdAsync(string userId)
//        {
//            string sql = "select * from sys_user where userid = @userid";

//            return await _connection.QuerySingleOrDefaultAsync<SysUser>(sql, new
//            {
//                userid = userId
//            });
//        }
//        public async Task<SysUser> FindByNameAsync(string userName)
//        {            
//            string sql = "select * from sys_user where username = @username";
//            var a = await _connection.QuerySingleOrDefaultAsync<SysUser>(sql, new
//            {
//                username = userName
//            });

//            return a;
//        }
//    }
//}
