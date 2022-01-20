#region 功能與歷史修改描述

/*
    描述:Program 配置
    建立日期:2021-11-17

    描述:程式碼風格調整
    修改日期:2022-01-20

 */

#endregion

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ShopStore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .UseIISIntegration() //Out-Proccess
                    //.UseIIS() In-Proccess
                    .UseStartup<Startup>();
                    //.UseUrls("http://192.168.6.4:5051");
                });
    }
}
