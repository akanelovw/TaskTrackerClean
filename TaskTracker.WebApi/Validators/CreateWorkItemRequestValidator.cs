using FluentValidation;
using TaskTracker.Application.WorkItems.CreateWorkItem;

namespace TaskTracker.Api.Validators;

public class CreateWorkItemRequestValidator : AbstractValidator<CreateWorkItemRequest>
{
    public CreateWorkItemRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.ProjectId)
            .GreaterThan(0);

        RuleFor(x => x.Priority)
            .IsInEnum();
    }
}