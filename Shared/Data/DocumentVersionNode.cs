using System;

namespace GraphHop.Shared.Data;

/// <summary>
/// A version of a grasshopper document.
/// </summary>
public struct DocumentVersionNode
{
    [IdAttribute]
    public Guid VersionId;
    
    // TODO add properties like timestamp, author, copyright, description, etc
}