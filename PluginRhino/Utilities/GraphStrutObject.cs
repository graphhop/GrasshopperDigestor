using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using GraphHop.Shared.Data;
using Grasshopper.Kernel;

namespace GraphHop.PluginRhino.Utilities
{
    /// <summary>
    /// Represents a collection of various nodes related to a graph structure in Rhino.
    /// </summary>
    public class GraphStrutObject
    {
        // List to store component definition nodes
        /// <summary>
        /// Represents a collection of component definition nodes stored in a dictionary.
        /// Each node contains information about a specific component definition.
        /// </summary>
        public Dictionary<Guid,ComponentDefinitionNode> ComponentDefinitionNodes = new();

        // List to store component instance nodes
        /// <summary>
        /// List to store component instance nodes.
        /// </summary>
        public Dictionary<Guid,ComponentInstanceNode> ComponentInstanceNodes = new();

        /// <summary>
        /// Represents a collection of input nodes associated with a GraphStrutObject.
        /// </summary>
        public Dictionary<Guid, DataInputNode> InputNodes = new();

        /// <summary>
        /// Represents a collection of output nodes in the GraphStrutObject.
        /// Output nodes are used to store information about the output connections of components in the graph.
        /// </summary>
        public Dictionary<Guid, DataOutputNode> OutputNodes = new();

        // Node to store document version information
        /// <summary>
        /// Node to store document version information.
        /// </summary>
        public DocumentVersionNode DocumentVersionNode = new DocumentVersionNode();

        // Node to store document information
        /// <summary>
        /// Represents a node to store document information in the GraphHop system.
        /// </summary>
        public DocumentNode DocumentNode = new DocumentNode();


