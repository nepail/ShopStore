using Microsoft.Extensions.Configuration;
using ShopStore.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopStore.Common
{
    public class DBHelper : IDBHelper
    {        

        public static string connstring { get; private set; }
        public DBHelper(IConfiguration configuration)
        {
            connstring = configuration["ConnectionStrings: DBConnection"];
        }        
    }
}
