﻿using System;

namespace GraphHop.Shared.Data;

/// <summary>
/// An output of an instance of a component.
/// </summary>
public struct DataOutputNode
{
    /// <summary>
    /// https://developer.rhino3d.com/api/grasshopper/html/P_Grasshopper_Kernel_GH_InstanceDescription_InstanceGuid.htm
    /// </summary>
    public Guid InstanceGuid;

    /// <summary>
    /// https://developer.rhino3d.com/api/grasshopper/html/P_Grasshopper_Kernel_GH_InstanceDescription_NickName.htm
    /// </summary>
    public string NickName;

    // TODO: Reverse, DataMapping, etc
}