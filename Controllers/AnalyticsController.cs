using MentalTrack.Data;
using MentalTrack.Enums;
using MentalTrack.Models;
using MentalTrack.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;



namespace MentalTrack.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly ILogger<AnalyticsController> _logger;
        private readonly AppDbContext _context;
        private readonly EmbeddingService _embeddingService;
        private readonly EmbeddingConverter _embeddingConverter;
        private readonly CosineSimilarityService _similarityService;

        public AnalyticsController(AppDbContext context, EmbeddingService embeddingService, EmbeddingConverter embeddingConverter, CosineSimilarityService similarityService, ILogger<AnalyticsController> logger)
        {
            _context = context;
            _embeddingService = embeddingService;
            _embeddingConverter = embeddingConverter;
            _similarityService = similarityService;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AddNewUserState()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewUserState(string content, UserStateEnum state)
        {
            _context.UserStates.Add(new UserStatesEmb(state, content, await _embeddingService.GetEmbedding(content)));
            _context.SaveChanges();
            return RedirectToAction("AddNewUserState");

        }
        public void SimilarUserStates()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var entryParts = _context.EntryParts
                .Where(e => e.Embedding != null
                    && e.JournalEntry.UserId == userId)
                .ToList();



            var allUserStates = _context.UserStates
                .Where(s => s.Embedding != null)
                .ToList();

            var existingMatches = _context.EntryStates
      .Where(x => x.JournalEntryPart.JournalEntry.UserId == userId)
      .Select(x => new { x.JournalEntryPartId, x.UserStateId })
      .AsEnumerable()
      .Select(x => (x.JournalEntryPartId, x.UserStateId))
      .ToHashSet();

    //vytvorim hash set pro zjisteni duplicit, protoze list by sezral moc pameti

            var matches = new List<EntryStateScore>();

            foreach (var part in entryParts)
            {
                var entryVector = _embeddingConverter.ConvertToFloatList(part.Embedding);

                var bestMatches = allUserStates
                    .Select(state => new
                    {
                        state.Id,
                        Score = _similarityService.Calculate(
                            entryVector,
                            _embeddingConverter.ConvertToFloatList(state.Embedding))
                    })
                    .OrderByDescending(x => x.Score)
                    .Take(5); 

                foreach (var match in bestMatches)
                {
                    if (match.Score > 0.05)
                    {
                        if (!existingMatches.Contains((part.Id, match.Id))){ 
                        
                            matches.Add(new EntryStateScore(part.Id, match.Id, match.Score));


                        }
                    }
                }

          



            }

            _context.EntryStates.AddRange(matches);
            _context.SaveChanges();
        }

        
       

    }
}
