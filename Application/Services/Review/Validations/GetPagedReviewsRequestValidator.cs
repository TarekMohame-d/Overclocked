using Application.Common.Enums;
using Application.Services.Review.DTOs.Request;
using FluentValidation;

namespace Application.Services.Review.Validations;

public class GetPagedReviewsRequestValidator : AbstractValidator<GetPagedReviewsQuery>
{
    public GetPagedReviewsRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("{PropertyName} must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("{PropertyName} must be between 1 and 100.");

        RuleFor(x => x.SortBy)
            .Must(value => string.IsNullOrWhiteSpace(value) || Enum.TryParse<ReviewSortField>(value, true, out _))
            .WithMessage("{PropertyName} must be one of: " + string.Join(", ", Enum.GetNames<ReviewSortField>()));

        RuleFor(x => x.Direction)
            .Must(value => string.IsNullOrWhiteSpace(value) || Enum.TryParse<SortDirection>(value, true, out _))
            .WithMessage("{PropertyName} must be one of: " + string.Join(", ", Enum.GetNames<SortDirection>()));
    }
}
