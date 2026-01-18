using FluentValidation;

namespace Overclocked.Application.Features.OrderUseCases.GetPagedOrders;

public class GetPagedOrdersRequestValidator : AbstractValidator<GetPagedOrdersRequest>
{
    public GetPagedOrdersRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");

        RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("{PropertyName} must be between 1 and 100.");

        RuleFor(x => x.Year)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .InclusiveBetween(DateTime.UtcNow.Year - 5, DateTime.UtcNow.Year)
            .WithMessage("{PropertyName} must be between {From} and {To}.");
    }
}
