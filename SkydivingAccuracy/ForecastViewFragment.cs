using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using SkydivingAccuracy.BusinessLogic;

namespace SkydivingAccuracy
{
    public class ForecastViewFragment : Fragment
    {
        private const string TorontoLocation = "TORONTO. ONT";
        private static WeatherHtmlParser _weatherHtmlParser;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.ForecastView, container, false);
        }

        public override void OnResume()
        {
            base.OnResume();

            LoadWeatherForecast();
        }

        private void LoadWeatherForecast()
        {
            var progress = ProgressDialog.Show(Activity, "Loading...", "Retrieving weather forecast...", true);

            Task.Factory.StartNew(() =>
            {
                HttpClient httpClient = new HttpClient();

                Dictionary<string, string> parameters = new Dictionary<string, string>()
                {
                    {"fd_text", "both"},
                    {"Langue", "anglais"},
                    {"NoSession", "NS_Inconnu"},
                    {"Region", "33"}
                };

                var response =
                    httpClient.PostAsync("https://flightplanning.navcanada.ca/cgi-bin/Fore-obs/fd.cgi",
                        new FormUrlEncodedContent(parameters)).Result;
                string responseData = response.Content.ReadAsStringAsync().Result;

                _weatherHtmlParser = new WeatherHtmlParser(responseData);

                progress.Dismiss();


                var forecastListView = Activity.FindViewById<ListView>(Resource.Id.forecastListView);
                var adapter = new ForecastAdapter(Activity, _weatherHtmlParser.GetForecasts(TorontoLocation));

                Activity.RunOnUiThread(() =>
                {
                    forecastListView.Adapter = adapter;
                });
            });
        }
    }
}