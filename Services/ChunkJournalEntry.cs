using MentalTrack.Data;
using MentalTrack.Models;

//zpetne si rikam, ze tahle sluzba by byla lepsi jako metoda v DiaryControlleru
namespace MentalTrack.Services
{
	public class ChunkJournalEntry
	{
		private readonly AppDbContext _context;
		private readonly ExternalAPIService _externalAPIService;
		private readonly ILogger<ExternalAPIService> _logger;

		public ChunkJournalEntry(AppDbContext context, ExternalAPIService externalAPIService, ILogger<ExternalAPIService> logger)
		{
			_context = context;
			_externalAPIService = externalAPIService;
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

					var embeddingTask = _externalAPIService.GetEmbedding(part.Content);

					var sentimentTask = _externalAPIService.GetSentiment(part);

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
