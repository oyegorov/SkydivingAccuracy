using Android.App;
using Android.OS;
using Android.Views;

namespace SkydivingAccuracy
{
    public class ForecastViewFragment : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.ForecastView, container, false);
        }
    }
}