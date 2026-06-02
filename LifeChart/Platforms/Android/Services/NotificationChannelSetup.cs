using Android.App;
using Android.Media;
using Android.OS;

namespace LifeChart.Platforms.Android.Services;

public static class NotificationChannelSetup
{
    public const string ChannelId = "medication_alarms";

    public static void CreateChannel(NotificationManager manager)
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.O) return;

        var channel = new NotificationChannel(
            ChannelId,
            "Medikamentenalarme",
            NotificationImportance.High)
        {
            Description = "Erinnerungen zur Medikamenteneinnahme"
        };

        // Lautlos aber DND-Bypass und Vibration
        var audioAttributes = new AudioAttributes.Builder()!
            .SetUsage(AudioUsageKind.Alarm)!
            .SetContentType(AudioContentType.Sonification)!
            .Build()!;

        channel.SetSound(null, audioAttributes);
        channel.EnableVibration(true);
        channel.SetBypassDnd(true);
        channel.LockscreenVisibility = NotificationVisibility.Public;

        manager.CreateNotificationChannel(channel);
    }
}
