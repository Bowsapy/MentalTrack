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

        public Sentiment( JournalEntryPart journalEntryPart, TextPolarityEnum mainPolarity, double positive, double neutral, double negative)
        {
            JournalEntryPart = journalEntryPart;
            MainPolarity = mainPolarity;
            Positive = positive;
            Neutral = neutral;
            Negative = negative;
        }
    }
}