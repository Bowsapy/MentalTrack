using MentalTrack.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace MentalTrack.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<JournalEntry> Entries { get; set; }
        public DbSet<UserStatesEmb> UserStates { get; set; }
        public DbSet<EntryStateScore> EntryStates { get; set; }



    }
}

        
