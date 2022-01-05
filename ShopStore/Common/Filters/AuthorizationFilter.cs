using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopStore.Common.Filters
{
    public class AuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly IDistributedCache _cache;

        public AuthorizationFilter(IDistributedCache cache)
        {
            _cache = cache;
        }

        async Task IAsyncAuthorizationFilter.OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                string userid = context.HttpContext.User.FindFirst("Account").Value;

                var cookieUser = context.HttpContext.Request.Cookies[userid];
                var cacheUser = _cache.GetString(userid);

                if (!context.HttpContext.Request.Cookies[userid].Equals(_cache.GetString(userid)))
                {
                    string cookieType = context.HttpContext.User.Identity.AuthenticationType;
                    string controller = cookieType == "manager" ? cookieType : "Home";

                    //將使用者登出
                    await context.HttpContext.SignOutAsync(context.HttpContext.User.Identity.AuthenticationType);
                    //導至錯誤頁面
                    context.Result = new RedirectToRouteResult(
                        new RouteValueDictionary
                        {
                            {"controller", controller },
                            {"action", "Index" }
                        });
                }
            }
        }
    }
}
