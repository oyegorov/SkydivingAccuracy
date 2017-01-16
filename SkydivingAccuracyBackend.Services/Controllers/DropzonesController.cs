using System;
using System.Collections.Generic;
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
        public async Task<IActionResult> Get([FromQuery]double? latitude, [FromQuery]double? longitude, [FromQuery] string name)
        {
            IEnumerable<Dropzone> dropzones = Dropzones.GetAll();
            if (name != null)
                dropzones = dropzones.Where(d => d.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) != -1);

            if (latitude != null && longitude != null)
                return new OkObjectResult(GetNearest(dropzones, latitude.Value, longitude.Value));

            return new OkObjectResult(dropzones.OrderBy(d => d.Name).FirstOrDefault());
        }

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery]string name, [FromQuery]int? take)
        {
            if (String.IsNullOrEmpty(name))
                return new EmptyResult();

            var matchingDropzones =
                Dropzones.GetAll().Where(d => d.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) != -1);
            if (take != null)
                matchingDropzones = matchingDropzones.Take(take.Value);

            return new OkObjectResult(matchingDropzones);
        }

        private Dropzone GetNearest(IEnumerable<Dropzone> dropzones, double latitude, double longitude)
        {
            double distance = double.MaxValue;
            var location = new GeoCoordinate(latitude, longitude);
            Dropzone closestDropzone = null;

            foreach (var dropzone in dropzones)
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

            return closestDropzone;
        }
    }
}
