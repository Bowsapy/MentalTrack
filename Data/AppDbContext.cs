using MentalTrack.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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



    }
}

        
