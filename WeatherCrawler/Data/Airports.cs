using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CsvHelper;
using SkydivingAccuracyBackend.Data.Model;

namespace WeatherCrawler.Data
{
    public static class Airports
    {
        private static Dictionary<string, Airport> _airportsDictionary;

        public static void Load()
        {
            _airportsDictionary = new Dictionary<string, Airport>();

            var assembly = Assembly.GetEntryAssembly();

            using (var reader = new StreamReader(assembly.GetManifestResourceStream("WeatherCrawler.Airports.csv")))
            {
                var csvReader = new CsvReader(reader);

                csvReader.Configuration.RegisterClassMap<AirportMap>();
                var records = csvReader.GetRecords<Airport>();

                foreach (var airport in records.Where(r => !String.IsNullOrEmpty(r.Code)))
                {
                    if (!_airportsDictionary.ContainsKey(airport.Code))
                        _airportsDictionary.Add(airport.Code, airport);
                }
            }
        }

        public static Airport GetByCode(string code)
        {
            if (_airportsDictionary == null || !_airportsDictionary.ContainsKey(code))
                return null;

            return _airportsDictionary[code];
        }
    }
}
