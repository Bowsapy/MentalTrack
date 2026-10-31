using MentalTrack.Enums;
using MentalTrack.Models;

namespace MentalTrack.ViewModels
{
    public class ActivitesViewModel
    {
        public ActivitesViewModel(Dictionary<MoodEnum, Dictionary<UserStateEnum, int>> userStateMatches, List<UserstateInDay> userstatesProgress)
        {
            UserStateMatches = userStateMatches;
            UserstatesProgress = userstatesProgress;
        }

        public Dictionary<MoodEnum, Dictionary<UserStateEnum, int>> UserStateMatches { get; set; }
        public List<UserstateInDay> UserstatesProgress { get; set; }
    }
}
