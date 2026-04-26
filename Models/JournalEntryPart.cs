namespace MentalTrack.Models
{
    public class JournalEntryPart
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int JournalEntryId { get; set; }
        public string[]? Embedding { get; set; }

        public JournalEntry JournalEntry { get; set; }

        public JournalEntryPart(string content, int journalEntryId)
        {
            Content = content;
            JournalEntryId = journalEntryId;
        }
        public JournalEntryPart() { }
    }
    
}
