using EwidencjaSprzetuOOP.Data.Repositories;
using EwidencjaSprzetuOOP.Domain.Entities;
using EwidencjaSprzetuOOP.Domain.Validation;

namespace EwidencjaSprzetuOOP.Services;

public sealed class PracownikService
{
    private readonly IPracownikRepository _repo;
    public PracownikService(IPracownikRepository repo) => _repo = repo;

    public List<Pracownik> GetAll() => _repo.GetAll();

    public int Add(Pracownik p)
    {
        Validators.PositiveInt(p.DzialId, "DzialId");
        Validators.Required(p.Imie, "Imie");
        Validators.Required(p.Nazwisko, "Nazwisko");
        Validators.Email(p.Email, "Email");
        return _repo.Add(p);
    }

    public void Update(Pracownik p)
    {
        Validators.PositiveInt(p.PracownikId, "PracownikId");
        Validators.PositiveInt(p.DzialId, "DzialId");
        Validators.Required(p.Imie, "Imie");
        Validators.Required(p.Nazwisko, "Nazwisko");
        Validators.Email(p.Email, "Email");
        _repo.Update(p);
    }

    public void SetActive(int id, bool active) => _repo.SetActive(id, active);
}
