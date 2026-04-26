using MentalTrack.Data;
using MentalTrack.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace MentalTrack.Services
{
    public class ChunkJournalEntry
    {
        private readonly AppDbContext _context;
        private readonly EmbeddingService _embeddingService;

        public ChunkJournalEntry(AppDbContext context, EmbeddingService embeddingService)
        {
            _context = context;
            _embeddingService = embeddingService;
        }
        public async Task ChunkEntry(JournalEntry entry)
        {
            string[] chunks = entry.Content.Split('.', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < chunks.Length; i++)
            {
                JournalEntryPart part = new JournalEntryPart(chunks[i],entry.Id);
                part.Embedding = await _embeddingService.GetEmbedding(part.Content);

                _context.EntryParts.Add(part);
                

            }
            _context.SaveChanges();


        }

    }
}
