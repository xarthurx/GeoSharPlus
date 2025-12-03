using System;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace GSPdemoGH {

  /// <summary>
  /// Demo component: Point3d Round-Trip using GSP.Extensions
  /// Demonstrates sending a point from C# to C++ and back.
  /// </summary>
  public class DemoPointRoundTrip : GH_Component {
    protected override System.Drawing.Bitmap Icon => null;
    public override Guid ComponentGuid => new Guid("3c3c70a0-2323-330d-a4a1-564a7de97171");

    public DemoPointRoundTrip()
      : base("Point RoundTrip", "GSP_PtRT",
        "Round-trip a point through the C++ library (C# -> C++ -> C#).",
        "GSP", "Examples") {
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddPointParameter("Point", "P", "Input point", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddPointParameter("Point", "P", "Point after round-trip", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      var pt = Point3d.Unset;
      if (!DA.GetData(0, ref pt)) return;

      var result = GSP.Extensions.ExampleUtils.RoundTrip(pt);
      if (result.HasValue)
        DA.SetData(0, result.Value);
      else
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Round-trip failed.");
    }
  }

  /// <summary>
  /// Demo component: Point3d Array Round-Trip using GSP.Extensions
  /// </summary>
  public class DemoPointArrayRoundTrip : GH_Component {
    protected override System.Drawing.Bitmap Icon => null;
    public override Guid ComponentGuid => new Guid("4d4d81b1-3434-441e-b5b2-675b8eef8383");

    public DemoPointArrayRoundTrip()
      : base("Points RoundTrip", "GSP_PtsRT",
        "Round-trip a list of points through the C++ library.",
        "GSP", "Examples") {
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddPointParameter("Points", "P", "Input points", GH_ParamAccess.list);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddPointParameter("Points", "P", "Points after round-trip", GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      var points = new System.Collections.Generic.List<Point3d>();
      if (!DA.GetDataList(0, points)) return;

      var result = GSP.Extensions.ExampleUtils.RoundTrip(points.ToArray());
      if (result != null)
        DA.SetDataList(0, result);
      else
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Round-trip failed.");
    }
  }

  /// <summary>
  /// Demo component: Mesh Round-Trip using GSP.Extensions
  /// Demonstrates sending a mesh from C# to C++ and back.
  /// </summary>
  public class DemoMeshRoundTrip : GH_Component {
    protected override System.Drawing.Bitmap Icon => null;
    public override Guid ComponentGuid => new Guid("5e5e92c2-4545-552f-c6c3-786c9ff09494");

    public DemoMeshRoundTrip()
      : base("Mesh RoundTrip", "GSP_MeshRT",
        "Round-trip a mesh through the C++ library (C# -> C++ -> C#).",
        "GSP", "Examples") {
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddMeshParameter("Mesh", "M", "Input mesh", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddMeshParameter("Mesh", "M", "Mesh after round-trip", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      var mesh = new Mesh();
      if (!DA.GetData(0, ref mesh)) return;

      var result = GSP.Extensions.ExampleUtils.RoundTrip(mesh);
      if (result != null && result.IsValid)
        DA.SetData(0, result);
      else
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Round-trip failed or mesh invalid.");
    }
  }
}
