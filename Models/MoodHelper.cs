using MentalTrack.Models;


public static class MoodHelper
{
    public static string GetMoodBadgeBG(MoodEnum mood)
    {
        return mood switch
        {
            MoodEnum.VeryGood => "very-good-mood-bg",
            MoodEnum.Good => "good-mood-bg",
            MoodEnum.Neutral => "neutral-mood-bg",
            MoodEnum.Bad => "bad-mood-bg",
            MoodEnum.VeryBad => "very-bad-mood-bg",
            _ => "bg-secondary"
        };
    }
    public static string GetMoodBadgeTextColor(MoodEnum mood)
    {
        return mood switch
        {
            MoodEnum.VeryGood => "very-good-mood-fg",
            MoodEnum.Good => "good-mood-fg",
            MoodEnum.Neutral => "neutral-mood-fg",
            MoodEnum.Bad => "bad-mood-fg",
            MoodEnum.VeryBad => "very-bad-mood-fg",
            _ => "bg-secondary"
        };
    }
}