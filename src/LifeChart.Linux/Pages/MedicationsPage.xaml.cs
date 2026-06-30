using LifeChart.Application.DTOs;
using LifeChart.Application.Localization;
using LifeChart.Application.UseCases.Medications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace LifeChart.Linux.Pages;

public partial class MedicationsPage : ContentPage
{
    public MedicationsPage()
    {
        InitializeComponent();
        Title = L.Medications_Title;
        ActiveListLabel.Text = L.Medications_ActiveList;
        AddButton.Text = L.Medications_AddNew;
        EmptyLabel.Text = L.Medications_Empty;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadMedicationsAsync();
    }

    private async void OnAddClicked(object? sender, EventArgs e)
        => await Navigation.PushAsync(new MedicationFormPage());

    private async Task LoadMedicationsAsync()
    {
        ListLayout.Children.Clear();

        var medications = (await IPlatformApplication.Current!.Services
            .GetRequiredService<GetActiveMedicationsUseCase>()
            .ExecuteAsync()).ToList();

        EmptyLabel.IsVisible = medications.Count == 0;

        foreach (var med in medications)
            ListLayout.Children.Add(BuildMedicationCard(med));
    }

    private View BuildMedicationCard(MedicationDto med)
    {
        var intakeSummary = string.Join(", ",
            med.IntakeTimes.Select(i => $"{i.Time} ({i.DoseCount}x)"));

        var stockColor = med.IsStockLow ? Colors.OrangeRed : Colors.Green;

        var editButton = new Button
        {
            Text = L.Common_Edit,
            HeightRequest = 36,
            Padding = new Thickness(8, 0),
        };
        editButton.Clicked += async (_, _) =>
            await Navigation.PushAsync(new MedicationFormPage(med));

        var deleteButton = new Button
        {
            Text = L.Common_Delete,
            HeightRequest = 36,
            Padding = new Thickness(8, 0),
            BackgroundColor = Colors.IndianRed,
            TextColor = Colors.White,
        };
        deleteButton.Clicked += async (_, _) => await OnDeleteClicked(med);

        return new Frame
        {
            Padding = new Thickness(12),
            CornerRadius = 8,
            Content = new VerticalStackLayout
            {
                Spacing = 4,
                Children =
                {
                    new Label { Text = med.Name, FontSize = 16, FontAttributes = FontAttributes.Bold },
                    new Label { Text = med.Dosage, FontSize = 13, TextColor = Colors.Gray },
                    new Label { Text = L.Fmt("Medications_IntakeFmt", intakeSummary), FontSize = 13 },
                    new Label
                    {
                        Text = L.Fmt("Medications_StockFmt", med.CurrentStock, med.MinStock),
                        FontSize = 13,
                        TextColor = stockColor,
                    },
                    new HorizontalStackLayout
                    {
                        Spacing = 8,
                        Margin = new Thickness(0, 4, 0, 0),
                        Children = { editButton, deleteButton },
                    }
                }
            }
        };
    }

    private async Task OnDeleteClicked(MedicationDto med)
    {
        var confirmed = await DisplayAlert(
            L.Medications_DeleteTitle,
            L.Fmt("Medications_DeleteConfirmFmt", med.Name),
            L.Common_Delete, L.Common_Cancel);

        if (!confirmed) return;

        await IPlatformApplication.Current!.Services
            .GetRequiredService<DeactivateMedicationUseCase>()
            .ExecuteAsync(med.Id);

        await LoadMedicationsAsync();
    }
}
