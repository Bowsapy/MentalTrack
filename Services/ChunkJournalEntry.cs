using MentalTrack.Controllers;
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
        private readonly SentimentService _sentimentService;
        private readonly ILogger<SentimentService> _logger;


        public ChunkJournalEntry(AppDbContext context, EmbeddingService embeddingService, SentimentService sentimentService, ILogger<SentimentService> logger)
        {
            _context = context;
            _embeddingService = embeddingService;
            _sentimentService = sentimentService;
            _logger = logger;
        }
        public async Task ChunkEntry(JournalEntry entry)
        {
            string[] chunks = entry.Content.Split('.', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < chunks.Length; i++)
            {
                JournalEntryPart part = new JournalEntryPart(chunks[i],entry.Id);
                part.Embedding = await _embeddingService.GetEmbedding(part.Content);
                Sentiment sentiment = await _sentimentService.AnalyzeAsync(part);
                _logger.LogWarning((sentiment.MainPolarity).ToString() + "-------------------------------"); 
               


                _context.EntryParts.Add(part);
                

            }
            _context.SaveChanges();


        }

    }
}
