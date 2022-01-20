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
                //option.InstanceName = "MyWebSite_"; //Redis ��ǰ�Y�ִ�
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

            //��̨�����aƷ�a��MD5�a���� DataProtection API����Ҫ�����@�μӽ��܃�����g����tIIS�����e
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(@"D:\DataProtection\"));            

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("RequireAdministratorRole",
            //         policy => policy.RequireRole("Administrator"));
            //});

            //services.AddDefaultIdentity<ShopUserRole>(options =>
            //{
            //    options.Password.RequiredLength = 4;             //�ܴa�L��
            //    options.Password.RequireLowercase = false;       //����С��Ӣ��
            //    options.Password.RequireUppercase = false;       //������Ӣ��
            //    options.Password.RequireNonAlphanumeric = false; //������̖
            //    options.Password.RequireDigit = false;           //��������
            //})            

            services.AddSession(option =>
            {
                //�O�����ڕr�g
                //option.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            //������ӆ���ڙ��^�V��
            services.AddMvc(option =>
            {
                option.Filters.Add<ActionFilter>();
                option.Filters.Add<AuthorizationFilter>();
            });

            //���É��s�ؑ�
            services.AddResponseCompression(option =>
            {
                //ͬ�r���� Gzip �� Brotil���s
                option.Providers.Add<BrotliCompressionProvider>();
                option.Providers.Add<GzipCompressionProvider>();
                option.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
            });

            services.Configure<BrotliCompressionProviderOptions>(option =>
            {
                //�Զ��x���s���e
                option.Level = (CompressionLevel)5;
            });

            services.AddMemoryCache();

            //SignalR �A�O�_��JsonProtocol
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

            //�����ȡ�O��
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


            //���É��s�ؑ�
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
                //����ȫ����C
                .RequireAuthorization();

                endpoints.MapHub<ChatHub>("/chatHub");
                endpoints.MapHub<ServerHub>("/ServerHub");
            });
        }
    }
}
