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
        private readonly AppDbContext _context;
        private readonly EmbeddingService _embeddingService;
        private readonly EmbeddingConverter _embeddingConverter;
        private readonly CosineSimilarityService _similarityService;

        public AnalyticsController(AppDbContext context, EmbeddingService embeddingService, EmbeddingConverter embeddingConverter, CosineSimilarityService similarityService)
        {
            _context = context;
            _embeddingService = embeddingService;
            _embeddingConverter = embeddingConverter;
            _similarityService = similarityService;
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
        public IActionResult SimilarUserStates()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEntries = _context.Entries.Where(e => e.UserId == userId).ToList();
            var allUserStates = _context.UserStates .ToList();
            
            var matches = userEntries
    .Where(e => e.Embedding != null)
    .SelectMany(entry => allUserStates.Where(s => s.Embedding != null),
        (entry, state) => new
        {
            Entry = entry,
            State = state,
            Score = _similarityService.Calculate(
                _embeddingConverter.ConvertToFloatList(entry.Embedding),
                _embeddingConverter.ConvertToFloatList(state.Embedding)
            )
        })
    .Where(x => x.Score > 0.6)
    .GroupBy(x => x.Entry)
    .ToDictionary(
        g => g.Key,
        g => g
            .OrderByDescending(x => x.Score) //  (seřazení podle relevance)
            .Select(x => x.State)
           
    );


            return View(matches);



        }
    }
}
