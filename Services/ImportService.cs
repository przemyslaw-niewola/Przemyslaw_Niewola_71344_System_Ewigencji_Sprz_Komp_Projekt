using System.Globalization;
using Microsoft.Data.SqlClient;
using EwidencjaSprzetuOOP.Domain;
using EwidencjaSprzetuOOP.Domain.Entities;
using EwidencjaSprzetuOOP.Utils;

namespace EwidencjaSprzetuOOP.Services;

public sealed class ImportService
{
    public sealed record ImportSummary(int Inserted, int Skipped, int Errors);

    private readonly SprzetService _sprzet;
    private readonly PracownikService _prac;
    private readonly SlownikiService _slow;
    private readonly SerwisService _serwis;
    private readonly PrzypisanieService _przy;

    public ImportService(
        SprzetService sprzet,
        PracownikService prac,
        SlownikiService slow,
        SerwisService serwis,
        PrzypisanieService przy)
    {
        _sprzet = sprzet;
        _prac = prac;
        _slow = slow;
        _serwis = serwis;
        _przy = przy;
    }


    public ImportSummary ImportDzialy(string csvPath, char sep = ';')
    {
        var data = CsvImporter.Read(csvPath, sep);
        int idxNazwa = FindCol(data.Headers, "Nazwa");

        int inserted = 0, skipped = 0, errors = 0;

        foreach (var r in data.Rows)
        {
            try
            {
                var nazwa = Get(r, idxNazwa)?.Trim();
                if (string.IsNullOrWhiteSpace(nazwa)) { skipped++; continue; }

                _slow.DzialyAdd(nazwa);
                inserted++;
            }
            catch (SqlException ex) when (IsDuplicate(ex))
            {
                skipped++;
            }
            catch
            {
                errors++;
            }
        }

        return new ImportSummary(inserted, skipped, errors);
    }

    public ImportSummary ImportLokalizacje(string csvPath, char sep = ';')
    {
        var data = CsvImporter.Read(csvPath, sep);
        int idxNazwa = FindCol(data.Headers, "Nazwa");
        int idxAdres = TryFindCol(data.Headers, "Adres");

        int inserted = 0, skipped = 0, errors = 0;

        foreach (var r in data.Rows)
        {
            try
            {
                var nazwa = Get(r, idxNazwa)?.Trim();
                if (string.IsNullOrWhiteSpace(nazwa)) { skipped++; continue; }

                var adres = idxAdres >= 0 ? Get(r, idxAdres)?.Trim() : null;

                _slow.LokalizacjeAdd(new Lokalizacja { Nazwa = nazwa, Adres = ToNull(adres) });
                inserted++;
            }
            catch (SqlException ex) when (IsDuplicate(ex))
            {
                skipped++;
            }
            catch
            {
                errors++;
            }
        }

        return new ImportSummary(inserted, skipped, errors);
    }

    public ImportSummary ImportDostawcy(string csvPath, char sep = ';')
    {
        var data = CsvImporter.Read(csvPath, sep);
        int idxNazwa = FindCol(data.Headers, "Nazwa");
        int idxEmail = TryFindCol(data.Headers, "Email");
        int idxTel = TryFindCol(data.Headers, "Telefon");

        int inserted = 0, skipped = 0, errors = 0;

        foreach (var r in data.Rows)
        {
            try
            {
                var nazwa = Get(r, idxNazwa)?.Trim();
                if (string.IsNullOrWhiteSpace(nazwa)) { skipped++; continue; }

                var email = idxEmail >= 0 ? ToNull(Get(r, idxEmail)?.Trim()) : null;
                var tel = idxTel >= 0 ? ToNull(Get(r, idxTel)?.Trim()) : null;

                _slow.DostawcyAdd(new Dostawca { Nazwa = nazwa, Email = email, Telefon = tel });
                inserted++;
            }
            catch (SqlException ex) when (IsDuplicate(ex))
            {
                skipped++;
            }
            catch
            {
                errors++;
            }
        }

        return new ImportSummary(inserted, skipped, errors);
    }
    public ImportSummary ImportPracownicy(string csvPath, char sep = ';')
    {
        var data = CsvImporter.Read(csvPath, sep);
        int idxDzial = FindCol(data.Headers, "DzialId");
        int idxImie = FindCol(data.Headers, "Imie");
        int idxNazw = FindCol(data.Headers, "Nazwisko");
        int idxEmail = FindCol(data.Headers, "Email");
        int idxTel = TryFindCol(data.Headers, "Telefon");
        int idxAkt = TryFindCol(data.Headers, "Aktywny");

        int inserted = 0, skipped = 0, errors = 0;

        foreach (var r in data.Rows)
        {
            try
            {
                var dzialId = ParseInt(Get(r, idxDzial));
                var imie = Get(r, idxImie)?.Trim();
                var nazw = Get(r, idxNazw)?.Trim();
                var email = Get(r, idxEmail)?.Trim();

                if (dzialId <= 0 || string.IsNullOrWhiteSpace(imie) || string.IsNullOrWhiteSpace(nazw) || string.IsNullOrWhiteSpace(email))
                {
                    skipped++;
                    continue;
                }

                var tel = idxTel >= 0 ? ToNull(Get(r, idxTel)?.Trim()) : null;
                var aktywny = idxAkt >= 0 ? ParseBool(Get(r, idxAkt), defaultValue: true) : true;

                _prac.Add(new Pracownik
                {
                    DzialId = dzialId,
                    Imie = imie!,
                    Nazwisko = nazw!,
                    Email = email!,
                    Telefon = tel,
                    Aktywny = aktywny
                });

                inserted++;
            }
            catch (SqlException ex) when (IsDuplicate(ex))
            {
                skipped++;
            }
            catch
            {
                errors++;
            }
        }

        return new ImportSummary(inserted, skipped, errors);
    }

