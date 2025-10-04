using FluentValidation;

namespace Application.Features.Product.Queries.GetPagedProducts;

public class GetPagedProductsQueryValidation : AbstractValidator<GetPagedProductsQuery>
{
    private static readonly HashSet<string> _validSortFields = new()
    {
        "name",
        "price",
        "rating"
    };

    public GetPagedProductsQueryValidation()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("{PropertyName} must be greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithMessage("{PropertyName} must be greater than or equal to 1.");

        RuleFor(x => x.SortBy)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(BeValidSortBy)
                .WithMessage($"{{PropertyName}} must be in 'field_direction' format. Valid fields: {string.Join(", ", _validSortFields)}. Valid directions: asc, desc.");
    }

    private bool BeValidSortBy(string sortBy)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return false;

        var parts = sortBy.Split('_');
        if (parts.Length != 2)
            return false;

        var field = parts[0].ToLowerInvariant();
        var direction = parts[1].ToLowerInvariant();

        bool isFieldValid = _validSortFields.Contains(field);
        bool isDirectionValid = direction == "asc" || direction == "desc";

        return isFieldValid && isDirectionValid;
    }
}
