using EwidencjaSprzetuOOP.Services;
using EwidencjaSprzetuOOP.Utils;

namespace EwidencjaSprzetuOOP.Services;

public sealed class ExportService
{
    private readonly SprzetService _sprzet;
    private readonly PracownikService _prac;
    private readonly PrzypisanieService _przy;
    private readonly SerwisService _serwis;
    private readonly RaportyService _rap;

    public ExportService(
        SprzetService sprzet,
        PracownikService prac,
        PrzypisanieService przy,
        SerwisService serwis,
        RaportyService rap)
    {
        _sprzet = sprzet;
        _prac = prac;
        _przy = przy;
        _serwis = serwis;
        _rap = rap;
    }

    public string ExportSprzety(string folder)
    {
        var file = Path.Combine(folder, $"sprzety_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        var data = _sprzet.GetAll();

        return CsvExporter.Export(
            file,
            headers: new[] { "SprzetId", "NumerEwidencyjny", "TypSprzetu", "Status", "DataZakupu", "DataKoncaGwarancji", "AdresIp", "AdresMac", "Uwagi" },
            rows: data.Select(s => (IReadOnlyList<string?>)new[]
            {
                s.SprzetId.ToString(),
                s.NumerEwidencyjny,
                s.TypSprzetu,
                $"{(int)s.Status} ({s.Status})",
                s.DataZakupu?.ToString("yyyy-MM-dd"),
                s.DataKoncaGwarancji?.ToString("yyyy-MM-dd"),
                s.AdresIp,
                s.AdresMac,
                s.Uwagi
            })
        );
    }

    public string ExportPracownicy(string folder)
    {
        var file = Path.Combine(folder, $"pracownicy_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        var data = _prac.GetAll();

        return CsvExporter.Export(
            file,
            headers: new[] { "PracownikId", "DzialId", "Imie", "Nazwisko", "Email", "Telefon", "Aktywny" },
            rows: data.Select(p => (IReadOnlyList<string?>)new[]
            {
                p.PracownikId.ToString(),
                p.DzialId.ToString(),
                p.Imie,
                p.Nazwisko,
                p.Email,
                p.Telefon,
                p.Aktywny ? "1" : "0"
            })
        );
    }

    public string ExportAktywnePrzypisania(string folder)
    {
        var file = Path.Combine(folder, $"przypisania_aktywne_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        var data = _przy.GetActive();

        return CsvExporter.Export(
            file,
            headers: new[] { "PrzypisanieId", "SprzetId", "PracownikId", "PrzypisanoDnia", "Uwagi" },
            rows: data.Select(a => (IReadOnlyList<string?>)new[]
            {
                a.PrzypisanieId.ToString(),
                a.SprzetId.ToString(),
                a.PracownikId.ToString(),
                a.PrzypisanoDnia.ToString("yyyy-MM-dd HH:mm:ss"),
                a.Uwagi
            })
        );
    }

    public string ExportSerwisy(string folder)
    {
        var file = Path.Combine(folder, $"serwisy_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        var data = _serwis.GetLatest(200);

        return CsvExporter.Export(
            file,
            headers: new[] { "SerwisId", "SprzetId", "DostawcaId", "WykonanoDnia", "RodzajSerwisu", "Koszt", "Opis" },
            rows: data.Select(s => (IReadOnlyList<string?>)new[]
            {
                s.SerwisId.ToString(),
                s.SprzetId.ToString(),
                s.DostawcaId?.ToString(),
                s.WykonanoDnia.ToString("yyyy-MM-dd HH:mm:ss"),
                s.RodzajSerwisu,
                s.Koszt?.ToString(),
                s.Opis
            })
        );
    }

    public string ExportRaportGwarancjaWygasaW(string folder, int dni)
    {
        var file = Path.Combine(folder, $"raport_gwarancja_{dni}dni_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        var data = _rap.GwarancjaWygasaW(dni);

        return CsvExporter.Export(
            file,
            headers: new[] { "NumerEwidencyjny", "TypSprzetu", "DataKoncaGwarancji" },
            rows: data.Select(x => (IReadOnlyList<string?>)new[]
            {
                x.NumerEw,
                x.Typ,
                x.DataGwar.ToString("yyyy-MM-dd")
            })
        );
    }

    public string ExportRaportKosztySerwisow(string folder)
    {
        var file = Path.Combine(folder, $"raport_koszty_serwisow_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        var data = _rap.KosztySerwisowNaSprzet();

        return CsvExporter.Export(
            file,
            headers: new[] { "SprzetId", "SumaKosztow" },
            rows: data.Select(x => (IReadOnlyList<string?>)new[]
            {
                x.SprzetId.ToString(),
                x.SumaKosztow.ToString()
            })
        );
    }
}
