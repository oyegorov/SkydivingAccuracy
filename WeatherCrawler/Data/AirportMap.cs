using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using SkydivingAccuracyBackend.Data.Model;

namespace WeatherCrawler.Data
{
    public sealed class AirportMap : CsvClassMap<Airport>
    {
        public AirportMap()
        {
            Map(m => m.Name).Name("name");
            Map(m => m.Longitude).Name("longitude_deg");
            Map(m => m.Latitude).Name("latitude_deg");
            Map(m => m.Code).Name("iata_code");
        }
    }
}
