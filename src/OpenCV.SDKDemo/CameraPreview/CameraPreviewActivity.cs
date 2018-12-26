using OpenCV.SDKDemo.Utilities;
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


namespace OpenCV.SDKDemo.CameraPreview
{
    [Activity(Label = ActivityTags.CameraPreview)]
    public class CameraPreviewActivity : Activity, IOnMapReadyCallback, ILocationListener, GoogleMap.IOnInfoWindowClickListener
    {
        GoogleMap map;
        Spinner spinner;
        LocationManager locationManager;
        String provider;

        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;
            //MarkerOptions markerOptions = new MarkerOptions();
            //markerOptions.SetPosition(new LatLng(16.03, 108));
            //markerOptions.SetTitle("My Position");
            //googleMap.AddMarker(markerOptions);

            //Optional
            googleMap.UiSettings.ZoomControlsEnabled = true;
            googleMap.UiSettings.CompassEnabled = true;
            googleMap.MoveCamera(CameraUpdateFactory.ZoomIn());
            googleMap.UiSettings.MyLocationButtonEnabled = true;
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.CameraPreview);
            spinner = FindViewById<Spinner>(Resource.Id.spinner);
            MapFragment mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

            spinner.ItemSelected += Spinner_ItemSelected;

            spinner.ItemSelected += Spinner_ItemSelected;

            locationManager = (LocationManager)GetSystemService(Context.LocationService);
            provider = locationManager.GetBestProvider(new Criteria(), false);

            Location location = locationManager.GetLastKnownLocation(provider);
            if (location == null)
                System.Diagnostics.Debug.WriteLine("No Location");
        }

        private void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            switch (e.Position)
            {
                case 0: //Hybird
                    map.MapType = GoogleMap.MapTypeHybrid;
                    break;
                case 1: //None
                    map.MapType = GoogleMap.MapTypeNone;
                    break;
                case 2: //Normal
                    map.MapType = GoogleMap.MapTypeNormal;
                    break;
                case 3: //Statellite
                    map.MapType = GoogleMap.MapTypeSatellite;
                    break;
                case 4: //Terrain
                    map.MapType = GoogleMap.MapTypeTerrain;
                    break;
                default:
                    map.MapType = GoogleMap.MapTypeNone;
                    break;
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            locationManager.RequestLocationUpdates(provider, 400, 1, this);
        }

        protected override void OnPause()
        {
            base.OnPause();
            locationManager.RemoveUpdates(this);
        }

        public void OnLocationChanged(Location location)
        {
            Double lat, lng;
            lat = location.Latitude;
            lng = location.Longitude;

            MarkerOptions makerOptions = new MarkerOptions();
            makerOptions.SetPosition(new LatLng(lat, lng));
            makerOptions.SetTitle("My Position");
            map.AddMarker(makerOptions);

            //Move Camera
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(new LatLng(lat, lng));
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            map.MoveCamera(cameraUpdate);
            map.MyLocationEnabled = true;
        }

        public void OnProviderDisabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            throw new NotImplementedException();
        }

        public void OnInfoWindowClick(Marker marker)
        {
            Toast.MakeText(this, $"Icon {marker.Title} is clicked", ToastLength.Short).Show();
        }
    }
}


