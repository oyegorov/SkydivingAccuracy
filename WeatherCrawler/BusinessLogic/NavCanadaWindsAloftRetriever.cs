using System;
using System.Collections.Generic;
using System.Net.Http;
using SkydivingAccuracyBackend.Data.Model;

namespace WeatherCrawler.BusinessLogic
{
    public class NavCanadaWindsAloftRetriever : IWindsAloftRetriever
    {
        public List<WindsAloft> GetWindsAloft()
        {
            HttpClient httpClient = new HttpClient();

            Dictionary<string, string> parameters = new Dictionary<string, string>()
                {
                    { "fd_text", "both" },
                    { "Langue", "anglais" },
                    { "NoSession", "NS_Inconnu" },
                    { "Region", "33" }
                };

            var response =
                httpClient.PostAsync("https://flightplanning.navcanada.ca/cgi-bin/Fore-obs/fd.cgi",
                    new FormUrlEncodedContent(parameters)).Result;
            string responseData = response.Content.ReadAsStringAsync().Result;

            var windsAloftForecasts = NavCanadaParser.ParseWeatherHtml(responseData);
            DateTime updatedOn = DateTime.Now.ToUniversalTime();
            foreach (var windsAloftForecast in windsAloftForecasts)
                windsAloftForecast.UpdatedOn = updatedOn;

            return windsAloftForecasts;
        }
    }
}
