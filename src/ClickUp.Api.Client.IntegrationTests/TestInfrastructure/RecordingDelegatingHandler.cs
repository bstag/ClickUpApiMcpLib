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
            // For now, let's assume we always try to record if this handler is in the pipeline
            // and the request is a GET request (simplification for PoC)
            if (request.Method == HttpMethod.Get && response.IsSuccessStatusCode)
            {
                var path = GenerateFilePath(request);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    await File.WriteAllTextAsync(path, content, cancellationToken);
                    Console.WriteLine($"[RecordingDelegatingHandler] Recorded response for {request.Method} {request.RequestUri} to {path}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RecordingDelegatingHandler] Failed to record response for {request.Method} {request.RequestUri} to {path}. Error: {ex.Message}");
                    // Optionally re-throw or handle as needed
                }
            }

            return response;
        }

        private string GenerateFilePath(HttpRequestMessage request)
        {
            var uri = request.RequestUri;
            var method = request.Method.Method; // GET, POST, etc.

            // Simplified naming: method_host_pathsegments_queryhash.json
            // Example: GET_api_clickup_com_api_v2_user.json
            // Example: GET_api_clickup_com_api_v2_team_teamId_space_spaceId_folder.json
            // Using a hash for the query string to keep filenames manageable and somewhat deterministic for identical queries.

            // Expected URI structure: /api/v2/{resource_type}/{id_if_any}/{sub_resource_if_any}
            // Examples:
            // /api/v2/user -> AuthorizationService, GetAuthorizedUser
            // /api/v2/team -> AuthorizationService, GetAuthorizedTeams (Workspaces)
            // /api/v2/space/{space_id} -> SpaceService, GetSpace
            // /api/v2/space -> SpaceService, CreateSpace (POST) or GetSpaces (GET)
            // /api/v2/list/{list_id}/task -> TaskService, GetTasksInList or CreateTaskInList

            var pathSegments = uri.AbsolutePath.Trim('/').Split('/');
            string serviceName = "UnknownService";
            string methodName = request.Method.Method; // Start with HTTP method e.g., GET, POST

            if (pathSegments.Length >= 2 && pathSegments[0] == "api" && pathSegments[1] == "v2")
            {
                // Remove "api" and "v2"
                var resourcePathSegments = pathSegments.Skip(2).ToList();

                if (resourcePathSegments.Any())
                {
                    var primaryResource = resourcePathSegments[0].ToLowerInvariant();
                    resourcePathSegments.RemoveAt(0); // Consume the primary resource segment

                    switch (primaryResource)
                    {
                        case "user":
                            serviceName = "AuthorizationService";
                            methodName += "AuthorizedUser"; // e.g., GetAuthorizedUser
                            break;
                        case "team": // "Team" in API URL often refers to Workspaces
                            serviceName = "AuthorizationService";
                            methodName += "AuthorizedTeams"; // e.g., GetAuthorizedTeams
                            break;
                        case "space":
                            serviceName = "SpaceService";
                            if (resourcePathSegments.Any() && !Guid.TryParse(resourcePathSegments[0], out _)) // e.g. /space/{space_id}/view
                            {
                                methodName += CapitalizeFirstLetter(resourcePathSegments[0]); // e.g. GetViews for /space/{id}/view
                                resourcePathSegments.RemoveAt(0);
                            }
                            else // /space or /space/{id}
                            {
                                methodName += (resourcePathSegments.Any() ? "Space" : "Spaces"); // GetSpace, GetSpaces, CreateSpace, UpdateSpace, DeleteSpace
                            }
                            break;
                        case "folder":
                            serviceName = "FolderService";
                             if (resourcePathSegments.Any() && !Guid.TryParse(resourcePathSegments[0], out _))
                            {
                                methodName += CapitalizeFirstLetter(resourcePathSegments[0]);
                                resourcePathSegments.RemoveAt(0);
                            }
                            else
                            {
                                methodName += (resourcePathSegments.Any() ? "Folder" : "Folders");
                            }
                            break;
                        case "list":
                            serviceName = "ListService";
                            if (resourcePathSegments.Any() && !Guid.TryParse(resourcePathSegments[0], out _)) // e.g. /list/{list_id}/task
                            {
                                methodName += CapitalizeFirstLetter(resourcePathSegments[0]); // e.g., GetTasks, CreateTask (from list)
                                resourcePathSegments.RemoveAt(0);
                            }
                            else
                            {
                                methodName += (resourcePathSegments.Any() ? "List" : "Lists");
                            }
                            break;
                        case "task":
                            serviceName = "TaskService";
                             if (resourcePathSegments.Any() && !Guid.TryParse(resourcePathSegments[0], out _))
                            {
                                methodName += CapitalizeFirstLetter(resourcePathSegments[0]);
                                resourcePathSegments.RemoveAt(0);
                            }
                            else
                            {
                                methodName += (resourcePathSegments.Any() ? "Task" : "Tasks");
                            }
                            break;
                        case "comment": // Assuming /api/v2/comment or similar for a dedicated comment endpoint if it exists
                                        // More likely comments are sub-resources, e.g., /api/v2/task/{task_id}/comment
                            serviceName = "CommentService";
                            methodName += "Comment"; // Needs to be more specific based on actual comment endpoints
                            break;
                        // Add more cases for other services:
                        // Goals, Tags, CustomFields, Members, Guests, Roles, Views, Webhooks etc.
                        // Example for a sub-resource like task comments:
                        // if path was /task/{task_id}/comment, primaryResource would be "task"
                        // then we'd need to check resourcePathSegments for "comment"
                        default:
                            serviceName = CapitalizeFirstLetter(primaryResource) + "Service";
                            methodName += CapitalizeFirstLetter(primaryResource);
                            break;
                    }

                    // Append remaining path segments to method name for specificity if any
                    // e.g. /list/{list_id}/task -> ListService, GetTasks (if GET)
                    // e.g. /task/{task_id}/comment -> TaskService, GetComments (if GET)
                    // This part needs careful thought to align with actual service method names.
                    // The switch above is a start.
                    // For now, the primary resource determines the service and the base of the method name.
                    // If there are sub-resources like /task/{id}/comment, the method name might become GetTaskComments.
                    // This simple version might result in GetComment for /task/{id}/comment which is okay for now.
                }
                else
                {
                    methodName += "Root"; // e.g. GetRoot for /api/v2/
                }
            }
            else
            {
                // Non-standard path, use a generic structure
                var pathPart = string.Join("_", pathSegments).Replace(".", "_");
                methodName += CapitalizeFirstLetter(pathPart);
            }

            // Default scenario name
            var scenarioName = "RecordedResponse"; // Base name, will append .json and possibly query hash

            if (request.Method == HttpMethod.Get && !string.IsNullOrWhiteSpace(uri.Query))
            {
                using var sha256 = SHA256.Create();
                var queryBytes = Encoding.UTF8.GetBytes(uri.Query);
                var queryHashBytes = sha256.ComputeHash(queryBytes);
                var queryHash = BitConverter.ToString(queryHashBytes).Replace("-", "").ToLowerInvariant().Substring(0, 8);
                scenarioName += $"_{queryHash}";
            }
            scenarioName += ".json";

            // Construct the full path
            var directoryPath = Path.Combine(_basePath, serviceName, methodName);
            var filePath = Path.Combine(directoryPath, scenarioName);

            // Log the generated path for debugging
            Console.WriteLine($"[RecordingDelegatingHandler] Determined Path: Service='{serviceName}', Method='{methodName}', File='{filePath}' for URI '{uri}'");

            return filePath;
        }

        private static string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }
}
