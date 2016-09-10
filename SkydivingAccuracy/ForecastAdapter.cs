using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using SkydivingAccuracy.Model;
using Object = Java.Lang.Object;

namespace SkydivingAccuracy
{
    class ForecastAdapter : BaseAdapter
    {
        private readonly Activity _activity;
        private readonly List<Forecast> _forecasts;

        public ForecastAdapter(Activity activity, List<Forecast> forecasts)
        {
            _activity = activity;
            _forecasts = forecasts;
        }

        public override Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? _activity.LayoutInflater.Inflate(Resource.Layout.ForecastListItem, parent, false);
            var forecast = _forecasts[position];

            var forUse = view.FindViewById<TextView>(Resource.Id.forUse);
            forUse.Text = $"For use: {forecast.ValidFrom}.00 - {forecast.ValidTo}.00";

            var description = view.FindViewById<TextView>(Resource.Id.forecastDescription);
            description.Text = forecast.Description;

            foreach (var altitudeWeatherInfo in forecast.AltitudeWeatherInfos)
            {
                var wind = view.FindViewById<TextView>(GetResourceId("wind", altitudeWeatherInfo.Altitude));
                wind.Text = altitudeWeatherInfo.Knots == null ? "Calm" : $"{altitudeWeatherInfo.Knots} knots";

                var az = view.FindViewById<TextView>(GetResourceId("az", altitudeWeatherInfo.Altitude));
                az.Text = altitudeWeatherInfo.Direction == null ? "(no direction)" : $"{altitudeWeatherInfo.Direction}°";

                var temp = view.FindViewById<TextView>(GetResourceId("temp", altitudeWeatherInfo.Altitude));
                temp.Text = altitudeWeatherInfo.Temperature == null ? "" : $"{altitudeWeatherInfo.Temperature}° C";
            }
           
            return view;
        }

        public override int Count
        {
            get { return _forecasts.Count; }
        }

        private int GetResourceId(string resourceName, int altitude)
        {
            return (int)typeof(Resource.Id).GetField(resourceName + altitude.ToString()).GetValue(null);
        }
    }
}