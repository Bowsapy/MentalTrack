using MentalTrack.Models;

namespace MentalTrack.ViewModels
{
    public class DatetimeAnalysisViewModel
    {
        public DatetimeAnalysisViewModel(Dictionary <DayPhasesEnum, int> moodInDayPhases, Dictionary <string, int> moodInWeek)
        {
            MoodInDayPhases = moodInDayPhases;
            MoodInWeek = moodInWeek;
        }

        public Dictionary<DayPhasesEnum, int> MoodInDayPhases { get; set; }
        public Dictionary<string, int> MoodInWeek {  get; set; }
    }
}
