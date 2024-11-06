using OpenAI.Files;
using System;
using System.Diagnostics;
using System.IO;

namespace GraphHop.SharedRhino
{
    public static class ChatGPTUtility
    {
        public static void SaveAndOpenImage(OpenAIFileClient fileClient, string imageFileId)
        {
            OpenAIFile imageInfo = fileClient.GetFile(imageFileId);
            BinaryData imageBytes = fileClient.DownloadFile(imageFileId);

            // Ensure the output directory exists
            string outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "output");
            Directory.CreateDirectory(outputDirectory);

            // Save the image to the output directory
            string imagePath = Path.Combine(outputDirectory, $"{imageInfo.Filename}.png");
            using FileStream stream = File.OpenWrite(imagePath);
            imageBytes.ToStream().CopyTo(stream);

            Debug.WriteLine($"<image: {imagePath}>");

            // Open the image
            Process.Start(new ProcessStartInfo(imagePath) { UseShellExecute = true });
        }
    }
}
