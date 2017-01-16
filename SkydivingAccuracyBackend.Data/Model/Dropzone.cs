using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkydivingAccuracyBackend.Data.Model
{
    public class Dropzone
    {
        public string Name { get; set; }

        public string Location { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Url { get; set; }
    }
}
