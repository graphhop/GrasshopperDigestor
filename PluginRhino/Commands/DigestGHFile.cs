using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System.IO;
using Grasshopper.Kernel;
using PluginTemplate.PluginRhino.Utilities;

namespace PluginTemplate.PluginRhino
{
    public class DigestGHFile : Command
    {
        public DigestGHFile()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }
        ///<summary>The only instance of this command.</summary>
        public static DigestGHFile Instance { get; private set; }
        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "DigestGHFile";
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            /// The following code shows a possibility to differentiate between versions of RhinoCommon and .NET at compile time.
            /// The RHINOCOMMON_* constants are defined in "CommonReferencesRhino.csproj"
#if RHINOCOMMON_EQUAL_7
            RhinoApp.WriteLine("RHINOCOMMON_EQUAL_7 is defined.");
#endif
#if RHINOCOMMON_EQUAL_8
            RhinoApp.WriteLine("RHINOCOMMON_EQUAL_8 is defined.");
            var fileDialog = new Rhino.UI.OpenFileDialog
            {
                Filter = "Grasshopper Files (*.gh)|*.gh",
                Title = "Select a Grasshopper File"
            };
            if (!fileDialog.ShowOpenDialog())
                return Result.Cancel;
            string filePath = fileDialog.FileName;
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                RhinoApp.WriteLine("Invalid file path.");
                return Result.Failure;
            }
            // Load the Grasshopper document
            var io = new GH_DocumentIO();
            if (!io.Open(filePath))
            {
                RhinoApp.WriteLine("Failed to open the Grasshopper file.");
                return Result.Failure;
            }
            var ghDocument = io.Document;
            if (ghDocument == null)
            {
                RhinoApp.WriteLine("Failed to load the Grasshopper document.");
                return Result.Failure;
            }
            // Iterate through all document objects and print their names
            //foreach (IGH_DocumentObject obj in ghDocument.Objects)
            //{
            //    RhinoApp.WriteLine($"Object Name: {obj.NickName}");
            //}

            GHDigestUtility.IterateDocumentObjects(ghDocument);
            return Result.Success;
#endif
#if RHINOCOMMON_GREATER_EQUAL_7
            RhinoApp.WriteLine("RHINOCOMMON_GREATER_EQUAL_7 is defined.");
#endif
#if RHINOCOMMON_GREATER_EQUAL_8
            RhinoApp.WriteLine("RHINOCOMMON_GREATER_EQUAL_8 is defined.");
#endif
            /// see https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives
#if NETFRAMEWORK
            RhinoApp.WriteLine("NETFRAMEWORK is defined.");
#endif
#if NET7_0_OR_GREATER
            RhinoApp.WriteLine("NET7_0_OR_GREATER is defined.");
#endif
            return Result.Success;
        }
    }
}