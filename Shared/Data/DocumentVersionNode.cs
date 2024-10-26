using System;

namespace GraphHop.SharedRhino.Data;

/// <summary>
/// A version of a grasshopper document.
/// </summary>
public struct DocumentVersionNode
{
    public Guid VersionId;
    
    // TODO add properties like timestamp, author, copyright, description, etc
}