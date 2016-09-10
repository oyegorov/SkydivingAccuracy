using Android.App;
using Android.Content.Res;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;

namespace SkydivingAccuracy
{
    [Activity(Label = "Skydiving Accuracy Aid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private MapViewFragment _mapViewFragment;
        private ForecastViewFragment _forecastViewFragment;

        private DrawerLayout _drawer;
        private ActionBarDrawerToggleImplementation _drawerToggle;
        private ListView _drawerList;

        private string _drawerTitle;
        private string _title;
        private string[] _menuItems = { "Landing Map", "Weather" };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            _mapViewFragment = new MapViewFragment();
            _forecastViewFragment = new ForecastViewFragment();

            _title = _drawerTitle = Title;
            _drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            _drawerList = FindViewById<ListView>(Resource.Id.left_drawer);

            _drawer.SetDrawerShadow(Resource.Drawable.drawer_shadow_dark, (int)GravityFlags.Start);

            _drawerList.Adapter = new ArrayAdapter<string>(this, Resource.Layout.DrawerListItem, _menuItems);
            _drawerList.ItemClick += (sender, args) => SelectItem(args.Position);


            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetHomeButtonEnabled(true);

            _drawerToggle = new ActionBarDrawerToggleImplementation(this, _drawer,
                                                      Resource.Drawable.ic_drawer_light,
                                                      Resource.String.DrawerOpen,
                                                      Resource.String.DrawerClose);

            _drawerToggle.DrawerClosed += delegate
            {
                ActionBar.Title = _title;
                InvalidateOptionsMenu();
            };

            _drawerToggle.DrawerOpened += delegate
            {
                ActionBar.Title = _drawerTitle;
                InvalidateOptionsMenu();
            };

            _drawer.SetDrawerListener(_drawerToggle);

            if (null == savedInstanceState)
                SelectItem(0);
        }

        private void SelectItem(int position)
        {
            Fragment fragment = (position == 0) ? (Fragment)_mapViewFragment : _forecastViewFragment;
            
            FragmentManager.BeginTransaction()
                .Replace(Resource.Id.content_frame, fragment)
                .Commit();

            _drawerList.SetItemChecked(position, true);
            ActionBar.Title = _title = _menuItems[position];
            _drawer.CloseDrawer(_drawerList);
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            _drawerToggle.SyncState();
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            _drawerToggle.OnConfigurationChanged(newConfig);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            // MenuInflater.Inflate(Resource.m.main, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            var drawerOpen = _drawer.IsDrawerOpen(Resource.Id.left_drawer);
            return base.OnPrepareOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (_drawerToggle.OnOptionsItemSelected(item))
                return true;

            return base.OnOptionsItemSelected(item);
        }
    }
}

