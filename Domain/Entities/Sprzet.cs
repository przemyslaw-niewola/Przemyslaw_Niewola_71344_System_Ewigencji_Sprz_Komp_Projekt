using EwidencjaSprzetuOOP.Domain;

namespace EwidencjaSprzetuOOP.Domain.Entities;

public sealed class Sprzet : EntityBase
{
    public int SprzetId { get; set; }

    public string TypSprzetu { get; set; } = "";
    public string NumerEwidencyjny { get; set; } = "";
    public string? NumerSeryjny { get; set; }

    public StatusSprzetu Status { get; set; } = StatusSprzetu.WMagazynie;

    public DateTime? DataZakupu { get; set; }
    public DateTime? DataKoncaGwarancji { get; set; }

    public int? LokalizacjaId { get; set; }
    public int? DostawcaId { get; set; }

    public string? Uwagi { get; set; }

    public string? Procesor { get; set; }
    public int? RamGb { get; set; }
    public int? DyskGb { get; set; }
    public string? SystemOperacyjny { get; set; }

    public decimal? PrzekatnaCala { get; set; }
    public string? Obudowa { get; set; }
    public string? Rozdzielczosc { get; set; }
    public bool? Kolorowa { get; set; }

    public string? AdresIp { get; set; }
    public string? AdresMac { get; set; }

    public override int Id { get => SprzetId; set => SprzetId = value; }
    public override string DisplayName => $"{NumerEwidencyjny} ({TypSprzetu})";
}
