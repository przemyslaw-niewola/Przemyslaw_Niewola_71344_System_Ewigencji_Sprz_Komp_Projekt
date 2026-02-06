namespace EwidencjaSprzetuOOP.Domain.Entities;

public abstract class EntityBase
{
    public abstract int Id { get; set; }

    public virtual string DisplayName => GetType().Name;

    public override string ToString() => $"{GetType().Name} #{Id} - {DisplayName}";
}
