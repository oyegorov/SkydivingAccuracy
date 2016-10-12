using System;
using System.Collections.Generic;
using System.Net.Http;
using SkydivingAccuracyBackend.Data.Model;

namespace WeatherCrawler.BusinessLogic
{
    public class NoaaWindsAloftRetriever : IWindsAloftRetriever
    {
        const string NoaaUrlTemplate = "https://aviationweather.gov/windtemp/data?level=l&&region=all&layout=on&fcst={0}";

        public List<WindsAloft> GetWindsAloft()
        {
            DateTime now = DateTime.UtcNow;
            string[] fcsts = { "06", "12", "24" };

            HttpClient httpClient = new HttpClient();
            List<WindsAloft> result = new List<WindsAloft>();
            foreach (var fcst in fcsts)
            {
                var response = httpClient.GetAsync(String.Format(NoaaUrlTemplate, fcst)).Result;
                string responseData = response.Content.ReadAsStringAsync().Result;

                result.AddRange(NoaaParser.ParseWeatherHtml(responseData, now));
            }

            return result;
        }
    }
}
