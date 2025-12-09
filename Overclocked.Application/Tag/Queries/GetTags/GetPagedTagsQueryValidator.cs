using FluentValidation;
using Overclocked.Application.Common.Enums;

namespace Overclocked.Application.Tag.Queries.GetTags;

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
            .NotEmpty()
            .WithMessage("{PropertyName} is required.");

        RuleFor(x => x.SortBy)
            .Must(value => Enum.TryParse<TagSortField>(value, true, out _))
            .WithMessage("{PropertyName} must be one of: " + string.Join(", ", Enum.GetNames<TagSortField>()));

        RuleFor(x => x.Direction)
            .Must(value => Enum.TryParse<SortDirection>(value, true, out _))
            .WithMessage("{PropertyName} must be one of: " + string.Join(", ", Enum.GetNames<SortDirection>()));
    }
}
