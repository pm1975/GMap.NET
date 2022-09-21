using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml;

namespace WinFormsApp_Gmap.NET
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Gmap_Load(object sender, EventArgs e)
        {
            gmap.Bearing = 0;
            gmap.CanDragMap = true;
            gmap.DragButton = MouseButtons.Left;
            gmap.GrayScaleMode = true;
            gmap.MarkersEnabled = true;
            gmap.MaxZoom = 21;
            gmap.MinZoom = 3;
            gmap.MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;
            gmap.NegativeMode = true;
            gmap.PolygonsEnabled = true;
            gmap.RoutesEnabled = true;
            gmap.ShowTileGridLines = false;
            gmap.Zoom = 7;
            gmap.ShowCenter = false;
            gmap.MapProvider = GMapProviders.GoogleMap;
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            gmap.Position = new PointLatLng(51.4165, 17.91745);
            gmap.Position = new PointLatLng(51.53555, 17.9737);
            string searchString = "Nowa Wieś 90, 33-336 Łabowa, Polska";
            new GMapControl().GetPositionByKeywords(searchString, out PointLatLng point);
            GMapOverlay markersOverlay = new GMapOverlay("markers");
            GMarkerGoogle markerHome = new GMarkerGoogle(new PointLatLng(51.4165, 17.91745), GMarkerGoogleType.yellow);
            GMarkerGoogle markerWork = new GMarkerGoogle(new PointLatLng(51.53555, 17.9737), GMarkerGoogleType.yellow);
            GMarkerGoogle markerParentHome = new GMarkerGoogle(point, GMarkerGoogleType.yellow);
            markerHome.ToolTip = new GMapRoundedToolTip(markerHome);
            markerHome.ToolTipText = "\nOlszyna 74A";
            markerWork.ToolTipText = "\nStara Droga 1";
            markerParentHome.ToolTipText = $"\n{searchString}";
            markersOverlay.Markers.Add(markerHome);
            markersOverlay.Markers.Add(markerWork);
            markersOverlay.Markers.Add(markerParentHome);
            gmap.Overlays.Add(markersOverlay);
            CreateCircle(51.53555, 17.9737, 10000, 3);
        }

        private void CreateCircle(double lat, double lon, double radius, int ColorIndex)
        {
            GMapOverlay markers = new GMapOverlay("markers2");
            PointLatLng circleCenter = new PointLatLng(lat, lon);
            int segments = 1080;
            List<PointLatLng> gpollist = new List<PointLatLng>();
            for (int i = 0; i < segments; i++)
            {
                gpollist.Add(FindPointAtDistanceFrom(circleCenter, i * (Math.PI / 180), radius / 1000));
            }
            GMapPolygon polygon = new GMapPolygon(gpollist, "Circle");
            Color color = Color.FromArgb(20, Color.Black);
            switch (ColorIndex)
            {
                case 1:
                    color = Color.FromArgb(80, Color.Red);
                    break;
                case 2:
                    color = Color.FromArgb(70, Color.Orange);
                    break;
                case 3:
                    color = Color.FromArgb(20, Color.Aqua);
                    break;
            }
            polygon.Fill = new SolidBrush(color);
            polygon.Stroke = new Pen(color, 1);
            markers.Polygons.Add(polygon);
            gmap.Overlays.Add(markers);
        }

        public static PointLatLng FindPointAtDistanceFrom(PointLatLng startPoint, double initialBearingRadians, double distanceKilometres)
        {
            const double radiusEarthKilometres = 6371.01;
            var distRatio = distanceKilometres / radiusEarthKilometres;
            var distRatioSine = Math.Sin(distRatio);
            var distRatioCosine = Math.Cos(distRatio);
            var startLatRad = DegreesToRadians(startPoint.Lat);
            var startLonRad = DegreesToRadians(startPoint.Lng);
            var startLatCos = Math.Cos(startLatRad);
            var startLatSin = Math.Sin(startLatRad);
            var endLatRads = Math.Asin((startLatSin * distRatioCosine) + (startLatCos * distRatioSine * Math.Cos(initialBearingRadians)));
            var endLonRads = startLonRad + Math.Atan2(Math.Sin(initialBearingRadians) * distRatioSine * startLatCos, distRatioCosine - startLatSin * Math.Sin(endLatRads));
            return new PointLatLng(RadiansToDegrees(endLatRads), RadiansToDegrees(endLonRads));
        }

        public static double DegreesToRadians(double degrees)
        {
            const double degToRadFactor = Math.PI / 180;
            return degrees * degToRadFactor;
        }

        public static double RadiansToDegrees(double radians)
        {
            const double radToDegFactor = 180 / Math.PI;
            return radians * radToDegFactor;
        }
    }
}
