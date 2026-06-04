using LifeChart.Application.DTOs;
using LifeChart.Application.Localization;
using LifeChart.Application.Settings;
using LifeChart.Application.UseCases.Entries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace LifeChart.Linux.Pages;

public class TodayPage : ContentPage
{
    // Quick fields (always visible)
    private readonly Label _moodValueLabel = new();
    private readonly Label _funcValueLabel = new();
    private readonly Slider _moodSlider = new() { Minimum = -5, Maximum = 5, Value = 0 };
    private readonly Slider _funcSlider = new() { Minimum = -5, Maximum = 5, Value = 0 };

    // Extended fields (inserted dynamically)
    private readonly Label _sleepValueLabel = new();
    private readonly Slider _sleepSlider = new() { Minimum = 0, Maximum = 24, Value = 7 };
    private readonly Switch _medicationSwitch = new();
    private readonly Switch _menstrualSwitch = new();
    private readonly Switch _hypomaniaSwitch = new();
    private readonly Editor _symptomsEditor = new() { HeightRequest = 60 };
    private readonly Editor _notesEditor = new() { HeightRequest = 60 };

    private readonly Button _moreButton = new()
    {
        HorizontalOptions = LayoutOptions.Start,
        BackgroundColor = Colors.Transparent,
        TextColor = Colors.CornflowerBlue,
        Padding = new Thickness(0),
    };
    private readonly Button _saveButton = new() { HorizontalOptions = LayoutOptions.Center };
    private readonly Label _statusLabel = new() { HorizontalOptions = LayoutOptions.Center };
    private readonly Button _crisisButton = new()
    {
        HorizontalOptions = LayoutOptions.Center,
        BackgroundColor = Colors.OrangeRed,
        TextColor = Colors.White,
    };

    // Reference to the main layout for dynamic child manipulation
    private VerticalStackLayout _mainLayout = null!;

    // Extended views built once, inserted/removed as a block
    private List<View> _extendedViews = null!;
    private bool _isExtended;

    public TodayPage()
    {
        Title = L.Today_Title;
        UpdateValueLabels();

        _symptomsEditor.Placeholder = L.Today_SymptomsPlaceholder;
        _notesEditor.Placeholder = L.Today_NotesPlaceholder;
        _moreButton.Text = L.Today_More;
        _saveButton.Text = L.Common_Save;
        _crisisButton.Text = L.Today_ShowCrisisResources;

        _moodSlider.ValueChanged += (_, _) => UpdateValueLabels();
        _funcSlider.ValueChanged += (_, _) => UpdateValueLabels();
        _sleepSlider.ValueChanged += (_, _) => UpdateValueLabels();
        _saveButton.Clicked += OnSaveClicked;
        _crisisButton.Clicked += (_, _) => NavigateToCrisisTab();
        _moreButton.Clicked += (_, _) => InsertExtendedFields();

        _extendedViews =
        [
            MakeSliderRow(L.Today_Sleep, _sleepSlider, _sleepValueLabel),
            MakeSwitchRow(L.Today_MedicationTaken, _medicationSwitch),
            MakeSwitchRow(L.Today_MenstrualCycle, _menstrualSwitch),
            MakeSwitchRow(L.Today_Hypomania, _hypomaniaSwitch),
            new Label { Text = L.Today_Symptoms, FontSize = 13 },
            _symptomsEditor,
            new Label { Text = L.Today_Notes, FontSize = 13 },
            _notesEditor,
        ];

        _mainLayout = new VerticalStackLayout
        {
            Spacing = 12,
            Padding = new Thickness(24, 16),
            Children =
            {
                new Label
                {
                    Text = L.Fmt("Today_EntryFor", DateTime.Today.ToString("dddd, dd. MMMM yyyy")),
                    FontSize = 16,
                    FontAttributes = FontAttributes.Bold,
                },
                MakeSliderRow(L.Today_Mood, _moodSlider, _moodValueLabel),
                MakeSliderRow(L.Today_Functionality, _funcSlider, _funcValueLabel),
                _moreButton,
                _saveButton,
                _statusLabel,
            }
        };

        Content = new ScrollView { Content = _mainLayout };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ApplyInputModeAsync();
        await LoadTodayAsync();
    }

    private async Task ApplyInputModeAsync()
    {
        var settings = GetService<ISettingsService>().Load();

        switch (settings.InputMode)
        {
            case InputMode.Full:
                InsertExtendedFields();
                break;

            case InputMode.AlwaysAsk:
                var choice = await DisplayActionSheet(
                    L.Today_InputModeTitle, L.Common_Cancel, null,
                    L.Today_InputModeQuick,
                    L.Today_InputModeFull);
                if (choice == L.Today_InputModeFull)
                    InsertExtendedFields();
                break;
        }
    }

