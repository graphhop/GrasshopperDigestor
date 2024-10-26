using System;

namespace GraphHop.Shared.Data;

public struct DataOutputNode
{
    /// <summary>
    /// https://developer.rhino3d.com/api/grasshopper/html/P_Grasshopper_Kernel_GH_InstanceDescription_InstanceGuid.htm
    /// </summary>
    public Guid InstanceGuid;

    /// <summary>
    /// Guid of the node thid Output is connected from
    /// </summary>
    public Guid TargetGuid;

    /// <summary>
    /// https://developer.rhino3d.com/api/grasshopper/html/P_Grasshopper_Kernel_GH_InstanceDescription_Name.htm
    /// </summary>
    public string Name;

    /// <summary>
    /// https://developer.rhino3d.com/api/grasshopper/html/P_Grasshopper_Kernel_GH_InstanceDescription_NickName.htm
    /// </summary>
    public string NickName;

    // TODO: Reverse, DataMapping, etc
}