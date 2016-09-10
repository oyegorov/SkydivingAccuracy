using Android.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Views;

namespace SkydivingAccuracy
{
    public class MapViewFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater p0, ViewGroup p1, Bundle p2)
        {
            var mapView = p0.Inflate(Resource.Layout.MapView, p1, false);

            MapFragment mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.mapControl);
            GoogleMap map = mapFragment.Map;
            if (map != null)
            {
                InitializeMap(map);
            }

            return mapView;
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