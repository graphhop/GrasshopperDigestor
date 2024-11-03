using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Grasshopper.Kernel;

namespace GraphHop.SharedRhino
{
    public class GHDigestUtility
    {
        private GH_DocumentIO _ioDoc;
        private GH_Document _ghDoc;
        private List<string> _outPutList;

        //seperator
        public string newFileSeperator = "----------New GH File---------";
        public string newComponentSeperator = "----------New Component---------";
        public string newInputOutputSeperator = "----------Input&Outputs---------";
        public GHDigestUtility()
        {
            _outPutList = new List<string>();
        }

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

        public string ParseGHToConsole()
        {
            try
            {
                _outPutList.Append(new String(""));
                _outPutList.Append(new String(newFileSeperator));
                PrintDocumentProperties(_ghDoc);
                // Iterate through all document objects and print their names
                foreach (IGH_DocumentObject obj in _ghDoc.Objects)
                {
                    _outPutList.Append(new String(""));
                    PrintDocumentObjectProperties(obj);
                    _outPutList.Append(new String(newInputOutputSeperator));
                    GetConnectedObjects(obj);
                }

                foreach (var line in _outPutList)
                {
                    Debug.WriteLine(line);
                }
                //parse output list to one single string
                return string.Join(Environment.NewLine, _outPutList);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string ParseGHFileNoPrint()
        {
            try
            {
                _outPutList.Append(new String(""));
                _outPutList.Append(new String(newFileSeperator));
                PrintDocumentProperties(_ghDoc);
                // Iterate through all document objects and print their names
                foreach (IGH_DocumentObject obj in _ghDoc.Objects)
                {
                    _outPutList.Append(new String(""));
                    PrintDocumentObjectProperties(obj);
                    _outPutList.Append(new String(newInputOutputSeperator));
                    GetConnectedObjects(obj);
                }
                //parse output list to one single string
                return string.Join(Environment.NewLine, _outPutList);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void GetConnectedObjects(IGH_DocumentObject obj)
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
                _outPutList.Add("The object is neither a parameter nor a component.");
            }
        }

        public void PrintDocumentProperties(GH_Document ghDocument)
        {
            _outPutList.Add($"Author: {ghDocument.Author}");
            _outPutList.Add($"DisplayName: {ghDocument.DisplayName ?? "None"}");
            _outPutList.Add($"DocumentID: {ghDocument.DocumentID}");
            _outPutList.Add($"FilePath: {ghDocument.FilePath ?? "None"}");
            _outPutList.Add($"Owner: {ghDocument.Owner}");
            _outPutList.Add($"RuntimeID: {ghDocument.RuntimeID}");
            _outPutList.Add($"SolutionState: {ghDocument.SolutionState}");
        }

        public void PrintDocumentObjectProperties(IGH_DocumentObject obj)
        {
            _outPutList.Add(newComponentSeperator);
            //_outPutList.Add($"Attributes: {obj.Attributes}");
            _outPutList.Add($"Category: {obj.Category ?? "None"}");
            _outPutList.Add($"ComponentGuid: {obj.ComponentGuid}");
            _outPutList.Add($"Description: {obj.Description ?? "None"}");
            _outPutList.Add($"Exposure: {obj.Exposure}");
            //_outPutList.Add($"HasCategory: {obj.HasCategory}");
            //_outPutList.Add($"HasSubCategory: {obj.HasSubCategory}");
            //_outPutList.Add($"IconDisplayMode: {obj.IconDisplayMode}");
            _outPutList.Add($"InstanceDescription: {obj.InstanceDescription ?? "None"}");
            _outPutList.Add($"InstanceGuid: {obj.InstanceGuid}");
            _outPutList.Add($"Keywords: {JoinKeywords(obj.Keywords)}");
            _outPutList.Add($"Name: {obj.Name ?? "None"}");
            _outPutList.Add($"NickName: {obj.NickName ?? "None"}");
            _outPutList.Add($"Obsolete: {obj.Obsolete}");
            //_outPutList.Add($"SubCategory: {obj.SubCategory ?? "None"}");
        }

        private  string JoinKeywords(IEnumerable<string> keywords)
        {
            return keywords != null ? string.Join(", ", keywords) : "None";
        }

        private void ProcessSource(IGH_Param source)
        {
            // Example processing logic for sources
            string parentComponentName = source.Attributes?.GetTopLevel?.DocObject?.Name ?? "Unknown";
            _outPutList.Add($"Input Type: {source.GetType().Name}, NickName: {source.NickName}, Parent Component: {parentComponentName}");
            // Add additional processing logic here
        }

        private void ProcessRecipient(IGH_Param recipient)
        {
            // Example processing logic for recipients
            string parentComponentName = recipient.Attributes?.GetTopLevel?.DocObject?.Name ?? "Unknown";
            _outPutList.Add($"Output Type: {recipient.GetType().Name}, NickName: {recipient.NickName}, Parent Component: {parentComponentName}");
            // Add additional processing logic here
        }
    }
}
