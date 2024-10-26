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
        // add ComponentInstanceNode for each document object
        //if ComponentDefinitionNode doesnt exist add one
        public List<ComponentDefinitionNode> ComponentDefinitionNodes = new List<ComponentDefinitionNode>();
        public List<ComponentInstanceNode> ComponentInstanceNodes = new List<ComponentInstanceNode>();
        public DocumentVersionNode DocumentVersionNode = new DocumentVersionNode();
        public DocumentNode DocumentNode = new DocumentNode();

        public void GetConnectedObjects(IGH_DocumentObject obj, ComponentInstanceNode componentInstanceNode)
        {
            // Check if the object is a parameter
            if (obj is IGH_Param param)
            {
                // Get the sources (inputs)
                foreach (var source in param.Sources)
                {
                    // Process each source
                    ProcessInput(source, componentInstanceNode);
                }

                // Get the recipients (outputs)
                foreach (var recipient in param.Recipients)
                {
                    // Process each recipient
                    ProcessOutput(recipient, componentInstanceNode);
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
                        ProcessInput(source, componentInstanceNode);
                    }
                }

                // Get the outputs
                foreach (var output in component.Params.Output)
                {
                    foreach (var recipient in output.Recipients)
                    {
                        // Process each recipient
                        ProcessOutput(recipient, componentInstanceNode);
                    }
                }
            }
            else
            {
                Debug.WriteLine("The object is neither a parameter nor a component.");
            }
        }

        public void IterateDocumentObjects(GH_Document ghDocument)
        {
            try
            {
                PopulateDocumentProperties(ghDocument);
                // Iterate through all document objects and print their names
                foreach (IGH_DocumentObject obj in ghDocument.Objects)
                {
                    //check if object is a new component definition
                    if (ComponentDefinitionNodes.All(x => x.ComponentGuid != obj.ComponentGuid))
                    {
                        ComponentDefinitionNodes.Add(new ComponentDefinitionNode
                        {
                            ComponentGuid = obj.ComponentGuid,
                            Name = obj.Name,
                            //ComponentCategory = obj.Category,
                            //ComponentSubCategory = obj.SubCategory,
                            //ComponentDescription = obj.Description
                        });
                    }
                    var componentInstanceNode = PopulateDocumentObjectProperties(obj);
                    GetConnectedObjects(obj, componentInstanceNode);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void PopulateDocumentProperties(GH_Document ghDocument)
        {
            DocumentNode.DocumentID = ghDocument.DocumentID;
            //Update Version each time the struct object is instantiated
            DocumentVersionNode.VersionId = Guid.NewGuid();
        }

        public ComponentInstanceNode PopulateDocumentObjectProperties(IGH_DocumentObject obj)
        {
            ComponentInstanceNode componentInstanceNode = new ComponentInstanceNode
            {
                InstanceGuid = obj.InstanceGuid,
                NickName = obj.NickName,
                X = obj.Attributes.Pivot.X,
                Y = obj.Attributes.Pivot.Y
            };

            ComponentInstanceNodes.Add(componentInstanceNode);
            return componentInstanceNode;
        }

        private void ProcessInput(IGH_Param source, ComponentInstanceNode componentInstanceNode)
        {
            componentInstanceNode.Inputs.Add(new DataInputNode
            {
                TargetGuid = source.Attributes?.GetTopLevel?.DocObject?.InstanceGuid??Guid.Empty,
                InstanceGuid = source.InstanceGuid,
                NickName = source.NickName,
                Name = source.GetType().Name
            });

        }

        private void ProcessOutput(IGH_Param recipient, ComponentInstanceNode componentInstanceNode)
        {
            componentInstanceNode.Outputs.Add(new DataOutputNode
            {
                TargetGuid = recipient.Attributes?.GetTopLevel?.DocObject?.InstanceGuid ?? Guid.Empty,
                InstanceGuid = recipient.InstanceGuid,
                NickName = recipient.NickName,
                Name = recipient.GetType().Name
            });
        }
    }
}
