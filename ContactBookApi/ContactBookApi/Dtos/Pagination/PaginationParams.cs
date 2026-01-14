namespace ContactBookApi.Dtos.Pagination;

public sealed class PaginationParams
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;

    internal int SafePage => Page < 1 ? 1 : Page;

    internal int SafePageSize =>
        PageSize switch
        {
            < 1 => 20,
            > 100 => 100,
            _ => PageSize
        };
}