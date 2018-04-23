using System;
using System.Windows.Forms;
using DevExpress.XtraMap;
using System.Drawing;
using DevExpress.Map.Native;
using DevExpress.XtraMap.Native;

namespace CustomProjection {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }



        private void Form1_Load(object sender, EventArgs e) {
            // Create a map control and specify its dock style.
            MapControl map = new MapControl();
            this.Controls.Add(map);
            map.Dock = DockStyle.Fill;

            // Create a file layer and assign a shape loader to it.
            VectorFileLayer layer = new VectorFileLayer();
            layer.FileLoader = CreateShapefileLoader();

            // Specify a custom map projection. 
            layer.Projection = new CustomProjection();
            map.Layers.Add(layer);


        }

        public class CustomProjection : IProjection {

            public const double LonToKilometersRatio = 100.0;
            public const double LatToKilometersRatio = 111.12;

            double offsetX = 0.5;
            double offsetY = 0.5;
            double scaleX = 0.5;
            double scaleY = -0.25;

            public const double MinLatitude = -90.0;
            public const double MaxLatitude = 90.0;
            public const double MinLongitude = -180.0;
            public const double MaxLongitude = 180.0;

            bool IsValidPoint(double x, double y) {
                if (Math.Pow(x, 2) + Math.Pow(y, 2) > 1)
                    return false;
                else
                    return true;
            }

            public MapUnit GeoPointToMapUnit(GeoPoint geoPoint) {
                double lonInRadian = MathUtils.Degree2Radian(Math.Min(MaxLongitude, Math.Max(MinLongitude, geoPoint.Longitude)));
                double latInRadian = MathUtils.Degree2Radian(Math.Min(MaxLatitude, Math.Max(MinLatitude, geoPoint.Latitude)));
                double z = Math.Sqrt(1 + Math.Cos(latInRadian) * Math.Cos(lonInRadian / 2));
                double x = Math.Cos(latInRadian) * Math.Sin(lonInRadian / 2) / z;
                double y = Math.Sin(latInRadian) / z;

                return new MapUnit(x * scaleX + offsetX, y * scaleY + offsetY);
            }

            public GeoPoint MapUnitToGeoPoint(MapUnit mapUnit) {
                double x = (mapUnit.X - offsetX) / scaleX;
                double y = Math.Min(1, Math.Max(-1, (mapUnit.Y - offsetY) / scaleY));

                if (IsValidPoint(x, y)) {
                    double z = Math.Sqrt(1 - 0.5 * Math.Pow(x, 2) - 0.5 * Math.Pow(y, 2));
                    double c = Math.Sqrt(2) * z * x / (2 * Math.Pow(z, 2) - 1);
                    double lon = 2 * Math.Atan(c);
                    double lat = Math.Asin(Math.Min(Math.Max(Math.Sqrt(2) * z * y, -1), 1));
                    return new GeoPoint(Math.Min(MaxLatitude, Math.Max(MinLatitude, MathUtils.Radian2Degree(lat))), Math.Min(MaxLongitude, Math.Max(MinLongitude, MathUtils.Radian2Degree(lon))));
                }
                else {
                    int signX = (x < 0) ? -1 : 1;
                    int signY = (y < 0) ? -1 : 1;
                    return new GeoPoint(MaxLatitude * signY, MaxLongitude * signX);
                }
            }

            public MapSize GeoToKilometersSize(GeoPoint anchorPoint, MapSize size) {
                return new MapSize(size.Width * LonToKilometersRatio * Math.Cos(MathUtils.Degree2Radian(anchorPoint.Latitude)), size.Height * LatToKilometersRatio);
            }

            public MapSize KilometersToGeoSize(GeoPoint anchorPoint, MapSize size) {
                return new MapSize(size.Width / LonToKilometersRatio / Math.Cos(MathUtils.Degree2Radian(anchorPoint.Latitude)), size.Height / LatToKilometersRatio);
            }
        }



        private ShapefileLoader CreateShapefileLoader() {
            // Create an object to load data from a Shapefile.
            ShapefileLoader loader = new ShapefileLoader();

            // Determine the path to the Shapefile.
            Uri baseUri = new Uri(System.Reflection.Assembly.GetEntryAssembly().Location);
            string shapefilePath = "../../Data/Countries.shp";
            loader.FileUri = new Uri(baseUri, shapefilePath);

            return loader;
        }

    }
}
