using Pgvector;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MentalTrack.Models
{
    public class JournalEntry
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string UserId { get; set; }
        public User? User { get; set; }
        public MoodEnum Mood { get; set; }

        [Column(TypeName = "vector(1536)")]
        public Vector? Embedding { get; set; }
        public DayPhasesEnum DayPhase { get; set; }

    }
}