using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using GB = GeoBridgeNET;

namespace GeoBridgeDemoGH
{

  public class DemoPointRoundTrip : GH_Component
  {
    protected override System.Drawing.Bitmap Icon => null;
    public override Guid ComponentGuid => new Guid("4d4d80b0-3434-441e-b5b2-675b8eef8282");

    public DemoPointRoundTrip()
      : base("Point RoundTrip", "GB_pt",
        "Conduct a round trip of C# -> Cpp -> C# for a single Rhino point.",
        "GeoBridge", "Examples")
    {
    }

    protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
    {
      pManager.AddPointParameter("Point", "Pin", "A Rhino point as input.", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
    {
      pManager.AddPointParameter("Point", "Pout", "A Rhino point as output.", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // Input retrieval
      Point3d pt = new Point3d();
      if (!DA.GetData(0, ref pt))
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No points provided.");
        return;
      }

      var ptr = GB.GeoMarshal.ToNativePoint3d(pt);

      var backPt = GB.GeoMarshal.FromNativePoint3d(ptr);

      DA.SetData(0, backPt);
    }

  }

  public class GeoBridgeDemoGHComponent : GH_Component
  {
    protected override System.Drawing.Bitmap Icon => null;
    public override Guid ComponentGuid => new Guid("8ea30250-d74b-4061-9250-ec4c29f966e1");

    public GeoBridgeDemoGHComponent()
      : base("Poly RoundTrip", "GB_poly",
        "Conduct a round trip of C# -> Cpp -> C# for a single Rhino polyline.",
        "GeoBridge", "Examples")
    {
    }

    protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
    {
      pManager.AddCurveParameter("Polyline", "Cin", "A polyline as input.", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
    {
      pManager.AddCurveParameter("Polyline", "Cout", "A polyline as output.", GH_ParamAccess.item);
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