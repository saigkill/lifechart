using Android.App;
using Android.Content;
using Android.OS;
using LifeChart.Application.DTOs;
using LifeChart.Application.Interfaces;
using LifeChart.Platforms.Android.Receivers;

namespace LifeChart.Platforms.Android.Services;

public class AndroidAlarmService : IAlarmService
{
    private const string ActionMedicationAlarm = "lifechart.MEDICATION_ALARM";
    private const int BaseAlarmId = 10000;

    public bool HasRequiredPermissions
    {
        get
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.S) return true;
            var alarmManager = GetAlarmManager();
            return alarmManager?.CanScheduleExactAlarms() ?? false;
        }
    }

    public async Task<bool> RequestPermissionsAsync()
    {
        // POST_NOTIFICATIONS — Laufzeit-Permission anfordern
        var status = await Permissions.RequestAsync<Permissions.PostNotifications>();
        if (status != PermissionStatus.Granted) return false;

        // SCHEDULE_EXACT_ALARM — führt zur Systemeinstellung
        if (Build.VERSION.SdkInt >= BuildVersionCodes.S && !HasRequiredPermissions)
        {
            var intent = new Intent(
                global::Android.Provider.Settings.ActionRequestScheduleExactAlarm);
            intent.SetFlags(ActivityFlags.NewTask);
            global::Android.App.Application.Context.StartActivity(intent);
            return false;
        }

        return true;
    }

    public Task ScheduleAsync(IEnumerable<MedicationDto> medications)
    {
        var alarmManager = GetAlarmManager();
        if (alarmManager is null) return Task.CompletedTask;

        var context = global::Android.App.Application.Context;

        foreach (var medication in medications)
        {
            for (int i = 0; i < medication.IntakeTimes.Count; i++)
            {
                var intakeTime = medication.IntakeTimes[i];
                var alarmId = ComputeAlarmId(medication.Id, i);

                var triggerTime = ComputeNextTriggerMs(intakeTime.Time);
                var pendingIntent = CreatePendingIntent(
                    context, alarmId, medication.Name, intakeTime.DoseCount);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                    alarmManager.SetExactAndAllowWhileIdle(
                        AlarmType.RtcWakeup, triggerTime, pendingIntent);
                else
                    alarmManager.SetExact(
                        AlarmType.RtcWakeup, triggerTime, pendingIntent);
            }
        }

        return Task.CompletedTask;
    }

    public Task CancelAllAsync()
    {
        // Alle bekannten Alarm-IDs stornieren
        // In einer vollständigen Implementierung würden die IDs persistent gespeichert.
        // Für den Moment: App-Neustart lädt Alarme neu.
        return Task.CompletedTask;
    }

    private static AlarmManager? GetAlarmManager()
        => global::Android.App.Application.Context
            .GetSystemService(Context.AlarmService) as AlarmManager;

    private static long ComputeNextTriggerMs(string timeStr)
    {
        if (!TimeOnly.TryParse(timeStr, out var time))
            return DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeMilliseconds();

        var now = DateTime.Now;
        var trigger = new DateTime(now.Year, now.Month, now.Day, time.Hour, time.Minute, 0);
        if (trigger <= now)
            trigger = trigger.AddDays(1);

        return new DateTimeOffset(trigger).ToUnixTimeMilliseconds();
    }

    private static PendingIntent CreatePendingIntent(
        Context context, int alarmId, string medicationName, int doseCount)
    {
        var intent = new Intent(context, typeof(MedicationAlarmReceiver));
        intent.SetAction(ActionMedicationAlarm);
        intent.PutExtra(MedicationAlarmReceiver.ExtraMedicationName, medicationName);
        intent.PutExtra(MedicationAlarmReceiver.ExtraDoseCount, doseCount);
        intent.PutExtra(MedicationAlarmReceiver.ExtraAlarmId, alarmId);

        var flags = Build.VERSION.SdkInt >= BuildVersionCodes.S
            ? PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
            : PendingIntentFlags.UpdateCurrent;

        return PendingIntent.GetBroadcast(context, alarmId, intent, flags)!;
    }

    private static int ComputeAlarmId(int medicationId, int timeIndex)
        => BaseAlarmId + medicationId * 100 + timeIndex;
}
