using Microsoft.AspNetCore.Mvc;
using MentalTrack.Data;
using MentalTrack.Models;
using MentalTrack.Services;
using MentalTrack.Enums;



namespace MentalTrack.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly EmbeddingService _embeddingService;

        public AnalyticsController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AddNewUserState()
        {
            return View();
        }
        public async void CreateNewUserState(string content, UserStateEnum state)
        {
            _context.UserStates.Add(new UserStatesEmb(state, content, await _embeddingService.GetEmbedding(content)));
        }
    }
}
