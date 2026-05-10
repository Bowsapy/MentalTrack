using MentalTrack.Enums;

public class WorkingWithDates
{
    public DayInWeekEnum GetDayOfWeek(DateTime date)
    {
        return date.DayOfWeek switch
        {
            DayOfWeek.Monday => DayInWeekEnum.Monday,
            DayOfWeek.Tuesday => DayInWeekEnum.Tuesday,
            DayOfWeek.Wednesday => DayInWeekEnum.Wednesday,
            DayOfWeek.Thursday => DayInWeekEnum.Thursday,
            DayOfWeek.Friday => DayInWeekEnum.Friday,
            DayOfWeek.Saturday => DayInWeekEnum.Saturday,
            DayOfWeek.Sunday => DayInWeekEnum.Sunday,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}