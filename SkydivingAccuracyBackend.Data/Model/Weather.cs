using System;

namespace SkydivingAccuracyBackend.Data.Model
{
    public class Weather
    {
        public string Location { get; set; }
        
        public DateTime UpdatedOn { get; set; }

        public Forecast ActiveForecast { get; set; }
    }
}
