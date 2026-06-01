using FluentValidation;
using TaskTracker.Application.WorkItems.ChangeStatus;

namespace TaskTracker.Api.Validators;

public class ChangeWorkItemStatusRequestValidator : AbstractValidator<ChangeWorkItemStatusRequest>
{
    public ChangeWorkItemStatusRequestValidator()
    {
        RuleFor(x => x.WorkItemId)
            .GreaterThan(0);

        RuleFor(x => x.Status)
            .IsInEnum();
    }
}