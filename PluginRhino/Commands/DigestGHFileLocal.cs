using System.IO;
using Grasshopper.Kernel;
using GraphHop.SharedRhino;
using Rhino;
using Rhino.Commands;
using OpenAI.Chat;
using OpenAI.Files;
using OpenAI.Assistants;
using System;
using System.Diagnostics;
using System.Threading;
using OpenAI;
using System.ClientModel;

namespace GraphHop.PluginRhino.Commands
{
    public class DigestGHFileLocal : Command
    {
        public DigestGHFileLocal()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static DigestGHFileLocal Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "DigestGHFileLocal";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            /// The following code shows a possibility to differentiate between versions of RhinoCommon and .NET at compile time.

            /// The RHINOCOMMON_* constants are defined in "CommonReferencesRhino.csproj"
#if RHINOCOMMON_EQUAL_7
                    RhinoApp.WriteLine("RHINOCOMMON_EQUAL_7 is defined.");
                    return Result.Failure;
#endif

#if RHINOCOMMON_EQUAL_8
            //RhinoApp.WriteLine("RHINOCOMMON_EQUAL_8 is defined.");
#endif

#if RHINOCOMMON_GREATER_EQUAL_7
            //RhinoApp.WriteLine("RHINOCOMMON_GREATER_EQUAL_7 is defined.");
#endif

#if RHINOCOMMON_GREATER_EQUAL_8
            //RhinoApp.WriteLine("RHINOCOMMON_GREATER_EQUAL_8 is defined.");
#endif

            /// see https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives
#if NETFRAMEWORK
                    RhinoApp.WriteLine("NETFRAMEWORK is defined.");
                    return Result.Failure;
#endif

#if NET7_0_OR_GREATER
            RhinoApp.WriteLine("NET7_0_OR_GREATER is defined.");
#endif
            var fileDialog = new Rhino.UI.OpenFileDialog
            {
                Filter = "Grasshopper Files (*.gh;*.ghx)|*.gh;*.ghx",
                Title = "Select a Grasshopper File"
            };
            if (!fileDialog.ShowOpenDialog())
                return Result.Cancel;
            string filePath = fileDialog.FileName;
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                RhinoApp.WriteLine("Invalid file path.");
                return Result.Failure;
            }

            GHDigestUtility gHDigestUtility = new GHDigestUtility();
            if (!gHDigestUtility.LoadDocument(filePath, out var errmsg))
            {
                RhinoApp.WriteLine("Failed to load the Grasshopper document.");
                return Result.Failure;
            }

            string ParsedGHString = gHDigestUtility.ParseGHFileNoPrint();

            Debug.WriteLine("----------------------------------");

            // Ensure the API key is stored in an environment variable named OPENAI_API_KEY
            string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                RhinoApp.WriteLine("OpenAI API key is not set.");
                return Result.Failure;
            }

            // Create the OpenAI client
            // Assistants is a beta API and subject to change; acknowledge its experimental status by suppressing the matching warning.
#pragma warning disable OPENAI001
            OpenAIClient openAIClient = new(apiKey);
            OpenAIFileClient fileClient = openAIClient.GetOpenAIFileClient();
            AssistantClient assistantClient = openAIClient.GetAssistantClient();

            // Upload the ParsedGHString as a document
            using Stream document = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(ParsedGHString));
            OpenAIFile ghFile = fileClient.UploadFile(document, "parsed_gh_file.txt", FileUploadPurpose.Assistants);

            // Create an assistant with the necessary tools
            AssistantCreationOptions assistantOptions = new()
            {
                Name = "GH File Assistant",
                Instructions = "You are an assistant that helps answer questions about a Grasshopper file. " +
                "When asked to generate a graph, chart, or other visualization, use the code interpreter tool to do so.",
                Tools =
                        {
                            new FileSearchToolDefinition(),
                            new CodeInterpreterToolDefinition(),
                        },
                ToolResources = new()
                {
                    FileSearch = new()
                    {
                        NewVectorStores =
                                {
                                    new VectorStoreCreationHelper(new[] { ghFile.Id }),
                                }
                    }
                },
            };

            Assistant assistant = assistantClient.CreateAssistant("gpt-4o", assistantOptions);

            // Create a thread with a user query about the data
            ThreadCreationOptions threadOptions = new()
            {
                InitialMessages = { "Please summarize the purpose and function of Grasshopper file. and draw a diagram representing the rough workflow" }
            };

            ThreadRun threadRun = assistantClient.CreateThreadAndRun(assistant.Id, threadOptions);

            // Check back to see when the run is done
            do
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                threadRun = assistantClient.GetRun(threadRun.ThreadId, threadRun.Id);
            } while (!threadRun.Status.IsTerminal);

            // Print out the full history for the thread that includes the augmented generation
            CollectionResult<ThreadMessage> messages = assistantClient.GetMessages(threadRun.ThreadId, new MessageCollectionOptions() { Order = MessageCollectionOrder.Ascending });

            foreach (ThreadMessage message in messages)
            {
                Debug.Write($"[{message.Role.ToString().ToUpper()}]: ");
                foreach (MessageContent contentItem in message.Content)
                {
                    if (!string.IsNullOrEmpty(contentItem.Text))
                    {
                        Debug.WriteLine($"{contentItem.Text}");

                        if (contentItem.TextAnnotations.Count > 0)
                        {
                            Debug.WriteLine("");
                        }

                        // Include annotations, if any.
                        foreach (TextAnnotation annotation in contentItem.TextAnnotations)
                        {
                            if (!string.IsNullOrEmpty(annotation.InputFileId))
                            {
                                Debug.WriteLine($"* File citation, file ID: {annotation.InputFileId}");
                            }
                            if (!string.IsNullOrEmpty(annotation.OutputFileId))
                            {
                                Debug.WriteLine($"* File output, new file ID: {annotation.OutputFileId}");
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(contentItem.ImageFileId))
                    {
                        OpenAIFile imageInfo = fileClient.GetFile(contentItem.ImageFileId);
                        BinaryData imageBytes = fileClient.DownloadFile(contentItem.ImageFileId);
                        using FileStream stream = File.OpenWrite($"{imageInfo.Filename}.png");
                        imageBytes.ToStream().CopyTo(stream);

                        Debug.WriteLine($"<image: {imageInfo.Filename}.png>");
                    }
                }
                Debug.WriteLine("");
            }

            // Optionally, delete any persistent resources you no longer need.
            _ = assistantClient.DeleteThread(threadRun.ThreadId);
            _ = assistantClient.DeleteAssistant(assistant.Id);
            _ = fileClient.DeleteFile(ghFile.Id);

            return Result.Success;
        }
    }
}
