using FluentValidation;
using TaskTracker.Application.Projects.GetProjectsList;

namespace TaskTracker.Api.Validators;

public class GetProjectsListRequestValidator : AbstractValidator<GetProjectsListRequest>
{
    public GetProjectsListRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.Search)
            .MaximumLength(100)
            .When(x => x.Search != null);

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue);

        RuleFor(x => x.Priority)
            .IsInEnum()
            .When(x => x.Priority.HasValue);
    }
}