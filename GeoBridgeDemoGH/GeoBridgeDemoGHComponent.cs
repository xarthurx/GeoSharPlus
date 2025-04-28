using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
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
      var poly = new Polyline();
      if (!DA.GetData(0, ref poly) || poly.Count == 0)
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No points provided.");
        return;
      }

      var ptr = GB.GeometryMarshal.ToNativePolyline(poly);

      var backPoly = GB.GeometryMarshal.FromNativePolyline(ptr);

      DA.SetData(0, backPoly);
    }

  }
}