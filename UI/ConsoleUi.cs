using EwidencjaSprzetuOOP.Domain;
using EwidencjaSprzetuOOP.Domain.Entities;
using EwidencjaSprzetuOOP.Domain.Exceptions;
using EwidencjaSprzetuOOP.Services;
using EwidencjaSprzetuOOP.Utils;

namespace EwidencjaSprzetuOOP.UI;

public sealed class ConsoleUi
{
    private readonly SprzetService _sprzet;
    private readonly PracownikService _prac;
    private readonly PrzypisanieService _przy;
    private readonly SerwisService _serwis;
    private readonly RaportyService _rap;
    private readonly SlownikiService _slow;
    private readonly ExportService _export;
    private readonly ImportService _import;

    public ConsoleUi(
        SprzetService sprzet,
        PracownikService prac,
        PrzypisanieService przy,
        SerwisService serwis,
        RaportyService rap,
        SlownikiService slow,
        ExportService export,
        ImportService import)
    {
        _sprzet = sprzet;
        _prac = prac;
        _przy = przy;
        _serwis = serwis;
        _rap = rap;
        _slow = slow;
        _export = export;
        _import = import;
    }

    public void Run()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== SYSTEM EWIDENCJI SPRZĘTU (C# + SQL Server) ===");
            Console.WriteLine("1) Sprzęty");
            Console.WriteLine("2) Pracownicy");
            Console.WriteLine("3) Przypisania (wydaj/zwrot)");
            Console.WriteLine("4) Serwisy");
            Console.WriteLine("5) Raporty");
            Console.WriteLine("6) Słowniki (Działy/Lokalizacje/Dostawcy)");
            Console.WriteLine("7) Pliki CSV (Import/Eksport)");
            Console.WriteLine("0) Wyjście");
            Console.Write("Wybór: ");
            var key = Console.ReadLine();

            try
            {
                switch (key)
                {
                    case "1": MenuSprzety(); break;
                    case "2": MenuPracownicy(); break;
                    case "3": MenuPrzypisania(); break;
                    case "4": MenuSerwisy(); break;
                    case "5": MenuRaporty(); break;
                    case "6": MenuSlowniki(); break;
                    case "7": MenuCsv(); break;
                    case "0": return;
                    default: break;
                }
            }
            catch (ValidationException ex) { ShowErr(ex.Message); }
            catch (BusinessRuleException ex) { ShowErr(ex.Message); }
            catch (NotFoundException ex) { ShowErr(ex.Message); }
            catch (Exception ex) { ShowErr("Błąd krytyczny: " + ex.Message); }
        }
    }

    private void MenuSprzety()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== SPRZĘTY ===");
            Console.WriteLine("1) Lista");
            Console.WriteLine("2) Dodaj");
            Console.WriteLine("3) Edytuj");
            Console.WriteLine("4) Usuń");
            Console.WriteLine("0) Wstecz");
            Console.Write("Wybór: ");
            var k = Console.ReadLine();

            switch (k)
            {
                case "1":
                    var all = _sprzet.GetAll();
                    Console.WriteLine("ID | NumerEw | Typ | Status");
                    foreach (var s in all)
                        Console.WriteLine($"{s.SprzetId} | {s.NumerEwidencyjny} | {s.TypSprzetu} | {(int)s.Status}({s.Status})");
                    Pause();
                    break;

                case "2":
                    var ns = new Sprzet
                    {
                        TypSprzetu = ConsoleInput.ReadRequired("TypSprzetu: "),
                        NumerEwidencyjny = ConsoleInput.ReadRequired("NumerEwidencyjny (np. EW-0005): "),
                        NumerSeryjny = ConsoleInput.ReadOptional("NumerSeryjny (opcjonalnie): "),
                        Status = StatusSprzetu.WMagazynie,
                        Uwagi = ConsoleInput.ReadOptional("Uwagi (opcjonalnie): "),
                        AdresIp = ConsoleInput.ReadOptional("Adres IP (opcjonalnie): "),
                        AdresMac = ConsoleInput.ReadOptional("Adres MAC (opcjonalnie): ")
                    };
                    ns.RamGb = ConsoleInput.ReadNullableInt("RAM GB (opcjonalnie): ");
                    ns.DyskGb = ConsoleInput.ReadNullableInt("Dysk GB (opcjonalnie): ");
                    var id = _sprzet.Add(ns);
                    ShowOk($"Dodano sprzęt. ID={id}");
                    break;

                case "3":
                    var sid = ConsoleInput.ReadInt("Podaj SprzetId do edycji: ");
                    var sOld = _sprzet.GetById(sid);
                    if (sOld is null) throw new NotFoundException("Nie znaleziono sprzętu.");

                    Console.WriteLine($"Aktualnie: {sOld.NumerEwidencyjny}, {sOld.TypSprzetu}, Status={sOld.Status}");
                    sOld.TypSprzetu = ConsoleInput.ReadRequired("Nowy TypSprzetu: ");
                    sOld.NumerEwidencyjny = ConsoleInput.ReadRequired("Nowy NumerEwidencyjny: ");
                    sOld.NumerSeryjny = ConsoleInput.ReadOptional("Nowy NumerSeryjny (puste = null): ");
                    sOld.Uwagi = ConsoleInput.ReadOptional("Uwagi (puste = null): ");

                    Console.WriteLine("Status: 0=Magazyn 1=Przypisany 2=Serwis 3=Wycofany");
                    sOld.Status = (StatusSprzetu)ConsoleInput.ReadInt("Nowy Status (0-3): ");

                    _sprzet.Update(sOld);
                    ShowOk("Zapisano zmiany.");
                    break;

                case "4":
                    var did = ConsoleInput.ReadInt("Podaj SprzetId do usunięcia: ");
                    _sprzet.Delete(did);
                    ShowOk("Usunięto (jeśli nie było powiązań FK).");
                    break;

                case "0":
                    return;
            }
        }
    }

    private void MenuPracownicy()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== PRACOWNICY ===");
            Console.WriteLine("1) Lista");
            Console.WriteLine("2) Dodaj");
            Console.WriteLine("3) Edytuj");
            Console.WriteLine("4) Aktywuj/Dezaktywuj");
            Console.WriteLine("0) Wstecz");
            Console.Write("Wybór: ");
            var k = Console.ReadLine();

            switch (k)
            {
                case "1":
                    var all = _prac.GetAll();
                    Console.WriteLine("ID | DzialId | Imie Nazwisko | Email | Aktywny");
                    foreach (var p in all)
                        Console.WriteLine($"{p.PracownikId} | {p.DzialId} | {p.Imie} {p.Nazwisko} | {p.Email} | {p.Aktywny}");
                    Pause();
                    break;

                case "2":
                    var pnew = new Pracownik
                    {
                        DzialId = ConsoleInput.ReadInt("DzialId: "),
                        Imie = ConsoleInput.ReadRequired("Imię: "),
                        Nazwisko = ConsoleInput.ReadRequired("Nazwisko: "),
                        Email = ConsoleInput.ReadRequired("Email: "),
                        Telefon = ConsoleInput.ReadOptional("Telefon (opcjonalnie): "),
                        Aktywny = true
                    };
                    var id = _prac.Add(pnew);
                    ShowOk($"Dodano pracownika ID={id}");
                    break;

                case "3":
                    var pid = ConsoleInput.ReadInt("PracownikId do edycji: ");
                    var pedit = new Pracownik
                    {
                        PracownikId = pid,
                        DzialId = ConsoleInput.ReadInt("Nowy DzialId: "),
                        Imie = ConsoleInput.ReadRequired("Nowe Imię: "),
                        Nazwisko = ConsoleInput.ReadRequired("Nowe Nazwisko: "),
                        Email = ConsoleInput.ReadRequired("Nowy Email: "),
                        Telefon = ConsoleInput.ReadOptional("Telefon (opcjonalnie): "),
                        Aktywny = ConsoleInput.ReadYesNo("Aktywny?")
                    };
                    _prac.Update(pedit);
                    ShowOk("Zapisano zmiany.");
                    break;

                case "4":
                    var pid2 = ConsoleInput.ReadInt("PracownikId: ");
                    var active = ConsoleInput.ReadYesNo("Ustawić jako AKTYWNY?");
                    _prac.SetActive(pid2, active);
                    ShowOk("Zmieniono status.");
                    break;

                case "0":
                    return;
            }
        }
    }

    private void MenuPrzypisania()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== PRZYPISANIA ===");
            Console.WriteLine("1) Lista aktywnych");
            Console.WriteLine("2) Wydaj sprzęt (nowe przypisanie)");
            Console.WriteLine("3) Zwrot sprzętu (zamknij przypisanie po SprzetId)");
            Console.WriteLine("0) Wstecz");
            Console.Write("Wybór: ");
            var k = Console.ReadLine();

            switch (k)
            {
                case "1":
                    var act = _przy.GetActive();
                    Console.WriteLine("PrzypisanieId | SprzetId | PracownikId | PrzypisanoDnia | Uwagi");
                    foreach (var a in act)
                        Console.WriteLine($"{a.PrzypisanieId} | {a.SprzetId} | {a.PracownikId} | {a.PrzypisanoDnia} | {a.Uwagi}");
                    Pause();
                    break;

                case "2":
                    var sid = ConsoleInput.ReadInt("SprzetId: ");
                    var pid = ConsoleInput.ReadInt("PracownikId: ");
                    var uw = ConsoleInput.ReadOptional("Uwagi (opcjonalnie): ");
                    var newId = _przy.Assign(sid, pid, uw);
                    ShowOk($"Dodano przypisanie ID={newId}");
                    break;

                case "3":
                    var rsid = ConsoleInput.ReadInt("SprzetId do zwrotu: ");
                    _przy.ReturnBySprzetId(rsid);
                    ShowOk("Zwrócono sprzęt (zamknięto przypisanie).");
                    break;

                case "0":
                    return;
            }
        }
    }

    private void MenuSerwisy()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== SERWISY ===");
            Console.WriteLine("1) Ostatnie serwisy");
            Console.WriteLine("2) Dodaj serwis");
            Console.WriteLine("0) Wstecz");
            Console.Write("Wybór: ");
            var k = Console.ReadLine();

            switch (k)
            {
                case "1":
                    var list = _serwis.GetLatest(50);
                    Console.WriteLine("SerwisId | SprzetId | DostawcaId | Data | Rodzaj | Koszt");
                    foreach (var s in list)
                        Console.WriteLine($"{s.SerwisId} | {s.SprzetId} | {s.DostawcaId} | {s.WykonanoDnia} | {s.RodzajSerwisu} | {s.Koszt}");
                    Pause();
                    break;

                case "2":
                    var sprzetId = ConsoleInput.ReadInt("SprzetId: ");
                    var dostId = ConsoleInput.ReadNullableInt("DostawcaId (opcjonalnie): ");
                    var rodzaj = ConsoleInput.ReadRequired("Rodzaj serwisu (np. Naprawa/Przegląd): ");
                    var opis = ConsoleInput.ReadOptional("Opis (opcjonalnie): ");
                    var koszt = ConsoleInput.ReadNullableDecimal("Koszt (opcjonalnie): ");
                    var setStatus = ConsoleInput.ReadYesNo("Ustawić status sprzętu na 'W serwisie'?");
                    var id = _serwis.Add(sprzetId, dostId, rodzaj, opis, koszt, setStatus);
                    ShowOk($"Dodano wpis serwisu ID={id}");
                    break;

                case "0":
                    return;
            }
        }
    }

    private void MenuRaporty()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== RAPORTY ===");
            Console.WriteLine("1) Sprzęt w magazynie (Status=0)");
            Console.WriteLine("2) Sprzęt w serwisie (Status=2)");
            Console.WriteLine("3) Gwarancja wygasa w X dni");
            Console.WriteLine("4) Koszty serwisów per sprzęt");
            Console.WriteLine("0) Wstecz");
            Console.Write("Wybór: ");
            var k = Console.ReadLine();

            switch (k)
            {
                case "1":
                    foreach (var x in _rap.SprzetPoStatusie(0))
                        Console.WriteLine($"{x.NumerEw} | {x.Typ} | {x.Status}");
                    Pause();
                    break;

                case "2":
                    foreach (var x in _rap.SprzetPoStatusie(2))
                        Console.WriteLine($"{x.NumerEw} | {x.Typ} | {x.Status}");
                    Pause();
                    break;

                case "3":
                    var dni = ConsoleInput.ReadInt("Podaj liczbę dni (np. 60): ");
                    foreach (var x in _rap.GwarancjaWygasaW(dni))
                        Console.WriteLine($"{x.NumerEw} | {x.Typ} | {x.DataGwar:yyyy-MM-dd}");
                    Pause();
                    break;

                case "4":
                    foreach (var x in _rap.KosztySerwisowNaSprzet())
                        Console.WriteLine($"SprzetId={x.SprzetId} | Suma kosztów={x.SumaKosztow}");
                    Pause();
                    break;

                case "0":
                    return;
            }
        }
    }

    private void MenuSlowniki()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== SŁOWNIKI ===");
            Console.WriteLine("1) Działy");
            Console.WriteLine("2) Lokalizacje");
            Console.WriteLine("3) Dostawcy");
            Console.WriteLine("0) Wstecz");
            Console.Write("Wybór: ");

            var k = Console.ReadLine();
            switch (k)
            {
                case "1": SlownikiDzialy(); break;
                case "2": SlownikiLokalizacje(); break;
                case "3": SlownikiDostawcy(); break;
                case "0": return;
            }
        }
    }

    private void SlownikiDzialy()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== DZIAŁY ===");
            Console.WriteLine("1) Lista");
            Console.WriteLine("2) Dodaj");
            Console.WriteLine("3) Edytuj");
            Console.WriteLine("4) Usuń");
            Console.WriteLine("0) Wstecz");
            var k = Console.ReadLine();

            switch (k)
            {
                case "1":
                    foreach (var d in _slow.DzialyGetAll())
                        Console.WriteLine($"{d.DzialId} | {d.Nazwa}");
                    Pause();
                    break;
                case "2":
                    var n = ConsoleInput.ReadRequired("Nazwa działu: ");
                    var id = _slow.DzialyAdd(n);
                    ShowOk($"Dodano dział ID={id}");
                    break;
                case "3":
                    var idu = ConsoleInput.ReadInt("DzialId: ");
                    var nn = ConsoleInput.ReadRequired("Nowa nazwa: ");
                    _slow.DzialyUpdate(idu, nn);
                    ShowOk("Zapisano.");
                    break;
                case "4":
                    var idd = ConsoleInput.ReadInt("DzialId do usunięcia: ");
                    _slow.DzialyDelete(idd);
                    ShowOk("Usunięto.");
                    break;
                case "0": return;
            }
        }
    }

    private void SlownikiLokalizacje()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== LOKALIZACJE ===");
            Console.WriteLine("1) Lista");
            Console.WriteLine("2) Dodaj");
            Console.WriteLine("0) Wstecz");
            var k = Console.ReadLine();

            switch (k)
            {
                case "1":
                    foreach (var l in _slow.LokalizacjeGetAll())
                        Console.WriteLine($"{l.LokalizacjaId} | {l.Nazwa} | {l.Adres}");
                    Pause();
                    break;
                case "2":
                    var lnew = new Lokalizacja
                    {
                        Nazwa = ConsoleInput.ReadRequired("Nazwa: "),
                        Adres = ConsoleInput.ReadOptional("Adres (opcjonalnie): ")
                    };
                    var id = _slow.LokalizacjeAdd(lnew);
                    ShowOk($"Dodano lokalizację ID={id}");
                    break;
                case "0": return;
            }
        }
    }

    private void SlownikiDostawcy()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== DOSTAWCY ===");
            Console.WriteLine("1) Lista");
            Console.WriteLine("2) Dodaj");
            Console.WriteLine("0) Wstecz");
            var k = Console.ReadLine();

            switch (k)
            {
                case "1":
                    foreach (var d in _slow.DostawcyGetAll())
                        Console.WriteLine($"{d.DostawcaId} | {d.Nazwa} | {d.Email} | {d.Telefon}");
                    Pause();
                    break;
                case "2":
                    var dnew = new Dostawca
                    {
                        Nazwa = ConsoleInput.ReadRequired("Nazwa: "),
                        Email = ConsoleInput.ReadOptional("Email (opcjonalnie): "),
                        Telefon = ConsoleInput.ReadOptional("Telefon (opcjonalnie): ")
                    };
                    var id = _slow.DostawcyAdd(dnew);
                    ShowOk($"Dodano dostawcę ID={id}");
                    break;
                case "0": return;
            }
        }
    }

    private void MenuCsv()
    {
        var folder = Path.Combine(AppContext.BaseDirectory, "exports");
        Directory.CreateDirectory(folder);

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== PLIKI CSV (IMPORT / EKSPORT) ===");
            Console.WriteLine($"Folder eksportów: {folder}");
            Console.WriteLine();
            Console.WriteLine("1) EKSPORT: Sprzęty");
            Console.WriteLine("2) EKSPORT: Pracownicy");
            Console.WriteLine("3) EKSPORT: Aktywne przypisania");
            Console.WriteLine("4) EKSPORT: Serwisy");
            Console.WriteLine("5) EKSPORT: Raport gwarancja w X dni");
            Console.WriteLine("6) EKSPORT: Raport koszty serwisów");
            Console.WriteLine();
            Console.WriteLine("7) IMPORT: Działy");
            Console.WriteLine("8) IMPORT: Lokalizacje");
            Console.WriteLine("9) IMPORT: Dostawcy");
            Console.WriteLine("10) IMPORT: Pracownicy");
            Console.WriteLine("11) IMPORT: Sprzęty");
            Console.WriteLine("0) Wstecz");
            Console.Write("Wybór: ");

            var k = Console.ReadLine();
            switch (k)
            {
                case "1":
                    ShowOk("Zapisano: " + _export.ExportSprzety(folder));
                    break;
                case "2":
                    ShowOk("Zapisano: " + _export.ExportPracownicy(folder));
                    break;
                case "3":
                    ShowOk("Zapisano: " + _export.ExportAktywnePrzypisania(folder));
                    break;
                case "4":
                    ShowOk("Zapisano: " + _export.ExportSerwisy(folder));
                    break;
                case "5":
                    var dni = ConsoleInput.ReadInt("Podaj liczbę dni (np. 60): ");
                    ShowOk("Zapisano: " + _export.ExportRaportGwarancjaWygasaW(folder, dni));
                    break;
                case "6":
                    ShowOk("Zapisano: " + _export.ExportRaportKosztySerwisow(folder));
                    break;

                case "7":
                    {
                        var path = ConsoleInput.ReadRequired("Ścieżka do CSV (Działy): ");
                        var s = _import.ImportDzialy(path);
                        ShowOk($"Import Działy: dodano={s.Inserted}, pominięto={s.Skipped}, błędy={s.Errors}");
                        break;
                    }
                case "8":
                    {
                        var path = ConsoleInput.ReadRequired("Ścieżka do CSV (Lokalizacje): ");
                        var s = _import.ImportLokalizacje(path);
                        ShowOk($"Import Lokalizacje: dodano={s.Inserted}, pominięto={s.Skipped}, błędy={s.Errors}");
                        break;
                    }
                case "9":
                    {
                        var path = ConsoleInput.ReadRequired("Ścieżka do CSV (Dostawcy): ");
                        var s = _import.ImportDostawcy(path);
                        ShowOk($"Import Dostawcy: dodano={s.Inserted}, pominięto={s.Skipped}, błędy={s.Errors}");
                        break;
                    }
                case "10":
                    {
                        var path = ConsoleInput.ReadRequired("Ścieżka do CSV (Pracownicy): ");
                        var s = _import.ImportPracownicy(path);
                        ShowOk($"Import Pracownicy: dodano={s.Inserted}, pominięto={s.Skipped}, błędy={s.Errors}");
                        break;
                    }
                case "11":
                    {
                        var path = ConsoleInput.ReadRequired("Ścieżka do CSV (Sprzęty): ");
                        var s = _import.ImportSprzety(path);
                        ShowOk($"Import Sprzęty: dodano={s.Inserted}, pominięto={s.Skipped}, błędy={s.Errors}");
                        break;
                    }

                case "0":
                    return;
            }
        }
    }

    private static void Pause()
    {
        Console.WriteLine();
        Console.WriteLine("Enter aby kontynuować...");
        Console.ReadLine();
    }

    private static void ShowOk(string msg)
    {
        Console.WriteLine();
        Console.WriteLine("[OK] " + msg);
        Pause();
    }

    private static void ShowErr(string msg)
    {
        Console.WriteLine();
        Console.WriteLine("[BŁĄD] " + msg);
        Pause();
    }
}
