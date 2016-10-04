using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SkydivingAccuracyBackend.Data.DataAccess;
using SkydivingAccuracyBackend.Data.Model;

namespace SkydivingAccuracyBackend.Services.Controllers
{
    [Route("api/[controller]")]
    public class WeatherForecastsController : Controller
    {
        [HttpGet("{location}")]
        public IActionResult Get(string location)
        {
            using (var db = new SkydivingAccuracyDbContext(Startup.Configuration["DbFilePath"]))
            {
                var weatherInfo = db.WeatherInfos.FirstOrDefault(w => w.Location.Equals(location, StringComparison.OrdinalIgnoreCase));
                if (weatherInfo == null)
                    return new NotFoundResult();

                return new OkObjectResult(weatherInfo.ToWeather());
            }
        }
    }
}
