using Application.Services.Tag.DTOs.Request;
using FluentValidation;

namespace Application.Services.Tag.Validations;

public class GetPagedTagsRequestValidator : AbstractValidator<GetPagedTagsRequest>
{
    public GetPagedTagsRequestValidator()
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
    }
}
