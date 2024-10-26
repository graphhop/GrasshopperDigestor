using GraphHop.Shared.Data;
using System;

namespace GraphHop.SharedRhino.Data;

/// <summary>
/// A plugin.
/// </summary>
public struct PluginNode
{
    /// <summary>
    /// The plugin's name.
    /// </summary>
    public string Name;

    /// <summary>
    /// The plugin's unique ID.
    /// </summary>
    [IdAttribute]
    public Guid Guid;
    
}