using AspNetCoreGeneratedDocument;
using Humanizer;
using MentalTrack.Constants;
using MentalTrack.Data;
using MentalTrack.Enums;
using MentalTrack.Models;
using MentalTrack.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
            int mode = _context.Entries.Where(x=> x.UserId == CurrentUserId).GroupBy(x => x.Mood).OrderByDescending(x => x.Count()).Select(x => (int)x.Key).FirstOrDefault();
            return mode;

        }
        public int GetEntriesCount( string CurrentUserId)
        {

            int totalCount = _context.Entries.Where(x=> x.UserId == CurrentUserId).Count();
            return totalCount;

        }
        public Dictionary<MoodEnum, double> GetMoodPercentages(string CurrentUserId)
        {
            int count = GetEntriesCount(CurrentUserId);

            return _context.Entries.Where(x=> x.UserId == CurrentUserId).GroupBy(x => x.Mood).ToDictionary(x => x.Key, x => Math.Round(((double)x.Count() / count) * 100));


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

        public void FindEntryStateMatches(string CurrentUserId)
        {


            //ziskej journal entry parts konkretniho usera jako list
            var entryParts = _context.EntryParts
                .Where(e => e.Embedding != null
                    && e.JournalEntry.UserId == CurrentUserId).Include(x => x.Sentiment)
                .ToList();


            //ziskej userstates jako list
            var allUserStates = _context.UserStates
                .Where(s => s.Embedding != null).Include(x => x.Sentiment)
                .ToList();

            var existingMatches = _context.EntryStates
      .Where(x => x.JournalEntry.UserId == CurrentUserId)
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
                        if (!existingMatches.Contains((part.JournalEntryId, match.Id)))
                        {

                            matches.Add(new EntryStateScore(part.JournalEntryId, match.Id, match.Score));
                        }


                        //pokud dosavadni entrystates neobsahuji tuhle shodu tak ji tam pridej (kontrola starych shod ktery uz tam jsou pridany

                    }
                }
            }
            _context.EntryStates.AddRange(matches);
            _context.SaveChanges();
        }


        public Dictionary<MoodEnum, Dictionary<UserStateEnum, int>> GetMoodStatesMatches(string CurrentUserId)
        {
            FindEntryStateMatches(CurrentUserId);
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


    }
}
