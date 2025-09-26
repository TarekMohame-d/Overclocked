using FluentValidation;

namespace Application.Features.Tag.Queries.GetPagedTags;

public class GetPagedTagsQueryValidation : AbstractValidator<GetPagedTagsQuery>
{
    public GetPagedTagsQueryValidation()
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
                .WithMessage("{PropertyName} must be in the format 'name_asc' or 'name_desc'.");
    }

    private bool BeValidSortBy(string sortBy)
    {
        var parts = sortBy.Split('_');
        if (parts.Length != 2) return false;

        var field = parts[0];
        var direction = parts[1];

        return field == "name" && (direction == "asc" || direction == "desc");
    }
}
