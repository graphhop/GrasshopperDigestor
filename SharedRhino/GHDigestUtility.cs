﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Grasshopper.Kernel;

namespace GraphHop.SharedRhino
{
    public class GHDigestUtility
    {
        private GH_DocumentIO _ioDoc;
        private GH_Document _ghDoc;

        public bool LoadDocument(string filePath, out string errmsg)
        {
            errmsg = "";
            // Load the Grasshopper document
            _ioDoc = new GH_DocumentIO();
            if (!_ioDoc.Open(filePath))
            {
                errmsg = "Failed to open the Grasshopper file.";
                return false;
            }
            _ghDoc = _ioDoc.Document;
            if (_ghDoc is null)
            {
                errmsg = "Failed to load the Grasshopper document.";
                return false;
            }

            return true;
        }

        public  void GetConnectedObjects(IGH_DocumentObject obj)
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
                Debug.WriteLine("The object is neither a parameter nor a component.");
            }
        }

        public  void IterateDocumentObjects()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("");
                PrintDocumentProperties(_ghDoc);
                // Iterate through all document objects and print their names
                foreach (IGH_DocumentObject obj in _ghDoc.Objects)
                {
                    Debug.WriteLine("");
                    PrintDocumentObjectProperties(obj);
                    Debug.WriteLine($"Object Name: {obj.Name}, NickName:  {obj.NickName} ");
                    Debug.WriteLine("----------Input&Outputs---------");
                    GetConnectedObjects(obj);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public  void PrintDocumentProperties(GH_Document ghDocument)
        {
            Debug.WriteLine($"Author: {ghDocument.Author}");
            Debug.WriteLine($"DisplayName: {ghDocument.DisplayName ?? "None"}");
            Debug.WriteLine($"DocumentID: {ghDocument.DocumentID}");
            Debug.WriteLine($"FilePath: {ghDocument.FilePath ?? "None"}");
            Debug.WriteLine($"Owner: {ghDocument.Owner}");
            Debug.WriteLine($"RuntimeID: {ghDocument.RuntimeID}");
            Debug.WriteLine($"SolutionState: {ghDocument.SolutionState}");
        }

        public  void PrintDocumentObjectProperties(IGH_DocumentObject obj)
        {
            Debug.WriteLine("----------Component Metadata---------");
            //Debug.WriteLine($"Attributes: {obj.Attributes}");
            Debug.WriteLine($"Category: {obj.Category ?? "None"}");
            Debug.WriteLine($"ComponentGuid: {obj.ComponentGuid}");
            Debug.WriteLine($"Description: {obj.Description ?? "None"}");
            Debug.WriteLine($"Exposure: {obj.Exposure}");
            Debug.WriteLine($"HasCategory: {obj.HasCategory}");
            Debug.WriteLine($"HasSubCategory: {obj.HasSubCategory}");
            Debug.WriteLine($"IconDisplayMode: {obj.IconDisplayMode}");
            Debug.WriteLine($"InstanceDescription: {obj.InstanceDescription ?? "None"}");
            Debug.WriteLine($"InstanceGuid: {obj.InstanceGuid}");
            Debug.WriteLine($"Keywords: {JoinKeywords(obj.Keywords)}");
            Debug.WriteLine($"Name: {obj.Name ?? "None"}");
            Debug.WriteLine($"NickName: {obj.NickName ?? "None"}");
            Debug.WriteLine($"Obsolete: {obj.Obsolete}");
            Debug.WriteLine($"SubCategory: {obj.SubCategory ?? "None"}");
        }

        private  string JoinKeywords(IEnumerable<string> keywords)
        {
            return keywords != null ? string.Join(", ", keywords) : "None";
        }

        private  void ProcessSource(IGH_Param source)
        {
            // Example processing logic for sources
            string parentComponentName = source.Attributes?.GetTopLevel?.DocObject?.Name ?? "Unknown";
            Debug.WriteLine($"Input Type: {source.GetType().Name}, NickName: {source.NickName}, Parent Component: {parentComponentName}");
            // Add additional processing logic here
        }

        private  void ProcessRecipient(IGH_Param recipient)
        {
            // Example processing logic for recipients
            string parentComponentName = recipient.Attributes?.GetTopLevel?.DocObject?.Name ?? "Unknown";
            Debug.WriteLine($"Output Type: {recipient.GetType().Name}, NickName: {recipient.NickName}, Parent Component: {parentComponentName}");
            // Add additional processing logic here
        }
    }
}
