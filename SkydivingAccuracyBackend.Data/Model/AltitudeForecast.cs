namespace SkydivingAccuracyBackend.Data.Model
{
    public class AltitudeForecast
    {
        public int Altitude { get; set; }

        public int? Knots { get; set; }

        public int? Direction { get; set; }

        public int? Temperature { get; set; }
    }
}