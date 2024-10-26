using Grasshopper.Kernel;
using Rhino.Commands;
using Rhino;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            else
            {
                System.Diagnostics.Debug.WriteLine("The object is not a parameter.");
            }
        }

        public static void IterateDocumentObjects(GH_Document ghDocument)
        {

            // Iterate through all document objects and print their names
            foreach (IGH_DocumentObject obj in ghDocument.Objects)
            {
                RhinoApp.WriteLine($"Object Name: {obj.NickName}");
                GetConnectedObjects(obj);
            }
        }

        private static void ProcessSource(IGH_Param source)
        {
            // Example processing logic for sources
            System.Diagnostics.Debug.WriteLine($"Source Type: {source.GetType().Name}, NickName: {source.NickName}");
            // Add additional processing logic here
        }

        private static void ProcessRecipient(IGH_Param recipient)
        {
            // Example processing logic for recipients
            System.Diagnostics.Debug.WriteLine($"Recipient Type: {recipient.GetType().Name}, NickName: {recipient.NickName}");
            // Add additional processing logic here
        }
    }
}