    // Replaces _moreButton with the extended views in-place
    private void InsertExtendedFields()
    {
        if (_isExtended) return;
        _isExtended = true;

        var idx = _mainLayout.Children.IndexOf(_moreButton);
        _mainLayout.Children.RemoveAt(idx);

        foreach (var (view, i) in _extendedViews.Select((v, i) => (v, i)))
            _mainLayout.Children.Insert(idx + i, view);
    }

    private async Task LoadTodayAsync()
    {
        var entry = await GetService<GetTodayEntryUseCase>().ExecuteAsync();
        if (entry is null) return;

        _moodSlider.Value = entry.Mood;
        _funcSlider.Value = entry.Functionality;
        _sleepSlider.Value = entry.SleepHours;
        _medicationSwitch.IsToggled = entry.MedicationTaken;
        _menstrualSwitch.IsToggled = entry.MenstrualCycle;
        _hypomaniaSwitch.IsToggled = entry.IsHypomanic;
        _symptomsEditor.Text = entry.Symptoms ?? string.Empty;
        _notesEditor.Text = entry.Notes ?? string.Empty;
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        _saveButton.IsEnabled = false;
        _statusLabel.Text = string.Empty;

        try
        {
            int sleepHours = (int)Math.Round(_sleepSlider.Value);
            bool medicationTaken = _medicationSwitch.IsToggled;
            bool menstrualCycle = _menstrualSwitch.IsToggled;
            string? symptoms = null;
            string? notes = null;

            if (!_isExtended)
            {
                // Quick mode: carry over existing extended data
                var existing = await GetService<GetTodayEntryUseCase>().ExecuteAsync();
                if (existing is not null)
                {
                    sleepHours = existing.SleepHours;
                    medicationTaken = existing.MedicationTaken;
                    menstrualCycle = existing.MenstrualCycle;
                    symptoms = existing.Symptoms;
                    notes = existing.Notes;
                }
            }
            else
            {
                symptoms = string.IsNullOrWhiteSpace(_symptomsEditor.Text) ? null : _symptomsEditor.Text;
                notes = string.IsNullOrWhiteSpace(_notesEditor.Text) ? null : _notesEditor.Text;
            }

            var dto = new DailyEntryDto(
                DateOnly.FromDateTime(DateTime.Today),
                (int)Math.Round(_moodSlider.Value),
                (int)Math.Round(_funcSlider.Value),
                sleepHours, medicationTaken, menstrualCycle, symptoms, notes,
                _hypomaniaSwitch.IsToggled);

            var result = await GetService<SaveDailyEntryUseCase>().ExecuteAsync(dto);

            if (result.IsCritical)
            {
                _statusLabel.TextColor = Colors.OrangeRed;
                _statusLabel.Text = L.Today_CriticalValues;
                if (!_mainLayout.Children.Contains(_crisisButton))
                    _mainLayout.Children.Add(_crisisButton);
            }
            else
            {
                _statusLabel.TextColor = Colors.Green;
                _statusLabel.Text = L.Today_Saved;
                _mainLayout.Children.Remove(_crisisButton);
            }
        }
        catch (Exception ex)
        {
            _statusLabel.TextColor = Colors.Red;
            _statusLabel.Text = L.Fmt("Common_ErrorFmt", ex.Message);
        }
        finally
        {
            _saveButton.IsEnabled = true;
        }
    }

    private void UpdateValueLabels()
    {
        _moodValueLabel.Text = $"{(int)Math.Round(_moodSlider.Value):+#;-#;0}";
        _funcValueLabel.Text = $"{(int)Math.Round(_funcSlider.Value):+#;-#;0}";
        _sleepValueLabel.Text = $"{(int)Math.Round(_sleepSlider.Value)} h";
    }

    private static View MakeSliderRow(string label, Slider slider, Label valueLabel)
    {
        valueLabel.FontSize = 14;
        valueLabel.FontAttributes = FontAttributes.Bold;
        valueLabel.WidthRequest = 48;
        valueLabel.HorizontalTextAlignment = TextAlignment.End;

        return new VerticalStackLayout
        {
            Spacing = 4,
            Children =
            {
                new HorizontalStackLayout
                {
                    Children =
                    {
                        new Label { Text = label, FontSize = 14, HorizontalOptions = LayoutOptions.FillAndExpand },
                        valueLabel,
                    }
                },
                slider,
            }
        };
    }

    private static View MakeSwitchRow(string label, Switch sw) =>
        new HorizontalStackLayout
        {
            Spacing = 12,
            Children =
            {
                new Label { Text = label, FontSize = 14, VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.FillAndExpand },
                sw,
            }
        };

    private static void NavigateToCrisisTab()
    {
        var tabbedPage = Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault()?.Page as TabbedPage;
        if (tabbedPage is null) return;

        var crisisTab = tabbedPage.Children.OfType<CrisisPage>().FirstOrDefault();
        if (crisisTab is not null)
            tabbedPage.CurrentPage = crisisTab;
    }

    private static T GetService<T>() where T : notnull =>
        IPlatformApplication.Current!.Services.GetRequiredService<T>();
}
