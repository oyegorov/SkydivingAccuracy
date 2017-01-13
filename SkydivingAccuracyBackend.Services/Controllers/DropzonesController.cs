using System;
using System.Linq;
using System.Threading.Tasks;
using GeoCoordinatePortable;
using Microsoft.AspNetCore.Mvc;
using SkydivingAccuracyBackend.Data.Model;
using SkydivingAccuracyBackend.Services.BusinessLogic;

namespace SkydivingAccuracyBackend.Services.Controllers
{
    [Route("api/[controller]")]
    public class DropzonesController
    {
        [HttpGet("nearest")]
        public async Task<IActionResult> Get([FromQuery]double longitude, [FromQuery]double latitude)
        {
            double distance = double.MaxValue;
            var location = new GeoCoordinate(latitude, longitude);
            Dropzone closestDropzone = null;

            foreach (var dropzone in Dropzones.GetAll())
            {
                if (dropzone.Latitude == null || dropzone.Longitude == null)
                    continue;

                var dropzoneLocation = new GeoCoordinate(dropzone.Latitude.Value, dropzone.Longitude.Value);

                double currentDistance = dropzoneLocation.GetDistanceTo(location);
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    closestDropzone = dropzone;
                }
            }

            return new OkObjectResult(closestDropzone);
        }

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery]string name, [FromQuery]int? take)
        {
            if (String.IsNullOrEmpty(name) || name.Length < 3)
                return new EmptyResult();

            var matchingDropzones =
                Dropzones.GetAll().Where(d => d.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) != -1);
            if (take != null)
                matchingDropzones = matchingDropzones.Take(take.Value);

            return new OkObjectResult(matchingDropzones);
        }
    }
}
