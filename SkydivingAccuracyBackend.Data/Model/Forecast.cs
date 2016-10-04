namespace SkydivingAccuracyBackend.Data.Model
{
    public class Forecast
    {
        public string Description { get; set; }

        public int ValidFrom { get; set; }

        public int ValidTo { get; set; }

        public AltitudeForecast[] AltitudeForecasts { get; set; }
    }
}