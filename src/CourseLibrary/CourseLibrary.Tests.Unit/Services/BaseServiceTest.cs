using Tynamix.ObjectFiller;

namespace CourseLibrary.Tests.Unit.Services;

public class BaseServiceTest
{
    private static readonly Random _random = new();

    public static IEnumerable<object[]> InvalidMinuteCases()
    {
        int randomMoreThanMinuteFromNow = GetRandomNumber();
        int randomMoreThanMinuteBeforeNow = GetNegativeRandomNumber();

        return new List<object[]>
        {
            new object[] { randomMoreThanMinuteFromNow },
            new object[] { randomMoreThanMinuteBeforeNow }
        };
    }

    protected static DateTimeOffset GetRandomDateTime() =>
        new DateTimeRange(earliestDate: new DateTime()).GetValue();

    protected static int GetRandomNumber() => new IntRange(min: 2, max: 10).GetValue();

    protected static int GetNegativeRandomNumber() => -1 * GetRandomNumber();

    protected static string GetRandomMessage() => new MnemonicString().GetValue();

    protected static string GetRandomWord() => new MnemonicString(1, 1, 100).GetValue();

    protected static List<string> GetRandomWords()
    {
        List<string> words = new();

        for (int i = 0; i < GetRandomNumber(); i++)
        {
            words.Add(GetRandomMessage());
        }

        return words;
    }

    protected static List<Guid> GetRandomGuidList()
    {
        List<Guid> guids = new();

        for (int i = 0; i < GetRandomNumber(); i++)
        {
            guids.Add(Guid.NewGuid());
        }

        return guids;
    }

    protected static bool GetRandomBoolean() =>
      _random.NextDouble() >= 0.5;
}