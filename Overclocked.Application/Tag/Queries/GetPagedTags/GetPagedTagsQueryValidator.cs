using FluentValidation;
using Overclocked.Application.Common.Enums;

namespace Overclocked.Application.Tag.Queries.GetPagedTags;

public class GetPagedTagsQueryValidator : AbstractValidator<GetPagedTagsQuery>
{
    public GetPagedTagsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("{PropertyName} must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("{PropertyName} must be between 1 and 100.");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm))
            .WithMessage("{PropertyName} term must not exceed 100 characters");

        RuleFor(x => x.SortBy)
            .Must(value => Enum.TryParse<TagSortField>(value, true, out _))
            .WithMessage("{PropertyName} must be one of: " + string.Join(", ", Enum.GetNames<TagSortField>()));

        RuleFor(x => x.Direction)
            .Must(value => Enum.TryParse<SortDirection>(value, true, out _))
            .WithMessage("{PropertyName} must be one of: " + string.Join(", ", Enum.GetNames<SortDirection>()));
    }
}
