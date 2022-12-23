namespace CourseLibrary.API.Pagination;

public class PaginationMetaData
{
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
    public string PreviousPageLink { get; set; } = string.Empty;
    public string NextPageLink { get; set; } = string.Empty;
}
