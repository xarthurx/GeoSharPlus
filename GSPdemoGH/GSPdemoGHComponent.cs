using System;
using System.Runtime.InteropServices;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace GSPdemoGH
{

  public class DemoMeshRoundTrip : GH_Component
  {
    protected override System.Drawing.Bitmap Icon => null;
    public override Guid ComponentGuid => new Guid("4d4d80b0-3434-441e-b5b2-675b8eef8282");

    public DemoMeshRoundTrip()
      : base("Point RoundTrip", "GB_pt",
        "Conduct a round trip of C# -> Cpp -> C# for a single Rhino point.",
        "GSP", "Examples")
    {
    }

    protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
    {
      pManager.AddMeshParameter("MeshIn", "meshIn", "An input mesh from Rhino.", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
    {
      pManager.AddMeshParameter("MeshOut", "meshOut", "Converted Rhino mesh from the cpp side.", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // Input retrieval
      var mesh = new Mesh();
      if (!DA.GetData("MeshIn", ref mesh))
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid mesh provided.");
        return;
      }
      var buf = GSP.Wrapper.ToMeshBuffer(mesh);
      GSP.NativeBridge.MeshRoundTrip(buf, buf.Length, out IntPtr outBuf, out int outSize);

      var byteArray = new byte[outSize];
      Marshal.Copy(outBuf, byteArray, 0, outSize);
      Marshal.FreeCoTaskMem(outBuf); // Free the unmanaged memory

      // Convert the byte buffer back to a Mesh
      var resMesh = GSP.Wrapper.FromMeshBuffer(byteArray);
      if (resMesh.IsValid)
      {
        DA.SetData("MeshOut", resMesh);
      }
      else
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Output mesh is not valid.");
      }
    }
  }
}