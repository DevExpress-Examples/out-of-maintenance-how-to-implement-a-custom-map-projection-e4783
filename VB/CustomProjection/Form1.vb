Imports Microsoft.VisualBasic
Imports System
Imports System.Windows.Forms
Imports DevExpress.XtraMap
Imports System.Drawing
Imports DevExpress.Map.Native
Imports DevExpress.XtraMap.Native

Namespace CustomProjection
    Partial Public Class Form1
        Inherits Form
        Public Sub New()
            InitializeComponent()
        End Sub



        Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            ' Create a map control and specify its dock style.
            Dim map As New MapControl()
            Me.Controls.Add(map)
            map.Dock = DockStyle.Fill

            ' Create a file layer and assign a shape loader to it.
            Dim layer As New VectorFileLayer()
            layer.FileLoader = CreateShapefileLoader()

            ' Specify a custom map projection. 
            layer.Projection = New CustomProjection()
            map.Layers.Add(layer)

        End Sub

        Public Class CustomProjection
            Implements IProjection

            Public Const LonToKilometersRatio As Double = 100.0
            Public Const LatToKilometersRatio As Double = 111.12

            Private offsetX As Double = 0.5
            Private offsetY As Double = 0.5
            Private scaleX As Double = 0.5
            Private scaleY As Double = -0.25

            Public Const MinLatitude As Double = -90.0
            Public Const MaxLatitude As Double = 90.0
            Public Const MinLongitude As Double = -180.0
            Public Const MaxLongitude As Double = 180.0

            Private Function IsValidPoint(ByVal x As Double, ByVal y As Double) As Boolean
                If Math.Pow(x, 2) + Math.Pow(y, 2) > 1 Then
                    Return False
                Else
                    Return True
                End If
            End Function

            Public Function GeoPointToMapUnit(geoPoint As GeoPoint) As MapUnit Implements IProjection.GeoPointToMapUnit



                Dim lonInRadian As Double = MathUtils.Degree2Radian(Math.Min(MaxLongitude, Math.Max(MinLongitude, geoPoint.Longitude)))
                Dim latInRadian As Double = MathUtils.Degree2Radian(Math.Min(MaxLatitude, Math.Max(MinLatitude, geoPoint.Latitude)))
                Dim z As Double = Math.Sqrt(1 + Math.Cos(latInRadian) * Math.Cos(lonInRadian / 2))
                Dim x As Double = Math.Cos(latInRadian) * Math.Sin(lonInRadian / 2) / z
                Dim y As Double = Math.Sin(latInRadian) / z

                Return New MapUnit(x * scaleX + offsetX, y * scaleY + offsetY)
            End Function

            Public Function MapUnitToGeoPoint(mapUnit As MapUnit) As GeoPoint Implements IProjection.MapUnitToGeoPoint

                Dim x As Double = (mapUnit.X - offsetX) / scaleX
                Dim y As Double = Math.Min(1, Math.Max(-1, (mapUnit.Y - offsetY) / scaleY))

                If IsValidPoint(x, y) Then
                    Dim z As Double = Math.Sqrt(1 - 0.5 * Math.Pow(x, 2) - 0.5 * Math.Pow(y, 2))
                    Dim c As Double = Math.Sqrt(2) * z * x / (2 * Math.Pow(z, 2) - 1)
                    Dim lon As Double = 2 * Math.Atan(c)
                    Dim lat As Double = Math.Asin(Math.Min(Math.Max(Math.Sqrt(2) * z * y, -1), 1))
                    Return New GeoPoint(Math.Min(MaxLatitude, Math.Max(MinLatitude, MathUtils.Radian2Degree(lat))), Math.Min(MaxLongitude, Math.Max(MinLongitude, MathUtils.Radian2Degree(lon))))
                Else
                    Dim signX As Integer = If((x < 0), -1, 1)
                    Dim signY As Integer = If((y < 0), -1, 1)
                    Return New GeoPoint(MaxLatitude * signY, MaxLongitude * signX)
                End If
            End Function

            Public Function GeoToKilometersSize(ByVal anchorPoint As GeoPoint, ByVal size As MapSize) As MapSize Implements IProjection.GeoToKilometersSize

                Return New MapSize(size.Width * LonToKilometersRatio * Math.Cos(MathUtils.Degree2Radian(anchorPoint.Latitude)), size.Height * LatToKilometersRatio)
            End Function

            Public Function KilometersToGeoSize(ByVal anchorPoint As GeoPoint, ByVal size As MapSize) As MapSize Implements IProjection.KilometersToGeoSize

                Return New MapSize(size.Width / LonToKilometersRatio / Math.Cos(MathUtils.Degree2Radian(anchorPoint.Latitude)), size.Height / LatToKilometersRatio)
            End Function


        End Class



        Private Function CreateShapefileLoader() As ShapefileLoader
            ' Create an object to load data from a Shapefile.
            Dim loader As New ShapefileLoader()

            ' Determine the path to the Shapefile.
            Dim baseUri As New Uri(System.Reflection.Assembly.GetEntryAssembly().Location)
            Dim shapefilePath As String = "../../Data/Countries.shp"
            loader.FileUri = New Uri(baseUri, shapefilePath)

            Return loader
        End Function

    End Class
End Namespace
