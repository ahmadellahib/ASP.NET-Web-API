using System.ComponentModel;

namespace CourseLibrary.API.Models;

public class ResourceParameters
{
    private const int maxPageSize = 100;
    private int _pageSize = maxPageSize;
    private int _pageNumber = 1;

    [DefaultValue("Name")]
    public virtual string OrderBy { get; set; } = string.Empty;

    [DefaultValue(1)]
    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = (value < 1) ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value < 1 || value > maxPageSize) ? maxPageSize : value;
    }

    public string SearchQuery { get; set; } = string.Empty;
}
