namespace SkydivingAccuracyBackend.Data.Model
{
    public class MetarStation
    {
        public string IcaoCode { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double Elevation { get; set; }

        public string Country { get; set; }

        public string Region { get; set; }

        public string City { get; set; }
    }
}
