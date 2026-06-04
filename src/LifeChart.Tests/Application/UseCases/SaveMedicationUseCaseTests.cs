using FluentAssertions;
using LifeChart.Application.DTOs;
using LifeChart.Application.UseCases.Medications;
using LifeChart.Domain.Medications;
using NSubstitute;

namespace LifeChart.Tests.Application.UseCases;

public class SaveMedicationUseCaseTests
{
    private readonly IMedicationRepository _repository = Substitute.For<IMedicationRepository>();
    private readonly SaveMedicationUseCase _useCase;

    public SaveMedicationUseCaseTests()
        => _useCase = new SaveMedicationUseCase(_repository);

    [Fact]
    public async Task ExecuteAsync_NewMedication_CallsRepositorySave()
    {
        var dto = CreateDto(id: 0);
        await _useCase.ExecuteAsync(dto);
        await _repository.Received(1).SaveAsync(Arg.Any<Medication>());
    }

    [Fact]
    public async Task ExecuteAsync_ExistingMedication_LoadsAndUpdates()
    {
        var existing = new Medication("Alt", "100mg", 5, 10,
            [new IntakeTime(new TimeOnly(8, 0), 1)]);

        _repository.GetByIdAsync(42).Returns(existing);

        var dto = CreateDto(id: 42, name: "Neu", dosage: "200mg");
        await _useCase.ExecuteAsync(dto);

        existing.Name.Should().Be("Neu");
        existing.Dosage.Should().Be("200mg");
        await _repository.Received(1).SaveAsync(existing);
    }

    [Fact]
    public async Task ExecuteAsync_NonExistentId_ThrowsInvalidOperationException()
    {
        _repository.GetByIdAsync(99).Returns((Medication?)null);

        var dto = CreateDto(id: 99);
        var act = async () => await _useCase.ExecuteAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*99*");
    }

    [Fact]
    public async Task ExecuteAsync_ParsesIntakeTimes()
    {
        Medication? saved = null;
        await _repository.SaveAsync(Arg.Do<Medication>(m => saved = m));

        var dto = CreateDto(id: 0, intakeTimes:
        [
            new IntakeTimeDto("08:00", 1),
            new IntakeTimeDto("20:00", 2)
        ]);

        await _useCase.ExecuteAsync(dto);

        saved.Should().NotBeNull();
        saved!.IntakeTimes.Should().HaveCount(2);
        saved.IntakeTimes[0].Time.Should().Be(new TimeOnly(8, 0));
        saved.IntakeTimes[1].DoseCount.Should().Be(2);
    }

    private static SaveMedicationDto CreateDto(
        int id = 0,
        string name = "Lithium",
        string dosage = "600mg",
        IReadOnlyList<IntakeTimeDto>? intakeTimes = null) => new(
            Id: id,
            Name: name,
            Dosage: dosage,
            MinStock: 5,
            CurrentStock: 20,
            IntakeTimes: intakeTimes ?? [new IntakeTimeDto("08:00", 1)]);
}
