using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using SkydivingAccuracyBackend.Data.Model;

namespace WeatherCrawler.Data
{
    public static class Airports
    {
        private static Dictionary<string, Airport> _airportsDictionary;

        public static void Load(string fileName)
        {
            _airportsDictionary = new Dictionary<string, Airport>();

            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                var csvReader = new CsvReader(new StreamReader(fileStream));

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
