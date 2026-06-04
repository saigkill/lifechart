using System.Runtime.InteropServices;
using Microsoft.Maui.Platforms.Linux.Gtk4.Platform;
using Microsoft.Maui.Hosting;

namespace LifeChart;

public class Program : GtkMauiApplication
{
    protected override MauiApp CreateMauiApp() => LinuxMauiProgram.CreateMauiApp();

    public static void Main(string[] args)
    {
        SuppressGtkNoise();
        var app = new Program();
        app.Run(args);
    }

    // Unterdrückt bekannte Pango/GLib CRITICAL-Warnungen aus dem experimentellen
    // GTK4-MAUI-Host, die durch Widget-Initialisierungsreihenfolge entstehen.
    private static void SuppressGtkNoise()
    {
        try
        {
            const uint Critical = 1 << 3;
            const uint Warning  = 1 << 4;
            const uint Mask     = Critical | Warning;

            g_log_set_handler("Pango",       Mask, _silent, IntPtr.Zero);
            g_log_set_handler("GLib-GObject", Mask, _silent, IntPtr.Zero);
            g_log_set_handler("Gdk",          Mask, _silent, IntPtr.Zero);
            g_log_set_handler("Gtk",          Mask, _silent, IntPtr.Zero);
        }
        catch
        {
            // P/Invoke fehlgeschlagen — kein Problem, nur kosmetisch
        }
    }

    private static readonly GLogFunc _silent = (_, _, _, _) => { };

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void GLogFunc(
        string? domain, uint level, string message, IntPtr userData);

    [DllImport("libglib-2.0.so.0", CallingConvention = CallingConvention.Cdecl)]
    private static extern void g_log_set_handler(
        string? logDomain, uint logLevels, GLogFunc logFunc, IntPtr userData);
}
