using System.IO;
using System.Collections.Generic;
using GraphHop.Shared.Data;
using GraphHop.SharedRhino;
using Grasshopper.Kernel;
using Rhino;
using Rhino.Commands;

namespace GraphHop.PluginRhino.Commands
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
            RhinoApp.WriteLine("RHINOCOMMON_EQUAL_7 is defined. Unsupported");
            return Result.Failure;
#endif
#if RHINOCOMMON_EQUAL_8
            RhinoApp.WriteLine("RHINOCOMMON_EQUAL_8 is defined.");
#endif
#if RHINOCOMMON_GREATER_EQUAL_7
            RhinoApp.WriteLine("RHINOCOMMON_GREATER_EQUAL_7 is defined. Unsupported");
#endif
#if RHINOCOMMON_GREATER_EQUAL_8
            RhinoApp.WriteLine("RHINOCOMMON_GREATER_EQUAL_8 is defined.");
#endif
            /// see https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives
#if NETFRAMEWORK
            RhinoApp.WriteLine("NETFRAMEWORK is defined.");
            return Result.Failure;
#endif
#if NET7_0_OR_GREATER
            RhinoApp.WriteLine("NET7_0_OR_GREATER is defined.");
#endif
            var fileDialog = new Rhino.UI.OpenFileDialog
            {
                Filter = "Grasshopper Files (*.gh;*.ghx)|*.gh;*.ghx",
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

            GHDigestUtility.IterateDocumentObjects(ghDocument);

            GraphStrutObject graphStrut = new GraphStrutObject();
            graphStrut.IterateDocumentObjects(ghDocument);
            
            PluginRhino.Gremlin.Add(graphStrut.DocumentNode);
            PluginRhino.Gremlin.Add(graphStrut.DocumentVersionNode);
            PluginRhino.Gremlin.Connect(graphStrut.DocumentNode,graphStrut.DocumentVersionNode);

            foreach (var defNode in graphStrut.ComponentDefinitionNodes.Values)
            {
                PluginRhino.Gremlin.Add(defNode);
            }
            
            foreach (var inputNode in graphStrut.InputNodes.Values)
            {
                PluginRhino.Gremlin.Add(inputNode);
            }
            foreach (var outputNode in graphStrut.OutputNodes.Values)
            {
                PluginRhino.Gremlin.Add(outputNode);
            }
            
            foreach (var instanceNode in graphStrut.ComponentInstanceNodes.Values)
            {
                PluginRhino.Gremlin.Add(instanceNode);
                PluginRhino.Gremlin.Connect(instanceNode,
                    graphStrut.ComponentDefinitionNodes[instanceNode.ComponentGuid]);
                PluginRhino.Gremlin.Connect(graphStrut.DocumentVersionNode, instanceNode);
                foreach (var inputId in instanceNode.Inputs)
                {
                    PluginRhino.Gremlin.Connect(graphStrut.InputNodes[inputId],
                        instanceNode);
                }
                
                foreach (var outputId in instanceNode.Outputs)
                {
                    PluginRhino.Gremlin.Connect(instanceNode,
                        graphStrut.OutputNodes[outputId]);
                }
            }
            
            foreach (var outputNode in graphStrut.OutputNodes.Values)
            {
                if (graphStrut.InputNodes.TryGetValue(outputNode.TargetGuid, out var inputNode))
                {
                    PluginRhino.Gremlin.Connect(outputNode,inputNode);
                }
            }
            RhinoApp.WriteLine($"Uploaded: {filePath}");

            return Result.Success;

        }
    }
}