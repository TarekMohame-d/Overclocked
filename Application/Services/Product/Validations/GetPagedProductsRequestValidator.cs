using Application.Services.Product.DTOs.Request;
using FluentValidation;

namespace Application.Services.Product.Validations;

public class GetPagedProductsRequestValidator : AbstractValidator<GetPagedProductsRequest>
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
            .IsInEnum()
            .WithMessage("{PropertyName} must be one of: Id, Name, Price, Rating");

        RuleFor(x => x.Direction)
            .IsInEnum()
            .WithMessage("{PropertyName} must be either Asc or Desc");

        RuleFor(x => x.Search)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Search))
            .WithMessage("{PropertyName} term must not exceed 200 characters");

        RuleFor(x => x.Category)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Category))
            .WithMessage("{PropertyName} must not exceed 100 characters.");

        RuleFor(x => x.Brand)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Brand))
            .WithMessage("{PropertyName} must not exceed 100 characters.");
    }
}
