using EwidencjaSprzetuOOP.Data.Repositories;
using EwidencjaSprzetuOOP.Domain.Entities;
using EwidencjaSprzetuOOP.Domain.Validation;

namespace EwidencjaSprzetuOOP.Services;

public sealed class SprzetService
{
    private readonly ISprzetRepository _repo;
    public SprzetService(ISprzetRepository repo) => _repo = repo;

    public List<Sprzet> GetAll() => _repo.GetAll();

    public int Add(Sprzet s)
    {
        Validators.Required(s.TypSprzetu, "TypSprzetu");
        Validators.Required(s.NumerEwidencyjny, "NumerEwidencyjny");
        Validators.MaxLen(s.NumerEwidencyjny, 50, "NumerEwidencyjny");
        return _repo.Add(s);
    }

    public void Update(Sprzet s)
    {
        Validators.PositiveInt(s.SprzetId, "SprzetId");
        Validators.Required(s.TypSprzetu, "TypSprzetu");
        Validators.Required(s.NumerEwidencyjny, "NumerEwidencyjny");
        _repo.Update(s);
    }

    public Sprzet? GetById(int id) => _repo.GetById(id);

    public void Delete(int id) => _repo.Delete(id);
}
