using System.Net.Http.Json;
using System.Text.Json;
using TaskTracker.Application.Documents.GetProjectDocuments;
using TaskTracker.Application.Projects.AddProjectMember;
using TaskTracker.Application.Projects.AssignProjectManager;
using TaskTracker.Application.Projects.ChangeProjectPriority;
using TaskTracker.Application.Projects.ChangeProjectStatus;
using TaskTracker.Application.Projects.CreateProject;
using TaskTracker.Application.Projects.GetProjectDetails;
using TaskTracker.Application.Projects.GetProjectsList;
using TaskTracker.Application.Projects.UpdateProject;
using TaskTracker.Application.Users.CreateUser;
using TaskTracker.Application.Users.GetUserById;
using TaskTracker.Application.Users.GetUsers;
using TaskTracker.Application.Users.UpdateUser;
using TaskTracker.Application.WorkItems.AssignUser;
using TaskTracker.Application.WorkItems.ChangePriority;
using TaskTracker.Application.WorkItems.ChangeStatus;
using TaskTracker.Application.WorkItems.CreateWorkItem;
using TaskTracker.Application.WorkItems.GetWorkItems;
using TaskTracker.Application.WorkItems.UpdateWorkItem;
using TaskTracker.Domain.Enums;
using TaskTracker.Maui.Common.Responses;

namespace TaskTracker.Maui.Services;

public class ApiService
{
    private readonly HttpClient _http;

    public ApiService(HttpClient http)
    {
        _http = http;
    }
    // ================= PROJECTS =================
    public async Task<ApiResult<List<GetProjectsListResponse>>> GetProjectsAsync(int page = 1, int pageSize = 10)
    {
        var response = await _http.GetAsync($"api/projects?Page={page}&PageSize={pageSize}");
        return await SendAsync<List<GetProjectsListResponse>>(response);
    }

    public async Task<ApiResult<GetProjectDetailsResponse>> GetProjectAsync(int id)
    {
        var response = await _http.GetAsync($"api/projects/{id}");
        return await SendAsync<GetProjectDetailsResponse>(response);
    }

    public async Task<ApiResult<object>> CreateProjectAsync(CreateProjectRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/projects", request);
        return await SendAsync<object>(response);
    }

    public async Task<ApiResult<object>> UpdateProjectAsync(UpdateProjectRequest request)
    {
        var response = await _http.PutAsJsonAsync(
            $"api/projects/{request.Id}",
            request);

        return await SendAsync<object>(response);
    }

