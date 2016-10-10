using System;
using System.Collections.Generic;
using System.Net.Http;
using SkydivingAccuracyBackend.Data.Model;

namespace WeatherCrawler.BusinessLogic
{
    public class NavCanadaWindsAloftRetriever : IWindsAloftRetriever
    {
        private const string NavCanadaRetrievalUrl = "https://flightplanning.navcanada.ca/cgi-bin/Fore-obs/fd.cgi";

        private static readonly Dictionary<string, string> RegionCodes = new Dictionary<string, string>
        {
            { "Pacific", "31" },
            { "Prairies", "32" },
            { "OntarioQuebec", "33" },
            { "Atlantic", "34" }
        };

        public List<WindsAloft> GetWindsAloft()
        {
            HttpClient httpClient = new HttpClient();

            List<WindsAloft> canadaWideWindsAloft = new List<WindsAloft>();

            Dictionary<string, string> parameters = new Dictionary<string, string>()
                {
                    { "fd_text", "both" },
                    { "Langue", "anglais" },
                    { "NoSession", "NS_Inconnu" },
                };
            
            foreach (var regionCode in RegionCodes.Values)
            {
                parameters["Region"] = regionCode;

                var response = httpClient.PostAsync(NavCanadaRetrievalUrl,
                   new FormUrlEncodedContent(parameters)).Result;
                string responseData = response.Content.ReadAsStringAsync().Result;

                DateTime updatedOn = DateTime.Now.ToUniversalTime();
                var regionalWindsAloft = NavCanadaParser.ParseWeatherHtml(responseData, updatedOn);

                canadaWideWindsAloft.AddRange(regionalWindsAloft);
            }
           

            return canadaWideWindsAloft;
        }
    }
}
