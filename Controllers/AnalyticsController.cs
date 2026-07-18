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
        private readonly SentimentService _sentimentService;
        private readonly StatisticsService _statisticsService;


        public AnalyticsController(AppDbContext context, EmbeddingService embeddingService, SentimentService sentimentService, EmbeddingConverter embeddingConverter, CosineSimilarityService similarityService, ILogger<AnalyticsController> logger,WorkingWithDates dateService, StatisticsService statistics)
        {
            _context = context;
            _embeddingService = embeddingService;
            _sentimentService = sentimentService;
            _logger = logger;
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
            string user_id = GetCurrentUser();
            Dictionary<MoodEnum,Dictionary<UserStateEnum,int>> USMatches = _statisticsService.GetMoodStatesMatches(user_id);
            Dictionary<string, int> WDMatches = _statisticsService.ViewMoodOnWeekDays(user_id);
            Dictionary<string, int> DPMatches = _statisticsService.ViewMoodOnDayPhases(user_id);


            MoodSummaryViewModel moodsumVM = new MoodSummaryViewModel(_statisticsService.GetMoodPercentages(GetCurrentUser()),_statisticsService.GetEntriesMode(user_id),_statisticsService.GetEntriesCount(user_id));

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
        public IActionResult WordCountAnalysis()
        {
            return View(_statisticsService.GetWordCountAnalysis(GetCurrentUser()));
        }
        public IActionResult AIAnalysis()
        {
            return View(_statisticsService.GetMoodStatesMatches(GetCurrentUser()));
        }
        public IActionResult MoodInWeek()
        {
            return View(_statisticsService.ViewMoodOnWeekDays(GetCurrentUser()));
        }
        public IActionResult MoodInDay()
        {
            return View(_statisticsService.ViewMoodOnDayPhases(GetCurrentUser()));
        }








    }

}
