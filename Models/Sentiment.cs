using MentalTrack.Enums;
namespace MentalTrack.Models
{
    public class Sentiment
    {
        public int Id { get; set; }

        public int JournalEntryPartId { get; set; }
        public JournalEntryPart JournalEntryPart { get; set; }

        public TextPolarityEnum MainPolarity { get; set; }

        public double Positive { get; set; }
        public double Neutral { get; set; }
        public double Negative { get; set; }

        public Sentiment() { } 
    }
}