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
        private readonly StatisticsService _statisticsService;


        public AnalyticsController(AppDbContext context, EmbeddingService embeddingService, SentimentService sentimentService, EmbeddingConverter embeddingConverter, CosineSimilarityService similarityService, ILogger<AnalyticsController> logger,WorkingWithDates dateService, StatisticsService statistics)
        {
            _context = context;
            _embeddingService = embeddingService;
            _embeddingConverter = embeddingConverter;
            _sentimentService = sentimentService;
            _similarityService = similarityService;
            _logger = logger;
            _dateService = dateService;
            _statisticsService = statistics;
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

       
   


        public IActionResult MoodSummary()
        {
            Dictionary<MoodEnum,Dictionary<UserStateEnum,int>> USMatches = _statisticsService.GetMoodStatesMatches(GetCurrentUser());
            Dictionary<string, int> WDMatches = _statisticsService.ViewMoodOnWeekDays(GetCurrentUser());
            Dictionary<string, int> DPMatches = _statisticsService.ViewMoodOnDayPhases(GetCurrentUser());


            MoodSummaryViewModel moodsumVM = new MoodSummaryViewModel(USMatches, WDMatches, DPMatches,_statisticsService.GetMoodPercentages(),_statisticsService.GetEntriesMode(),_statisticsService.GetEntriesCount());

            return View(moodsumVM);
        }
        public IActionResult ShowMoodProgress()
        {

            return View("ShowMoodProgress", _statisticsService.GetDataForGraphForAllEntries(GetCurrentUser()));

        }

        public IActionResult ShowMoodProgressMonthly()
        {



            return View("ShowMoodProgress", _statisticsService.GetMonthlyDataForGraph(GetCurrentUser()));
        }

        public IActionResult ShowMoodProgressDaily()
        {


            return View("ShowMoodProgress", _statisticsService.GetDailyDataForGraph(GetCurrentUser()));
        }
 

 






    }

}
