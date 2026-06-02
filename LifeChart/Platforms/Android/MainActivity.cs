using Android.App;
using Android.Content.PM;
using Plugin.Fingerprint;

namespace LifeChart;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    ConfigurationChanges =
        ConfigChanges.ScreenSize |
        ConfigChanges.Orientation |
        ConfigChanges.UiMode |
        ConfigChanges.ScreenLayout |
        ConfigChanges.SmallestScreenSize |
        ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Android.OS.Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Plugin.Fingerprint benötigt die aktuelle Activity
        CrossFingerprint.SetCurrentActivityResolver(() => this);

        // Notification Channel einmalig beim Start anlegen
        var notificationManager =
            (Android.App.NotificationManager?)
            GetSystemService(NotificationService);

        if (notificationManager is not null)
            Platforms.Android.Services.NotificationChannelSetup
                .CreateChannel(notificationManager);
    }
}
