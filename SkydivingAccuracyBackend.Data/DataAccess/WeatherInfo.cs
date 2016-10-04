using System;
using Newtonsoft.Json;
using SkydivingAccuracyBackend.Data.Model;

namespace SkydivingAccuracyBackend.Data.DataAccess
{
    public class WeatherInfo
    {
        public int WeatherInfoId { get; set; }

        public string Location { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string ForecastData { get; set; }

        public static WeatherInfo FromWeather(Weather weather)
        {
            if (weather == null)
                return null;

            return new WeatherInfo
            {
                Location = weather.Location,
                UpdatedOn = weather.UpdatedOn,
                ForecastData = JsonConvert.SerializeObject(weather.ActiveForecasts)
            };
        }

        public Weather ToWeather()
        {
            return new Weather
            {
                Location = Location,
                UpdatedOn = UpdatedOn,
                ActiveForecasts = JsonConvert.DeserializeObject<Forecast[]>(ForecastData)
            };
        }
    }
}
