using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using GeoCoordinatePortable;
using Microsoft.ApplicationInsights.AspNetCore;
using SkydivingAccuracyBackend.Data.Model;

namespace SkydivingAccuracyBackend.Services.BusinessLogic
{
    public static class MetarStations
    {
        private static readonly List<MetarStation> AllStations = new List<MetarStation>();

        public static void Load()
        {
            var assembly = Assembly.GetEntryAssembly();

            using (var reader = new StreamReader(assembly.GetManifestResourceStream("SkydivingAccuracyBackend.Services.MetarStations.csv")))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("#") || string.IsNullOrEmpty(line)) continue;

                    var fields = line.Split(',');

                    AllStations.Add(new MetarStation
                    {
                        IcaoCode = fields[0],
                        Latitude = Convert.ToDouble(fields[1]),
                        Longitude = Convert.ToDouble(fields[2]),
                        Elevation = Convert.ToDouble(fields[3]),
                        Country = fields[4],
                        Region = fields[5],
                        City = fields[6]
                    });
                }
            }
        }

        public static MetarStation GetClosestStation(GeoCoordinate location, out double distance)
        {
            MetarStation closestStation = null;
            distance = double.MaxValue;
            foreach (var metarStation in AllStations)
            {
                double currentDistance = location.GetDistanceTo(new GeoCoordinate(metarStation.Latitude, metarStation.Longitude));
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    closestStation = metarStation;
                }
            }

            return closestStation;
        }

        public static async Task<GroundWeather> GetGroundForecast(MetarStation metarStation)
        {
            if (metarStation == null)
                throw new ArgumentNullException(nameof(metarStation));

            string lookupUrl = $"https://www.aviationweather.gov/adds/dataserver_current/httpparam?dataSource=metars&requestType=retrieve&format=xml&stationString={metarStation.IcaoCode}&hoursBeforeNow=24";

            HttpClient client = new HttpClient();
            
            var response = await client.GetAsync(lookupUrl);
            string payload = await response.Content.ReadAsStringAsync();

            var document = XDocument.Parse(payload);
                      
            var temp = Convert.ToDouble(XmlTools.GetElementContent("temp_c", document, "data", "METAR"));
            var windSpeed = Convert.ToDouble(XmlTools.GetElementContent("wind_speed_kt", document, "data", "METAR"));
            var windGust = Convert.ToDouble(XmlTools.GetElementContent("wind_gust_kt", document, "data", "METAR"));
            var pressure =
                Convert.ToDouble(XmlTools.GetElementContent("altim_in_hg", document, "data", "METAR")) / 0.0393700732914;
            var windHeading = Convert.ToInt32(XmlTools.GetElementContent("wind_dir_degrees", document, "data", "METAR"));
            var dewpoint = Convert.ToDouble(XmlTools.GetElementContent("dewpoint_c", document, "data", "METAR"));
            var visibility =
                Convert.ToDouble(XmlTools.GetElementContent("visibility_statute_mi", document, "data", "METAR")) * 0.62137119;

            var skyConditionElements = XmlTools.GetElements("sky_condition", document, "data", "METAR");
            var skyConditions =
                skyConditionElements.Select(
                    s => new SkyCondition(s.Attribute("cloud_base_ft_agl")?.Value, s.Attribute("sky_cover")?.Value)).ToArray();

            var groundForecast = new GroundWeather
            {
                MetarStation = metarStation,
                Temperature = (int)temp,
                Dewpoint = (int)dewpoint,
                WindSpeed = (int)windSpeed,
                WindGust = (int)windGust,
                WindHeading = windHeading,
                Pressure = pressure,
                Visibility = visibility,
                SkyConditions = skyConditions
            };
                
            return groundForecast;
        }

        private static class XmlTools
        {
            public static IEnumerable<XElement> GetElements(string targetElement, XDocument doc, params string[] elements)
            {
                return GetNode(doc, elements).Elements(targetElement);
            }

            public static string GetElementContent(string targetElement, XDocument doc, params string[] elements)
            {
                var content = GetNode(doc, elements).Element(targetElement)?.Value;
                return content;
            }

            public static XElement GetNode(XDocument doc, params string[] elements)
            {
                var node = doc.Root;
                node = elements.Aggregate(node, (current, value) => current.Element(value));
                return node;
            }
        }
    }
}
