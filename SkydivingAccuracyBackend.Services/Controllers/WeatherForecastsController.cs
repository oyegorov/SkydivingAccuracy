using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SkydivingAccuracyBackend.Data.DataAccess;
using SkydivingAccuracyBackend.Data.Model;

namespace SkydivingAccuracyBackend.Services.Controllers
{
    [Route("api/[controller]")]
    public class WeatherForecastsController : Controller
    {
        [HttpGet]
        public IActionResult Get([FromQuery]double longitude, [FromQuery]double latitude)
        {
            DateTime requestedDateTime = DateTime.UtcNow;

            using (var db = new SkydivingAccuracyDbContext(Startup.Configuration["DbFilePath"]))
            {
                double distance = double.MaxValue;

                var now = DateTime.UtcNow;

                WindsAloftForecastDto closestWindsAloftForecastDto = null;

                foreach (var windsAloftDto in db.WindsAloft.Where(w => w.ValidFrom <= now && now < w.ValidTo))
                {
                    double currentDistance = HaversineDistance(latitude, longitude, windsAloftDto.Latitude, windsAloftDto.Longitude);
                    if (currentDistance < distance)
                    {
                        distance = currentDistance;
                        closestWindsAloftForecastDto = windsAloftDto;
                    }
                }
                
                var weather = new Weather
                {
                    RequestedOn = requestedDateTime,
                    WindsAloftForecast = closestWindsAloftForecastDto == null ? null : closestWindsAloftForecastDto.ToWindsAloftForecast()
                };

                return new OkObjectResult(weather);
            }
        }
        
        private static double HaversineDistance(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            const double R = 6371;
            var lat = ConvertToRadians(latitude1 - latitude2);
            var lng = ConvertToRadians(longitude1 - longitude2);
            var h1 = Math.Sin(lat / 2) * Math.Sin(lat / 2) +
                          Math.Cos(ConvertToRadians(latitude1)) * Math.Cos(ConvertToRadians(latitude2)) *
                          Math.Sin(lng / 2) * Math.Sin(lng / 2);
            var h2 = 2 * Math.Asin(Math.Min(1, Math.Sqrt(h1)));
            return R * h2;
        }

        private static double ConvertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }
    }
}
