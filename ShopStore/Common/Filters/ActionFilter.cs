using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShopStore.Common
{

    //[ServiceFilter(typeof(ActionFilter))]
    public class ActionFilter : Attribute, IActionFilter
    {
        //private readonly IDistributedCache _cache;

        //public ActionFilter(IDistributedCache cache)
        //{
        //    _cache = cache;
        //}
        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //throw new NotImplementedException();
        }

        //public async void OnActionExecuting(ActionExecutingContext context)
        //{
        //    if (context.HttpContext.User.Identity.IsAuthenticated)
        //    {
        //        string userid = context.HttpContext.User.FindFirst("Account").Value;

        //        var cookieUser = context.HttpContext.Request.Cookies[userid];
        //        var cacheUser = _cache.GetString(userid);

        //        if (!context.HttpContext.Request.Cookies[userid].Equals(_cache.GetString(userid)))
        //        {
        //            //將使用者登出
        //            await context.HttpContext.SignOutAsync();
        //            //導至錯誤頁面
        //            context.Result = new RedirectToRouteResult(
        //                new RouteValueDictionary
        //                {
        //                    {"controller", "Home" },
        //                    {"action", "Error" }
        //                });
        //        }                               
        //    }           
        //}
    }
}
