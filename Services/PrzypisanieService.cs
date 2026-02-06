using Microsoft.Data.SqlClient;
using EwidencjaSprzetuOOP.Data.Repositories;
using EwidencjaSprzetuOOP.Domain;
using EwidencjaSprzetuOOP.Domain.Entities;
using EwidencjaSprzetuOOP.Domain.Exceptions;
using EwidencjaSprzetuOOP.Domain.Validation;

namespace EwidencjaSprzetuOOP.Services;

public sealed class PrzypisanieService
{
    private readonly IPrzypisanieRepository _przyRepo;
    private readonly ISprzetRepository _sprzetRepo;

    public PrzypisanieService(IPrzypisanieRepository przyRepo, ISprzetRepository sprzetRepo)
    {
        _przyRepo = przyRepo;
        _sprzetRepo = sprzetRepo;
    }

    public List<Przypisanie> GetActive() => _przyRepo.GetActive();

    public int Assign(int sprzetId, int pracownikId, string? uwagi)
    {
        Validators.PositiveInt(sprzetId, "SprzetId");
        Validators.PositiveInt(pracownikId, "PracownikId");

        var sprzet = _sprzetRepo.GetById(sprzetId);
        if (sprzet is null) throw new NotFoundException("Nie znaleziono sprzętu o podanym ID.");

        if (sprzet.Status == StatusSprzetu.Wycofany)
            throw new BusinessRuleException("Nie można przypisać sprzętu wycofanego.");

        if (sprzet.Status == StatusSprzetu.WSerwisie)
            throw new BusinessRuleException("Nie można przypisać sprzętu będącego w serwisie.");

        try
        {
            var id = _przyRepo.Add(new Przypisanie
            {
                SprzetId = sprzetId,
                PracownikId = pracownikId,
                Uwagi = uwagi
            });

            _sprzetRepo.SetStatus(sprzetId, StatusSprzetu.Przypisany);
            return id;
        }
        catch (SqlException ex) when (ex.Message.Contains("UX_Przy_JednoAktywneNaSprzet"))
        {
            throw new BusinessRuleException("Ten sprzęt ma już aktywne przypisanie (nie można dodać drugiego).");
        }
    }

    public void ReturnBySprzetId(int sprzetId)
    {
        Validators.PositiveInt(sprzetId, "SprzetId");

        var active = _przyRepo.GetActiveBySprzetId(sprzetId);
        if (active is null)
            throw new BusinessRuleException("Ten sprzęt nie ma aktywnego przypisania.");

        _przyRepo.CloseAssignment(active.PrzypisanieId);
        _sprzetRepo.SetStatus(sprzetId, StatusSprzetu.WMagazynie);
    }
}
