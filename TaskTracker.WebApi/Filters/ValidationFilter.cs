using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TaskTracker.Api.Common;

namespace TaskTracker.Api.Filters;

public class ValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _provider;

    public ValidationFilter(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        foreach (var arg in context.ActionArguments.Values)
        {
            if (arg == null) continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(arg.GetType());
            var validator = _provider.GetService(validatorType);

            if (validator == null) continue;

            var result = await ((IValidator)validator)
                .ValidateAsync(new ValidationContext<object>(arg));

            if (!result.IsValid)
            {
                context.Result = new BadRequestObjectResult(
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = result.Errors.Select(x => new ApiError
                        {
                            Field = x.PropertyName,
                            Error = x.ErrorMessage
                        }).ToList()
                    });

                return;
            }
        }

        await next();
    }
}