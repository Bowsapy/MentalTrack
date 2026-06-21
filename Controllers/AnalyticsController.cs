using MentalTrack.Constants;
using MentalTrack.Data;
using MentalTrack.Enums;
using MentalTrack.Models;
using MentalTrack.Services;
using MentalTrack.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
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
        private readonly WorkingWithDates _dateService;

        public AnalyticsController(AppDbContext context, EmbeddingService embeddingService, EmbeddingConverter embeddingConverter, CosineSimilarityService similarityService, ILogger<AnalyticsController> logger,WorkingWithDates dateService)
        {
            _context = context;
            _embeddingService = embeddingService;
            _embeddingConverter = embeddingConverter;
            _similarityService = similarityService;
            _logger = logger;
            _dateService = dateService;
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
   
        public IActionResult Features()
        {
            return View();
        }

        public void FindEntryStateMatches()
        {
            //ziska prihlaseneho usera
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            //ziskej journal entry parts konkretniho usera jako list
            var entryParts = _context.EntryParts
                .Where(e => e.Embedding != null
                    && e.JournalEntry.UserId == userId)
                .ToList();


            //ziskej userstates jako list
            var allUserStates = _context.UserStates
                .Where(s => s.Embedding != null)
                .ToList();

            var existingMatches = _context.EntryStates
      .Where(x => x.JournalEntryPart.JournalEntry.UserId == userId)
      .Select(x => new { x.JournalEntryPartId, x.UserStatesEmbId })
      .AsEnumerable()
      .Select(x => (x.JournalEntryPartId, x.UserStatesEmbId))
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
                    if (match.Score > AppConstants.MinScore)
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


        public Dictionary<MoodEnum, Dictionary<UserStateEnum,int>> GetMoodStatesMatches()
        {
            FindEntryStateMatches();
            var result = _context.EntryStates
                .GroupBy(es => new
                {
                    Mood = es.JournalEntryPart.JournalEntry.Mood,
                    UserState = es.UserStatesEmb.UserState
                })
                .Select(g => new
                {
                    g.Key.Mood,
                    g.Key.UserState,
                    Count = g.Count()
                })
                .ToList()
                .GroupBy(x => x.Mood)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToDictionary(x => x.UserState, x => x.Count)
                );
            return result;
            //vytvori Dictionary<Mood, Dictionary<UserStateEnum,count( userstates) - umoznuje zjistit váhu shody>
        }
 

        public Dictionary<string,int> ViewMoodOnWeekDays()
        {
            var data = _context.Entries
       .AsEnumerable()
       .GroupBy(e => e.CreatedAt.DayOfWeek)
       .ToDictionary(
           g => g.Key.ToString(),
           g => (int)g.Average(x => (int)x.Mood)
       );
            return data;
        }
        public Dictionary<string,int> ViewMoodOnDayPhases()
        {
            var data = _context.Entries
       .AsEnumerable()
       .GroupBy(e => e.DayPhase)
       .ToDictionary(
           g => g.Key.ToString(),
           g => (int)g.Average(x => (int)x.Mood)
       );
            return data;
        }

        public IActionResult MoodSummary()
        {
            Dictionary<MoodEnum,Dictionary<UserStateEnum,int>> USMatches = GetMoodStatesMatches();
            Dictionary<string, int> WDMatches = ViewMoodOnWeekDays();
            Dictionary<string, int> DPMatches = ViewMoodOnDayPhases();


            MoodSummaryViewModel moodsumVM = new MoodSummaryViewModel(USMatches, WDMatches, DPMatches);

            return View(moodsumVM);
        }
        public IActionResult ShowMoodProgress()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var entries = _context.Entries
                .Where(e => e.Mood != null && e.CreatedAt != null && e.UserId == userId)
                .OrderBy(e => e.CreatedAt)
                .ToList(); // ToList() vyhodnotí query a pošle jen validní data

            var createdAts = entries
            .Select(e => e.CreatedAt.ToString("dd.MM.yyyy"))
            .ToArray();

            var moods = entries
                .Select(e => e.Mood)
                .ToArray();

            MoodGraphViewModel mgwm = new MoodGraphViewModel(createdAts, moods);
            return View(mgwm);

        }






    }

}