    public async Task<ApiResult<object>> DeleteProjectAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/projects/{id}");
        return await SendAsync<object>(response);
    }

    public async Task<ApiResult<object>> ChangeProjectStatusAsync(
    int projectId,
    ProjectStatus status)
    {
        var response = await _http.PutAsJsonAsync(
            $"api/projects/{projectId}/status",
            new ChangeProjectStatusRequest
            {
                ProjectId = projectId,
                Status = status
            });

        return await SendAsync<object>(response);
    }

    public async Task<ApiResult<object>> ChangeProjectPriorityAsync(
    int projectId,
    ProjectPriority priority)
    {
        var response = await _http.PutAsJsonAsync(
            $"api/projects/{projectId}/priority",
            new ChangeProjectPriorityRequest
            {
                ProjectId = projectId,
                Priority = priority
            });

        return await SendAsync<object>(response);
    }

    public async Task<ApiResult<object>> AssignManagerAsync(
    int projectId,
    string userId)
    {
        var response = await _http.PutAsJsonAsync(
            $"api/projects/{projectId}/manager",
            new AssignProjectManagerRequest
            {
                ProjectId = projectId,
                UserId = userId
            });

        return await SendAsync<object>(response);
    }

    public async Task<ApiResult<object>> AddMemberAsync(int projectId, string userId)
    {
        var response = await _http.PostAsJsonAsync(
            $"api/projects/{projectId}/members",
            new AddProjectMemberRequest
            {
                ProjectId = projectId,
                UserId = userId
            });

        return await SendAsync<object>(response);
    }

    public async Task<ApiResult<object>> RemoveMemberAsync(int projectId, string userId)
    {
        var response = await _http.DeleteAsync(
            $"api/projects/{projectId}/members/{userId}");

        return await SendAsync<object>(response);
    }
    // ================= WORKITEMS =================
    public async Task<ApiResult<List<GetWorkItemsResponse>>> GetWorkItemsAsync(int projectId, int page = 1, int pageSize = 5)
    {
        var response = await _http.GetAsync(
            $"api/workitems?ProjectId={projectId}&Page={page}&PageSize={pageSize}");

        return await SendAsync<List<GetWorkItemsResponse>>(response);
    }

    public async Task<ApiResult<object>> CreateWorkItemAsync(CreateWorkItemRequest request)
    {
        var response = await _http.PostAsJsonAsync(
            "api/workitems",
            request);

        return await SendAsync<object>(response);
    }

    public async Task<ApiResult<object>> ChangeWorkItemStatusAsync(
    int workItemId,
    WorkItemStatus status)
    {
        var response = await _http.PutAsJsonAsync(
            $"api/workitems/{workItemId}/status",
            new ChangeWorkItemStatusRequest
            {
                WorkItemId = workItemId,
                Status = status
            });

        return await SendAsync<object>(response);
    }
    public async Task<ApiResult<object>> ChangeWorkItemPriorityAsync(
    int workItemId,
    WorkItemPriority priority)
    {
        var response = await _http.PutAsJsonAsync(
            $"api/workitems/{workItemId}/priority",
            new ChangeWorkItemPriorityRequest
            {
                WorkItemId = workItemId,
                Priority = priority
            });

        return await SendAsync<object>(response);
    }

    public async Task<ApiResult<object>> AssignWorkItemUserAsync(
    int workItemId,
    string userId)
    {
        var response = await _http.PutAsJsonAsync(
            $"api/workitems/{workItemId}/assign",
            new AssignUserRequest
            {
                WorkItemId = workItemId,
                UserId = userId
            });

        return await SendAsync<object>(response);
    }

    public async Task<ApiResult<object>> UpdateWorkItemAsync(UpdateWorkItemRequest request)
    {
        var response = await _http.PutAsJsonAsync(
            $"api/workitems/{request.Id}",
            request);

        return await SendAsync<object>(response);
    }

    public async Task<ApiResult<object>> DeleteWorkItemAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/workitems/{id}");

        return await SendAsync<object>(response);
    }

    // ================= DOCUMENTS =================
    public async Task<List<GetProjectDocumentsResponse>> GetProjectDocumentsAsync(int projectId)
    {
        var result = await _http.GetFromJsonAsync<ApiResponse<List<GetProjectDocumentsResponse>>>(
            $"api/project/documents/{projectId}");

        return result?.Data ?? [];
    }

    public async Task<ApiResult<object>> UploadDocumentAsync(
    int projectId,
    string fileName,
    Stream fileStream)
    {
        using var content = new MultipartFormDataContent();

        content.Add(
            new StreamContent(fileStream),
            "File",
            fileName);

        var response = await _http.PostAsync(
            $"api/project/documents/{projectId}",
            content);

        return await SendAsync<object>(response);
    }

    public async Task<ApiResult<object>> DeleteDocumentAsync(int projectId, int documentId)
    {
        var response = await _http.DeleteAsync(
            $"api/project/documents/{projectId}/{documentId}");

        return await SendAsync<object>(response);
    }

    public async Task<byte[]> DownloadDocumentAsync(int documentId)
    {
        return await _http.GetByteArrayAsync(
            $"api/project/documents/download/{documentId}");
    }

    // ================= USERS =================
    public async Task<ApiResult<List<GetUsersResponse>>> GetUsersAsync(
    string? role = null,
    int page = 1,
    int pageSize = 1000)
    {
        var url = $"api/users?Page={page}&PageSize={pageSize}";

        if (!string.IsNullOrWhiteSpace(role))
            url += $"&Role={role}";

        var response = await _http.GetAsync(url);

        return await SendAsync<List<GetUsersResponse>>(response);
    }

    public async Task<ApiResult<GetUserByIdResponse>> GetUserByIdAsync(string id)
    {
        var response = await _http.GetAsync($"api/users/{id}");

        return await SendAsync<GetUserByIdResponse>(response);
    }

    public async Task<ApiResult<object>> CreateUserAsync(CreateUserRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/users", request);
        return await SendAsync<object>(response);
    }

    public async Task<ApiResult<object>> UpdateUserAsync(UpdateUserRequest request)
    {
        var response = await _http.PutAsJsonAsync($"api/users/{request.UserId}",request);
        return await SendAsync<object>(response);
    }

    public async Task<ApiResult<object>> DeleteUserAsync(string id)
    {
        var response = await _http.DeleteAsync($"api/users/{id}");
        return await SendAsync<object>(response);
    }
    private async Task<ApiResult<T>> SendAsync<T>(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(json))
        {
            return new ApiResult<T>
            {
                Success = response.IsSuccessStatusCode,
                GlobalError = response.IsSuccessStatusCode
                    ? null
                    : response.ReasonPhrase
            };
        }

        try
        {
            var api = JsonSerializer.Deserialize<ApiResponse<T>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (api == null)
            {
                return new ApiResult<T>
                {
                    Success = false,
                    GlobalError = "Invalid server response"
                };
            }

            return new ApiResult<T>
            {
                Success = api.Success,
                Data = api.Data,
                GlobalError = api.Success ? null : api.Message,
                FieldErrors = api.Errors
            };
        }
        catch (JsonException)
        {
            return new ApiResult<T>
            {
                Success = false,
                GlobalError = json
            };
        }
    }
}