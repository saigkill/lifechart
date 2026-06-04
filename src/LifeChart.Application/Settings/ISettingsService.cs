namespace LifeChart.Application.Settings;

public interface ISettingsService
{
    AppSettings Load();
    Task SaveAsync(AppSettings settings);
}
