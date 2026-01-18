namespace Overclocked.Application.Features.PaymentUseCases.DTOs.Responses;

public record PaymentResponse
{
    public required Dictionary<string, List<string>> Payments { get; init; } = [];
}
