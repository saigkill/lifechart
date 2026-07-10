using System.Globalization;
using LifeChart.Domain.Medications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LifeChart.Infrastructure.Persistence.Configurations;

public class MedicationConfiguration : IEntityTypeConfiguration<Medication>
{
    public void Configure(EntityTypeBuilder<Medication> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name).IsRequired().HasMaxLength(200);
        builder.Property(m => m.Dosage).IsRequired().HasMaxLength(100);

        builder.OwnsMany(m => m.IntakeTimes, b =>
        {
            b.WithOwner().HasForeignKey("MedicationId");
            b.Property<int>("Id").ValueGeneratedOnAdd();
            b.HasKey("Id");
            b.Property(i => i.Time)
                .HasConversion(
                    v => v.ToString("HH:mm"),
                    v => TimeOnly.Parse(v, CultureInfo.CurrentCulture));
            b.Property(i => i.DoseCount);
        });
    }
}
