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
        private readonly ILogger<AccountController> _logger;
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

            var userEntries = _context.Entries
                .Where(e => e.UserId == userId && e.Embedding != null)
                .ToList();

            var allUserStates = _context.UserStates
                .Where(s => s.Embedding != null)
                .ToList();

            var matches = userEntries
    .SelectMany(entry => allUserStates,
        (entry, state) => new EntryStateScore(
            entry.Id,
            state.Id,
            _similarityService.Calculate(
                _embeddingConverter.ConvertToFloatList(entry.Embedding),
                _embeddingConverter.ConvertToFloatList(state.Embedding)
            )
        )
    )
    .Where(x => x.SimScore > 0.4).ToList();
   _context.EntryStates.AddRange(matches);
    _context.SaveChanges();



            return View();
        }

        
       

    }
}
