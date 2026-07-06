using MentalTrack.Constants;
using MentalTrack.Data;
using MentalTrack.Enums;
using MentalTrack.Models;
using MentalTrack.Services;
using MentalTrack.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly SentimentService _sentimentService;


        public AnalyticsController(AppDbContext context, EmbeddingService embeddingService, SentimentService sentimentService, EmbeddingConverter embeddingConverter, CosineSimilarityService similarityService, ILogger<AnalyticsController> logger,WorkingWithDates dateService)
        {
            _context = context;
            _embeddingService = embeddingService;
            _embeddingConverter = embeddingConverter;
            _sentimentService = sentimentService;
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

            UserStatesEmb NewUserStateEmb = new UserStatesEmb(state, content, await _embeddingService.GetEmbedding(content));
            Sentiment NewSentiment =await _sentimentService.AnalyzeAsync(NewUserStateEmb);
            NewUserStateEmb.Sentiment = NewSentiment; 


            _context.UserStates.Add(NewUserStateEmb);
            _context.Sentiments.Add(NewSentiment);


            _context.SaveChanges();
            return RedirectToAction("AddNewUserState");

        }
   
        public IActionResult Features()
        {
      


            return View();
        }
        public string GetCurrentUser()
        {
            
         var userId =  User.FindFirstValue(ClaimTypes.NameIdentifier);
        return userId;
        }

        public void FindEntryStateMatches()
        {


            //ziskej journal entry parts konkretniho usera jako list
            var entryParts = _context.EntryParts
                .Where(e => e.Embedding != null
                    && e.JournalEntry.UserId == GetCurrentUser()).Include(x => x.Sentiment)
                .ToList();


            //ziskej userstates jako list
            var allUserStates = _context.UserStates
                .Where(s => s.Embedding != null).Include(x => x.Sentiment)
                .ToList();

            var existingMatches = _context.EntryStates
      .Where(x => x.JournalEntry.UserId == GetCurrentUser())
      .Select(x => new { x.JournalEntryId, x.UserStatesEmbId })
      .AsEnumerable()
      .Select(x => (x.JournalEntryId, x.UserStatesEmbId))
      .ToHashSet();

    //vytvorim hash set pro zjisteni poctu, protoze list by sezral moc pameti

            var matches = new List<EntryStateScore>();

            foreach (var part in entryParts)
            {
                var entryVector = _embeddingConverter.ConvertToFloatList(part.Embedding);

                Sentiment Part_sentiment = part.Sentiment;
                var bestMatches = allUserStates.Where(x => x.Sentiment.MainPolarity == Part_sentiment.MainPolarity)
                    .Select(state => new
                    {
                        state.Id,
                        Score = _similarityService.Calculate(
                            entryVector,
                            _embeddingConverter.ConvertToFloatList(state.Embedding))
                    })
                    .OrderByDescending(x => x.Score)
                    .Take(5); 
                //vytvori anonym. strkturu kde je userstate id, skore s entries


                foreach (var match in bestMatches)
                {
                    if (match.Score > AppConstants.MinScore)
                    {
                        if (!existingMatches.Contains((part.JournalEntryId, match.Id))){ 
                        
                            matches.Add(new EntryStateScore(part.JournalEntryId, match.Id, match.Score));                    }


                        //pokud dosavadni entrystates neobsahuji tuhle shodu tak ji tam pridej (kontrola starych shod ktery uz tam jsou pridany

                    }
                }
            }
            _context.EntryStates.AddRange(matches);
            _context.SaveChanges();
        }


        public Dictionary<MoodEnum, Dictionary<UserStateEnum, int>> GetMoodStatesMatches()
        {
            FindEntryStateMatches();
            //zjisti shody a vytvori EntryStates v db (je to hruza, chtelo by to cele prepsat)

            var result = _context.EntryStates
                .Where(x => x.JournalEntry.UserId == GetCurrentUser())
                .Select(x => new
                {
                    x.JournalEntryId,
                    Mood = x.JournalEntry.Mood,
                    UserState = x.UserStatesEmb.UserState
                })
                .Distinct()
                .GroupBy(x => new
                {
                    x.Mood,
                    x.UserState
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
        }

        public Dictionary<string,int> ViewMoodOnWeekDays()
        {
            var data = _context.Entries.Where(x => x.UserId == GetCurrentUser())
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
            var data = _context.Entries.Where(x => x.UserId == GetCurrentUser())
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
                .Where(e => e.Mood != null && e.CreatedAt != null && e.UserId == GetCurrentUser())
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

        public IActionResult ShowMoodProgressMonthly()
        {

            var monthlyData = _context.Entries
                .Where(e => e.Mood != null && e.UserId == GetCurrentUser())
                .GroupBy(e => new
                {
                    e.CreatedAt.Year,
                    e.CreatedAt.Month
                })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Select(g => new
                {
                    Label = $"{g.Key.Month:D2}/{g.Key.Year}",
                    AverageMood = g.Average(x => (int)x.Mood)
                })
                .ToList();

            var createdAts = monthlyData
                .Select(x => x.Label)
                .ToArray();

            var moods = monthlyData
        .Select(x => (MoodEnum)(int)Math.Round(x.AverageMood))
        .ToArray();

            MoodGraphViewModel mgwm = new MoodGraphViewModel(createdAts, moods, "Your average monthly mood");

            return View("ShowMoodProgress", mgwm);
        }

        public IActionResult ShowMoodProgressDaily()
        {

            var dailyData = _context.Entries
                .Where(e => e.Mood != null && e.UserId == GetCurrentUser())
                .GroupBy(e => new
                {
                    e.CreatedAt.Year,
                    e.CreatedAt.Month,
                    e.CreatedAt.Day

                })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .ThenBy(g => g.Key.Day)

                .Select(g => new
                {
                    Label = $"{g.Key.Day:D2}/{g.Key.Month:D2}/{g.Key.Year}",
                    AverageMood = g.Average(x => (int)x.Mood)
                })
                .ToList();

            var createdAts = dailyData
                .Select(x => x.Label)
                .ToArray();

            var moods = dailyData
        .Select(x => (MoodEnum)(int)Math.Round(x.AverageMood))
        .ToArray();

            MoodGraphViewModel mgwm = new MoodGraphViewModel(createdAts, moods,"Your average daily mood");

            return View("ShowMoodProgress", mgwm);
        }






    }

}
