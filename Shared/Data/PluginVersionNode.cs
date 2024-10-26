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
    public string Version;

    /// <summary>
    /// The plugin's assembly version.
    /// </summary>
    public Guid AssemblyVersion;
    
}