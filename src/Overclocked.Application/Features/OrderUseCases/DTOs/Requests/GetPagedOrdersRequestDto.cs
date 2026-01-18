namespace Overclocked.Application.Features.OrderUseCases.DTOs.Requests;

public record GetPagedOrdersRequestDto(int? Page, int? PageSize, string? Direction);
