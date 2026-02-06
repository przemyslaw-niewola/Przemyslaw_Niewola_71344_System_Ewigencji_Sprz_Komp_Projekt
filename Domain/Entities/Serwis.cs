namespace EwidencjaSprzetuOOP.Domain.Entities;

public sealed class Serwis : EntityBase
{
    public int SerwisId { get; set; }
    public int SprzetId { get; set; }
    public int? DostawcaId { get; set; }

    public DateTime WykonanoDnia { get; set; }
    public string RodzajSerwisu { get; set; } = "";
    public string? Opis { get; set; }
    public decimal? Koszt { get; set; }

    public override int Id { get => SerwisId; set => SerwisId = value; }
    public override string DisplayName => $"{RodzajSerwisu} (Sprzet {SprzetId})";
}
