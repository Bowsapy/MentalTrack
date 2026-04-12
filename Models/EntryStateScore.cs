using System.ComponentModel.DataAnnotations;

namespace MentalTrack.Models
{
    public class EntryStateScore
    {
        public int Id { get; set; }
        public int JournalEntryId {  get; set; }
        public int UserStateId { get; set; }
        public double SimScore { get; set; }

        public EntryStateScore(int journalEntryId, int userStateId, double simScore)
        {
            JournalEntryId = journalEntryId;
            UserStateId = userStateId;
            SimScore = simScore;
        }
        public EntryStateScore() { }
    }
}
