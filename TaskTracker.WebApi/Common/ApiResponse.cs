namespace TaskTracker.Api.Common;

public class ApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<ApiError>? Errors { get; set; }

    public static ApiResponse Ok(string? message = null)
        => new() { Success = true, Message = message };

    public static ApiResponse<T> Ok<T>(T data, string? message = null)
        => new() { Success = true, Data = data, Message = message };

    public static ApiResponse Fail(string message)
        => new() { Success = false, Message = message };
}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }
}

public class ApiError
{
    public string Field { get; set; } = null!;
    public string Error { get; set; } = null!;
}