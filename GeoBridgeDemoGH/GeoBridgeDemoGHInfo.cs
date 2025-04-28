using System;
using System.Drawing;
using Grasshopper;
using Grasshopper.Kernel;

namespace GeoBridgeDemoGH
{
    public class GeoBridgeDemoGHInfo : GH_AssemblyInfo
    {
        public override string Name => "GeoBridgeDemoGH";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "A demo GH plugin project for the GeoBridgeRHGH.";

        public override Guid Id => new Guid("66feb2ff-9edb-47c9-8418-3e98270bc29b");

        //Return a string identifying you or your company.
        public override string AuthorName => "Dr. Zhao MA";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "zhma@ethz.ch";

        //Return a string representing the version.  This returns the same version as the assembly.
        public override string AssemblyVersion => GetType().Assembly.GetName().Version.ToString();
    }
}