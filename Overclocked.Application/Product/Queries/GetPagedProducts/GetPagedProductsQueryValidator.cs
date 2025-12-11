using FluentValidation;
using Overclocked.Application.Common.Enums;

namespace Overclocked.Application.Product.Queries.GetPagedProducts;

public class GetPagedProductsQueryValidator : AbstractValidator<GetPagedProductsQuery>
{
    public GetPagedProductsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("{PropertyName} must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("{PropertyName} must be between 1 and 100.");

        RuleFor(x => x.SortBy)
            .Must(value => string.IsNullOrWhiteSpace(value) || Enum.TryParse<ProductSortField>(value, true, out _))
            .WithMessage("{PropertyName} must be one of: " + string.Join(", ", Enum.GetNames<ProductSortField>()));

        RuleFor(x => x.Direction)
            .Must(value => string.IsNullOrWhiteSpace(value) || Enum.TryParse<SortDirection>(value, true, out _))
            .WithMessage("{PropertyName} must be one of: " + string.Join(", ", Enum.GetNames<SortDirection>()));

        RuleFor(x => x.SearchTerm)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm))
            .WithMessage("{PropertyName} term must not exceed 100 characters");
    }
}
