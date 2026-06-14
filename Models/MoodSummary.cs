using MentalTrack.Enums;

namespace MentalTrack.Models
{
    public class MoodSummary
    {
        public int Id { get; set; }
        public MoodEnum[] Mood { get; set; }
        public UserStateEnum[] UserStates { get; set; } 
        public DayPhasesEnum[] DayPhases { get; set; }
        public DayInWeekEnum[] WeekDays {  get; set; }
        
     }
}
