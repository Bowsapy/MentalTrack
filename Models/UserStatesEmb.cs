using MentalTrack.Enums;

namespace MentalTrack.Models
{
    public class UserStatesEmb
    {
        public int Id { get; set; }
        public UserStateEnum UserState { get; set; }
        public string[]? Embedding { get; set; }

    }
}
