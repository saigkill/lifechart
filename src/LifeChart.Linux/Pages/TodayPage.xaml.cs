using LifeChart.Application.DTOs;
using LifeChart.Application.Interfaces;
using LifeChart.Application.Localization;
using LifeChart.Application.Settings;
using LifeChart.Application.UseCases.Entries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace LifeChart.Linux.Pages;

public partial class TodayPage : ContentPage
{
    private bool _isExtended;

    public TodayPage()
    {
        InitializeComponent();
        Title = L.Today_Title;
        DateLabel.Text = L.Fmt("Today_EntryFor", DateTime.Today.ToString("dddd, dd. MMMM yyyy"));
        MoodLabel.Text = L.Today_Mood;
        FuncLabel.Text = L.Today_Functionality;
        SleepLabel.Text = L.Today_Sleep;
        MedicationLabel.Text = L.Today_MedicationTaken;
        MenstrualLabel.Text = L.Today_MenstrualCycle;
        HypomaniaLabel.Text = L.Today_Hypomania;
        SymptomsLabel.Text = L.Today_Symptoms;
        SymptomsEditor.Placeholder = L.Today_SymptomsPlaceholder;
        NotesLabel.Text = L.Today_Notes;
        NotesEditor.Placeholder = L.Today_NotesPlaceholder;
        MoreButton.Text = L.Today_More;
        SaveButton.Text = L.Common_Save;
        CrisisButton.Text = L.Today_ShowCrisisResources;

        UpdateValueLabels();

        // GTK4 backend does not collapse invisible layouts correctly;
        // remove the extended section from the tree and re-insert on demand.
        MainLayout.Children.Remove(ExtendedSection);
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
                ShowExtendedSection();
                break;

            case InputMode.AlwaysAsk:
                var choice = await DisplayActionSheet(
                    L.Today_InputModeTitle, L.Common_Cancel, null,
                    L.Today_InputModeQuick,
                    L.Today_InputModeFull);
                if (choice == L.Today_InputModeFull)
                    ShowExtendedSection();
                break;
        }
    }

    private void OnMoreClicked(object? sender, EventArgs e) => ShowExtendedSection();

    private void ShowExtendedSection()
    {
        if (_isExtended) return;
        _isExtended = true;
        var idx = MainLayout.Children.IndexOf(MoreButton);
        MainLayout.Children.Remove(MoreButton);
        MainLayout.Children.Insert(idx, ExtendedSection);
    }

    private async Task LoadTodayAsync()
    {
        var entry = await GetService<GetTodayEntryUseCase>().ExecuteAsync();
        if (entry is null) return;

        MoodSlider.Value = entry.Mood;
        FuncSlider.Value = entry.Functionality;
        SleepSlider.Value = entry.SleepHours;
        MedicationSwitch.IsToggled = entry.MedicationTaken;
        MenstrualSwitch.IsToggled = entry.MenstrualCycle;
        HypomaniaSwitch.IsToggled = entry.IsHypomanic;
        SymptomsEditor.Text = entry.Symptoms ?? string.Empty;
        NotesEditor.Text = entry.Notes ?? string.Empty;
    }

    private void OnSliderValueChanged(object? sender, ValueChangedEventArgs e)
        => UpdateValueLabels();

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        SaveButton.IsEnabled = false;
        StatusLabel.Text = string.Empty;

        try
        {
            int sleepHours = (int)Math.Round(SleepSlider.Value);
            bool medicationTaken = MedicationSwitch.IsToggled;
            bool menstrualCycle = MenstrualSwitch.IsToggled;
            string? symptoms = null;
            string? notes = null;

            if (!_isExtended)
            {
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
                symptoms = string.IsNullOrWhiteSpace(SymptomsEditor.Text) ? null : SymptomsEditor.Text;
                notes = string.IsNullOrWhiteSpace(NotesEditor.Text) ? null : NotesEditor.Text;
            }

            var dto = new DailyEntryDto(
                DateOnly.FromDateTime(DateTime.Today),
                (int)Math.Round(MoodSlider.Value),
                (int)Math.Round(FuncSlider.Value),
                sleepHours, medicationTaken, menstrualCycle, symptoms, notes,
                HypomaniaSwitch.IsToggled);

            var result = await GetService<SaveDailyEntryUseCase>().ExecuteAsync(dto);

            if (result.IsCritical)
            {
                StatusLabel.TextColor = Colors.OrangeRed;
                StatusLabel.Text = L.Today_CriticalValues;
                CrisisButton.IsVisible = true;
            }
            else
            {
                StatusLabel.TextColor = Colors.Green;
                StatusLabel.Text = L.Today_Saved;
                CrisisButton.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            StatusLabel.TextColor = Colors.Red;
            StatusLabel.Text = L.Fmt("Common_ErrorFmt", ex.Message);
        }
        finally
        {
            SaveButton.IsEnabled = true;
        }
    }

    private void OnCrisisButtonClicked(object? sender, EventArgs e)
    {
        // Walk up the visual tree to find the hosting TabbedPage (robust across hosts/wrappers).
        Element? element = this;
        while (element is not null and not TabbedPage)
            element = element.Parent;

        if (element is TabbedPage tabbedPage)
        {
            var crisisTab = FindCrisisPage(tabbedPage);
            if (crisisTab is not null)
            {
                // If wrapped in a NavigationPage, switch to that wrapper instead.
                tabbedPage.CurrentPage = crisisTab.Parent as Page ?? crisisTab;
                return;
            }
        }

        // Fallback: push as modal so a critical user action never silently fails.
        Navigation.PushModalAsync(new CrisisPage());
    }

    private static CrisisPage? FindCrisisPage(TabbedPage tabbedPage)
        => tabbedPage.Children.OfType<CrisisPage>().FirstOrDefault()
           ?? tabbedPage.Children.OfType<NavigationPage>()
                        .Select(np => np.RootPage as CrisisPage)
                        .FirstOrDefault(p => p is not null);  

    private void UpdateValueLabels()
    {
        MoodValueLabel.Text = $"{(int)Math.Round(MoodSlider.Value):+#;-#;0}";
        FuncValueLabel.Text = $"{(int)Math.Round(FuncSlider.Value):+#;-#;0}";
        SleepValueLabel.Text = $"{(int)Math.Round(SleepSlider.Value)} h";
    }

    private static T GetService<T>() where T : notnull =>
        IPlatformApplication.Current!.Services.GetRequiredService<T>();
}