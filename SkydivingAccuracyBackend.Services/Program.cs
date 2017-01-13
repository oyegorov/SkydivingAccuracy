using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;

namespace SkydivingAccuracyBackend.Services
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: dotnet SkydivingAccuracyBackend.Services.dll [dbFileName]");
                return -1;
            }

            string dbFileName = args[0];
            if (!File.Exists(dbFileName))
            {
                Console.WriteLine($"Database does not exist: {dbFileName}");
                return -1;
            }

            Startup.DbFileName = dbFileName;

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();

            return 0;
        }
    }
}
