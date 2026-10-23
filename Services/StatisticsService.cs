using MentalTrack.Constants;
using MentalTrack.Data;
using MentalTrack.Enums;
using MentalTrack.Models;
using MentalTrack.ViewModels;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;

namespace MentalTrack.Services
{
    public class StatisticsService
    {

        private readonly AppDbContext _context;
        private readonly EmbeddingService _embeddingService;
        private readonly CosineSimilarityService _similarityService;
        private readonly EmbeddingConverter _embeddingConverter;




        public StatisticsService(AppDbContext context, EmbeddingConverter embeddingConverter, EmbeddingService embeddingService, CosineSimilarityService similarity)
        {
            _context = context;
            _similarityService = similarity;
            _embeddingService = embeddingService;
            _embeddingConverter = embeddingConverter;
        }



        public int GetEntriesMode(string CurrentUserId)
        {
            int mode = _context.Entries.Where(x => x.UserId == CurrentUserId).GroupBy(x => x.Mood).OrderByDescending(x => x.Count()).Select(x => (int)x.Key).FirstOrDefault();
            return mode;

        }
        public int GetEntriesCount(string CurrentUserId)
        {

            int totalCount = _context.Entries.Where(x => x.UserId == CurrentUserId).Count();
            return totalCount;

        }
        public Dictionary<MoodEnum, double> GetMoodPercentages(string CurrentUserId)
        {
            int count = GetEntriesCount(CurrentUserId);

            return _context.Entries.Where(x => x.UserId == CurrentUserId).GroupBy(x => x.Mood).ToDictionary(x => x.Key, x => Math.Round(((double)x.Count() / count) * 100));


        }
        public Dictionary<string, int> ViewMoodOnWeekDays(string CurrentUserId)
        {
            var data = _context.Entries.Where(x => x.UserId == CurrentUserId)
            .AsEnumerable()
            .GroupBy(e => e.CreatedAt.DayOfWeek)
            .OrderBy(g => (int)Enum.Parse<DayInWeekEnum>(g.Key.ToString()))
            .ToDictionary(
            g => g.Key.ToString(),
            g => (int)g.Average(x => (int)x.Mood)
       );

            return data;
        }
        public Dictionary<DayPhasesEnum, int> ViewMoodOnDayPhases(string CurrentUserId)
        {
            var data = _context.Entries.Where(x => x.UserId == CurrentUserId)
           .AsEnumerable()
           .GroupBy(e => e.DayPhase)
           .OrderBy(g => (int)Enum.Parse<DayPhasesEnum>(g.Key.ToString()))
           .ToDictionary(
           g => g.Key,
           g => (int)g.Average(x => (int)x.Mood)
       );

            return data;
        }
        public void FindEntryStateMatches2(string CurrentUserId)
        {
            HashSet<JournalEntryPart> entryParts = _context.EntryParts
              .Where(e => e.Embedding != null
                  && e.JournalEntry.UserId == CurrentUserId).Include(x => x.Sentiment)
              .ToHashSet();

            //ziskej userstates jako list
            HashSet<UserStatesEmb> allUserStates = _context.UserStates
                .Where(s => s.Embedding != null).Include(x => x.Sentiment)
                .ToHashSet();

            var existingMatches = _context.EntryStates
              .Where(x => x.JournalEntry.UserId == CurrentUserId)
              .Select(x => new { x.JournalEntryId, x.UserStatesEmbId })
              .AsEnumerable()
              .Select(x => (x.JournalEntryId, x.UserStatesEmbId))
              .ToHashSet();

            foreach (var part in entryParts)
            {
                var validUserstates = allUserStates.Where(x => x.Sentiment.MainPolarity == part.Sentiment.MainPolarity);
                var partEmbedding = part.Embedding;
                foreach (var state in validUserstates)
                {
                    double score = _similarityService.Calculate(partEmbedding, state.Embedding);
                    if (score > AppConstants.MinScore && !existingMatches.Contains((part.JournalEntryId, state.Id)))
                    {
                        _context.EntryStates.Add(new EntryStateScore(part.JournalEntry.Id, state.Id, score));
                    }
                }
            }
            _context.SaveChanges();
        }


