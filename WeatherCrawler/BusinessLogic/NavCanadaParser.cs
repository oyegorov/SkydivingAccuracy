using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using SkydivingAccuracyBackend.Data.Model;
using WeatherCrawler.Data;

namespace WeatherCrawler.BusinessLogic
{
    internal static class NavCanadaParser
    {
        private const string HighLevelFd = "High Level FD";
        private static readonly Regex TableRegex = new Regex(@"\<table(.*?)\<\/table\>", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex BasedOnRegex = new Regex(@"BASED ON (?<basedOn>\d\d)", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex AirportCodeRegex = new Regex(@"STN\n(?<airportCode>\w*)\s", RegexOptions.Singleline | RegexOptions.Compiled);

        public static List<WindsAloft> ParseWeatherHtml(string weatherHtml, DateTime updatedOn)
        {
            DateTime now = DateTime.UtcNow;
            List<WindsAloft> result = new List<WindsAloft>();

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

                var airport = GetAirport(header.XPathSelectElements(".//td").First().Value);
                if (airport == null)
                    continue;

                var trEntries = tableDocument.XPathSelectElements("//tr[position()>1]");

                bool nextDay = false;
                foreach (var trEntry in trEntries)
                {
                    WindsAloft windsAloft = new WindsAloft();
                    windsAloft.Airport = airport;

                    var tdEntries = trEntry.XPathSelectElements(".//td").ToArray();
                    if (tdEntries.Length != 7)
                        continue;

                    string description = tdEntries[0].Element("font").Value;

                    string timeRange = tdEntries[1].Element("font").Element("b").Value;
                    var timeRangeTokens = timeRange.Split('-');
                    if (timeRangeTokens.Length != 2)
                        continue;

                    int basedOnDay = Int32.Parse(BasedOnRegex.Match(description).Groups["basedOn"].Value);
                    DateTime baseDay = (basedOnDay == now.ToUniversalTime().Day) ? now.Date : now.Date.AddDays(-1);

                    int validFrom = Int32.Parse(timeRangeTokens[0]);
                    int validTo = Int32.Parse(timeRangeTokens[1]);

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

                    windsAloft.ValidFrom = validFromDate.ToUniversalTime();
                    windsAloft.ValidTo = validToDate.ToUniversalTime();
                    windsAloft.WindsAloftRecords = new WindsAloftRecord[]
                    {
                        FdUtils.DecodeFdInfo(3000, tdEntries[2].Element("font").Value),
                        FdUtils.DecodeFdInfo(6000, tdEntries[3].Element("font").Value),
                        FdUtils.DecodeFdInfo(9000, tdEntries[4].Element("font").Value),
                        FdUtils.DecodeFdInfo(12000, tdEntries[5].Element("font").Value),
                        FdUtils.DecodeFdInfo(18000, tdEntries[6].Element("font").Value)
                    };

                    windsAloft.UpdatedOn = updatedOn;

                    result.Add(windsAloft);
                }
            }

            return result;
        }

        private static Airport GetAirport(string airportData)
        {
            string airportCode = AirportCodeRegex.Match(airportData).Groups["airportCode"].Value;
            return Airports.GetByCode(airportCode);
        }
    }
}
