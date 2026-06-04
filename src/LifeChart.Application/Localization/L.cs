using System.Globalization;
using System.Resources;

namespace LifeChart.Application.Localization;

/// <summary>
/// Static accessor for localized strings. Call SetCulture() once at app startup.
/// </summary>
public static class L
{
    private static readonly ResourceManager RM = new(
        "LifeChart.Application.Resources.AppStrings",
        typeof(L).Assembly);

    public static void SetCulture(CultureInfo culture)
    {
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
    }

    private static string Get(string key) =>
        RM.GetString(key, CultureInfo.CurrentUICulture) ?? $"[{key}]";

    public static string Fmt(string key, params object[] args) =>
        string.Format(Get(key), args);

    // Common
    public static string Common_Save => Get(nameof(Common_Save));
    public static string Common_Cancel => Get(nameof(Common_Cancel));
    public static string Common_Delete => Get(nameof(Common_Delete));
    public static string Common_Edit => Get(nameof(Common_Edit));
    public static string Common_Error => Get(nameof(Common_Error));
    public static string Common_OK => Get(nameof(Common_OK));
    public static string Common_Name => Get(nameof(Common_Name));
    public static string Common_Password => Get(nameof(Common_Password));
    public static string Common_Username => Get(nameof(Common_Username));
    public static string Common_Time => Get(nameof(Common_Time));

    // Today
    public static string Today_Title => Get(nameof(Today_Title));
    public static string Today_HowAreYou => Get(nameof(Today_HowAreYou));
    public static string Today_Mood => Get(nameof(Today_Mood));
    public static string Today_Functionality => Get(nameof(Today_Functionality));
    public static string Today_Sleep => Get(nameof(Today_Sleep));
    public static string Today_MedicationTaken => Get(nameof(Today_MedicationTaken));
    public static string Today_MenstrualCycle => Get(nameof(Today_MenstrualCycle));
    public static string Today_Hypomania => Get(nameof(Today_Hypomania));
    public static string Today_Symptoms => Get(nameof(Today_Symptoms));
    public static string Today_SymptomsPlaceholder => Get(nameof(Today_SymptomsPlaceholder));
    public static string Today_Notes => Get(nameof(Today_Notes));
    public static string Today_NotesPlaceholder => Get(nameof(Today_NotesPlaceholder));
    public static string Today_More => Get(nameof(Today_More));
    public static string Today_Saved => Get(nameof(Today_Saved));
    public static string Today_AlreadySaved => Get(nameof(Today_AlreadySaved));
    public static string Today_CriticalValues => Get(nameof(Today_CriticalValues));
    public static string Today_ShowCrisisResources => Get(nameof(Today_ShowCrisisResources));
    public static string Today_InputModeTitle => Get(nameof(Today_InputModeTitle));
    public static string Today_InputModeQuick => Get(nameof(Today_InputModeQuick));
    public static string Today_InputModeFull => Get(nameof(Today_InputModeFull));

    // Medications
    public static string Medications_Title => Get(nameof(Medications_Title));
    public static string Medications_ActiveList => Get(nameof(Medications_ActiveList));
    public static string Medications_Empty => Get(nameof(Medications_Empty));
    public static string Medications_AddNew => Get(nameof(Medications_AddNew));
    public static string Medications_AddTitle => Get(nameof(Medications_AddTitle));
    public static string Medications_DeleteTitle => Get(nameof(Medications_DeleteTitle));

    // Medication Form
    public static string MedForm_TitleAdd => Get(nameof(MedForm_TitleAdd));
    public static string MedForm_TitleEdit => Get(nameof(MedForm_TitleEdit));
    public static string MedForm_Dosage => Get(nameof(MedForm_Dosage));
    public static string MedForm_DosagePlaceholder => Get(nameof(MedForm_DosagePlaceholder));
    public static string MedForm_NamePlaceholder => Get(nameof(MedForm_NamePlaceholder));
    public static string MedForm_MinStock => Get(nameof(MedForm_MinStock));
    public static string MedForm_CurrentStock => Get(nameof(MedForm_CurrentStock));
    public static string MedForm_IntakeTimes => Get(nameof(MedForm_IntakeTimes));
    public static string MedForm_AddTime => Get(nameof(MedForm_AddTime));
    public static string MedForm_Doses => Get(nameof(MedForm_Doses));
    public static string MedForm_NameRequired => Get(nameof(MedForm_NameRequired));
    public static string MedForm_IntakeRequired => Get(nameof(MedForm_IntakeRequired));

    // Chart
    public static string Chart_Title => Get(nameof(Chart_Title));
    public static string Chart_Header => Get(nameof(Chart_Header));
    public static string Chart_Days30 => Get(nameof(Chart_Days30));
    public static string Chart_Days60 => Get(nameof(Chart_Days60));
    public static string Chart_Days90 => Get(nameof(Chart_Days90));
    public static string Chart_Empty => Get(nameof(Chart_Empty));
    public static string Chart_Hypomania => Get(nameof(Chart_Hypomania));
    public static string Chart_ExportPdf => Get(nameof(Chart_ExportPdf));
    public static string Chart_ExportPdfAlt => Get(nameof(Chart_ExportPdfAlt));
    public static string Chart_PdfExported => Get(nameof(Chart_PdfExported));
    public static string Chart_OpenFile => Get(nameof(Chart_OpenFile));

