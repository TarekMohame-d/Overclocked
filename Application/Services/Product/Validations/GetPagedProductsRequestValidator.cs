using Application.Common.Enums;
using Application.Services.Product.DTOs.Request;
using FluentValidation;

namespace Application.Services.Product.Validations;

public class GetPagedProductsRequestValidator : AbstractValidator<GetPagedProductsQuery>
{
    public GetPagedProductsRequestValidator()
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

        RuleFor(x => x.Search)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Search))
            .WithMessage("{PropertyName} term must not exceed 100 characters");

        RuleFor(x => x.Category)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.Category))
            .WithMessage("{PropertyName} must not exceed 50 characters.");

        RuleFor(x => x.Brand)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.Brand))
            .WithMessage("{PropertyName} must not exceed 50 characters.");
    }
}
