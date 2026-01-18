namespace Overclocked.Application.Features.ReviewUseCases.DTOs.Requests;

public record GetPagedReviewsQuery(int? Page, int? PageSize, string? SortBy, string? Direction);
