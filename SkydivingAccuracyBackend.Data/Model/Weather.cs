using System;

namespace SkydivingAccuracyBackend.Data.Model
{
    public class Weather
    {
        public DateTime RequestedOn { get; set; }

        public WindsAloftForecast WindsAloftForecast { get; set; }
    }
}
