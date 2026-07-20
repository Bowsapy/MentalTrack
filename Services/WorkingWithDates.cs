using MentalTrack.Enums;
using MentalTrack.Models;

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

    //zavest konstanty pro denni faze, by bylo fajn
    public DayPhasesEnum GetDayPhase(DateTime date)
    {
        int hour = date.Hour;
        DayPhasesEnum phase = DayPhasesEnum.Morning;


        if (hour >= 0 & hour < 6)
        {
            phase = DayPhasesEnum.Night;
        }
        else if (hour >= 6 & hour <= 11)
        {
            phase = DayPhasesEnum.Morning;

        }
        else if (hour >= 12 & hour < 18)
        {
            phase = DayPhasesEnum.Afternoon;

        }
        else if (hour >= 18 & hour < 24)
        {
            phase = DayPhasesEnum.Evening;

        }
        return phase;



    }

}