using MentalTrack.Enums;
using System;
using System.ComponentModel.DataAnnotations;

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
        public string[]? Embedding { get; set; }
        public string[]? UserStates { get; set; }

    }
}