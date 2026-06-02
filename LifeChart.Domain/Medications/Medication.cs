namespace LifeChart.Domain.Medications;

public class Medication
{
    private readonly List<IntakeTime> _intakeTimes = new();

    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Dosage { get; private set; } = string.Empty;
    public int MinStock { get; private set; }
    public int CurrentStock { get; private set; }
    public bool Active { get; private set; }
    public IReadOnlyList<IntakeTime> IntakeTimes => _intakeTimes.AsReadOnly();

    public Medication(
        string name,
        string dosage,
        int minStock,
        int currentStock,
        IEnumerable<IntakeTime> intakeTimes)
    {
        Name = name;
        Dosage = dosage;
        MinStock = minStock;
        CurrentStock = currentStock;
        Active = true;
        _intakeTimes.AddRange(intakeTimes);
    }

    public bool IsStockLow => CurrentStock <= MinStock;

    public void Update(
        string name,
        string dosage,
        int minStock,
        int currentStock,
        IEnumerable<IntakeTime> intakeTimes)
    {
        Name = name;
        Dosage = dosage;
        MinStock = minStock;
        CurrentStock = currentStock;
        _intakeTimes.Clear();
        _intakeTimes.AddRange(intakeTimes);
    }

    public void Deactivate() => Active = false;

    private Medication() { }
}
