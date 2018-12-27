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
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace XamarinGoogleMapDemo
{
    [Activity(Label = "XamarinGoogleMapDemo", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity,IOnMapReadyCallback, ILocationListener, GoogleMap.IOnInfoWindowClickListener
    {
        GoogleMap map;
        Spinner spinner;
        LocationManager locationManager;
        String provider;

        //Giai ma code
        private List<LatLng> DecodePolyline(string encodedPoints)
        {
            if (string.IsNullOrWhiteSpace(encodedPoints))
            {
                return null;
            }

            int index = 0;
            var polylineChars = encodedPoints.ToCharArray();
            var poly = new List<LatLng>();
            int currentLat = 0;
            int currentLng = 0;
            int next5Bits;

            while (index < polylineChars.Length)
            {
                // calculate next latitude
                int sum = 0;
                int shifter = 0;

                do
                {
                    next5Bits = polylineChars[index++] - 63;
                    sum |= (next5Bits & 31) << shifter;
                    shifter += 5;
                }
                while (next5Bits >= 32 && index < polylineChars.Length);

                if (index >= polylineChars.Length)
                {
                    break;
                }

                currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                // calculate next longitude
                sum = 0;
                shifter = 0;

                do
                {
                    next5Bits = polylineChars[index++] - 63;
                    sum |= (next5Bits & 31) << shifter;
                    shifter += 5;
                }
                while (next5Bits >= 32 && index < polylineChars.Length);

                if (index >= polylineChars.Length && next5Bits >= 32)
                {
                    break;
                }

                currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                var mLatLng = new LatLng(Convert.ToDouble(currentLat) / 100000.0, Convert.ToDouble(currentLng) / 100000.0);
                poly.Add(mLatLng);
            }

            return poly;
        }
        

        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;

            //Optional
            googleMap.UiSettings.ZoomControlsEnabled = true;
            googleMap.UiSettings.CompassEnabled = true;
            googleMap.MoveCamera(CameraUpdateFactory.ZoomIn());
            googleMap.UiSettings.MyLocationButtonEnabled = true;
        }

        public async void GetRawData(string textBox)
        {
            // Use https to satisfy iOS ATS requirements.
            var client = new HttpClient();
            var response = await client.GetAsync("https://maps.googleapis.com/maps/api/directions/json?" +
                "origin=Hai%20Chau%20Da%20Nang"
                +"&destination=Thanh%20Khe%20Da%20Nang"
                + "&key=AIzaSyDPAaZp3Xj9LggTsweR2twzeh3zP4j58pE");
            var responseString = await response.Content.ReadAsStringAsync();
            var JsonObject = JsonConvert.DeserializeObject(responseString);
            textBox = responseString;
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
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

            List<LatLng> lines = DecodePolyline("yf}`Bk|osS}E\\kBFo@UEEOCMFCNDLDB@?FjBNxAH~@z@xMh@~Hl@|GNtCHbAGXUr@wDnHcDxFY\\_@\\I@OBUNINkBNwBXgCTiHf@k@D}BXuAd@yAp@k@d@_@`@Uf@CAEAQAQDONCLAR@BKLU`@OPg@^eAn@SH_Cn@q@Na@PCCEEOCOBKNAH?BYLuE`ByD|AcJ|EsBx@_JxCoBt@o@n@@^@tBFzEP|@D~BHxEExGKhFIpDE~FSrIEvBC`E@tCT|GlCAXbJ`Bi@bAo@NGKi@?E?CDE@G");

            var polylineOptions = new PolylineOptions()
                            .InvokeColor(Android.Graphics.Color.Blue)
                            .InvokeWidth(4);

            foreach (LatLng line in lines)
            {
                polylineOptions.Add(line);
            }

            var editStartPoint = FindViewById<EditText>(Resource.Id.editStartPoint);
            var editEndPoint = FindViewById<EditText>(Resource.Id.editEndPoint);
            var btnFindPath = FindViewById<Button>(Resource.Id.button1);
            var txtResult = FindViewById<TextView>(Resource.Id.textResult);

            btnFindPath.Click += (e, o) =>
            {
                map.AddPolyline(polylineOptions);
            };

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
            lat = 16.0660217;
            lng = 108.2210158;

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
            //throw new NotImplementedException();
        }
        
        public void OnProviderEnabled(string provider)
        {
            //throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            //throw new NotImplementedException();
        }

        public void OnInfoWindowClick(Marker marker)
        {
            Toast.MakeText(this, $"Icon {marker.Title} is clicked", ToastLength.Short).Show();
        }
    }
}

