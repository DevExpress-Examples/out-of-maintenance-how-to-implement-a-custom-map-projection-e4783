# How to implement a custom map projection


<p>This example shows how to get a <a href="http://paulbourke.net/geometry/transformationprojection/"><u>Hammer-Aitoff</u></a> map projection for the shapes loaded from the Shapefiles <strong>(Countries.shp</strong>, <strong>Countries.dbf</strong>).</p><p><br />
</p>


<h3>Description</h3>

<p>To create a custom map projection, do the following:</p><p>1) Create a class implementing the IProjection interface;</p><p>2)  Implement the GeoPointToMapUnit and MapUnitToGeoPoint methods, which specify formulas to calculate custom projection coordinates and map geographical points (longitude and latitude);</p><p>3) Implement GeoToKilometersSize and KilometersToGeoSize methods to convert the specified size in geographical points to the corresponding size in kilometers for the specified anchor point and vice versa.</p><br />


<br/>


