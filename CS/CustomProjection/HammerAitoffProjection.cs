using System;
using DevExpress.XtraMap;

namespace CustomProjection {
    public class HammerAitoffProjection : ProjectionBase {
        const double minLatitude = -90.0;
        const double maxLatitude = 90.0;
        const double minLongitude = -180.0;
        const double maxLongitude = 180.0;

        public override double OffsetX { get { return 0.5; } }
        public override double OffsetY { get { return 0.5; } }
        public override double ScaleX { get { return 0.5; } }
        public override double ScaleY { get { return -0.25; } }

        bool IsValidPoint(double x, double y) {
            if (Math.Pow(x, 2) + Math.Pow(y, 2) > 1)
                return false;
            else
                return true;
        }

        static double RadianToDegree(double value) {
            return value * 180.0 / Math.PI;
        }

        static double DegreeToRadian(double value) {
            return value * Math.PI / 180.0;
        }

        public override MapUnit GeoPointToMapUnit(GeoPoint geoPoint) {
            double lonInRadian = DegreeToRadian(
                Math.Min(
                    maxLongitude,
                    Math.Max(minLongitude, geoPoint.Longitude)
                )
            );
            double latInRadian = DegreeToRadian(
                Math.Min(
                    maxLatitude,
                    Math.Max(minLatitude, geoPoint.Latitude)
                )
            );
            double z = Math.Sqrt(1 + Math.Cos(latInRadian) * Math.Cos(lonInRadian / 2));
            double x = Math.Cos(latInRadian) * Math.Sin(lonInRadian / 2) / z;
            double y = Math.Sin(latInRadian) / z;

            return new MapUnit(x * ScaleX + OffsetX, y * ScaleY + OffsetY);

        }

        public override GeoPoint MapUnitToGeoPoint(MapUnit mapPoint) {
            double x = (mapPoint.X - OffsetX) / ScaleX;
            double y = Math.Min(1, Math.Max(-1, (mapPoint.Y - OffsetY) / ScaleY));

            if (IsValidPoint(x, y)) {
                double z = Math.Sqrt(1 - 0.5 * Math.Pow(x, 2) - 0.5 * Math.Pow(y, 2));
                double c = Math.Sqrt(2) * z * x / (2 * Math.Pow(z, 2) - 1);
                double lon = 2 * Math.Atan(c);
                double lat = Math.Asin(Math.Min(Math.Max(Math.Sqrt(2) * z * y, -1), 1));
                double latInDegree = lat * 180.0 / Math.PI;
                double lonInDegree = lon * 180.0 / Math.PI;

                return new GeoPoint(
                    Math.Min(
                        maxLatitude,
                        Math.Max(minLatitude, RadianToDegree(lat))
                    ),
                    Math.Min(
                        maxLongitude,
                        Math.Max(minLongitude, RadianToDegree(lon))
                    )
                );
            }
            else {
                int signX = (x < 0) ? -1 : 1;
                int signY = (y < 0) ? -1 : 1;
                return new GeoPoint(maxLatitude * signY, maxLongitude * signX);
            }

        }

        public override MapSize GeoToKilometersSize(GeoPoint anchorPoint, MapSize size) {
            return new MapSize(              
                size.Width * LonToKilometersRatio * Math.Cos(DegreeToRadian(anchorPoint.Latitude)),
                size.Height * LatToKilometersRatio
            );
        }

        public override MapSize KilometersToGeoSize(GeoPoint anchorPoint, MapSize size) {
            return new MapSize(
                size.Width / LonToKilometersRatio / Math.Cos(DegreeToRadian(anchorPoint.Latitude)),
                size.Height / LatToKilometersRatio
            );
        }
    }
}
