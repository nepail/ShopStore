//using Microsoft.AspNetCore.Identity;
//using ShopStore.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace ShopStore.Providers
//{
//    public class CustomUserStore : IUserStore<SysUser>, IUserPasswordStore<SysUser>
//    {
//        private readonly SysUserDal _usersTable;

//        public CustomUserStore(SysUserDal usersTable)
//        {
//            _usersTable = usersTable;
//        }

//        #region createuser
//        public async Task<IdentityResult> CreateAsync(SysUser user,
//            CancellationToken cancellationToken = default(CancellationToken))
//        {
//            cancellationToken.ThrowIfCancellationRequested();
//            if (user == null) throw new ArgumentNullException(nameof(user));

//            return await _usersTable.CreateAsync(user);
//        }
//        #endregion

//        public async Task<IdentityResult> DeleteAsync(SysUser user,
//            CancellationToken cancellationToken = default(CancellationToken))
//        {
//            cancellationToken.ThrowIfCancellationRequested();
//            if (user == null) throw new ArgumentNullException(nameof(user));

//            return await _usersTable.DeleteAsync(user);

//        }

//        public void Dispose()
//        {
//        }

//        public async Task<SysUser> FindByIdAsync(string userId,
//            CancellationToken cancellationToken = default(CancellationToken))
//        {
//            cancellationToken.ThrowIfCancellationRequested();
//            if (userId == null) throw new ArgumentNullException(nameof(userId));
//            return await _usersTable.FindByIdAsync(userId);

//        }

//        public async Task<SysUser> FindByNameAsync(string userName,
//            CancellationToken cancellationToken = default(CancellationToken))
//        {
//            cancellationToken.ThrowIfCancellationRequested();
//            if (userName == null) throw new ArgumentNullException(nameof(userName));

//            return await _usersTable.FindByNameAsync(userName);
//        }

//        public Task<string> GetNormalizedUserNameAsync(SysUser user, CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<string> GetPasswordHashAsync(SysUser user, CancellationToken cancellationToken)
//        {
//            cancellationToken.ThrowIfCancellationRequested();
//            if (user == null) throw new ArgumentNullException(nameof(user));

//            return Task.FromResult(user.Password);
//        }

//        public Task<string> GetUserIdAsync(SysUser user, CancellationToken cancellationToken)
//        {
//            cancellationToken.ThrowIfCancellationRequested();
//            if (user == null) throw new ArgumentNullException(nameof(user));

//            return Task.FromResult(user.UserID);
//        }
//        public Task<bool> CheckPasswordAsync(SysUser user, string pwd,
//            CancellationToken cancellationToken = default(CancellationToken))
//        {
//            cancellationToken.ThrowIfCancellationRequested();
//            return Task.FromResult(user.Password == pwd);
//        }
//        public Task<bool> IsEmailConfirmedAsync(SysUser user,
//            CancellationToken cancellationToken = default(CancellationToken))
//        {
//            cancellationToken.ThrowIfCancellationRequested();
//            return Task.FromResult(true);
//        }

//        public Task<string> GetUserNameAsync(SysUser user, CancellationToken cancellationToken)
//        {
//            cancellationToken.ThrowIfCancellationRequested();
//            if (user == null) throw new ArgumentNullException(nameof(user));

//            return Task.FromResult(user.UserID);
//        }

//        public Task<bool> HasPasswordAsync(SysUser user, CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }

//        public Task SetNormalizedUserNameAsync(SysUser user, string normalizedName, CancellationToken cancellationToken)
//        {
//            cancellationToken.ThrowIfCancellationRequested();
//            if (user == null) throw new ArgumentNullException(nameof(user));
//            if (normalizedName == null) throw new ArgumentNullException(nameof(normalizedName));

//            user.NormalizedUserName = normalizedName;
//            return Task.FromResult<object>(null);
//        }

//        public Task SetPasswordHashAsync(SysUser user, string passwordHash, CancellationToken cancellationToken)
//        {
//            cancellationToken.ThrowIfCancellationRequested();
//            if (user == null) throw new ArgumentNullException(nameof(user));
//            if (passwordHash == null) throw new ArgumentNullException(nameof(passwordHash));

//            user.PasswordHash = passwordHash;
//            return Task.FromResult<object>(null);

//        }

//        public Task SetUserNameAsync(SysUser user, string userName, CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<IdentityResult> UpdateAsync(SysUser user, CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
