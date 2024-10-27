using System;

namespace GraphHop.Shared.Data;

/// <summary>
/// The definition (aka an actual implementation) of a component. This is independent of Grasshopper documents.
/// </summary>
public struct ComponentDefinitionNode
{
    /// <summary>
    /// https://developer.rhino3d.com/api/grasshopper/html/P_Grasshopper_Kernel_GH_DocumentObject_ComponentGuid.htm
    /// </summary>
    [IdAttribute]
    [EqualityCheck]
    public Guid ComponentGuid;

    /// <summary>
    /// https://developer.rhino3d.com/api/grasshopper/html/P_Grasshopper_Kernel_GH_InstanceDescription_Name.htm
    /// </summary>
    [EqualityCheck]
    public string Name;
    
    /// <summary>
    /// Icon
    /// </summary>
    [Serialize]
    public string Icon;

    /// <summary>
    /// https://developer.rhino3d.com/api/grasshopper/html/P_Grasshopper_Kernel_GH_InstanceDescription_Description.htm
    /// </summary>
    [Serialize]
    public string Description;
}