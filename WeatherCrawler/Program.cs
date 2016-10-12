using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using SkydivingAccuracyBackend.Data.DataAccess;
using WeatherCrawler.BusinessLogic;
using WeatherCrawler.Data;
using System.Linq;
using SkydivingAccuracyBackend.Data.Model;

namespace WeatherCrawler
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: dotnet WeatherCrawler [dbFileName] [airportsCsv]");
                return -1;
            }

            string dbFileName = args[0];
            string airportsCsv = args[1];
            if (!File.Exists(dbFileName))
            {
                Console.WriteLine($"Database does not exist: {dbFileName}");
                return -1;
            }
            if (!File.Exists(airportsCsv))
            {
                Console.WriteLine($"Airports CSV file does not exist: {dbFileName}");
                return -1;
            }

            Stopwatch sw = new Stopwatch();

            sw.Start();
            Console.WriteLine("Loading airports");

            try
            {
                Airports.Load(airportsCsv);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception has occured while trying to load airport information");
                Console.WriteLine(ex.Message);
                return -1;
            }
            sw.Stop();
            Console.WriteLine($"Done. Elapsed time: {sw.ElapsedMilliseconds}ms");


            Console.WriteLine("Weather crawler started.");
            var windsAloftRetriever= new AggregatedWindsAloftRetriever();

            Console.WriteLine("Retrieving winds aloft data from different sources");
            
            sw.Restart();
            var windsAloftForecasts = windsAloftRetriever.GetWindsAloft();
            sw.Stop();
            Console.WriteLine($"Done. Elapsed time: {sw.ElapsedMilliseconds}ms");

            Console.WriteLine("Saving data to the database.");


            sw.Restart();
            using (var db = new SkydivingAccuracyDbContext(dbFileName))
            {
                var transaction = db.Database.BeginTransaction();

                DateTime cutoffDate = DateTime.UtcNow.AddDays(-2);
                var itemsToDelete = db.WindsAloft.Where(w => w.UpdatedOn < cutoffDate || w.ValidTo < DateTime.UtcNow);
                db.RemoveRange(itemsToDelete);

                foreach (var windsAloftForecast in windsAloftForecasts)
                {
                    db.WindsAloft.Add(WindsAloftForecastDto.FromWindsAloftForecast(windsAloftForecast));
                }
                var count = db.SaveChanges();

                Console.WriteLine($"{count} records saved. Elapsed time: {sw.ElapsedMilliseconds}ms");

                transaction.Commit();
            }

            return 0;
        }
    }
}
