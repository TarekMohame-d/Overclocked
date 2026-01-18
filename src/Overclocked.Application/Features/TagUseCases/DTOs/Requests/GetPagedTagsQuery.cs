namespace Overclocked.Application.Features.TagUseCases.DTOs.Requests;

public record GetPagedTagsQuery(int? Page, int? PageSize, string? SearchTerm, string? SortBy, string? Direction);
