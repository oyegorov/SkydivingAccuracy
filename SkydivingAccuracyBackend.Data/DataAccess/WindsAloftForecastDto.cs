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

        public static WindsAloftForecastDto FromWindsAloftForecast(WindsAloft windsAloft)
        {
            if (windsAloft == null)
                throw new ArgumentNullException(nameof(windsAloft));
            if (windsAloft.WindsAloftRecords == null)
                throw new ArgumentNullException(nameof(windsAloft.WindsAloftRecords));
            if (windsAloft.Airport == null)
                throw new ArgumentNullException(nameof(windsAloft.Airport));

            return new WindsAloftForecastDto
            {
                UpdatedOn = windsAloft.UpdatedOn,
                ValidFrom = windsAloft.ValidFrom,
                ValidTo = windsAloft.ValidTo,
                AirportCode = windsAloft.Airport.Code,
                AirportName = windsAloft.Airport.Name,
                Latitude = windsAloft.Airport.Latitude,
                Longitude = windsAloft.Airport.Longitude,
                WindsAloftData = JsonConvert.SerializeObject(windsAloft.WindsAloftRecords)
            };
        }

        public WindsAloft ToWindsAloftForecast()
        {
            return new WindsAloft
            {
                UpdatedOn = DateTime.SpecifyKind(UpdatedOn, DateTimeKind.Utc),
                
                ValidFrom = DateTime.SpecifyKind(ValidFrom, DateTimeKind.Utc),

                ValidTo = DateTime.SpecifyKind(ValidTo, DateTimeKind.Utc),

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
