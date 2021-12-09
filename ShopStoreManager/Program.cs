using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShopStore.Models.Interface;
using ShopStore.Models.Service;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ShopStoreManager
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            //builder.Services.AddSingleton<IProducts, ProductsSVE>();
            //string connectionString = Configuration["SqlConStr"];
            //string connectionString = "Data Source=localhost;Initial Catalog=ShoppingDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            //builder.Services.AddTransient(e => new SqlConnection(connectionString));
            //builder.Services.AddTransient<IProducts, ProductsSVE>();

            await builder.Build().RunAsync();
        }
    }
}
