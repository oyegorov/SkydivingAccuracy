using System;

namespace SkydivingAccuracyBackend.Data.Model
{
    public class WindsAloftForecast
    {
        public DateTime UpdatedOn { get; set; }

        public Airport Airport { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }

        public WindsAloftRecord[] WindsAloftRecords { get; set; }
    }
}