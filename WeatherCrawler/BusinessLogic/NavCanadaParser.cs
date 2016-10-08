using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using SkydivingAccuracyBackend.Data.Model;

namespace WeatherCrawler.BusinessLogic
{
    internal static class NavCanadaParser
    {
        private const string HighLevelFd = "High Level FD";
        private static readonly Regex TableRegex = new Regex(@"\<table(.*?)\<\/table\>", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex BasedOnRegex = new Regex(@"BASED ON (?<basedOn>\d\d)", RegexOptions.Singleline | RegexOptions.Compiled);

        public static List<Weather> ParseWeatherHtml(string weatherHtml)
        {
            DateTime now = DateTime.Now;
            List<Weather> result = new List<Weather>();

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

                Weather weather = new Weather();
                weather.Location = airport;

                bool nextDay = false;
                foreach (var trEntry in trEntries)
                {
                    var tdEntries = trEntry.XPathSelectElements(".//td").ToArray();
                    if (tdEntries.Length != 7)
                        continue;

                    string description = tdEntries[0].Element("font").Value;

                    string timeRange = tdEntries[1].Element("font").Element("b").Value;
                    var timeRangeTokens = timeRange.Split('-');
                    if (timeRangeTokens.Length != 2)
                        continue;

                    int basedOnDay = Int32.Parse(BasedOnRegex.Match(description).Groups["basedOn"].Value);
                    DateTime baseDay = (basedOnDay == now.Day) ? now.Date : now.Date.AddDays(-1);

                    int validFrom = ((DateTime.Now - DateTime.UtcNow).Hours + Int32.Parse(timeRangeTokens[0]) + 24) % 24;
                    int validTo = ((DateTime.Now - DateTime.UtcNow).Hours + Int32.Parse(timeRangeTokens[1]) + 24) % 24;

                    DateTime validFromDate = baseDay.AddHours(validFrom);
                    DateTime validToDate = baseDay.AddHours(validTo);

                    if (nextDay)
                    {
                        validFromDate = validToDate.AddDays(1);
                        validToDate = validToDate.AddDays(1);
                    }

                    if (validTo < validFrom)
                    {
                        validToDate = validToDate.AddDays(1);
                        nextDay = true;
                    }

                    if (!(validFromDate < now && now < validToDate))
                        continue;

                    weather.ActiveForecast = new Forecast();
                    weather.ActiveForecast.Description = description;

                    weather.ActiveForecast.AltitudeForecasts = new AltitudeForecast[]
                    {
                        DecodeFdInfo(3000, tdEntries[2].Element("font").Value),
                        DecodeFdInfo(6000, tdEntries[3].Element("font").Value),
                        DecodeFdInfo(9000, tdEntries[4].Element("font").Value),
                        DecodeFdInfo(12000, tdEntries[5].Element("font").Value),
                        DecodeFdInfo(18000, tdEntries[6].Element("font").Value)
                    };
                }

                result.Add(weather);
            }

            return result;
        }

        private static AltitudeForecast DecodeFdInfo(int altitude, string fd)
        {
            AltitudeForecast altitudeForecast = new AltitudeForecast();

            altitudeForecast.Altitude = altitude;

            if (String.IsNullOrWhiteSpace(fd))
                return altitudeForecast;

            if (fd.Length > 4)
            {
                string temperatureInfo = fd.Substring(4);
                altitudeForecast.Temperature = Int32.Parse(temperatureInfo);
            }

            fd = fd.Substring(0, 4);

            if (fd == "9900")
                return altitudeForecast;

            int fdAngle = Int32.Parse(fd.Substring(0, 2));
            int fdKnots = Int32.Parse(fd.Substring(2, 2));

            if (fdAngle > 36)
            {
                altitudeForecast.Direction = fdAngle - 50;
                altitudeForecast.Knots = 100 + fdKnots;
            }
            else
            {
                altitudeForecast.Direction = fdAngle * 10;
                altitudeForecast.Knots = fdKnots;
            }

            return altitudeForecast;
        }

        private static string GetAirportName(string airportData)
        {
            return airportData.Split('-').Last().Trim();
        }
    }
}
