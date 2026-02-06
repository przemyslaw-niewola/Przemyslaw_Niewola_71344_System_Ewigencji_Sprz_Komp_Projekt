namespace EwidencjaSprzetuOOP.Domain.Entities;

public sealed class Przypisanie : EntityBase
{
    public int PrzypisanieId { get; set; }
    public int SprzetId { get; set; }
    public int PracownikId { get; set; }

    public DateTime PrzypisanoDnia { get; set; }
    public DateTime? ZwroconoDnia { get; set; }

    public string? Uwagi { get; set; }

    public override int Id { get => PrzypisanieId; set => PrzypisanieId = value; }
    public override string DisplayName => $"Sprzet {SprzetId} -> Pracownik {PracownikId}";
}
