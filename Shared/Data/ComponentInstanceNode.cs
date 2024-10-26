﻿using System;

namespace GraphHop.SharedRhino.Data;

public struct ComponentInstanceNode
{
    /// <summary>
    /// https://developer.rhino3d.com/api/grasshopper/html/P_Grasshopper_Kernel_GH_InstanceDescription_InstanceGuid.htm
    /// </summary>
    public Guid InstanceGuid;

    /// <summary>
    /// https://developer.rhino3d.com/api/grasshopper/html/P_Grasshopper_Kernel_GH_InstanceDescription_NickName.htm
    /// </summary>
    public string NickName;

    /// <summary>
    /// Pivot point of the component instance, X-coordinate.
    /// https://developer.rhino3d.com/api/grasshopper/html/P_Grasshopper_Kernel_IGH_Attributes_Pivot.htm
    /// </summary>
    public float X;

    /// <summary>
    /// Pivot point of the component instance, Y-coordinate.
    /// https://developer.rhino3d.com/api/grasshopper/html/P_Grasshopper_Kernel_IGH_Attributes_Pivot.htm
    /// </summary>
    public float Y;
}