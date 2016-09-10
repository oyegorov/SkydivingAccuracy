using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using SkydivingAccuracy.Model;

namespace SkydivingAccuracy.BusinessLogic
{
    internal class WeatherHtmlParser
    {
        private const string HighLevelFd = "High Level FD";
        private static Regex TableRegex = new Regex(@"\<table(.*?)\<\/table\>", RegexOptions.Singleline | RegexOptions.Compiled);
        private Dictionary<string, List<Forecast>> _forecasts;

        public WeatherHtmlParser(string weatherHtml)
        {
            _forecasts = ParseWeatherHtml(weatherHtml);
        }

        public string[] GetAirports()
        {
            return _forecasts.Keys.OrderBy(k => k).ToArray();
        }

        public List<Forecast> GetForecasts(string airport)
        {
            if (airport == null)
                throw new ArgumentNullException(nameof(airport));

            if (!_forecasts.ContainsKey(airport))
                throw new ArgumentException($"There is no forecast for the given airport: {airport}");

            return _forecasts[airport];
        }

        public List<Forecast> this[string airport] => GetForecasts(airport);

        private static Dictionary<string, List<Forecast>> ParseWeatherHtml(string weatherHtml)
        {
            Dictionary<string, List<Forecast>> result = new Dictionary<string, List<Forecast>>();

            var tableMatches = TableRegex.Matches(weatherHtml);

            foreach (Match tableMatch in tableMatches)
            {
                if (tableMatch.Value.IndexOf(HighLevelFd, StringComparison.OrdinalIgnoreCase) != -1)
                    break;

                var tableContent = tableMatch.Value.Replace("\"Arial\", Helvetica, sans-serif\"", "\"\"");

                XDocument tableDocument;
                try
                {
                    tableDocument = XDocument.Parse(tableContent);
                }
                catch
                {
                    continue;
                }

                var header = tableDocument.XPathSelectElement("//tr[position() = 1]");

                string airport = GetAirportName(header.XPathSelectElements(".//td").First().Value);

                var trEntries = tableDocument.XPathSelectElements("//tr[position()>1]");

                List<Forecast> forecasts = new List<Forecast>();
                foreach (var trEntry in trEntries)
                {
                    Forecast forecast = new Forecast();

                    var tdEntries = trEntry.XPathSelectElements(".//td").ToArray();
                    if (tdEntries.Length != 7)
                        continue;

                    forecast.Description = tdEntries[0].Element("font").Value;
                    string timeRange = tdEntries[1].Element("font").Element("b").Value;
                    var timeRangeTokens = timeRange.Split('-');
                    if (timeRangeTokens.Length != 2)
                        continue;

                    forecast.ValidFrom = ((DateTime.Now - DateTime.UtcNow).Hours + Int32.Parse(timeRangeTokens[0]) + 24) % 24;
                    forecast.ValidTo = ((DateTime.Now - DateTime.UtcNow).Hours + Int32.Parse(timeRangeTokens[1]) + 24) % 24;
                    forecast.AltitudeWeatherInfos = new AltitudeWeatherInfo[]
                    {
                        DecodeFdInfo(3000, tdEntries[2].Element("font").Value),
                        DecodeFdInfo(6000, tdEntries[3].Element("font").Value),
                        DecodeFdInfo(9000, tdEntries[4].Element("font").Value),
                        DecodeFdInfo(12000, tdEntries[5].Element("font").Value),
                        DecodeFdInfo(18000, tdEntries[6].Element("font").Value)
                    };

                    forecasts.Add(forecast);
                }

                result.Add(airport, forecasts);
            }

            return result;
        }

        private static AltitudeWeatherInfo DecodeFdInfo(int altitude, string fd)
        {
            AltitudeWeatherInfo altitudeWeatherInfo = new AltitudeWeatherInfo();

            altitudeWeatherInfo.Altitude = altitude;

            if (String.IsNullOrWhiteSpace(fd))
                return altitudeWeatherInfo;

            if (fd.Length > 4)
            {
                string temperatureInfo = fd.Substring(4);
                altitudeWeatherInfo.Temperature = Int32.Parse(temperatureInfo);
            }

            fd = fd.Substring(0, 4);

            if (fd == "9900")
                return altitudeWeatherInfo;

            int fdAngle = Int32.Parse(fd.Substring(0, 2));
            int fdKnots = Int32.Parse(fd.Substring(2, 2));

            if (fdAngle > 36)
            {
                altitudeWeatherInfo.Direction = fdAngle - 50;
                altitudeWeatherInfo.Knots = 100 + fdKnots;
            }
            else
            {
                altitudeWeatherInfo.Direction = fdAngle * 10;
                altitudeWeatherInfo.Knots = fdKnots;
            }

            return altitudeWeatherInfo;
        }

        private static string GetAirportName(string airportData)
        {
            return airportData.Split('-').Last().Trim();
        }
    }
}
