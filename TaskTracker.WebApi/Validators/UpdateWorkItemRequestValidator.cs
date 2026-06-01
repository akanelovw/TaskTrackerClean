using FluentValidation;
using TaskTracker.Application.WorkItems.UpdateWorkItem;

namespace TaskTracker.Api.Validators;

public class UpdateWorkItemRequestValidator : AbstractValidator<UpdateWorkItemRequest>
{
    public UpdateWorkItemRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Comment)
            .MaximumLength(1000)
            .When(x => x.Comment != null);
    }
}