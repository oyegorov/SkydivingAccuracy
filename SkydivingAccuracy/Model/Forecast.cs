using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SkydivingAccuracy.Model
{
    public class Forecast
    {
        public string Description { get; set; }

        public int ValidFrom { get; set; }

        public int ValidTo { get; set; }

        public AltitudeWeatherInfo[] AltitudeWeatherInfos { get; set; }
    }
}