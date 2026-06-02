using FluentAssertions;
using LifeChart.Domain.Medications;

namespace LifeChart.Tests.Domain.Medications;

public class MedicationTests
{
    private static Medication CreateMedication(int minStock = 5, int currentStock = 10)
        => new("Lithium", "600mg", minStock, currentStock,
            [new IntakeTime(new TimeOnly(8, 0), 1)]);

    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var intakeTime = new IntakeTime(new TimeOnly(8, 0), 2);
        var medication = new Medication("Lithium", "600mg", 5, 20, [intakeTime]);

        medication.Name.Should().Be("Lithium");
        medication.Dosage.Should().Be("600mg");
        medication.MinStock.Should().Be(5);
        medication.CurrentStock.Should().Be(20);
        medication.Active.Should().BeTrue();
        medication.IntakeTimes.Should().HaveCount(1);
    }

    [Theory]
    [InlineData(5, 5, true)]
    [InlineData(5, 4, true)]
    [InlineData(5, 6, false)]
    [InlineData(0, 0, true)]
    public void IsStockLow_ReturnsCorrectValue(int minStock, int currentStock, bool expected)
    {
        var medication = CreateMedication(minStock, currentStock);
        medication.IsStockLow.Should().Be(expected);
    }

    [Fact]
    public void Deactivate_SetsActiveFalse()
    {
        var medication = CreateMedication();
        medication.Deactivate();
        medication.Active.Should().BeFalse();
    }

    [Fact]
    public void Update_ChangesAllProperties()
    {
        var medication = CreateMedication();
        var newIntakeTime = new IntakeTime(new TimeOnly(20, 0), 2);

        medication.Update("Quetiapine", "50mg", 3, 15, [newIntakeTime]);

        medication.Name.Should().Be("Quetiapine");
        medication.Dosage.Should().Be("50mg");
        medication.MinStock.Should().Be(3);
        medication.CurrentStock.Should().Be(15);
        medication.IntakeTimes.Should().HaveCount(1);
        medication.IntakeTimes[0].Time.Should().Be(new TimeOnly(20, 0));
    }

    [Fact]
    public void Update_ReplacesIntakeTimes()
    {
        var medication = new Medication("Lithium", "600mg", 5, 10,
        [
            new IntakeTime(new TimeOnly(8, 0), 1),
            new IntakeTime(new TimeOnly(20, 0), 1)
        ]);

        medication.Update("Lithium", "600mg", 5, 10,
        [
            new IntakeTime(new TimeOnly(12, 0), 3)
        ]);

        medication.IntakeTimes.Should().HaveCount(1);
        medication.IntakeTimes[0].Time.Should().Be(new TimeOnly(12, 0));
    }
}
