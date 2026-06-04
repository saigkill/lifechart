using System.Diagnostics;
using LifeChart.Application.DTOs;
using LifeChart.Application.Interfaces;
using LifeChart.Application.Localization;
using LifeChart.Application.Settings;

namespace LifeChart.Linux.Services;

public class LinuxAlarmService : IAlarmService
{
    private readonly ISettingsService _settingsService;

    private static readonly string SystemdUserDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".config", "systemd", "user");

    private const string UnitPrefix = "lifechart";

    public LinuxAlarmService(ISettingsService settingsService)
        => _settingsService = settingsService;

    public bool HasRequiredPermissions => true;

    public Task<bool> RequestPermissionsAsync() => Task.FromResult(true);

    public async Task ScheduleAsync(IEnumerable<MedicationDto> medications)
    {
        await CancelAllAsync();
        Directory.CreateDirectory(SystemdUserDir);

        foreach (var med in medications)
            foreach (var intake in med.IntakeTimes)
                await CreateMedicationTimerAsync(med, intake);

        var settings = _settingsService.Load();
        if (settings.EveningReminderEnabled && settings.EveningReminderTime.HasValue)
            await CreateEveningReminderTimerAsync(settings.EveningReminderTime.Value);

        await RunSystemctlAsync("daemon-reload");
    }

    public async Task CancelAllAsync()
    {
        if (!Directory.Exists(SystemdUserDir)) return;

        foreach (var timerFile in Directory.GetFiles(SystemdUserDir, $"{UnitPrefix}-*.timer"))
        {
            var unitName = Path.GetFileName(timerFile);
            await RunSystemctlAsync("disable", "--now", unitName);
            File.Delete(timerFile);
            var serviceFile = Path.ChangeExtension(timerFile, ".service");
            if (File.Exists(serviceFile)) File.Delete(serviceFile);
        }

        await RunSystemctlAsync("daemon-reload");
    }

    private async Task CreateMedicationTimerAsync(MedicationDto med, IntakeTimeDto intake)
    {
        var safeName = SafeUnitName(med.Name);
        var safeTime = intake.Time.Replace(":", "");
        var unitName = $"{UnitPrefix}-med-{safeName}-{safeTime}";

        var serviceContent = $"""
            [Unit]
            Description=LifeChart Medikamenten-Erinnerung: {med.Name}

            [Service]
            Type=oneshot
            ExecStart=/usr/bin/notify-send -i dialog-information -u normal "LifeChart" "{L.Fmt("Alarm_MedicationFmt", med.Name, med.Dosage, intake.DoseCount)}"
            """;

        var timerContent = $"""
            [Unit]
            Description=LifeChart Timer: {med.Name} um {intake.Time}

            [Timer]
            OnCalendar=*-*-* {intake.Time}:00
            Persistent=true

            [Install]
            WantedBy=timers.target
            """;

        await WriteUnitAsync(unitName, serviceContent, timerContent);
    }

    private async Task CreateEveningReminderTimerAsync(TimeOnly time)
    {
        const string unitName = $"{UnitPrefix}-evening";

        var serviceContent = $"""
            [Unit]
            Description=LifeChart Abend-Erinnerung

            [Service]
            Type=oneshot
            ExecStart=/usr/bin/notify-send -i dialog-information -u normal "LifeChart" "{L.Alarm_Evening}"
            """;

        var timerContent = $"""
            [Unit]
            Description=LifeChart Abend-Erinnerung um {time:HH:mm}

            [Timer]
            OnCalendar=*-*-* {time:HH\:mm}:00
            Persistent=true

            [Install]
            WantedBy=timers.target
            """;

        await WriteUnitAsync(unitName, serviceContent, timerContent);
    }

    private static async Task WriteUnitAsync(string unitName, string serviceContent, string timerContent)
    {
        await File.WriteAllTextAsync(
            Path.Combine(SystemdUserDir, $"{unitName}.service"), serviceContent);
        await File.WriteAllTextAsync(
            Path.Combine(SystemdUserDir, $"{unitName}.timer"), timerContent);
        await RunSystemctlAsync("enable", "--now", $"{unitName}.timer");
    }

    private static async Task RunSystemctlAsync(params string[] args)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "systemctl",
            Arguments = $"--user {string.Join(' ', args)}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        using var process = Process.Start(psi);
        if (process is not null) await process.WaitForExitAsync();
    }

    private static string SafeUnitName(string name) =>
        new string(name.ToLower().Select(c => char.IsLetterOrDigit(c) ? c : '-').ToArray())
            .Trim('-');
}
