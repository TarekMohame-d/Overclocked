namespace Overclocked.Contracts.Tag;

public record GetPagedTagsRequest(
    int Page = 1,
    int PageSize = 10,
    string SearchTerm = "",
    string SortBy = "id",
    string Direction = "asc");
