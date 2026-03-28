using MentalTrack.Controllers;
using MentalTrack.Data;
using MentalTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
[Authorize]
public class DiaryController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<DiaryController> _logger;
    private readonly EmbeddingService _embeddingService;
    private readonly CosineSimilarityService _similarityService;
    public DiaryController(AppDbContext context, ILogger<DiaryController> logger, EmbeddingService embeddingService, CosineSimilarityService similarityService)
    {
        _context = context;
        _logger = logger;
        _embeddingService = embeddingService;
        _similarityService = similarityService;
    }

    public IActionResult ShowEntries()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var entries = _context.Entries
            //.Include(e => e.User) // pokud chceš načíst celý objekt User, jinak nepotřebuješ
            .Where(e => e.UserId == userId)  // filtr podle aktuálního uživatele
            .OrderByDescending(e => e.CreatedAt) // nejnovější nahoře
            .ToList();

        return View(entries);  // předá seznam do Index.cshtml
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
    public IActionResult ShowMoodProgress()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var entries = _context.Entries
            .Where(e => e.Mood != null && e.CreatedAt != null && e.UserId ==userId)
            .OrderBy(e => e.CreatedAt)
            .ToList(); // ToList() vyhodnotí query a pošle jen validní data
        return View(entries);
    }
    public IActionResult ShowDiaryContent(int id)
    {

        var entry = _context.Entries.FirstOrDefault(e => e.Id == id);
        if (entry == null) return NotFound(); 
        return View(entry);

    }
    [HttpPost]
    public IActionResult DeleteDiaryEntry(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // najdeme záznam, který patří tomuto uživateli
        var entry = _context.Entries.FirstOrDefault(e => e.Id == id && e.UserId == userId);
        if (entry == null)
            return NotFound();

        _context.Entries.Remove(entry);
        _context.SaveChanges();          

        return RedirectToAction("ShowEntries"); // 
    }

    [HttpPost]
    public async Task<IActionResult> Create(JournalEntry entry)
    {
        _logger.LogInformation("Create called");

        entry.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        entry.CreatedAt = DateTime.Now;

        ModelState.Remove("UserId");
        TryValidateModel(entry);

        if (!ModelState.IsValid)
        {
            return View(entry);
        }

        if (string.IsNullOrWhiteSpace(entry.Content))
        {
            ModelState.AddModelError("Content", "Content cannot be empty");
            return View(entry);
        }

        entry.Embedding = await _embeddingService.GetEmbedding(entry.Content);

        _context.Entries.Add(entry);
        _context.SaveChanges();

        return RedirectToAction("ShowEntries");
    }
    public IActionResult SimilarEntries(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var target = _context.Entries
            .FirstOrDefault(e => e.Id == id && e.UserId == userId);

        if (target == null)
            return NotFound();

        var targetVector = (target.Embedding).ToList();

        var entries = _context.Entries
            .Where(e => e.UserId == userId && e.Embedding != null)
            .ToList();


        var sorted = entries
            .Select(e => new
            {
                Entry = e,
                Score = _similarityService.Calculate(
                    targetVector,
                   e.Embedding.ToList()
                )
            })
            .OrderByDescending(x => x.Score)
            .Where(x => x.Score > 0.7)
            .Select(x => x.Entry)
            .ToList();

        return View(sorted);
    }




}