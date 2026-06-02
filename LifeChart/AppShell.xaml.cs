namespace LifeChart;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("SettingsPage", typeof(Pages.SettingsPage));
        Routing.RegisterRoute("CrisisPage", typeof(Pages.CrisisPage));
    }
}
