using MentalTrack.Data;
using MentalTrack.Enums;
using MentalTrack.Models;
using MentalTrack.Services;
using MentalTrack.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MentalTrack.Controllers
{
	public class AnalyticsController : Controller
	{
		private readonly ILogger<AnalyticsController> _logger;
		private readonly AppDbContext _context;
		private readonly ExternalAPIService _externalAPIService;
		private readonly StatisticsService _statisticsService;

		public AnalyticsController(AppDbContext context, ExternalAPIService externalAPIService, ILogger<AnalyticsController> logger, WorkingWithDates dateService, StatisticsService statistics)
		{
			_context = context;
			_externalAPIService = externalAPIService;
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
			UserStatesEmb NewUserStateEmb = new UserStatesEmb(state, content, await _externalAPIService.GetEmbedding(content));
			Sentiment NewSentiment = await _externalAPIService.GetSentiment(NewUserStateEmb);
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
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			return userId;
		}

		public IActionResult MoodSummary()
		{
			string user_id = GetCurrentUser();

			MoodSummaryViewModel moodsumVM = new MoodSummaryViewModel(_statisticsService.GetMoodPercentages(GetCurrentUser()), _statisticsService.GetEntriesMode(user_id), _statisticsService.GetEntriesCount(user_id));

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

		public async Task<IActionResult> Activities()
		{
			var result = await _statisticsService.GetMoodStatesMatches(GetCurrentUser());
			var result2 = await _statisticsService.GetUserStatesInDays(GetCurrentUser());

			var ActivitiesViewModel = new ActivitesViewModel(result, result2);

			return View(ActivitiesViewModel);
		}

		public IActionResult DatetimeAnalysis()
		{
			var dateTimeAnalysisViewModel = new DatetimeAnalysisViewModel(_statisticsService.ViewMoodOnDayPhases(GetCurrentUser()), _statisticsService.ViewMoodOnWeekDays(GetCurrentUser()));
			return View(dateTimeAnalysisViewModel);
		}

		public IActionResult MoodInDay()
		{
			return View(_statisticsService.ViewMoodOnDayPhases(GetCurrentUser()));
		}

		public async Task<IActionResult> UsInDay()
		{
			var result = await _statisticsService.GetUserStatesInDays(GetCurrentUser());

			return View(result);
		}
	}
}
