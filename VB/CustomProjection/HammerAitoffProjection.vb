Imports System
Imports DevExpress.XtraMap

Namespace CustomProjection
    Public Class HammerAitoffProjection
        Inherits ProjectionBase

        Private Const minLatitude As Double = -90.0
        Private Const maxLatitude As Double = 90.0
        Private Const minLongitude As Double = -180.0
        Private Const maxLongitude As Double = 180.0

        Public Overrides Property OffsetX() As Double
            Set(value As Double)
            End Set
            Get
                Return 0.5
            End Get
        End Property
        Public Overrides Property OffsetY() As Double
            Set(value As Double)
            End Set
            Get
                Return 0.5
            End Get
        End Property
        Public Overrides Property ScaleX() As Double
            Set(value As Double)
            End Set
            Get
                Return 0.5
            End Get
        End Property
        Public Overrides Property ScaleY() As Double
            Set(value As Double)
            End Set
            Get
                Return -0.25
            End Get
        End Property

        Private Function IsValidPoint(ByVal x As Double, ByVal y As Double) As Boolean
            If Math.Pow(x, 2) + Math.Pow(y, 2) > 1 Then
                Return False
            Else
                Return True
            End If
        End Function

        Private Shared Function RadianToDegree(ByVal value As Double) As Double
            Return value * 180.0 / Math.PI
        End Function

        Private Shared Function DegreeToRadian(ByVal value As Double) As Double
            Return value * Math.PI / 180.0
        End Function

        Public Overrides Function GeoPointToMapUnit(ByVal geoPoint As GeoPoint) As MapUnit
            Dim lonInRadian As Double = DegreeToRadian(Math.Min(maxLongitude, Math.Max(minLongitude, geoPoint.Longitude)))
            Dim latInRadian As Double = DegreeToRadian(Math.Min(maxLatitude, Math.Max(minLatitude, geoPoint.Latitude)))
            Dim z As Double = Math.Sqrt(1 + Math.Cos(latInRadian) * Math.Cos(lonInRadian / 2))
            Dim x As Double = Math.Cos(latInRadian) * Math.Sin(lonInRadian / 2) / z
            Dim y As Double = Math.Sin(latInRadian) / z

            Return New MapUnit(x * ScaleX + OffsetX, y * ScaleY + OffsetY)

        End Function

        Public Overrides Function MapUnitToGeoPoint(ByVal mapPoint As MapUnit) As GeoPoint
            Dim x As Double = (mapPoint.X - OffsetX) / ScaleX
            Dim y As Double = Math.Min(1, Math.Max(-1, (mapPoint.Y - OffsetY) / ScaleY))

            If IsValidPoint(x, y) Then
                Dim z As Double = Math.Sqrt(1 - 0.5 * Math.Pow(x, 2) - 0.5 * Math.Pow(y, 2))
                Dim c As Double = Math.Sqrt(2) * z * x / (2 * Math.Pow(z, 2) - 1)
                Dim lon As Double = 2 * Math.Atan(c)
                Dim lat As Double = Math.Asin(Math.Min(Math.Max(Math.Sqrt(2) * z * y, -1), 1))
                Dim latInDegree As Double = lat * 180.0 / Math.PI
                Dim lonInDegree As Double = lon * 180.0 / Math.PI

                Return New GeoPoint(Math.Min(maxLatitude, Math.Max(minLatitude, RadianToDegree(lat))), Math.Min(maxLongitude, Math.Max(minLongitude, RadianToDegree(lon))))
            Else
                Dim signX As Integer = If(x < 0, -1, 1)
                Dim signY As Integer = If(y < 0, -1, 1)
                Return New GeoPoint(maxLatitude * signY, maxLongitude * signX)
            End If

        End Function

        Public Overrides Function GeoToKilometersSize(ByVal anchorPoint As GeoPoint, ByVal size As MapSize) As MapSize
            Return New MapSize(size.Width * LonToKilometersRatio * Math.Cos(DegreeToRadian(anchorPoint.Latitude)), size.Height * LatToKilometersRatio)
        End Function

        Public Overrides Function KilometersToGeoSize(ByVal anchorPoint As GeoPoint, ByVal size As MapSize) As MapSize
            Return New MapSize(size.Width / LonToKilometersRatio / Math.Cos(DegreeToRadian(anchorPoint.Latitude)), size.Height / LatToKilometersRatio)
        End Function
    End Class
End Namespace
