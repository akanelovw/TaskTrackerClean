using CommunityToolkit.Mvvm.ComponentModel;
using TaskTracker.Maui.Common.Responses;

namespace TaskTracker.Maui.Features.Base;

public abstract class BaseViewModel : ObservableObject
{
    protected async Task HandleResult<T>(
    ApiResult<T> result,
    Action<T>? onSuccess = null)
    {
        if (result.FieldErrors?.Any() == true)
        {
            ApplyFieldErrors(result.FieldErrors);
            return;
        }

        if (!string.IsNullOrWhiteSpace(result.GlobalError))
        {
            await Shell.Current.DisplayAlertAsync(
                "Error",
                result.GlobalError,
                "OK");

            return;
        }

        if (result.Success && result.Data != null)
        {
            onSuccess?.Invoke(result.Data);
        }
    }
    protected async Task HandleResult<T>(
    ApiResult<T> result,
    Func<Task>? onSuccess)
    {
        if (result.FieldErrors?.Any() == true)
        {
            ApplyFieldErrors(result.FieldErrors);
            return;
        }

        if (!string.IsNullOrWhiteSpace(result.GlobalError))
        {
            await Shell.Current.DisplayAlertAsync(
                "Error",
                result.GlobalError,
                "OK");

            return;
        }

        if (result.Success)
        {
            if (onSuccess != null)
                await onSuccess();
        }
    }
    protected async Task HandleResult<T>(
    ApiResult<T> result,
    Func<T, Task>? onSuccess)
    {
        if (result.FieldErrors?.Any() == true)
        {
            ApplyFieldErrors(result.FieldErrors);
            return;
        }

        if (!string.IsNullOrWhiteSpace(result.GlobalError))
        {
            await Shell.Current.DisplayAlertAsync("Error", result.GlobalError, "OK");
            return;
        }

        if (result.Success && result.Data != null && onSuccess != null)
        {
            await onSuccess(result.Data);
        }
    }

    protected virtual void ApplyFieldErrors(List<ApiError> errors) { }
}