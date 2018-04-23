using System;
using System.Windows.Forms;
using DevExpress.XtraMap;

namespace CustomProjection {
    public partial class Form1 : Form {
        const string filepath = "../../Data/Countries.shp";
        
        GeoMapCoordinateSystem CoordinateSystem {
            get { return (GeoMapCoordinateSystem)mapControl1.CoordinateSystem; }
        }

        VectorItemsLayer MapLayer {
            get { return (VectorItemsLayer)mapControl1.Layers["MapLayer"]; }
        }

        ShapefileDataAdapter Adapter {
            get { return (ShapefileDataAdapter)MapLayer.Data; }
        }
        
        public Form1() {
            InitializeComponent();
            CoordinateSystem.Projection = new HammerAitoffProjection();

            Uri baseUri = new Uri(System.Reflection.Assembly.GetEntryAssembly().Location);
            Adapter.FileUri = new Uri(baseUri, filepath);
        }
    }
}
