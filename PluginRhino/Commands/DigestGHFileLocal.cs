using System.IO;
using Grasshopper.Kernel;
using GraphHop.SharedRhino;
using Rhino;
using Rhino.Commands;
using OpenAI.Chat;
using System;
using System.Diagnostics;

namespace GraphHop.PluginRhino.Commands
{
    public class DigestGHFileLocal : Command
    {
        public DigestGHFileLocal()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static DigestGHFileLocal Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "DigestGHFileLocal";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            /// The following code shows a possibility to differentiate between versions of RhinoCommon and .NET at compile time.

            /// The RHINOCOMMON_* constants are defined in "CommonReferencesRhino.csproj"
#if RHINOCOMMON_EQUAL_7
            RhinoApp.WriteLine("RHINOCOMMON_EQUAL_7 is defined.");
            return Result.Failure;
#endif

#if RHINOCOMMON_EQUAL_8
            RhinoApp.WriteLine("RHINOCOMMON_EQUAL_8 is defined.");
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

            GHDigestUtility gHDigestUtility = new GHDigestUtility();
            if (!gHDigestUtility.LoadDocument(filePath, out var errmsg))
            {
                RhinoApp.WriteLine("Failed to load the Grasshopper document.");
                return Result.Failure;
            }
            gHDigestUtility.ParseGHToConsole();

            Debug.WriteLine("----------------------------------");

            //need to store key inside environment variable as OPENAI_API_KEY andrestart VisualStudio

            ChatClient client = new(model: "gpt-4o", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

            ChatCompletion completion = client.CompleteChat("Say 'this is a test.'");

            Debug.WriteLine($"[ASSISTANT]: {completion.Content[0].Text}");

            return Result.Success;
        }
    }
}
