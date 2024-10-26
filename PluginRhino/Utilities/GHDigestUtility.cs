using Grasshopper.Kernel;
using Rhino.Commands;
using Rhino;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PluginTemplate.PluginRhino.Utilities
{
    public static class GHDigestUtility
    {
        public static void GetConnectedObjects(IGH_DocumentObject obj)
        {
            // Check if the object is a parameter
            if (obj is IGH_Param param)
            {
                // Get the sources (inputs)
                IList<IGH_Param> sources = param.Sources;
                foreach (var source in sources)
                {
                    // Process each source
                    ProcessSource(source);
                }

                // Get the recipients (outputs)
                IList<IGH_Param> recipients = param.Recipients;
                foreach (var recipient in recipients)
                {
                    // Process each recipient
                    ProcessRecipient(recipient);
                }
            }
            // Check if the object is a component
            else if (obj is IGH_Component component)
            {
                // Get the inputs
                foreach (var input in component.Params.Input)
                {
                    foreach (var source in input.Sources)
                    {
                        // Process each source
                        ProcessSource(source);
                    }
                }

                // Get the outputs
                foreach (var output in component.Params.Output)
                {
                    foreach (var recipient in output.Recipients)
                    {
                        // Process each recipient
                        ProcessRecipient(recipient);
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("The object is neither a parameter nor a component.");
            }
        }

        public static void IterateDocumentObjects(GH_Document ghDocument)
        {
            System.Diagnostics.Debug.WriteLine("");
            PrintDocumentProperties(ghDocument);
            // Iterate through all document objects and print their names
            foreach (IGH_DocumentObject obj in ghDocument.Objects)
            {
                System.Diagnostics.Debug.WriteLine("");
                System.Diagnostics.Debug.WriteLine($"Object Name: {obj.Name}, NickName:  {obj.NickName} ");
                System.Diagnostics.Debug.WriteLine("----------Input&Outputs---------");
                GetConnectedObjects(obj);
            }
        }

        public static void PrintDocumentProperties(GH_Document ghDocument)
        {
            Debug.WriteLine($"Author: {ghDocument.Author}");
            Debug.WriteLine($"DisplayName: {ghDocument.DisplayName}");
            Debug.WriteLine($"DocumentID: {ghDocument.DocumentID}");
            Debug.WriteLine($"FilePath: {ghDocument.FilePath}");
            Debug.WriteLine($"Owner: {ghDocument.Owner}");
            Debug.WriteLine($"RuntimeID: {ghDocument.RuntimeID}");
            Debug.WriteLine($"SolutionState: {ghDocument.SolutionState}");
        }

        private static void ProcessSource(IGH_Param source)
        {
            // Example processing logic for sources
            string parentComponentName = source.Attributes?.GetTopLevel?.DocObject?.Name ?? "Unknown";
            System.Diagnostics.Debug.WriteLine($"Input Type: {source.GetType().Name}, NickName: {source.NickName}, Parent Component: {parentComponentName}");
            // Add additional processing logic here
        }

        private static void ProcessRecipient(IGH_Param recipient)
        {
            // Example processing logic for recipients
            string parentComponentName = recipient.Attributes?.GetTopLevel?.DocObject?.Name ?? "Unknown";
            System.Diagnostics.Debug.WriteLine($"Output Type: {recipient.GetType().Name}, NickName: {recipient.NickName}, Parent Component: {parentComponentName}");
            // Add additional processing logic here
        }
    }
}
