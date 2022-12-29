using Bogus;
using Bogus.Premium;

namespace CourseLibrary.API.Extensions;

public static class BogusExtensions
{
    public static CourseCategory CourseCategory(this Faker faker)
    {
        return ContextHelper.GetOrSet(faker, () => new CourseCategory());
    }
}

public class CourseCategory : DataSet
{
    private static readonly string[] CategoriesNames =
       {"Development", "Art", "Languages", "Science", "Mathematics", "Cultural Studies", "Art Studio",
        "Wellness and Health", "Creative Writing", "Business", "Technology and Data Science"};

    public string Name()
    {
        return this.Random.ArrayElement(CategoriesNames);
    }

    public string Name(int index)
    {
        if (index < 0 || index > CategoriesNames.Length - 1)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        return CategoriesNames[index];
    }
}