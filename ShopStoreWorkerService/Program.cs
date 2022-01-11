using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShopStore.Models.Interface;
using ShopStore.Models.Service;
using System;
using System.Data.SqlClient;

namespace ShopStoreWorkerService
{
    public class Program
    {
        //public Program(IConfiguration configuration)
        //{
            
        //}
        //public static IConfiguration Configuration { get; }

        //public static IServiceProvider ServiceProvider { get; set; }

        //static void ConfigureServices()
        //{
        //    var services = new ServiceCollection();
        //    services.AddTransient<IManager, ManagerSVE>();
        //    ServiceProvider = services.BuildServiceProvider();
        //}


        public static void Main(string[] args)
        {
            //ConfigureServices();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    //string connectionString = "Data Source=localhost;Initial Catalog=ShoppingDB;User ID=shopstoreadmin;Password=pk!shopstoreadmin;Integrated Security=false;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                    //services.AddTransient(e => new SqlConnection(connectionString));
                    //services.AddSingleton(e => new SqlConnection(connectionString));
                    //services.AddSingleton<IManager, ManagerSVE>();
                    services.AddHostedService<Worker>();                    
                });
    }
}
