using MentalTrack.Enums;

namespace MentalTrack.Models;

public class MoodSummaryViewModel
{
    public MoodSummaryViewModel(
        Dictionary<MoodEnum, Dictionary<UserStateEnum, int>> userStatesMatches,
        Dictionary<string, int> daysInTheWeekMatches,
        Dictionary<string, int> dayPhasesMatches,
        Dictionary<MoodEnum, double> moodPercentages,
        int avg,
        int total


        )
    {
        UserStatesMatches = userStatesMatches;
        DaysInTheWeekMatches = daysInTheWeekMatches;
        DayPhasesMatches = dayPhasesMatches;
        MoodPercentages = moodPercentages;
        AverageMood = avg;
        EntriesCount = total;
    }

    public Dictionary<MoodEnum, Dictionary<UserStateEnum, int>> UserStatesMatches { get; set; }

    public Dictionary<string, int> DaysInTheWeekMatches { get; set; }

    public Dictionary<string, int> DayPhasesMatches { get; set; }
    public int AverageMood { get; set; }
    public int EntriesCount {  get; set; }

    public Dictionary<MoodEnum, double> MoodPercentages { get; set; }

}