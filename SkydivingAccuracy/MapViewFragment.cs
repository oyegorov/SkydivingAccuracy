using Android.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;

namespace SkydivingAccuracy
{
    public class MapViewFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var mapViewLayout = inflater.Inflate(Resource.Layout.MapView, container, false);

            MapView mapFragment = mapViewLayout.FindViewById<MapView>(Resource.Id.mapControl);
            mapFragment.OnCreate(savedInstanceState);
            mapFragment.OnResume();

            MapsInitializer.Initialize(Activity.ApplicationContext);

            GoogleMap map = mapFragment.Map;
            if (map != null)
            {
                InitializeMap(map);
            }

            return mapViewLayout;
        }

        private static void InitializeMap(GoogleMap map)
        {
            map.MapType = GoogleMap.MapTypeSatellite;
            
            LatLng location = new LatLng(44.237340, -79.640241);
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(location);
            builder.Zoom(16);
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            map.MoveCamera(cameraUpdate);

            MarkerOptions tuffetOptions = new MarkerOptions();
            tuffetOptions.SetPosition(new LatLng(44.237340, -79.640241));
            tuffetOptions.SetTitle("Tuffet!");
            map.AddMarker(tuffetOptions);
        }
    }
}