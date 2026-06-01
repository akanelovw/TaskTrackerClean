using FluentValidation;
using TaskTracker.Application.WorkItems.AssignUser;

namespace TaskTracker.Api.Validators;

public class AssignUserRequestValidator : AbstractValidator<AssignUserRequest>
{
    public AssignUserRequestValidator()
    {
        RuleFor(x => x.WorkItemId)
            .GreaterThan(0);

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}