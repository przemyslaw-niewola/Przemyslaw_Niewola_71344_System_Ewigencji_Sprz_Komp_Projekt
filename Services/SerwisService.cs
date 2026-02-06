using EwidencjaSprzetuOOP.Data.Repositories;
using EwidencjaSprzetuOOP.Domain;
using EwidencjaSprzetuOOP.Domain.Entities;
using EwidencjaSprzetuOOP.Domain.Validation;

namespace EwidencjaSprzetuOOP.Services;

public sealed class SerwisService
{
    private readonly ISerwisRepository _repo;
    private readonly ISprzetRepository _sprzetRepo;

    public SerwisService(ISerwisRepository repo, ISprzetRepository sprzetRepo)
    {
        _repo = repo;
        _sprzetRepo = sprzetRepo;
    }

    public List<Serwis> GetLatest(int top = 30) => _repo.GetLatest(top);

    public int Add(int sprzetId, int? dostawcaId, string rodzaj, string? opis, decimal? koszt, bool ustawStatusSerwis)
    {
        Validators.PositiveInt(sprzetId, "SprzetId");
        Validators.Required(rodzaj, "RodzajSerwisu");

        var id = _repo.Add(new Serwis
        {
            SprzetId = sprzetId,
            DostawcaId = dostawcaId,
            RodzajSerwisu = rodzaj,
            Opis = opis,
            Koszt = koszt
        });

        if (ustawStatusSerwis)
            _sprzetRepo.SetStatus(sprzetId, StatusSprzetu.WSerwisie);

        return id;
    }
}
