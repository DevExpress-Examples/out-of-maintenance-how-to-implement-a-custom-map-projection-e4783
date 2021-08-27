<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/128576518/13.1.5%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/E4783)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
<!-- default file list -->
*Files to look at*:

* [Form1.cs](./CS/CustomProjection/Form1.cs) (VB: [Form1.vb](./VB/CustomProjection/Form1.vb))
<!-- default file list end -->
# How to implement a custom map projection


<p>This example shows how to get a <a href="http://paulbourke.net/geometry/transformationprojection/"><u>Hammer-Aitoff</u></a> map projection for the shapes loaded from the Shapefiles <strong>(Countries.shp</strong>, <strong>Countries.dbf</strong>).</p><p><br />
</p>


<h3>Description</h3>

<p>To create a custom map projection, do the following:</p><p>1) Create a class implementing the IProjection interface;</p><p>2)  Implement the GeoPointToMapUnit and MapUnitToGeoPoint methods, which specify formulas to calculate custom projection coordinates and map geographical points (longitude and latitude);</p><p>3) Implement GeoToKilometersSize and KilometersToGeoSize methods to convert the specified size in geographical points to the corresponding size in kilometers for the specified anchor point and vice versa.</p><br />


<br/>


