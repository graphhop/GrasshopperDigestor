using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphHop.Shared.Data;
using Grasshopper.Kernel;
using static Grasshopper.Rhinoceros.Display.Params.Param_ModelView;

namespace GraphHop.PluginRhino.Utilities
{
    public class GraphStrutObject
    {
        // List to store component definition nodes
        public Dictionary<Guid,ComponentDefinitionNode> ComponentDefinitionNodes = new();

        // List to store component instance nodes
        public Dictionary<Guid,ComponentInstanceNode> ComponentInstanceNodes = new();

        public Dictionary<Guid, DataInputNode> InputNodes = new();

        public Dictionary<Guid, DataOutputNode> OutputNodes = new();

        // Node to store document version information
        public DocumentVersionNode DocumentVersionNode = new DocumentVersionNode();

        // Node to store document information
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
            if(!OutputNodes.ContainsKey(recipient.InstanceGuid))
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
    }
}
