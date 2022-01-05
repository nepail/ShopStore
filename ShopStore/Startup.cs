using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using ShopStore.Common;
using ShopStore.Common.Filters;
using System.Data.SqlClient;
using System.IO;

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

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
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

            string connectionString = Configuration["SqlConStr"];
            services.AddTransient(e => new SqlConnection(connectionString));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<ActionFilter>();
            services.AddScoped<AuthorizationFilter>();

            //加解密儲存空間
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(@"D:\DataProtection\"));

            //驗證
            //services.AddTransient<SysUserDal>();
            //services.AddIdentity<SysUser, SysUserRole>().AddDefaultTokenProviders();
            //services.AddTransient<IUserStore<SysUser>, CustomUserStore>();
            //services.AddTransient<IRoleStore<SysUserRole>, CustomRoleStore>();



            services.AddSession(option =>
            {
                //設定逾期時間
                //option.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            //全域驗證
            services.AddMvc(option =>
            {
                //option.Filters.Add(new AuthorizeFilter());
                //option.Filters.Add(new ActionFilter());
                option.Filters.Add<ActionFilter>();
                option.Filters.Add<AuthorizationFilter>();
            });
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
            //app.UseStaticFiles(new StaticFileOptions 
            //{ 
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Scripts")),
            //    RequestPath = new PathString("/scripts")
            //});
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "node_modules")),
                RequestPath = new PathString("/vendor")
            });
            app.UseAuthentication();
            app.UseRouting();
            app.UseSession();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
