Imports System
Imports System.Windows.Forms
Imports DevExpress.XtraMap

Namespace CustomProjection
    Partial Public Class Form1
        Inherits Form

        Private Const filepath As String = "../../Data/Countries.shp"

        Private ReadOnly Property CoordinateSystem() As GeoMapCoordinateSystem
            Get
                Return CType(mapControl1.CoordinateSystem, GeoMapCoordinateSystem)
            End Get
        End Property

        Private ReadOnly Property MapLayer() As VectorItemsLayer
            Get
                Return CType(mapControl1.Layers("MapLayer"), VectorItemsLayer)
            End Get
        End Property

        Private ReadOnly Property Adapter() As ShapefileDataAdapter
            Get
                Return CType(MapLayer.Data, ShapefileDataAdapter)
            End Get
        End Property

        Public Sub New()
            InitializeComponent()
            CoordinateSystem.Projection = New HammerAitoffProjection()

            Dim baseUri As New Uri(System.Reflection.Assembly.GetEntryAssembly().Location)
            Adapter.FileUri = New Uri(baseUri, filepath)
        End Sub
    End Class
End Namespace
