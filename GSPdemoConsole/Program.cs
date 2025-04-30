using System.Data.Common;

using GB = GeoBridgeNET;
using Rhino.Geometry;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var pt1 = new Point3d(1, 2, 3);
var pt2 = new Point3d(10, 20, 30);

var ptr = GB.GeoMarshal.ToNativePoint3d(pt);
var backPt = GB.GeoMarshal.FromNativePoint3d(ptr);