    public ImportSummary ImportSprzety(string csvPath, char sep = ';')
    {
        var data = CsvImporter.Read(csvPath, sep);

        int idxNE = FindCol(data.Headers, "NumerEwidencyjny");
        int idxTyp = FindCol(data.Headers, "TypSprzetu");
        int idxStatus = TryFindCol(data.Headers, "Status");
        int idxDZ = TryFindCol(data.Headers, "DataZakupu");
        int idxDG = TryFindCol(data.Headers, "DataKoncaGwarancji");
        int idxIP = TryFindCol(data.Headers, "AdresIp");
        int idxMAC = TryFindCol(data.Headers, "AdresMac");
        int idxUw = TryFindCol(data.Headers, "Uwagi");

        int inserted = 0, skipped = 0, errors = 0;

        foreach (var r in data.Rows)
        {
            try
            {
                var ne = Get(r, idxNE)?.Trim();
                var typ = Get(r, idxTyp)?.Trim();

                if (string.IsNullOrWhiteSpace(ne) || string.IsNullOrWhiteSpace(typ))
                {
                    skipped++;
                    continue;
                }

                var status = StatusSprzetu.WMagazynie;
                if (idxStatus >= 0)
                {
                    var raw = Get(r, idxStatus);
                    var sNum = ExtractLeadingInt(raw);
                    if (sNum is >= 0 and <= 3)
                        status = (StatusSprzetu)sNum;
                }

                var dz = idxDZ >= 0 ? ParseNullableDate(Get(r, idxDZ)) : null;
                var dg = idxDG >= 0 ? ParseNullableDate(Get(r, idxDG)) : null;

                var ip = idxIP >= 0 ? ToNull(Get(r, idxIP)?.Trim()) : null;
                var mac = idxMAC >= 0 ? ToNull(Get(r, idxMAC)?.Trim()) : null;
                var uw = idxUw >= 0 ? ToNull(Get(r, idxUw)?.Trim()) : null;

                _sprzet.Add(new Sprzet
                {
                    TypSprzetu = typ!,
                    NumerEwidencyjny = ne!,
                    Status = status,
                    DataZakupu = dz,
                    DataKoncaGwarancji = dg,
                    AdresIp = ip,
                    AdresMac = mac,
                    Uwagi = uw
                });

                inserted++;
            }
            catch (SqlException ex) when (IsDuplicate(ex))
            {
                skipped++;
            }
            catch
            {
                errors++;
            }
        }

        return new ImportSummary(inserted, skipped, errors);
    }


    private static bool IsDuplicate(SqlException ex)
        => ex.Number is 2601 or 2627;

    private static int FindCol(IReadOnlyList<string> headers, string name)
    {
        var idx = TryFindCol(headers, name);
        if (idx < 0)
            throw new InvalidOperationException($"CSV: brak wymaganej kolumny '{name}'.");
        return idx;
    }

    private static int TryFindCol(IReadOnlyList<string> headers, string name)
    {
        for (int i = 0; i < headers.Count; i++)
        {
            if (string.Equals(headers[i].Trim(), name, StringComparison.OrdinalIgnoreCase))
                return i;
        }
        return -1;
    }

    private static string? Get(IReadOnlyList<string> row, int idx)
    {
        if (idx < 0) return null;
        if (idx >= row.Count) return null;
        return row[idx];
    }

    private static string? ToNull(string? s)
        => string.IsNullOrWhiteSpace(s) ? null : s;

    private static int ParseInt(string? s)
    {
        if (int.TryParse((s ?? "").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var v)) return v;
        if (int.TryParse((s ?? "").Trim(), NumberStyles.Integer, CultureInfo.CurrentCulture, out v)) return v;
        return 0;
    }

    private static bool ParseBool(string? s, bool defaultValue)
    {
        if (string.IsNullOrWhiteSpace(s)) return defaultValue;

        s = s.Trim().ToLowerInvariant();
        if (s is "1" or "t" or "tak" or "true" or "yes") return true;
        if (s is "0" or "n" or "nie" or "false" or "no") return false;

        return defaultValue;
    }

    private static DateTime? ParseNullableDate(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        s = s.Trim();

        if (DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            return dt;
        if (DateTime.TryParse(s, CultureInfo.CurrentCulture, DateTimeStyles.None, out dt))
            return dt;
        if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            return dt;

        return null;
    }

    private static int ExtractLeadingInt(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return -1;
        s = s.Trim();
        int i = 0;
        while (i < s.Length && char.IsDigit(s[i])) i++;

        if (i == 0) return -1;

        var numStr = s[..i];
        return int.TryParse(numStr, out var v) ? v : -1;
    }
}
