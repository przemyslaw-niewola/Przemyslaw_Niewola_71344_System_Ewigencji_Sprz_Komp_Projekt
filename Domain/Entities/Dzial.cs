namespace EwidencjaSprzetuOOP.Domain.Entities;

public sealed class Dzial : EntityBase
{
    public int DzialId { get; set; }
    public string Nazwa { get; set; } = "";

    public override int Id { get => DzialId; set => DzialId = value; }
    public override string DisplayName => Nazwa;
}
