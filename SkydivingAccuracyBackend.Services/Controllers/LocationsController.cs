using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SkydivingAccuracyBackend.Data.DataAccess;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace SkydivingAccuracyBackend.Services.Controllers
{
    [Route("api/[controller]")]
    public class LocationsController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            using (var db = new SkydivingAccuracyDbContext(Startup.Configuration["DbFilePath"]))
            {
                var locations = db.WeatherInfos.Select(wi => wi.Location);
                return locations.ToArray();
            }
        }
    }
}
