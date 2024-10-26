using System;
using System.Diagnostics;
using GH_IO.Serialization;
using static System.ConsoleColor;

namespace GraphHop.SharedRhino;

public class GrasshopperParser
{

    public static void LoadGHFile(string filePath)
    {
        GH_Archive fileArchive = new();
        fileArchive.ReadFromFile(filePath);
        DisplayDocumentStatistics(fileArchive);
    }
    
    private static void DisplayDocumentStatistics(GH_Archive archive)
    {
        var root = archive.GetRootNode;
        var definition = root.FindChunk("Definition");
        var header = definition.FindChunk("DocumentHeader");
        var properties = definition.FindChunk("DefinitionProperties");

        var archiveVersion = root.GetVersion("ArchiveVersion");
        var pluginVersion = definition.GetVersion("plugin_version");
        var documentId = header.GetGuid("DocumentID");
        var documentDate = properties.GetDate("Date");
        var documentName = properties.GetString("Name");

        var objects = definition.FindChunk("DefinitionObjects");
        var numObjects = objects.GetInt32("ObjectCount");

        Debug.WriteLine($"Archive Version: {archiveVersion.ToString()}");
        Debug.WriteLine($"Plug-In Version: {pluginVersion.ToString()}");
        Debug.WriteLine($"Document ID:     {{{documentId}}}");
        Debug.WriteLine($"Document Date:   {documentDate.ToLongDateString()}");
        Debug.WriteLine($"Document Name:   {documentName}");
        Debug.WriteLine($"Number of Objects: {numObjects}");
    }
}