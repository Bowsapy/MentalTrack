using MentalTrack.Data;
using MentalTrack.Models;

namespace MentalTrack.Services
{
    public class StatisticsService
    {

        private readonly AppDbContext _context;

        public StatisticsService(AppDbContext context)
        {
            _context = context;
        }

        public int GetEntriesMode()
        {
            int mode = _context.Entries.GroupBy(x => x.Mood).OrderByDescending(x => x.Count()).Select(x => (int)x.Key).FirstOrDefault();
            return mode;

        }


        



    }
}
