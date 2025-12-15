using FluentValidation;
using Overclocked.Application.Common.Enums;

namespace Overclocked.Application.Review.Queries.GetPagedReviews;

public class GetPagedReviewsQueryValidator : AbstractValidator<GetPagedReviewsQuery>
{
    public GetPagedReviewsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("{PropertyName} must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("{PropertyName} must be between 1 and 100.");

        RuleFor(x => x.SortBy)
            .Must(value => Enum.TryParse<ReviewSortField>(value, true, out _))
            .WithMessage("{PropertyName} must be one of: " + string.Join(", ", Enum.GetNames<ReviewSortField>()))
            .When(x => !string.IsNullOrEmpty(x.SortBy));

        RuleFor(x => x.Direction)
            .Must(value => Enum.TryParse<SortDirection>(value, true, out _))
            .WithMessage("{PropertyName} must be one of: " + string.Join(", ", Enum.GetNames<SortDirection>()))
            .When(x => !string.IsNullOrEmpty(x.SortBy));
    }
}
