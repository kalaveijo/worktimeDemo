using Workhours.Services;
using Xunit;

namespace Workhours.Tests;

public class WeekCalculatorTests
{
    [Fact]
    public void GetWeekRange_ReturnsMondayToSunday()
    {
        var (weekStart, weekEnd) = WeekCalculator.GetWeekRange(new DateOnly(2026, 3, 27));

        Assert.Equal(new DateOnly(2026, 3, 23), weekStart);
        Assert.Equal(new DateOnly(2026, 3, 29), weekEnd);
    }
}