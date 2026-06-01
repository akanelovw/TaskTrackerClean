using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Api.Common;

namespace TaskTracker.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
    }

    private static async Task HandleException(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        context.Response.StatusCode = ex switch
        {
            BadRequestException => 400,
            NotFoundException => 404,
            ForbiddenException => 403,
            UnauthorizedException => 401,
            _ => 500
        };

        var response = new ApiResponse<object>
        {
            Success = false,
            Message = ex.Message,
            Errors = new List<ApiError>(),
            Data = null
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}