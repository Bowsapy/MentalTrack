using MentalTrack.Controllers;
using MentalTrack.Data;
using MentalTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
[Authorize]
public class DiaryController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<DiaryController> _logger;


    public DiaryController(AppDbContext context, ILogger<DiaryController> logger)
    {
        _context = context;
        _logger = logger;

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
    public IActionResult Create(JournalEntry entry)
    {
        _logger.LogInformation("---------------------------------------------------------------------");
        foreach (var state in ModelState)
        {
            foreach (var error in state.Value.Errors)
            {
                _logger.LogInformation("Pole '{0}': {1}", state.Key, error.ErrorMessage);
            }
        }        // nastavení UserId a času před validací
        entry.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        entry.CreatedAt = DateTime.Now;

        // odstranění staré chyby a znovu validace
        ModelState.Remove("UserId");
        TryValidateModel(entry);


        if (ModelState.IsValid)
        {
            _context.Entries.Add(entry);
            _context.SaveChanges();

            return RedirectToAction("ShowEntries");
        }

        foreach (var state in ModelState)
        {
            foreach (var error in state.Value.Errors)
            {
                _logger.LogInformation($"Pole '{state.Key}': {error.ErrorMessage}");
            }
        }
        return View(entry);
    }

    

}