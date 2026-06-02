using FluentAssertions;
using LifeChart.Application.DTOs;
using LifeChart.Application.UseCases.Entries;
using LifeChart.Domain.Entries;
using LifeChart.Domain.Services;
using NSubstitute;

namespace LifeChart.Tests.Application.UseCases;

public class SaveDailyEntryUseCaseTests
{
    private readonly IDailyEntryRepository _repository = Substitute.For<IDailyEntryRepository>();
    private readonly CrisisThresholdService _crisisService = new();
    private readonly SaveDailyEntryUseCase _useCase;

    public SaveDailyEntryUseCaseTests()
    {
        _useCase = new SaveDailyEntryUseCase(_repository, _crisisService);
    }

    [Fact]
    public async Task ExecuteAsync_CallsRepositorySave()
    {
        var dto = CreateDto(mood: 0, functionality: 0);
        await _useCase.ExecuteAsync(dto);
        await _repository.Received(1).SaveAsync(Arg.Any<DailyEntry>());
    }

    [Fact]
    public async Task ExecuteAsync_BothScoresCritical_ReturnsIsCriticalTrue()
    {
        var dto = CreateDto(mood: -5, functionality: -5);
        var result = await _useCase.ExecuteAsync(dto);
        result.IsCritical.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_OnlyMoodCritical_ReturnsIsCriticalFalse()
    {
        var dto = CreateDto(mood: -5, functionality: 0);
        var result = await _useCase.ExecuteAsync(dto);
        result.IsCritical.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_NeitherCritical_ReturnsIsCriticalFalse()
    {
        var dto = CreateDto(mood: 0, functionality: 0);
        var result = await _useCase.ExecuteAsync(dto);
        result.IsCritical.Should().BeFalse();
    }

    [Theory]
    [InlineData(-4, -4, true)]
    [InlineData(-4, -3, false)]
    [InlineData(-3, -4, false)]
    [InlineData(-3, -3, false)]
    public async Task ExecuteAsync_ThresholdCombinations_ReturnsExpected(
        int mood, int func, bool expected)
    {
        var dto = CreateDto(mood, func);
        var result = await _useCase.ExecuteAsync(dto);
        result.IsCritical.Should().Be(expected);
    }

    [Fact]
    public async Task ExecuteAsync_InvalidMoodScore_ThrowsArgumentOutOfRangeException()
    {
        var dto = CreateDto(mood: -6, functionality: 0);
        var act = async () => await _useCase.ExecuteAsync(dto);
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    private static DailyEntryDto CreateDto(int mood, int functionality) => new(
        Date: DateOnly.FromDateTime(DateTime.Today),
        Mood: mood,
        Functionality: functionality,
        SleepHours: 7,
        MedicationTaken: false,
        MenstrualCycle: false,
        Symptoms: null,
        Notes: null);
}
