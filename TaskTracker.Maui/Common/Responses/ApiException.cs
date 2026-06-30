namespace TaskTracker.Maui.Common.Responses;

public class ApiException : Exception
{
    public ApiException(string message) : base(message)
    {
    }
}