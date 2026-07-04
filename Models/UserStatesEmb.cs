using MentalTrack.Enums;

namespace MentalTrack.Models
{
    public class UserStatesEmb
    {
        public UserStatesEmb(UserStateEnum userState,string content, string[] embedding) { 
        
        Content = content;
        UserState = userState;
        Embedding = embedding;
        }
        public int Id { get; set; }
        public UserStateEnum UserState { get; set; }
        public string[]? Embedding { get; set; }

        public string Content { get; set; } = string.Empty;

        public Sentiment? Sentiment { get; set; }


    }
}
