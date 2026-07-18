using MentalTrack.Enums;

namespace MentalTrack.Models;

public class MoodSummaryViewModel
{
    public MoodSummaryViewModel(
        Dictionary<MoodEnum, double> moodPercentages,
        int mode,
        int total


        )
    {
 
        MoodPercentages = moodPercentages;
        ModeMood = mode;
        EntriesCount = total;
    }

    public int ModeMood { get; set; }
    public int EntriesCount {  get; set; }

    public Dictionary<MoodEnum, double> MoodPercentages { get; set; }

}