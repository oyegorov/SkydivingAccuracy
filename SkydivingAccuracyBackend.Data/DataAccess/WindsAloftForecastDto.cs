using System;
using Newtonsoft.Json;
using SkydivingAccuracyBackend.Data.Model;

namespace SkydivingAccuracyBackend.Data.DataAccess
{
    public class WindsAloftForecastDto
    {
        public int Id { get; set; }

        public DateTime UpdatedOn { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }
        
        public string AirportCode { get; set; }

        public string AirportName { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string WindsAloftData { get; set; }

        public static WindsAloftForecastDto FromWindsAloftForecast(WindsAloftForecast windsAloftForecast)
        {
            if (windsAloftForecast == null)
                throw new ArgumentNullException(nameof(windsAloftForecast));
            if (windsAloftForecast.WindsAloftRecords == null)
                throw new ArgumentNullException(nameof(windsAloftForecast.WindsAloftRecords));
            if (windsAloftForecast.Airport == null)
                throw new ArgumentNullException(nameof(windsAloftForecast.Airport));

            return new WindsAloftForecastDto
            {
                UpdatedOn = windsAloftForecast.UpdatedOn,
                ValidFrom = windsAloftForecast.ValidFrom,
                ValidTo = windsAloftForecast.ValidTo,
                AirportCode = windsAloftForecast.Airport.Code,
                AirportName = windsAloftForecast.Airport.Name,
                Latitude = windsAloftForecast.Airport.Latitude,
                Longitude = windsAloftForecast.Airport.Longitude,
                WindsAloftData = JsonConvert.SerializeObject(windsAloftForecast.WindsAloftRecords)
            };
        }

        public WindsAloftForecast ToWindsAloftForecast()
        {
            return new WindsAloftForecast
            {
                UpdatedOn = UpdatedOn,
                
                ValidFrom = ValidFrom,

                ValidTo = ValidTo,

                Airport = new Airport
                {
                    Code = AirportCode,
                    Longitude = Longitude,
                    Latitude = Latitude,
                    Name = AirportName
                },

                WindsAloftRecords = JsonConvert.DeserializeObject<WindsAloftRecord[]>(WindsAloftData)
            };
        }
    }
}
