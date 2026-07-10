using LifeChart.Application.DTOs;
using LifeChart.Application.Interfaces;
using LifeChart.Application.Localization;
using LifeChart.Application.Settings;
using Tmds.DBus.Protocol;

namespace LifeChart.Linux.Services;

public class LinuxAlarmService : IAlarmService
{
    private readonly ISettingsService _settingsService;

    // Unit files are written into the app's own XDG_DATA_HOME.
    // systemd (running on the host) can read these paths via LinkUnitFiles.
    private static readonly string UnitFileDir = Path.Combine(
        Environment.GetEnvironmentVariable("XDG_DATA_HOME")
            ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share"),
        "systemd-units");

    private const string SystemdDest = "org.freedesktop.systemd1";
    private const string SystemdPath = "/org/freedesktop/systemd1";
    private const string SystemdIface = "org.freedesktop.systemd1.Manager";
    private const string UnitPrefix = "lifechart";

    public LinuxAlarmService(ISettingsService settingsService)
        => _settingsService = settingsService;

    public bool HasRequiredPermissions => true;

    public Task<bool> RequestPermissionsAsync() => Task.FromResult(true);

    public async Task ScheduleAsync(IEnumerable<MedicationDto> medications)
    {
        await CancelAllAsync();
        Directory.CreateDirectory(UnitFileDir);

        var timerPaths = new List<string>();

        foreach (var med in medications)
            foreach (var intake in med.IntakeTimes)
                timerPaths.Add(await CreateMedicationTimerAsync(med, intake));

        var settings = _settingsService.Load();
        if (settings.EveningReminderEnabled && settings.EveningReminderTime.HasValue)
            timerPaths.Add(await CreateEveningReminderTimerAsync(settings.EveningReminderTime.Value));

        foreach (var timerPath in timerPaths)
        {
            await LinkUnitFilesAsync([timerPath]);
            await StartUnitAsync(Path.GetFileName(timerPath));
        }

        await ReloadAsync();
    }

    public async Task CancelAllAsync()
    {
        if (!Directory.Exists(UnitFileDir)) return;

        foreach (var timerFile in Directory.GetFiles(UnitFileDir, $"{UnitPrefix}-*.timer"))
        {
            var unitName = Path.GetFileName(timerFile);
            await StopUnitAsync(unitName);
            await DisableUnitFilesAsync([unitName]);
            File.Delete(timerFile);
            var serviceFile = Path.ChangeExtension(timerFile, ".service");
            if (File.Exists(serviceFile)) File.Delete(serviceFile);
        }

        await ReloadAsync();
    }

    private static async Task<string> CreateMedicationTimerAsync(MedicationDto med, IntakeTimeDto intake)
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

        var timerPath = Path.Combine(UnitFileDir, $"{unitName}.timer");
        await File.WriteAllTextAsync(Path.Combine(UnitFileDir, $"{unitName}.service"), serviceContent);
        await File.WriteAllTextAsync(timerPath, timerContent);
        return timerPath;
    }

    private static async Task<string> CreateEveningReminderTimerAsync(TimeOnly time)
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

        var timerPath = Path.Combine(UnitFileDir, $"{unitName}.timer");
        await File.WriteAllTextAsync(Path.Combine(UnitFileDir, $"{unitName}.service"), serviceContent);
        await File.WriteAllTextAsync(timerPath, timerContent);
        return timerPath;
    }

    // --- systemd D-Bus helpers (session bus, --talk-name=org.freedesktop.systemd1) ---

    private static Task ReloadAsync()
    {
        var conn = DBusConnection.Session;
        using var writer = conn.GetMessageWriter();
        writer.WriteMethodCallHeader(SystemdDest, SystemdPath, SystemdIface, "Reload", null, MessageFlags.None);
        return conn.CallMethodAsync(writer.CreateMessage());
    }

    private static Task LinkUnitFilesAsync(string[] paths)
    {
        var conn = DBusConnection.Session;
        using var writer = conn.GetMessageWriter();
        // signature: asbb  (string-array, bool runtime, bool force)
        writer.WriteMethodCallHeader(SystemdDest, SystemdPath, SystemdIface, "LinkUnitFiles", "asbb", MessageFlags.None);
        writer.WriteArray(paths);
        writer.WriteBool(false); // runtime
        writer.WriteBool(false); // force
        return conn.CallMethodAsync(writer.CreateMessage());
    }

    private static Task StartUnitAsync(string unitName)
    {
        var conn = DBusConnection.Session;
        using var writer = conn.GetMessageWriter();
        writer.WriteMethodCallHeader(SystemdDest, SystemdPath, SystemdIface, "StartUnit", "ss", MessageFlags.None);
        writer.WriteString(unitName);
        writer.WriteString("replace");
        return conn.CallMethodAsync(writer.CreateMessage());
    }

    private static Task StopUnitAsync(string unitName)
    {
        var conn = DBusConnection.Session;
        using var writer = conn.GetMessageWriter();
        writer.WriteMethodCallHeader(SystemdDest, SystemdPath, SystemdIface, "StopUnit", "ss", MessageFlags.None);
        writer.WriteString(unitName);
        writer.WriteString("replace");
        return conn.CallMethodAsync(writer.CreateMessage());
    }

    private static Task DisableUnitFilesAsync(string[] unitNames)
    {
        var conn = DBusConnection.Session;
        using var writer = conn.GetMessageWriter();
        // signature: asb  (string-array, bool runtime)
        writer.WriteMethodCallHeader(SystemdDest, SystemdPath, SystemdIface, "DisableUnitFiles", "asb", MessageFlags.None);
        writer.WriteArray(unitNames);
        writer.WriteBool(false); // runtime
        return conn.CallMethodAsync(writer.CreateMessage());
    }

    private static string SafeUnitName(string name) =>
        new string(name.ToLower().Select(c => char.IsLetterOrDigit(c) ? c : '-').ToArray())
            .Trim('-');
}
