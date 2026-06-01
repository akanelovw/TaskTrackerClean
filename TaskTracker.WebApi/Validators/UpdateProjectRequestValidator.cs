using FluentValidation;
using TaskTracker.Application.Projects.UpdateProject;

namespace TaskTracker.Api.Validators;

public class UpdateProjectRequestValidator : AbstractValidator<UpdateProjectRequest>
{
    public UpdateProjectRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.CustomerCompany)
            .NotEmpty();

        RuleFor(x => x.ExecutorCompany)
            .NotEmpty();

        RuleFor(x => x.StartTime)
            .NotEmpty();

        RuleFor(x => x.EndTime)
            .NotEmpty()
            .GreaterThan(x => x.StartTime);

        RuleFor(x => x.Priority)
            .IsInEnum();
    }
}