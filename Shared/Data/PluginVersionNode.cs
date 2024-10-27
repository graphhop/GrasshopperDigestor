using GraphHop.Shared.Data;
using System;

namespace GraphHop.SharedRhino.Data;

/// <summary>
/// A version of a plugin.
/// </summary>
public struct PluginVersionNode
{
    /// <summary>
    /// The plugin's version.
    /// </summary>
    [EqualityCheck]
    public string Version;

    /// <summary>
    /// The plugin's assembly version.
    /// </summary>
    [IdAttribute]
    [EqualityCheck]
    public Guid AssemblyVersion;
    
}