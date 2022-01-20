

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
