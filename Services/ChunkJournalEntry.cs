using MentalTrack.Data;
using MentalTrack.Models;

//zpetne si rikam, ze tahle sluzba by byla lepsi jako metoda v DiaryControlleru
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

            var parts = new JournalEntryPart[chunks.Length];
            var sentiments = new Sentiment[chunks.Length];

            using var semaphore = new SemaphoreSlim(4);

            var tasks = chunks.Select(async (chunk, i) =>
            {
                await semaphore.WaitAsync();

                try
                {

                    var part = new JournalEntryPart(chunk.Trim(), entry.Id);

                    var embeddingTask = _embeddingService.GetEmbedding(part.Content);

                    var sentimentTask = _sentimentService.AnalyzeAsync(part);

                    await Task.WhenAll(embeddingTask, sentimentTask);

                    part.Embedding = await embeddingTask;

                    var data = await sentimentTask;

                    var sentiment = new Sentiment(part, data.MainPolarity, data.Positive, data.Neutral, data.Negative);

                    part.Sentiment = sentiment;


                    parts[i] = part;
                    sentiments[i] = sentiment;
                }

                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);

            _context.EntryParts.AddRange(parts);
            _context.Sentiments.AddRange(sentiments);

            // totalStopwatch.Stop();

            //_logger.LogInformation(
            //"ChunkEntry completed in {ElapsedMs} ms",
            //totalStopwatch.ElapsedMilliseconds);
        }
    }
}
