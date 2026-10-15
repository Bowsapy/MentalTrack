using MentalTrack.Enums;
using Pgvector;
using System.ComponentModel.DataAnnotations.Schema;

namespace MentalTrack.Models
{
    public class UserStatesEmb
    {
        public UserStatesEmb()
        {
        }

        public UserStatesEmb(
            UserStateEnum userState,
            string content,
            Vector embedding)
        {
            Content = content;
            UserState = userState;
            Embedding = embedding;
        }

        public int Id { get; set; }

        public UserStateEnum UserState { get; set; }

        [Column(TypeName = "vector(1536)")]
        public Vector? Embedding { get; set; }

        public string Content { get; set; } = string.Empty;

        public Sentiment? Sentiment { get; set; }
    }
}