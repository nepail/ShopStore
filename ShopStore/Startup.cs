#region 功能與歷史修改描述

/*
    描述:Startup 配置
    建立日期:2021-11-17

    描述:程式碼風格調整
    修改日期:2022-01-20

 */

#endregion

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using ShopStore.Common;
using ShopStore.Common.Filters;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using ShopStore.Hubs;
using ShopStore.Hubs.Models.Services;

namespace ShopStore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            double LoginExpireMinute = this.Configuration.GetValue<double>("LoginExpireMinute");
            string connectionString = Configuration["SqlConStr"];

            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option =>
                {
                    option.LoginPath = new PathString("/Member/Login");
                    option.LogoutPath = new PathString("/Member/Logout");
                    option.AccessDeniedPath = new PathString("/Home/AccessDenied");
                })
                .AddCookie("manager", option =>
                {
                    option.LoginPath = new PathString("/Manager/Index");
                });

            services.AddStackExchangeRedisCache(option =>
            {
                option.Configuration = Configuration.GetSection("Redis")["ConnectionString"];
                //option.InstanceName = "MyWebSite_"; //Redis 的前綴字串
            });

            services.AddControllersWithViews();

            services.AddTransient<Models.Interface.IProducts, Models.Service.ProductsSVE>();
            services.AddTransient<Models.Interface.IMembers, Models.Service.MembersSVE>();
            services.AddTransient<Models.Interface.ICart, Models.Service.CartSVE>();
            services.AddTransient<Models.Interface.IOrders, Models.Service.OrderSVE>();
            services.AddTransient<Models.Interface.IManager, Models.Service.ManagerSVE>();
            services.AddTransient(e => new SqlConnection(connectionString));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ConUserService>();

            services.AddScoped<ActionFilter>();
            services.AddScoped<AuthorizationFilter>();

            //後台新增產品產生MD5碼呼叫 DataProtection API，需要加上這段加解密儲存空間，否則IIS會報錯
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(@"D:\DataProtection\"));            

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("RequireAdministratorRole",
            //         policy => policy.RequireRole("Administrator"));
            //});

            //services.AddDefaultIdentity<ShopUserRole>(options =>
            //{
            //    options.Password.RequiredLength = 4;             //密碼長度
            //    options.Password.RequireLowercase = false;       //包含小寫英文
            //    options.Password.RequireUppercase = false;       //包含大寫英文
            //    options.Password.RequireNonAlphanumeric = false; //包含符號
            //    options.Password.RequireDigit = false;           //包含數字
            //})            

            services.AddSession(option =>
            {
                //設定逾期時間
                //option.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            //加入自訂的授權過濾器
            services.AddMvc(option =>
            {
                option.Filters.Add<ActionFilter>();
                option.Filters.Add<AuthorizationFilter>();
            });

            //啟用壓縮回應
            services.AddResponseCompression(option =>
            {
                //同時啟用 Gzip 及 Brotil壓縮
                option.Providers.Add<BrotliCompressionProvider>();
                option.Providers.Add<GzipCompressionProvider>();
                option.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
            });

            services.Configure<BrotliCompressionProviderOptions>(option =>
            {
                //自定義壓縮級別
                option.Level = (CompressionLevel)5;
            });

            services.AddMemoryCache();

            //SignalR 預設開啟JsonProtocol
            services.AddSignalR().AddJsonProtocol();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            //加入快取設定
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    OnPrepareResponse = ctx =>
            //    {
            //        ctx.Context.Response.Headers[HeaderNames.CacheControl] = "public, max-age=86400";
            //    }            
            //});

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "node_modules")),
                RequestPath = new PathString("/vendor")
            });


            //啟用壓縮回應
            app.UseResponseCompression();

            app.UseAuthentication();

            app.UseRouting();

            app.UseSession();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}")
                //啟用全域驗證
                .RequireAuthorization();

                endpoints.MapHub<ChatHub>("/chatHub");
                endpoints.MapHub<ServerHub>("/ServerHub");
            });
        }
    }
}