        public async Task FindEntryStateMatches(string currentUserId)
        {
            // Načteme části deníku aktuálního uživatele.
            var entryParts = await _context.EntryParts
                .Where(e => e.Embedding != null
                    && e.JournalEntry.UserId == currentUserId)
                .Select(e => new
                {
                    e.Id,
                    e.JournalEntryId,
                    e.Embedding,
                    Polarity = e.Sentiment.MainPolarity
                })
                .ToListAsync();

            // Načteme existující shody, abychom je nevytvářeli znovu.
            var existingMatches = await _context.EntryStates
                .Where(x => x.JournalEntry.UserId == currentUserId)
                .Select(x => new
                {
                    x.JournalEntryId,
                    x.UserStatesEmbId
                })
                .ToListAsync();

            var existingMatchSet = existingMatches
                .Select(x => (x.JournalEntryId, x.UserStatesEmbId))
                .ToHashSet();

            var matches = new List<EntryStateScore>();

            foreach (var part in entryParts)
            {
                // PostgreSQL vyhledá 20 nejbližších UserStates
                // se stejnou polaritou.
                var candidates = await _context.UserStates
                    .Where(s => s.Embedding != null
                        && s.Sentiment.MainPolarity == part.Polarity)
                    .OrderBy(s =>
                        s.Embedding!.CosineDistance(part.Embedding))
                    .Take(20)
                    .Select(s => new
                    {
                        s.Id,
                        Distance = s.Embedding!.CosineDistance(
                            part.Embedding)
                    })
                    .ToListAsync();

                foreach (var candidate in candidates)
                {
                    double score = 1.0 - candidate.Distance;

                    if (score > AppConstants.MinScore
                        && !existingMatchSet.Contains(
                            (part.JournalEntryId, candidate.Id)))
                    {
                        matches.Add(new EntryStateScore(
                            part.JournalEntryId,
                            candidate.Id,
                            score));

                        // Zabrání duplicitám i v rámci aktuálního běhu.
                        existingMatchSet.Add(
                            (part.JournalEntryId, candidate.Id));
                    }
                }
            }

            if (matches.Count > 0)
            {
                _context.EntryStates.AddRange(matches);
                await _context.SaveChangesAsync();

            }
        }


        public async Task<Dictionary<MoodEnum, Dictionary<UserStateEnum, int>>> GetMoodStatesMatches(string CurrentUserId)
        {
            await FindEntryStateMatches(CurrentUserId);
            //zjisti shody a vytvori EntryStates v db (je to hruza, chtelo by to cele prepsat)

            var result = _context.EntryStates
                .Where(x => x.JournalEntry.UserId == CurrentUserId)
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

        public MoodGraphViewModel GetMonthlyDataForGraph(string CurrentUserId)
        {
            var monthlyData = _context.Entries
                   .Where(e => e.Mood != null && e.UserId == CurrentUserId)
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
            return mgwm;
        }

        public MoodGraphViewModel GetDailyDataForGraph(string CurrentUserId)

        {
            var dailyData = _context.Entries
            .Where(e => e.Mood != null && e.UserId == CurrentUserId)
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

            MoodGraphViewModel mgwm = new MoodGraphViewModel(createdAts, moods, "Your average daily mood");
            return mgwm;

        }

        public MoodGraphViewModel GetDataForGraphForAllEntries(string CurrentUserId)
        {

            var entries = _context.Entries
                .Where(e => e.Mood != null && e.CreatedAt != null && e.UserId == CurrentUserId)
                .OrderBy(e => e.CreatedAt)
                .ToList(); // ToList() vyhodnotí query a pošle jen validní data

            var createdAts = entries
            .Select(e => e.CreatedAt.ToString("dd.MM.yyyy"))
            .ToArray();

            var moods = entries
                .Select(e => e.Mood)
                .ToArray();

            MoodGraphViewModel mgwm = new MoodGraphViewModel(createdAts, moods, "All entries");
            return mgwm;

        }

        public WordCountAnalysisViewModel GetWordCountAnalysis(string CurrentUserId)
        {
            Dictionary<MoodEnum, Dictionary<string, int>> result =
             _context.Entries.Where(x => x.UserId == CurrentUserId)
           .AsEnumerable()
           //v podstate vezmu 

           .GroupBy(e => e.Mood)
            .ToDictionary(
            g => g.Key,
            g => g
        .SelectMany(e =>
            e.Content
             .Split(' ', StringSplitOptions.RemoveEmptyEntries)
             .Select(w => w.ToLower())
             .Where(w => !NonTrackedWords.English.Contains(w))
             .Distinct())          // (vem jeden entry content ten rozsekej na slova a vyrad duplictni slova) pro kazdou entry v jednom mood
        .GroupBy(w => w)
        .ToDictionary(
            x => x.Key,
            x => x.Count()
        ));


            WordCountAnalysisViewModel WCAVM = new WordCountAnalysisViewModel(result);
            return WCAVM;
        }
        public async Task<List<UserstatesProgressViewModel>> GetUserStatesInDays(string currentUserId)
        {
            await FindEntryStateMatches(currentUserId);
            var result = _context.EntryStates
               .Where(x => x.JournalEntry.UserId == currentUserId)
               .Select(x => new UserstatesProgressViewModel
               {
                   Date = DateOnly.FromDateTime(x.JournalEntry.CreatedAt),
                   State = x.UserStatesEmb.UserState.GetDisplayName(),
                   Value = 1
               })
               .ToList();

            return result;
        }

    }
}
