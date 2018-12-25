using Android.App;
using Android.Widget;
using Android.OS;
using Android.Gms.Maps;
using System;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.Runtime;
using Android.Content;
using Android.Graphics;

namespace XamarinGoogleMapDemo
{
    [Activity(Label = "XamarinGoogleMapDemo", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity,IOnMapReadyCallback
    {
        GoogleMap map;
        Spinner spinner;
        LocationManager locationManager;
        String provider;
        
         public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;

            //Optional
            googleMap.UiSettings.ZoomControlsEnabled = true;
            googleMap.UiSettings.CompassEnabled = true;
            googleMap.MoveCamera(CameraUpdateFactory.ZoomIn());
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            spinner = FindViewById<Spinner>(Resource.Id.spinner);
            MapFragment mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

        }
    }
}

