using System.Globalization;
using LifeChart.Application.DTOs;
using LifeChart.Application.Interfaces;
using LifeChart.Application.Localization;
using LifeChart.Application.UseCases.Medications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace LifeChart.Linux.Pages;

public partial class MedicationFormPage : ContentPage
{
    private readonly int _editId;
    private readonly List<(TimePicker Picker, Entry DoseEntry)> _intakeRows = [];

    public MedicationFormPage(MedicationDto? existing = null)
    {
        InitializeComponent();

        _editId = existing?.Id ?? 0;
        Title = existing is null ? L.MedForm_TitleAdd : L.MedForm_TitleEdit;

        NameFieldLabel.Text = L.Common_Name;
        NameEntry.Placeholder = L.MedForm_NamePlaceholder;
        DosageFieldLabel.Text = L.MedForm_Dosage;
        DosageEntry.Placeholder = L.MedForm_DosagePlaceholder;
        MinStockFieldLabel.Text = L.MedForm_MinStock;
        CurrentStockFieldLabel.Text = L.MedForm_CurrentStock;
        IntakeTimesLabel.Text = L.MedForm_IntakeTimes;
        AddTimeButton.Text = L.MedForm_AddTime;
        SaveButton.Text = L.Common_Save;
        CancelButton.Text = L.Common_Cancel;

        if (existing is not null)
        {
            NameEntry.Text = existing.Name;
            DosageEntry.Text = existing.Dosage;
            MinStockEntry.Text = existing.MinStock.ToString();
            CurrentStockEntry.Text = existing.CurrentStock.ToString();
            foreach (var it in existing.IntakeTimes)
                AddIntakeRow(TimeOnly.Parse(it.Time, CultureInfo.CurrentCulture), it.DoseCount);
        }

        if (_intakeRows.Count == 0)
            AddIntakeRow(new TimeOnly(8, 0), 1);
    }

    private void OnAddTimeClicked(object? sender, EventArgs e)
        => AddIntakeRow(new TimeOnly(8, 0), 1);

    private async void OnCancelClicked(object? sender, EventArgs e)
        => await Navigation.PopAsync();

    private void AddIntakeRow(TimeOnly time, int doseCount)
    {
        var picker = new TimePicker { Time = new TimeSpan(time.Hour, time.Minute, 0) };
        var doseEntry = new Entry
        {
            Text = doseCount.ToString(),
            Keyboard = Keyboard.Numeric,
            WidthRequest = 48,
        };
        var removeButton = new Button
        {
            Text = "X",
            WidthRequest = 40,
            HeightRequest = 40,
            Padding = 0,
            BackgroundColor = Colors.IndianRed,
            TextColor = Colors.White,
        };

        var row = new HorizontalStackLayout
        {
            Spacing = 8,
            Children =
            {
                new Label { Text = L.Common_Time, VerticalOptions = LayoutOptions.Center },
                picker,
                new Label { Text = L.MedForm_Doses, VerticalOptions = LayoutOptions.Center },
                doseEntry,
                removeButton,
            }
        };

        removeButton.Clicked += (_, _) =>
        {
            IntakeTimesLayout.Children.Remove(row);
            _intakeRows.Remove((picker, doseEntry));
        };

        _intakeRows.Add((picker, doseEntry));
        IntakeTimesLayout.Children.Add(row);
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text))
        {
            StatusLabel.TextColor = Colors.Red;
            StatusLabel.Text = L.MedForm_NameRequired;
            return;
        }

        if (!int.TryParse(MinStockEntry.Text, out var minStock))
            minStock = 0;
        if (!int.TryParse(CurrentStockEntry.Text, out var currentStock))
            currentStock = 0;

        var intakeTimes = _intakeRows
            .Select(r => new IntakeTimeDto(
                $"{(r.Picker.Time ?? TimeSpan.Zero).Hours:D2}:{(r.Picker.Time ?? TimeSpan.Zero).Minutes:D2}",
                int.TryParse(r.DoseEntry.Text, out var d) ? Math.Max(1, d) : 1))
            .ToList()
            .AsReadOnly();

        if (intakeTimes.Count == 0)
        {
            StatusLabel.TextColor = Colors.Red;
            StatusLabel.Text = L.MedForm_IntakeRequired;
            return;
        }

        var dto = new SaveMedicationDto(
            _editId,
            NameEntry.Text.Trim(),
            DosageEntry.Text?.Trim() ?? string.Empty,
            minStock,
            currentStock,
            intakeTimes);

        try
        {
            var services = IPlatformApplication.Current!.Services;
            await services.GetRequiredService<SaveMedicationUseCase>().ExecuteAsync(dto);

            var allMeds = await services.GetRequiredService<GetActiveMedicationsUseCase>().ExecuteAsync();
            await services.GetRequiredService<IAlarmService>().ScheduleAsync(allMeds);

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            StatusLabel.TextColor = Colors.Red;
            StatusLabel.Text = L.Fmt("Common_ErrorFmt", ex.Message);
        }
    }
}
