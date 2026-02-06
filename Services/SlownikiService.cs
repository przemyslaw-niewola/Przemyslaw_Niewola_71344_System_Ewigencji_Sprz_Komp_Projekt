using EwidencjaSprzetuOOP.Data.Repositories;
using EwidencjaSprzetuOOP.Domain.Entities;
using EwidencjaSprzetuOOP.Domain.Validation;

namespace EwidencjaSprzetuOOP.Services;

public sealed class SlownikiService
{
    private readonly IDzialRepository _dz;
    private readonly ILokalizacjaRepository _lok;
    private readonly IDostawcaRepository _dos;

    public SlownikiService(IDzialRepository dz, ILokalizacjaRepository lok, IDostawcaRepository dos)
    {
        _dz = dz;
        _lok = lok;
        _dos = dos;
    }

    public List<Dzial> DzialyGetAll() => _dz.GetAll();
    public int DzialyAdd(string nazwa)
    {
        Validators.Required(nazwa, "Nazwa działu");
        return _dz.Add(nazwa);
    }
    public void DzialyUpdate(int id, string nazwa) => _dz.Update(id, nazwa);
    public void DzialyDelete(int id) => _dz.Delete(id);

    public List<Lokalizacja> LokalizacjeGetAll() => _lok.GetAll();
    public int LokalizacjeAdd(Lokalizacja l)
    {
        Validators.Required(l.Nazwa, "Nazwa lokalizacji");
        return _lok.Add(l);
    }

    public List<Dostawca> DostawcyGetAll() => _dos.GetAll();
    public int DostawcyAdd(Dostawca d)
    {
        Validators.Required(d.Nazwa, "Nazwa dostawcy");
        return _dos.Add(d);
    }
}
