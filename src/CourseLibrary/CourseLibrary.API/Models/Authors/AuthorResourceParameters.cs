using System.ComponentModel;

namespace CourseLibrary.API.Models.Authors;

public class AuthorResourceParameters : ResourceParameters
{
    [DefaultValue(nameof(Author.Id))]
    public override string OrderBy { get; set; } = nameof(Author.Id);
}
