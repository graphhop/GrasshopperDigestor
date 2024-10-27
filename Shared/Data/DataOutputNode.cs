using System;

namespace GraphHop.Shared.Data;

/// <summary>
/// An output of an instance of a component.
/// </summary>
public struct DataOutputNode
{
    /// <summary>
    /// https://developer.rhino3d.com/api/grasshopper/html/P_Grasshopper_Kernel_GH_InstanceDescription_InstanceGuid.htm
    /// </summary>
    [IdAttribute]
    [EqualityCheck]
    public Guid InstanceGuid;

    /// <summary>
    /// Guid of the node thid Output is connected from
    /// </summary>
    public Guid TargetGuid;

    /// <summary>
    /// https://developer.rhino3d.com/api/grasshopper/html/P_Grasshopper_Kernel_GH_InstanceDescription_Name.htm
    /// </summary>
    [Serialize]
    public string Name;

    /// <summary>
    /// https://developer.rhino3d.com/api/grasshopper/html/P_Grasshopper_Kernel_GH_InstanceDescription_NickName.htm
    /// </summary>
    [EqualityCheck] 
    public string NickName;

    // TODO: Reverse, DataMapping, etc
}