    // Settings
    public static string Settings_Title => Get(nameof(Settings_Title));
    public static string Settings_Backup => Get(nameof(Settings_Backup));
    public static string Settings_CloudProvider => Get(nameof(Settings_CloudProvider));
    public static string Settings_NextcloudUrl => Get(nameof(Settings_NextcloudUrl));
    public static string Settings_AutoBackup => Get(nameof(Settings_AutoBackup));
    public static string Settings_BackupWarningPrefix => Get(nameof(Settings_BackupWarningPrefix));
    public static string Settings_BackupWarningSuffix => Get(nameof(Settings_BackupWarningSuffix));
    public static string Settings_Reminders => Get(nameof(Settings_Reminders));
    public static string Settings_EveningReminder => Get(nameof(Settings_EveningReminder));
    public static string Settings_EveningReminderEnable => Get(nameof(Settings_EveningReminderEnable));
    public static string Settings_CrisisHint => Get(nameof(Settings_CrisisHint));
    public static string Settings_Display => Get(nameof(Settings_Display));
    public static string Settings_InputMode => Get(nameof(Settings_InputMode));
    public static string Settings_ColorBlind => Get(nameof(Settings_ColorBlind));
    public static string Settings_App => Get(nameof(Settings_App));
    public static string Settings_Language => Get(nameof(Settings_Language));
    public static string Settings_Biometrics => Get(nameof(Settings_Biometrics));
    public static string Settings_SaveButton => Get(nameof(Settings_SaveButton));
    public static string Settings_Saved => Get(nameof(Settings_Saved));
    public static string Settings_NoBackup => Get(nameof(Settings_NoBackup));
    public static string Settings_LocalExport => Get(nameof(Settings_LocalExport));
    public static string Settings_InputQuick => Get(nameof(Settings_InputQuick));
    public static string Settings_InputFull => Get(nameof(Settings_InputFull));
    public static string Settings_InputAlwaysAsk => Get(nameof(Settings_InputAlwaysAsk));
    public static string Settings_LangSystem => Get(nameof(Settings_LangSystem));
    public static string Settings_LangGerman => Get(nameof(Settings_LangGerman));
    public static string Settings_LangEnglish => Get(nameof(Settings_LangEnglish));

    // Crisis
    public static string Crisis_Title => Get(nameof(Crisis_Title));
    public static string Crisis_NotAlone => Get(nameof(Crisis_NotAlone));
    public static string Crisis_Region => Get(nameof(Crisis_Region));
    public static string Crisis_Empty => Get(nameof(Crisis_Empty));
    public static string Crisis_Available24h => Get(nameof(Crisis_Available24h));
    public static string Crisis_Call => Get(nameof(Crisis_Call));
    public static string Crisis_Website => Get(nameof(Crisis_Website));

    // Onboarding
    public static string Onboarding_Title => Get(nameof(Onboarding_Title));
    public static string Onboarding_Welcome => Get(nameof(Onboarding_Welcome));
    public static string Onboarding_Subtitle => Get(nameof(Onboarding_Subtitle));
    public static string Onboarding_DisclaimerTitle => Get(nameof(Onboarding_DisclaimerTitle));
    public static string Onboarding_Disclaimer => Get(nameof(Onboarding_Disclaimer));
    public static string Onboarding_CrisisHotline => Get(nameof(Onboarding_CrisisHotline));
    public static string Onboarding_ReminderQuestion => Get(nameof(Onboarding_ReminderQuestion));
    public static string Onboarding_ReminderEnable => Get(nameof(Onboarding_ReminderEnable));
    public static string Onboarding_ReminderNote => Get(nameof(Onboarding_ReminderNote));
    public static string Onboarding_Start => Get(nameof(Onboarding_Start));

    // Lock
    public static string Lock_AuthPrompt => Get(nameof(Lock_AuthPrompt));
    public static string Lock_Retry => Get(nameof(Lock_Retry));

    // About
    public static string About_Title => Get(nameof(About_Title));
    public static string About_Version => Get(nameof(About_Version));
    public static string About_Description => Get(nameof(About_Description));
    public static string About_Developer => Get(nameof(About_Developer));
    public static string About_License => Get(nameof(About_License));
    public static string About_Copyright => Get(nameof(About_Copyright));
    public static string About_Wiki => Get(nameof(About_Wiki));
    public static string About_Issues => Get(nameof(About_Issues));
    public static string About_ExperimentalNote => Get(nameof(About_ExperimentalNote));

    // Alarm
    public static string Alarm_Evening => Get(nameof(Alarm_Evening));
}
