using LifeChart.Application.DTOs;

namespace LifeChart.Services;

// Trägt die Daten beim Navigieren zwischen MedicationsPage und MedicationFormPage.
public class MedicationFormService
{
    public MedicationDto? EditTarget { get; set; }
}
