namespace TaskTracker.Maui.Common.Responses;

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<ApiError>? Errors { get; set; }
}
public class ApiResult<T>
{
    public T? Data { get; set; }
    public bool Success { get; set; }

    public string? GlobalError { get; set; }

    public List<ApiError>? FieldErrors { get; set; }

    public bool HasFieldErrors => FieldErrors?.Any() == true;
}

public class ApiError
{
    public string Field { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
}

