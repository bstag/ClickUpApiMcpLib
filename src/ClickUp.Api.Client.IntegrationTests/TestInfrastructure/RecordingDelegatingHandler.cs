using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.IntegrationTests.TestInfrastructure
{
    public class RecordingDelegatingHandler : DelegatingHandler
    {
        private readonly string _basePath;
        // HttpClientHandler is a concrete HttpMessageHandler.
        // It's often used as the default final handler in the chain.
        private static readonly HttpMessageHandler _defaultInnerHandler = new HttpClientHandler();


        // Used to ensure that the handler is added only once per HttpClient.
        public bool IsAdded { get; internal set; }

        public RecordingDelegatingHandler(string basePath)
        {
            _basePath = basePath;
            // DelegatingHandler.InnerHandler expects an HttpMessageHandler.
            // If this handler is the last custom one before the actual transport,
            // it might be set by the HttpClient pipeline itself.
            // However, setting a default can be useful if it's used standalone or if the pipeline doesn't set one.
            // For now, let's rely on the pipeline to set it if this handler is added via AddHttpMessageHandler.
            // If constructing manually and it's the final delegating handler, then:
            // InnerHandler = new HttpClientHandler(); // or a shared instance
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            // Check if recording is enabled (e.g., via a toggle or if the response is successful)
            // Record all responses now
            var path = GenerateFilePath(request, response); // Pass response to include status for unique error file names
            var content = await response.Content.ReadAsStringAsync(cancellationToken); // Read content regardless of status

            try
            {
                var directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                await File.WriteAllTextAsync(path, content, cancellationToken);
                Console.WriteLine($"[RecordingDelegatingHandler] Recorded response for {request.Method} {request.RequestUri} (Status: {(int)response.StatusCode}) to {path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RecordingDelegatingHandler] Failed to record response for {request.Method} {request.RequestUri} to {path}. Error: {ex.Message}");
            }

            return response;
        }

        private string GenerateFilePath(HttpRequestMessage request, HttpResponseMessage httpResponse)
        {
            var uri = request.RequestUri;
            var method = request.Method.Method; // GET, POST, etc.
            var statusCode = (int)httpResponse.StatusCode;

            if (uri == null)
            {
                // Handle cases where RequestUri is null, though this is rare for outgoing client requests.
                // You might want to log this or generate a specific "UnknownUri" path.
                Console.WriteLine($"[RecordingDelegatingHandler] RequestUri is null for method {method}. Using default path.");
                return Path.Combine(_basePath, "UnknownService", $"{method}_UnknownUri", $"{(httpResponse.IsSuccessStatusCode ? "Success" : $"Error_{statusCode}")}.json");
            }

            var pathSegments = uri.AbsolutePath.Trim('/').Split('/');
            string serviceName = "UnknownService";
            string derivedMethodNamePart = "UnknownOperation"; // This will be like "GetSpace", "CreateTask"

            // Handle v3 paths (e.g., /v3/workspaces/{id}/channels)
            if (pathSegments.Length >= 1 && pathSegments[0].ToLowerInvariant() == "v3")
            {
                // Example: /v3/workspaces/{workspace_id}/channels/{channel_id}/messages
                // relevantPathSegments for this example: ["workspaces", "{workspace_id}", "channels", "{channel_id}", "messages"]
                var relevantPathSegments = pathSegments.Skip(1).ToList();
                if (relevantPathSegments.Any())
                {
                    // Determine service based on primary resource after /v3/
                    // For chat: /v3/workspaces/{id}/channels -> ChatService
                    // For chat: /v3/chat/messages/{id} -> ChatService
                    if ((relevantPathSegments.Contains("workspaces") && relevantPathSegments.Contains("channels")) ||
                        (relevantPathSegments.Contains("chat") && relevantPathSegments.Contains("messages")))
                    {
                        serviceName = "ChatService";
                        if (relevantPathSegments.Contains("channels"))
                        {
                            // /v3/workspaces/{wsId}/channels
                            // /v3/workspaces/{wsId}/channels/{chId}
                            // /v3/workspaces/{wsId}/channels/{chId}/messages
                            // /v3/workspaces/{wsId}/channels/{chId}/followers
                            // /v3/workspaces/{wsId}/channels/{chId}/members
                            derivedMethodNamePart = "Channels"; // Default for channel operations
                            if (relevantPathSegments.Last().ToLowerInvariant() == "messages") derivedMethodNamePart = "ChannelMessages";
                            else if (relevantPathSegments.Last().ToLowerInvariant() == "followers") derivedMethodNamePart = "ChannelFollowers";
                            else if (relevantPathSegments.Last().ToLowerInvariant() == "members") derivedMethodNamePart = "ChannelMembers";
                            // If an ID follows "channels", it's an operation on a specific channel.
                            // e.g. POST /channels -> CreateChannel, GET /channels/{id} -> GetChannel
                            // The HTTP method (GET, POST) will prefix this.
                        }
                        else if (relevantPathSegments.Contains("messages"))
                        {
                            // /v3/chat/messages/{msgId}
                            // /v3/chat/messages/{msgId}/reactions
                            // /v3/chat/messages/{msgId}/tagged_users
                            // /v3/chat/messages/{msgId}/messages (replies)
                            derivedMethodNamePart = "Messages"; // Default for message operations
                            if (relevantPathSegments.Last().ToLowerInvariant() == "reactions") derivedMethodNamePart = "MessageReactions";
                            else if (relevantPathSegments.Last().ToLowerInvariant() == "tagged_users") derivedMethodNamePart = "MessageTaggedUsers";
                            else if (relevantPathSegments.Contains("messages") && relevantPathSegments.Count(s => s=="messages") > 1) derivedMethodNamePart = "MessageReplies";

                        }
                    }
                    else
                    {
                        // Fallback for other potential v3 services
                        serviceName = CapitalizeFirstLetter(relevantPathSegments.First()) + "V3Service";
                        derivedMethodNamePart = CapitalizeFirstLetter(relevantPathSegments.Last());
                    }
                }
            }
            // Handle v2 paths (e.g., /api/v2/user)
            else if (pathSegments.Length >= 3 && pathSegments[0].ToLowerInvariant() == "api" && pathSegments[1].ToLowerInvariant() == "v2")
            {
                var relevantPathSegments = pathSegments.Skip(2).ToList();
                if (relevantPathSegments.Any())
                {
                    var primaryResource = relevantPathSegments[0].ToLowerInvariant();

                    switch (primaryResource)
                    {
                        case "user":
                            serviceName = "AuthorizationService";
                            derivedMethodNamePart = "AuthorizedUser";
                            break;
                        case "team":
                            // Check if the path is like /team/{team_id}/space
                            if (relevantPathSegments.Count > 1 && relevantPathSegments.Contains("space"))
                            {
                                serviceName = "SpaceService";
                                derivedMethodNamePart = (request.Method == HttpMethod.Post) ? "CreateSpace" : "GetSpaces";
                            }
                            else if (relevantPathSegments.Count > 2 && relevantPathSegments[2].ToLowerInvariant() == "user") // /team/{team_id}/user/{user_id}
                            {
                                serviceName = "UsersService";
                                derivedMethodNamePart = "User"; // Will be GETUser, PUTUser, DELETEUser
                            }
                            else // /team or /team/{team_id} (not user-specific)
                            {
                                serviceName = "AuthorizationService"; // Or TeamService if distinct
                                derivedMethodNamePart = "AuthorizedTeams"; // Or Team operation
                            }
                            break;
                        case "space":
                            serviceName = "SpaceService";
                            // /space/{space_id} -> GetSpace, UpdateSpace, DeleteSpace
                            // For sub-resources like /space/{id}/folder, /space/{id}/list, etc.
                            if (relevantPathSegments.Count > 2 && IsPotentialGuid(relevantPathSegments[1])) // e.g. /space/{id}/subResource
                            {
                                derivedMethodNamePart = CapitalizeFirstLetter(relevantPathSegments[2]); // e.g. Folder, List, Tag, View
                            }
                            else // Just /space/{id} or /space
                            {
                                derivedMethodNamePart = "Space"; // Will be prefixed with HTTP method later
                            }
                            break;
                        case "folder":
                            serviceName = "FolderService";
                            if (relevantPathSegments.Count > 2 && IsPotentialGuid(relevantPathSegments[1]))  // e.g. /folder/{id}/subResource
                            {
                                derivedMethodNamePart = CapitalizeFirstLetter(relevantPathSegments[2]);
                            }
                            else
                            {
                                derivedMethodNamePart = "Folder";
                            }
                            break;
                        case "list":
                            serviceName = "ListService";
                            if (relevantPathSegments.Count > 2 && IsPotentialGuid(relevantPathSegments[1])) // e.g. /list/{id}/subResource
                            {
                                derivedMethodNamePart = CapitalizeFirstLetter(relevantPathSegments[2]); // e.g. Task, Member
                            }
                            else
                            {
                                derivedMethodNamePart = "List";
                            }
                            break;
                        case "task":
                            serviceName = "TaskService";
                             if (relevantPathSegments.Count > 2 && IsPotentialGuid(relevantPathSegments[1])) // e.g. /task/{id}/subResource
                            {
                                derivedMethodNamePart = CapitalizeFirstLetter(relevantPathSegments[2]); // e.g. Comment, Time, Checklist
                            }
                            else
                            {
                                derivedMethodNamePart = "Task";
                            }
                            break;
                        case "comment":
                            serviceName = "CommentService";
                            derivedMethodNamePart = "Comment"; // Operations are usually on /comment/{id} or POST to parent
                            break;
                        // Add other primary resources here
                        default:
                            serviceName = CapitalizeFirstLetter(primaryResource) + "Service";
                            derivedMethodNamePart = CapitalizeFirstLetter(primaryResource);
                            break;
                    }
                }
                else
                {
                    serviceName = "RootService"; // e.g. for /api/v2/
                    derivedMethodNamePart = "RootOperation";
                }
            }
            else
            {
                var pathPart = string.Join("_", pathSegments).Replace(".", "_");
                serviceName = "ExternalOrUnknown";
                derivedMethodNamePart = CapitalizeFirstLetter(pathPart);
            }

            // Construct methodName like "GetSpace", "CreateTask", "DeleteComment"
            string fullMethodName = request.Method.Method + derivedMethodNamePart;


            // Scenario name based on success/error and hashes
            var scenarioName = $"{(httpResponse.IsSuccessStatusCode ? "Success" : $"Error_{statusCode}")}";

            string queryOrBodyHash = string.Empty;
            if (request.Method == HttpMethod.Get && !string.IsNullOrWhiteSpace(uri.Query))
            {
                using var sha256 = SHA256.Create();
                var queryBytes = Encoding.UTF8.GetBytes(uri.Query);
                var queryHashBytes = sha256.ComputeHash(queryBytes);
                queryOrBodyHash = BitConverter.ToString(queryHashBytes).Replace("-", "").ToLowerInvariant().Substring(0, 8);
                scenarioName += $"_query{queryOrBodyHash}";
            }
            else if (request.Method == HttpMethod.Post || request.Method == HttpMethod.Put)
            {
                var requestBody = request.Content?.ReadAsStringAsync().Result ?? string.Empty; // Sync for simplicity here
                if (!string.IsNullOrEmpty(requestBody))
                {
                    using var sha256 = SHA256.Create();
                    var bodyBytes = Encoding.UTF8.GetBytes(requestBody);
                    var bodyHashBytes = sha256.ComputeHash(bodyBytes);
                    queryOrBodyHash = BitConverter.ToString(bodyHashBytes).Replace("-", "").ToLowerInvariant().Substring(0, 8);
                    scenarioName += $"_body{queryOrBodyHash}";
                }
            }
            // For DELETE or GET without query and POST/PUT without body, scenarioName remains "Success" or "Error_XXX"

            scenarioName += ".json";

            var directoryPath = Path.Combine(_basePath, serviceName, fullMethodName);
            var filePath = Path.Combine(directoryPath, scenarioName);

            Console.WriteLine($"[RecordingDelegatingHandler] Determined Path: Service='{serviceName}', Method='{fullMethodName}', File='{filePath}' for URI '{uri}'");

            return filePath;
        }

                private static bool IsPotentialGuid(string segment)
                {
                    // Basic check, ClickUp IDs are not always GUIDs but often numeric or alphanumeric strings.
                    // This is a heuristic. A true GUID check is Guid.TryParse.
                    // For ClickUp, IDs can be plain numbers, or strings like "abc123xyz".
                    // We're checking if it's NOT a known sub-resource keyword.
                    // A more robust way would be to check if it matches known sub-resource names like "view", "tag", "comment", etc.
                    // For now, if it's not one of the explicit sub-resource checks above in the switch,
                    // and it's a segment after a primary resource, it's often an ID.
                    // This simple check helps differentiate /space/{id} from /space/feature or similar.
                    return !string.IsNullOrWhiteSpace(segment) && segment.Length > 3; // Arbitrary length, many IDs are longer
                }

        private static string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }
}
