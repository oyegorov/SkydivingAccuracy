using System;
using System.Diagnostics;
using System.IO;
using Microsoft.EntityFrameworkCore;
using SkydivingAccuracyBackend.Data.DataAccess;
using WeatherCrawler.BusinessLogic;

namespace WeatherCrawler
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: dotnet WeatherCrawler [dbFileName]");
                return -1;
            }

            string dbFileName = args[0];
            if (!File.Exists(dbFileName))
            {
                Console.WriteLine($"Specified file does not exist: {dbFileName}");
            }

            Console.WriteLine("Weather crawler started.");
            var weatherRetriever = new NavCanadaWeatherRetriever();

            Console.WriteLine("Retrieving data from NavCanada");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var weatherEntries = weatherRetriever.GetWeatherEntries();
            sw.Stop();
            Console.WriteLine($"Done. Elapsed time: {sw.ElapsedMilliseconds}ms");

            Console.WriteLine("Saving data to the database.");

            sw.Restart();
            using (var db = new SkydivingAccuracyDbContext(dbFileName))
            {
                var transaction = db.Database.BeginTransaction();

                db.Database.ExecuteSqlCommand("DELETE FROM WeatherInfos");

                foreach (var weather in weatherEntries)
                {
                    db.WeatherInfos.Add(WeatherInfo.FromWeather(weather));
                }
                var count = db.SaveChanges();

                Console.WriteLine($"{count} records saved. Elapsed time: {sw.ElapsedMilliseconds}ms");

                transaction.Commit();
            }

            return 0;
        }
    }
}
