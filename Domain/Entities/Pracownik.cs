namespace EwidencjaSprzetuOOP.Domain.Entities;

public sealed class Pracownik : EntityBase
{
    public int PracownikId { get; set; }
    public int DzialId { get; set; }
    public string Imie { get; set; } = "";
    public string Nazwisko { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Telefon { get; set; }
    public bool Aktywny { get; set; } = true;

    public override int Id { get => PracownikId; set => PracownikId = value; }
    public override string DisplayName => $"{Imie} {Nazwisko}";
}
