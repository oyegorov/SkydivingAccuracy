using System;

namespace SkydivingAccuracyBackend.Data.Model
{
    public class Weather
    {
        public DateTime RequestedOn { get; set; }

        public WindsAloft WindsAloft { get; set; }

        public GroundWeather GroundWeather { get; set; }
    }
}