        /// <summary>
        /// Iterates through all document objects and processes them.
        /// </summary>
        /// <param name="ghDocument">The Grasshopper document to iterate through.</param>
        public void IterateDocumentObjects(GH_Document ghDocument)
        {
            try
            {
                // Populate document properties
                PopulateDocumentProperties(ghDocument);

                // Iterate through all document objects
                foreach (IGH_DocumentObject obj in ghDocument.Objects)
                {
                    // Check if the object is a new component definition
                    if (!ComponentDefinitionNodes.ContainsKey(obj.ComponentGuid))
                    {
                        ComponentDefinitionNodes.Add(obj.ComponentGuid,new ComponentDefinitionNode
                        {
                            ComponentGuid = obj.ComponentGuid,
                            Name = obj.Name,
                            // Uncomment and add additional properties if needed
                            // ComponentCategory = obj.Category,
                            // ComponentSubCategory = obj.SubCategory,
                            // ComponentDescription = obj.Description
                        });
                    }

                    // Populate properties of the document object
                    var componentInstanceNode = PopulateDocumentObjectProperties(obj);

                    // Process connected objects
                    GetConnectedObjects(obj, componentInstanceNode);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Processes the connected objects (inputs and outputs) of a given document object.
        /// </summary>
        /// <param name="obj">The document object to process.</param>
        /// <param name="componentInstanceNode">The component instance node to update.</param>
        public void GetConnectedObjects(IGH_DocumentObject obj, ComponentInstanceNode componentInstanceNode)
        {
            // Check if the object is a parameter
            if (obj is IGH_Param param)
            {
                // Process each source (input)
                foreach (var source in param.Sources)
                {
                    ProcessInput(source, componentInstanceNode);
                }

                // Process each recipient (output)
                foreach (var recipient in param.Recipients)
                {
                    ProcessOutput(recipient, componentInstanceNode);
                }
            }
            // Check if the object is a component
            else if (obj is IGH_Component component)
            {
                // Process each input source
                foreach (var input in component.Params.Input)
                {
                    foreach (var source in input.Sources)
                    {
                        ProcessInput(source, componentInstanceNode);
                    }
                }

                // Process each output recipient
                foreach (var output in component.Params.Output)
                {
                    foreach (var recipient in output.Recipients)
                    {
                        ProcessOutput(recipient, componentInstanceNode);
                    }
                }
            }
            else
            {
                Debug.WriteLine("The object is neither a parameter nor a component.");
            }
        }

        /// <summary>
        /// Populates the properties of the document node.
        /// </summary>
        /// <param name="ghDocument">The Grasshopper document to process.</param>
        public void PopulateDocumentProperties(GH_Document ghDocument)
        {
            DocumentNode.DocumentID = ghDocument.DocumentID;
            // Update version each time the struct object is instantiated
            DocumentVersionNode.VersionId = Guid.NewGuid();
            DocumentNode.DisplayName = ghDocument.DisplayName;
            DocumentNode.FilePath = ghDocument.FilePath;
            DocumentNode.Owner = ghDocument?.Owner?.ToString() ?? "";
            DocumentNode.RuntimeID = ghDocument.RuntimeID;
        }

        /// <summary>
        /// Populates the properties of a document object and returns a component instance node.
        /// </summary>
        /// <param name="obj">The document object to process.</param>
        /// <returns>The populated component instance node.</returns>
        public ComponentInstanceNode PopulateDocumentObjectProperties(IGH_DocumentObject obj)
        {
            if (!ComponentInstanceNodes.TryGetValue(obj.InstanceGuid, out var componentInstanceNode))
            {
                componentInstanceNode = new ComponentInstanceNode
                {
                    InstanceGuid = obj.InstanceGuid,
                    ComponentGuid = obj.ComponentGuid,
                    NickName = obj.NickName,
                    Name = obj.Name,
                    Icon = ConvertBitmapToBase64(obj.Icon_24x24),
                    Description = obj.Description,
                    X = obj.Attributes.Pivot.X,
                    Y = obj.Attributes.Pivot.Y,
                    Inputs = new(),
                    Outputs = new()
                };

                ComponentInstanceNodes.Add(obj.InstanceGuid,componentInstanceNode);
            }
            
            return componentInstanceNode;
        }

        /// <summary>
        /// Processes an input source and updates the component instance node.
        /// </summary>
        /// <param name="source">The input source to process.</param>
        /// <param name="componentInstanceNode">The component instance node to update.</param>
        private void ProcessInput(IGH_Param source, ComponentInstanceNode componentInstanceNode)
        {
            if (!InputNodes.ContainsKey(source.InstanceGuid))
            {
                var inputNode = new DataInputNode
                {
                    TargetGuid = source.Attributes?.GetTopLevel?.DocObject?.InstanceGuid ?? Guid.Empty,
                    InstanceGuid = source.InstanceGuid,
                    NickName = source.NickName,
                    Name = source.GetType().Name
                };
                InputNodes.Add(inputNode.InstanceGuid, inputNode);
            }

            componentInstanceNode.Inputs.Add(source.InstanceGuid);
        }

        /// <summary>
        /// Processes an output recipient and updates the component instance node.
        /// </summary>
        /// <param name="recipient">The output recipient to process.</param>
        /// <param name="componentInstanceNode">The component instance node to update.</param>
        private void ProcessOutput(IGH_Param recipient, ComponentInstanceNode componentInstanceNode)
        {
            if (!OutputNodes.ContainsKey(recipient.InstanceGuid))
            {
                var outputNode = new DataOutputNode
                {
                    TargetGuid = recipient.Attributes?.GetTopLevel?.DocObject?.InstanceGuid ?? Guid.Empty,
                    InstanceGuid = recipient.InstanceGuid,
                    NickName = recipient.NickName,
                    Name = recipient.GetType().Name
                };
                OutputNodes.Add(outputNode.InstanceGuid, outputNode);
            }
            componentInstanceNode.Outputs.Add(recipient.InstanceGuid);
        }

        /// <summary>
        /// Converts a Bitmap image to a Base64 string.
        /// </summary>
        /// <param name="bitmap">The Bitmap image to convert.</param>
        /// <returns>The Base64 string representation of the Bitmap image.</returns>
        private string ConvertBitmapToBase64(Bitmap bitmap)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Save the bitmap to the memory stream in PNG format
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

                // Convert the memory stream to a byte array
                byte[] bitmapBytes = memoryStream.ToArray();

                // Convert the byte array to a Base64 string
                return Convert.ToBase64String(bitmapBytes);
            }
        }
    }
}
