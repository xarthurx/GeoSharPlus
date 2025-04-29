using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using GB = GeoBridgeNET;

namespace GeoBridgeDemoGH
{
  public class GeoBridgeDemoGHComponent : GH_Component
  {
    protected override System.Drawing.Bitmap Icon => null;
    public override Guid ComponentGuid => new Guid("8ea30250-d74b-4061-9250-ec4c29f966e1");

    public GeoBridgeDemoGHComponent()
      : base("Point Averager", "Avg",
        "Calculates the average of a collection of points using GeoBridgeNET",
        "GeoBridge", "Examples")
    {
    }

    protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
    {
      pManager.AddCurveParameter("Polyline", "P", "A polyline.", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
    {
      pManager.AddCurveParameter("Polyline", "P", "A polyline.", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // Input retrieval
      //var crv = new PolylineCurve();
      Curve crv = null; 
      if (!DA.GetData(0, ref crv))
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No points provided.");
        return;
      }

      if (!crv.TryGetPolyline(out Polyline poly))
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Curve is not a polyline");
      }

      var ptr = GB.GeoMarshal.ToNativePolyline(poly);

      var backPoly = GB.GeoMarshal.FromNativePolyline(ptr);

      DA.SetData(0, backPoly);
    }

  }
}