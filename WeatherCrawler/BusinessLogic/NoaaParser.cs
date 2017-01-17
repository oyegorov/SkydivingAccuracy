using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SkydivingAccuracyBackend.Data.Model;
using WeatherCrawler.Data;

namespace WeatherCrawler.BusinessLogic
{
    public static class NoaaParser
    {
        private static readonly Regex RawDataRegex = new Regex(@"\<pre\>(.*?)\<\/pre\>", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex BasedOnRegex = new Regex(@"DATA BASED ON (?<basedOn>\d{2})", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex ForUseRegex = new Regex(@"FOR USE (?<validFrom>\d\d)\d\d-(?<validTo>\d\d)\d\dZ", RegexOptions.Singleline | RegexOptions.Compiled);
        
        public static List<WindsAloft> ParseWeatherHtml(string weatherHtml, DateTime updatedOn)
        {
            List<WindsAloft> result = new List<WindsAloft>();

            DateTime now = DateTime.UtcNow;
            string rawData = RawDataRegex.Match(weatherHtml).Groups[1].Value;

            var currentlyParsing = CurrentlyParsing.BasedOnInfo;
            using (StringReader sr = new StringReader(rawData))
            {
                string line;

                DateTime? basedOnDateTime = null;
                DateTime? validFromDate = null;
                DateTime? validToDate = null;
                while ((line = sr.ReadLine()) != null)
                {
                    switch (currentlyParsing)
                    {
                        case CurrentlyParsing.BasedOnInfo:
                            var m = BasedOnRegex.Match(line);
                            if (m.Success)
                            {
                                int basedOnDay = Int32.Parse(m.Groups["basedOn"].Value);
                                basedOnDateTime = (basedOnDay == now.Day)
                                    ? now.Date
                                    : now.Date.AddDays(-1);
                                currentlyParsing = CurrentlyParsing.Validity;
                            }
                            break;
                        case CurrentlyParsing.Validity:
                            m = ForUseRegex.Match(line);

                            if (m.Success)
                            {
                                int validFrom = Int32.Parse(m.Groups["validFrom"].Value);
                                int validTo = Int32.Parse(m.Groups["validTo"].Value);

                                validFromDate = basedOnDateTime.Value.AddHours(validFrom);
                                validToDate = basedOnDateTime.Value.AddHours(validTo);
                                if (validTo < validFrom)
                                    validToDate = validToDate.Value.AddDays(1);

                                currentlyParsing = CurrentlyParsing.Entries;
                            }
                            break;
                        case CurrentlyParsing.Entries:
                            if (line.Length < 69)
                                continue;

                            string airportCode = line.Substring(0, 3);
                            var airport = Airports.GetByCode(airportCode);
                            if (airport == null)
                                continue;

                            string weatherAt3000 = line.Substring(4, 4);
                            string weatherAt6000 = line.Substring(9, 7);
                            string weatherAt9000 = line.Substring(17, 7);
                            string weatherAt12000 = line.Substring(25, 7);
                            string weatherAt18000 = line.Substring(33, 7);

                            WindsAloft windsAloft = new WindsAloft()
                            {
                                Airport = airport,
                                UpdatedOn = updatedOn,
                                Source = "aviationweather.gov",
                                ValidFrom = validFromDate.Value,
                                ValidTo = validToDate.Value,
                                WindsAloftRecords = new[]
                                {
                                    FdUtils.DecodeFdInfo(3000, weatherAt3000),
                                    FdUtils.DecodeFdInfo(6000, weatherAt6000),
                                    FdUtils.DecodeFdInfo(9000, weatherAt9000),
                                    FdUtils.DecodeFdInfo(12000, weatherAt12000),
                                    FdUtils.DecodeFdInfo(18000, weatherAt18000)
                                }
                            };
                            result.Add(windsAloft);

                            break;
                    }

                }
            }

            return result;
        }

        enum CurrentlyParsing
        {
            BasedOnInfo, Validity, Entries
        }
    }
}
