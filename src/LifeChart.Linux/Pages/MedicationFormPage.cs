using LifeChart.Application.DTOs;
using LifeChart.Application.Interfaces;
using LifeChart.Application.Localization;
using LifeChart.Application.UseCases.Medications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace LifeChart.Linux.Pages;

public class MedicationFormPage : ContentPage
{
    private readonly int _editId;
    private readonly Entry _nameEntry = new();
    private readonly Entry _dosageEntry = new();
    private readonly Entry _minStockEntry = new() { Placeholder = "0", Keyboard = Keyboard.Numeric };
    private readonly Entry _currentStockEntry = new() { Placeholder = "0", Keyboard = Keyboard.Numeric };
    private readonly VerticalStackLayout _intakeTimesLayout = new() { Spacing = 6 };
    private readonly Label _statusLabel = new() { HorizontalOptions = LayoutOptions.Center };
    private readonly List<(TimePicker Picker, Entry DoseEntry)> _intakeRows = [];

    public MedicationFormPage(MedicationDto? existing = null)
    {
        _editId = existing?.Id ?? 0;
        Title = existing is null ? L.MedForm_TitleAdd : L.MedForm_TitleEdit;

        _nameEntry.Placeholder = L.MedForm_NamePlaceholder;
        _dosageEntry.Placeholder = L.MedForm_DosagePlaceholder;

        if (existing is not null)
        {
            _nameEntry.Text = existing.Name;
            _dosageEntry.Text = existing.Dosage;
            _minStockEntry.Text = existing.MinStock.ToString();
            _currentStockEntry.Text = existing.CurrentStock.ToString();
            foreach (var it in existing.IntakeTimes)
                AddIntakeRow(TimeOnly.Parse(it.Time), it.DoseCount);
        }

        if (_intakeRows.Count == 0)
            AddIntakeRow(new TimeOnly(8, 0), 1);

        var addTimeButton = new Button
        {
            Text = L.MedForm_AddTime,
            HorizontalOptions = LayoutOptions.Start,
        };
        addTimeButton.Clicked += (_, _) => AddIntakeRow(new TimeOnly(8, 0), 1);

        var saveButton = new Button { Text = L.Common_Save, HorizontalOptions = LayoutOptions.Center };
        saveButton.Clicked += OnSaveClicked;

        var cancelButton = new Button
        {
            Text = L.Common_Cancel,
            HorizontalOptions = LayoutOptions.Center,
            BackgroundColor = Colors.Gray,
        };
        cancelButton.Clicked += async (_, _) => await Navigation.PopAsync();

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Spacing = 12,
                Padding = new Thickness(24, 16),
                Children =
                {
                    MakeField(L.Common_Name, _nameEntry),
                    MakeField(L.MedForm_Dosage, _dosageEntry),
                    MakeField(L.MedForm_MinStock, _minStockEntry),
                    MakeField(L.MedForm_CurrentStock, _currentStockEntry),

                    new Label { Text = L.MedForm_IntakeTimes, FontSize = 14, FontAttributes = FontAttributes.Bold },
                    _intakeTimesLayout,
                    addTimeButton,

                    new BoxView { HeightRequest = 8 },
                    saveButton,
                    cancelButton,
                    _statusLabel,
                }
            }
        };
    }

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
            _intakeTimesLayout.Children.Remove(row);
            _intakeRows.Remove((picker, doseEntry));
        };

        _intakeRows.Add((picker, doseEntry));
        _intakeTimesLayout.Children.Add(row);
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_nameEntry.Text))
        {
            _statusLabel.TextColor = Colors.Red;
            _statusLabel.Text = L.MedForm_NameRequired;
            return;
        }

        if (!int.TryParse(_minStockEntry.Text, out var minStock))
            minStock = 0;
        if (!int.TryParse(_currentStockEntry.Text, out var currentStock))
            currentStock = 0;

        var intakeTimes = _intakeRows
            .Select(r => new IntakeTimeDto(
                $"{(r.Picker.Time ?? TimeSpan.Zero).Hours:D2}:{(r.Picker.Time ?? TimeSpan.Zero).Minutes:D2}",
                int.TryParse(r.DoseEntry.Text, out var d) ? Math.Max(1, d) : 1))
            .ToList()
            .AsReadOnly();

        if (intakeTimes.Count == 0)
        {
            _statusLabel.TextColor = Colors.Red;
            _statusLabel.Text = L.MedForm_IntakeRequired;
            return;
        }

        var dto = new SaveMedicationDto(
            _editId,
            _nameEntry.Text.Trim(),
            _dosageEntry.Text?.Trim() ?? string.Empty,
            minStock,
            currentStock,
            intakeTimes);

        try
        {
            var services = IPlatformApplication.Current!.Services;
            await services.GetRequiredService<SaveMedicationUseCase>().ExecuteAsync(dto);

            // Alarme neu planen mit allen aktiven Medikamenten
            var allMeds = await services.GetRequiredService<GetActiveMedicationsUseCase>().ExecuteAsync();
            await services.GetRequiredService<IAlarmService>().ScheduleAsync(allMeds);

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            _statusLabel.TextColor = Colors.Red;
            _statusLabel.Text = L.Fmt("Common_ErrorFmt", ex.Message);
        }
    }

    private static View MakeField(string label, View control) =>
        new VerticalStackLayout
        {
            Spacing = 4,
            Children = { new Label { Text = label, FontSize = 13 }, control }
        };
}
