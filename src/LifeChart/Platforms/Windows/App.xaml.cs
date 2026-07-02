using System.Threading;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

namespace LifeChart.WinUI;

public partial class App : MauiWinUIApplication
{
    [STAThread]
    public static void Main(string[] args)
    {
        WinRT.ComWrappersSupport.InitializeComWrappers();
        Application.Start(_ =>
        {
            var context = new DispatcherQueueSynchronizationContext(
                DispatcherQueue.GetForCurrentThread());
            SynchronizationContext.SetSynchronizationContext(context);
            _ = new App();
        });
    }

    public App()
    {
        InitializeComponent();
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
