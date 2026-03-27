namespace Workhours.Services;

public static class WeekCalculator
{
    public static (DateOnly WeekStart, DateOnly WeekEnd) GetWeekRange(DateOnly date)
    {
        var dayOffset = ((int)date.DayOfWeek + 6) % 7;
        var weekStart = date.AddDays(-dayOffset);
        return (weekStart, weekStart.AddDays(6));
    }
}