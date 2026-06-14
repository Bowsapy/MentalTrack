using MentalTrack.Enums;
using MentalTrack.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;


namespace MentalTrack.Data;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<JournalEntry> Entries { get; set; }
    public DbSet<UserStatesEmb> UserStates { get; set; }
    public DbSet<EntryStateScore> EntryStates { get; set; }
    public DbSet<JournalEntryPart> EntryParts { get; set; }
    public DbSet<MoodSummary> MoodSummaries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<MoodSummary>()
            .Property(x => x.Mood)
            .HasConversion(enumArrayConverter<MoodEnum>());

        modelBuilder.Entity<MoodSummary>()
            .Property(x => x.UserStates)
            .HasConversion(enumArrayConverter<UserStateEnum>());

        modelBuilder.Entity<MoodSummary>()
            .Property(x => x.DayPhases)
            .HasConversion(enumArrayConverter<DayPhasesEnum>());

        modelBuilder.Entity<MoodSummary>()
            .Property(x => x.WeekDays)
            .HasConversion(enumArrayConverter<DayInWeekEnum>());
    }
    private static ValueConverter<TEnum[], string> enumArrayConverter<TEnum>()
    {
        return new ValueConverter<TEnum[], string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<TEnum[]>(v, (JsonSerializerOptions?)null)!
        );
    }
}