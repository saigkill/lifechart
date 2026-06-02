using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using LifeChart.Platforms.Android.Services;

namespace LifeChart.Platforms.Android.Receivers;

[BroadcastReceiver(Enabled = true, Exported = false)]
[IntentFilter(["lifechart.MEDICATION_ALARM"])]
public class MedicationAlarmReceiver : BroadcastReceiver
{
    public const string ExtraMedicationName = "medication_name";
    public const string ExtraDoseCount = "dose_count";
    public const string ExtraAlarmId = "alarm_id";

    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context is null || intent is null) return;

        var medicationName = intent.GetStringExtra(ExtraMedicationName) ?? "Medikament";
        var doseCount = intent.GetIntExtra(ExtraDoseCount, 1);
        var alarmId = intent.GetIntExtra(ExtraAlarmId, 0);

        ShowNotification(context, medicationName, doseCount, alarmId);
    }

    private static void ShowNotification(
        Context context, string medicationName, int doseCount, int notificationId)
    {
        var notificationManager =
            (NotificationManager?)context.GetSystemService(Context.NotificationService);
        if (notificationManager is null) return;

        NotificationChannelSetup.CreateChannel(notificationManager);

        var title = "Medikament einnehmen";
        var body = doseCount > 1
            ? $"{medicationName} — {doseCount}x einnehmen"
            : medicationName;

        var notification = new NotificationCompat.Builder(context, NotificationChannelSetup.ChannelId)
            .SetSmallIcon(global::Android.Resource.Drawable.IcDialogInfo)
            .SetContentTitle(title)
            .SetContentText(body)
            .SetCategory(NotificationCompat.CategoryAlarm)
            .SetPriority(NotificationCompat.PriorityMax)
            .SetVisibility(NotificationCompat.VisibilityPublic)
            .SetAutoCancel(true)
            .Build();

        notificationManager.Notify(notificationId, notification);
    }
}
