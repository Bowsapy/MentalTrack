using System.ComponentModel.DataAnnotations;

namespace MentalTrack.Models
{
    public class EntryStateScore
    {
        public int Id { get; set; }
        public int JournalEntryId {  get; set; }
        public JournalEntry JournalEntry { get; set; }
        public int UserStatesEmbId { get; set; }
        public UserStatesEmb UserStatesEmb { get; set; }
        public double SimScore { get; set; }

        public EntryStateScore(int journalEntryId, int userStateId, double simScore)
        {
            JournalEntryId = journalEntryId;
            UserStatesEmbId = userStateId;
            SimScore = simScore;
        }
        public EntryStateScore() { }
    }
}
