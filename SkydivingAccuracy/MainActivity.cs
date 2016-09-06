using Android.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;

namespace SkydivingAccuracy
{
    [Activity(Label = "SkydivingAccuracy", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            MapFragment mapFrag = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map);
            GoogleMap map = mapFrag.Map;
            if (map != null)
            {
                InitializeMap(map);
            }
        }

        private static void InitializeMap(GoogleMap map)
        {
            map.MapType = GoogleMap.MapTypeSatellite;


            LatLng location = new LatLng(44.237340, -79.640241);
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(location);
            builder.Zoom(18);
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

