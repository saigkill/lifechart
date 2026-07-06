namespace LifeChart;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("SettingsPage", typeof(Pages.SettingsPage));
        Routing.RegisterRoute("MedicationFormPage", typeof(Pages.MedicationFormPage));
    }

    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("SettingsPage");
    }
}
