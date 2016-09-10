using System;
using Android.App;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;

namespace SkydivingAccuracy
{
    public class ActionBarDrawerEventArgs : EventArgs
    {
        public View DrawerView { get; set; }
        public float SlideOffset { get; set; }
        public int NewState { get; set; }
    }

    public delegate void ActionBarDrawerChangedEventHandler(object s, ActionBarDrawerEventArgs e);

    public class ActionBarDrawerToggleImplementation : ActionBarDrawerToggle
    {
        public event ActionBarDrawerChangedEventHandler DrawerClosed;
        public event ActionBarDrawerChangedEventHandler DrawerOpened;
        public event ActionBarDrawerChangedEventHandler DrawerSlide;
        public event ActionBarDrawerChangedEventHandler DrawerStateChanged;

        public ActionBarDrawerToggleImplementation(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public ActionBarDrawerToggleImplementation(Activity activity, DrawerLayout drawerLayout, int drawerImageRes, int openDrawerContentDescRes, int closeDrawerContentDescRes) : base(activity, drawerLayout, drawerImageRes, openDrawerContentDescRes, closeDrawerContentDescRes)
        {
        }

        public ActionBarDrawerToggleImplementation(Activity activity, DrawerLayout drawerLayout, bool animate, int drawerImageRes, int openDrawerContentDescRes, int closeDrawerContentDescRes) : base(activity, drawerLayout, animate, drawerImageRes, openDrawerContentDescRes, closeDrawerContentDescRes)
        {
        }

        public override void OnDrawerClosed(View drawerView)
        {
            if (DrawerClosed != null)
                DrawerClosed(this, new ActionBarDrawerEventArgs { DrawerView = drawerView });
            base.OnDrawerClosed(drawerView);
        }

        public override void OnDrawerOpened(View drawerView)
        {
            if (DrawerOpened != null)
                DrawerOpened(this, new ActionBarDrawerEventArgs { DrawerView = drawerView });
            base.OnDrawerOpened(drawerView);
        }

        public override void OnDrawerSlide(View drawerView, float slideOffset)
        {
            if (DrawerSlide != null)
                DrawerSlide(this, new ActionBarDrawerEventArgs
                {
                    DrawerView = drawerView,
                    SlideOffset = slideOffset
                });
            base.OnDrawerSlide(drawerView, slideOffset);
        }

        public override void OnDrawerStateChanged(int newState)
        {
            if (DrawerStateChanged != null)
                DrawerStateChanged(this, new ActionBarDrawerEventArgs
                {
                    NewState = newState
                });
            base.OnDrawerStateChanged(newState);
        }
    }
}