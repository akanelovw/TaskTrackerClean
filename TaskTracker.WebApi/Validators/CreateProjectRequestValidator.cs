using FluentValidation;
using TaskTracker.Application.Projects.CreateProject;

namespace TaskTracker.Api.Validators;

public class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequest>
{
    public CreateProjectRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.CustomerCompany)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.ExecutorCompany)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.StartTime)
            .NotEmpty();

        RuleFor(x => x.EndTime)
            .NotEmpty()
            .GreaterThan(x => x.StartTime);

        RuleFor(x => x.Priority)
            .IsInEnum();
    }
}