namespace EwidencjaSprzetuOOP.Domain.Entities;

public sealed class Dostawca : EntityBase
{
    public int DostawcaId { get; set; }
    public string Nazwa { get; set; } = "";
    public string? Email { get; set; }
    public string? Telefon { get; set; }

    public override int Id { get => DostawcaId; set => DostawcaId = value; }
    public override string DisplayName => Nazwa;
}
