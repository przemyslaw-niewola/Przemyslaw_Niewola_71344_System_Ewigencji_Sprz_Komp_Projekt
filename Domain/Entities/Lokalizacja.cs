namespace EwidencjaSprzetuOOP.Domain.Entities;

public sealed class Lokalizacja : EntityBase
{
    public int LokalizacjaId { get; set; }
    public string Nazwa { get; set; } = "";
    public string? Adres { get; set; }

    public override int Id { get => LokalizacjaId; set => LokalizacjaId = value; }
    public override string DisplayName => Nazwa;
}
