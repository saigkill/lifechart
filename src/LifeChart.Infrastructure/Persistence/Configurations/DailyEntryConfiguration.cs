using LifeChart.Domain.Entries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LifeChart.Infrastructure.Persistence.Configurations;

public class DailyEntryConfiguration : IEntityTypeConfiguration<DailyEntry>
{
    public void Configure(EntityTypeBuilder<DailyEntry> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.Date).IsUnique();

        builder.Property(e => e.Date)
            .HasConversion(
                v => v.ToDateTime(TimeOnly.MinValue),
                v => DateOnly.FromDateTime(v));

        builder.Property(e => e.Mood)
            .HasConversion(
                v => v.Value,
                v => new MoodScore(v));

        builder.Property(e => e.Functionality)
            .HasConversion(
                v => v.Value,
                v => new FunctionalityScore(v));

        builder.Property(e => e.SleepHours)
            .HasConversion(
                v => v.Value,
                v => new SleepHours(v));

        builder.Property(e => e.Symptoms).HasMaxLength(1000);
        builder.Property(e => e.Notes).HasMaxLength(2000);
    }
}
