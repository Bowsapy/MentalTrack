using MentalTrack.Controllers;
using MentalTrack.Data;
using MentalTrack.Models;
using MentalTrack.Services;
using MentalTrack.ViewModels;
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
    private readonly EmbeddingConverter _embeddingConverter;
    private readonly ChunkJournalEntry _chunkJournalEntry;
    private readonly WorkingWithDates _workingWithDates;
    private readonly SentimentService _sentimentService;


    public DiaryController(AppDbContext context, WorkingWithDates workingWithDates, ILogger<DiaryController> logger, EmbeddingService embeddingService, CosineSimilarityService similarityService,EmbeddingConverter embeddingConverter,ChunkJournalEntry chunkJournalEntry)
    {
        _context = context;
        _logger = logger;
        _embeddingService = embeddingService;
        _similarityService = similarityService;
        _embeddingConverter = embeddingConverter;
        _chunkJournalEntry = chunkJournalEntry;
        _workingWithDates = workingWithDates;
    }

    public IActionResult ShowEntries()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var entries = _context.Entries
            
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


    public IActionResult ShowMoodProgressMonthly()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var monthlyData = _context.Entries
            .GroupBy(x => new { x.CreatedAt.Year, x.CreatedAt.Month })
            .Select(g => new
            {
                Label = $"{g.Key.Month:D2}/{g.Key.Year}",
                AverageMood = g.Average(x => (int)x.Mood)
            })
            .OrderBy(x => DateTime.ParseExact(x.Label, "MM/yyyy", null))
            .ToList();


        return View(monthlyData);
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

        var entry = _context.Entries.FirstOrDefault(e => e.Id == id && e.UserId == userId);
        if (entry == null)
            return NotFound();

        List<Sentiment> allSentiments = _context.Sentiments
            .Where(x => x.JournalEntryPart.JournalEntryId == id)
            .ToList();

        var entryParts = _context.EntryParts.Where(y => y.JournalEntry.Id == id);

        var entryStates = _context.EntryStates.Where(l => l.JournalEntryId == id);

        _context.Sentiments.RemoveRange(allSentiments);
        _context.EntryParts.RemoveRange(entryParts);
        _context.EntryStates.RemoveRange(entryStates);
       

        _context.Entries.Remove(entry);


   
        _context.SaveChanges();          

        return RedirectToAction("ShowEntries"); // Vyresit smazani parts + embedingu
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


        entry.DayPhase = _workingWithDates.GetDayPhase(entry.CreatedAt);

        _context.Entries.Add(entry);
        _context.SaveChanges();








        await _chunkJournalEntry.ChunkEntry(entry);





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

        var targetVector = _embeddingConverter.ConvertToFloatList(target.Embedding);

        var entries = _context.Entries
            .Where(e => e.UserId == userId && e.Embedding != null)
            .ToList();


        var sorted = entries
            .Select(e => new
            {
                Entry = e,
                Score = _similarityService.Calculate(
                    targetVector,
                   _embeddingConverter.ConvertToFloatList(e.Embedding)
                )
            })
            .OrderByDescending(x => x.Score)
            .Where(x => x.Score > 0.6)
            .Select(x => x.Entry)
            .ToList();

        return View(sorted);
    }




}