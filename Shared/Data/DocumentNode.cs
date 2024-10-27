using System;

namespace GraphHop.Shared.Data;

/// <summary>
/// A grasshopper document.
/// </summary>
public struct DocumentNode
{
    /// <summary>
    /// https://developer.rhino3d.com/api/grasshopper/html/P_Grasshopper_Kernel_GH_Document_DocumentID.htm
    /// </summary>
    [IdAttribute]
    [EqualityCheck]
    public Guid DocumentID;

    /// <summary>
    /// The author of the document.
    /// </summary>
    [Serialize]
    public string Author;

    /// <summary>
    /// The display name of the document.
    /// </summary>
    [Serialize]
    public string DisplayName;

    /// <summary>
    /// The file path of the document.
    /// </summary>
    [Serialize]
    public string FilePath;

    /// <summary>
    /// The owner of the document.
    /// </summary>
    [Serialize]
    public string Owner;
}
