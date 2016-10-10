namespace SkydivingAccuracyBackend.Data.Model
{
    public class WindsAloftRecord
    {
        public int Altitude { get; set; }

        public int? WindSpeed { get; set; }

        public int? WindHeading { get; set; }

        public int? Temperature { get; set; }
    }
}