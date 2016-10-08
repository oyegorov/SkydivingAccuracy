namespace SkydivingAccuracyBackend.Data.Model
{
    public class Forecast
    {
        public string Description { get; set; }

        public AltitudeForecast[] AltitudeForecasts { get; set; }
    }
}