using MentalTrack.Enums;

namespace MentalTrack.Models;

public class MoodSummaryViewModel
{
    public MoodSummaryViewModel(
        Dictionary<MoodEnum, Dictionary<UserStateEnum, int>> userStatesMatches,
        Dictionary<string, int> daysInTheWeekMatches,
        Dictionary<string, int> dayPhasesMatches)
    {
        UserStatesMatches = userStatesMatches;
        DaysInTheWeekMatches = daysInTheWeekMatches;
        DayPhasesMatches = dayPhasesMatches;
    }

    public Dictionary<MoodEnum, Dictionary<UserStateEnum, int>> UserStatesMatches { get; set; }

    public Dictionary<string, int> DaysInTheWeekMatches { get; set; }

    public Dictionary<string, int> DayPhasesMatches { get; set; }
}