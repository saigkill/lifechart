using Android.App;
using Android.Content;

namespace LifeChart.Platforms.Android.Receivers;

[BroadcastReceiver(Enabled = true, Exported = true)]
[IntentFilter([
    "android.intent.action.BOOT_COMPLETED",
    "android.intent.action.LOCKED_BOOT_COMPLETED"
])]
public class BootReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context is null) return;

        // Alarme nach Neustart neu planen — App über Intent starten
        var launchIntent = context.PackageManager?
            .GetLaunchIntentForPackage(context.PackageName ?? string.Empty);

        if (launchIntent is null) return;

        launchIntent.SetFlags(ActivityFlags.NewTask);
        launchIntent.PutExtra("reschedule_alarms", true);
        context.StartActivity(launchIntent);
    }
}
