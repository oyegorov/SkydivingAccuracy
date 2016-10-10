using System;
using System.Linq;
using System.Threading.Tasks;
using GeoCoordinatePortable;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SkydivingAccuracyBackend.Data.DataAccess;
using SkydivingAccuracyBackend.Data.Model;
using SkydivingAccuracyBackend.Services.BusinessLogic;

namespace SkydivingAccuracyBackend.Services.Controllers
{
    [Route("api/[controller]")]
    public class WeatherForecastsController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]double longitude, [FromQuery]double latitude)
        {
            DateTime requestedDateTime = DateTime.UtcNow;

            var location = new GeoCoordinate(latitude, longitude);

            var windsAloftForecast = await GetWindsAloftForecast(location);
            var groundForecast = await GetGroundForecast(location);

            return new OkObjectResult(new Weather
            {
                RequestedOn = requestedDateTime,
                WindsAloft = windsAloftForecast,
                GroundWeather = groundForecast
            });
        }

        private async Task<GroundWeather> GetGroundForecast(GeoCoordinate location)
        {
            double distance;
            var metarStation = MetarStations.GetClosestStation(location, out distance);
            var groundForecast = await MetarStations.GetGroundForecast(metarStation);
            groundForecast.DistanceToStation = distance;

            return groundForecast;
        }

        private Task<WindsAloft> GetWindsAloftForecast(GeoCoordinate location)
        {
            using (var db = new SkydivingAccuracyDbContext(Startup.Configuration["DbFilePath"]))
            {
                double distance = double.MaxValue;

                var now = DateTime.UtcNow;

                WindsAloftForecastDto closestWindsAloftForecastDto = null;

                foreach (var windsAloftDto in db.WindsAloft.Where(w => w.ValidFrom <= now && now < w.ValidTo))
                {
                    var windsAloftLocation = new GeoCoordinate(windsAloftDto.Latitude, windsAloftDto.Longitude);

                    double currentDistance = windsAloftLocation.GetDistanceTo(location);
                    if (currentDistance < distance)
                    {
                        distance = currentDistance;
                        closestWindsAloftForecastDto = windsAloftDto;
                    }
                }

                var windsAloftForecast = closestWindsAloftForecastDto?.ToWindsAloftForecast();

                return Task.FromResult(windsAloftForecast);
            }
        }
    }
}
