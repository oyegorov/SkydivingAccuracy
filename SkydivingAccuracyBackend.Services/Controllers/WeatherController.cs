using System;
using System.Linq;
using System.Threading.Tasks;
using GeoCoordinatePortable;
using Microsoft.AspNetCore.Mvc;
using SkydivingAccuracyBackend.Data.DataAccess;
using SkydivingAccuracyBackend.Data.Model;
using SkydivingAccuracyBackend.Services.BusinessLogic;

namespace SkydivingAccuracyBackend.Services.Controllers
{
    [Route("api/[controller]")]
    public class WeatherController : Controller
    {
        private const int CutoffDistanceInKms = 800;

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery]double longitude, [FromQuery]double latitude)
        {
            DateTime requestedDateTime = DateTime.UtcNow;

            var location = new GeoCoordinate(latitude, longitude);

            var windsAloftForecast = await GetWindsAloft(location);
            var groundForecast = await GetGroundWeather(location);

            return new OkObjectResult(new Weather
            {
                RequestedOn = requestedDateTime,
                WindsAloft = windsAloftForecast,
                GroundWeather = groundForecast
            });
        }

        private async Task<GroundWeather> GetGroundWeather(GeoCoordinate location)
        {
            double distance;
            GroundWeather groundForecast = null;
            do
            {
                var metarStation = MetarStations.GetClosestStation(location, out distance);

                if (distance > CutoffDistanceInKms * 1000)
                    return null;

                groundForecast = await MetarStations.GetGroundForecast(metarStation);

                if (groundForecast == null)
                    metarStation.ForecastUnavailable = true;
            }
            while (groundForecast == null);

            groundForecast.DistanceToStation = (int)distance / 1000;

            return groundForecast;
        }

        private Task<WindsAloft> GetWindsAloft(GeoCoordinate location)
        {
            using (var db = new SkydivingAccuracyDbContext(Startup.DbFileName))
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
                    else if (closestWindsAloftForecastDto != null &&
                             windsAloftDto.AirportCode == closestWindsAloftForecastDto.AirportCode &&
                             windsAloftDto.UpdatedOn > closestWindsAloftForecastDto.UpdatedOn)
                    {
                        closestWindsAloftForecastDto = windsAloftDto;
                    }
                }

                if (distance > CutoffDistanceInKms * 1000)
                    return Task.FromResult<WindsAloft>(null);

                var windsAloftForecast = closestWindsAloftForecastDto?.ToWindsAloftForecast();

                return Task.FromResult(windsAloftForecast);
            }
        }
    }
}
