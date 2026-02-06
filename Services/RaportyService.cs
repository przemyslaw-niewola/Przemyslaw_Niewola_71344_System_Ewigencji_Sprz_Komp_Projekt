using Microsoft.Data.SqlClient;
using EwidencjaSprzetuOOP.Data;

namespace EwidencjaSprzetuOOP.Services;

public sealed class RaportyService
{
    private readonly ISqlConnectionFactory _factory;
    public RaportyService(ISqlConnectionFactory factory) => _factory = factory;

    public List<(string NumerEw, string Typ, int Status)> SprzetPoStatusie(int status)
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
SELECT NumerEwidencyjny, TypSprzetu, Status
FROM dbo.Sprzety
WHERE Status=@s
ORDER BY NumerEwidencyjny;";
        cmd.Parameters.AddWithValue("@s", status);

        using var r = cmd.ExecuteReader();
        var list = new List<(string, string, int)>();
        while (r.Read())
            list.Add((r.GetString(0), r.GetString(1), r.GetInt32(2)));

        return list;
    }

    public List<(string NumerEw, string Typ, DateTime DataGwar)> GwarancjaWygasaW(int dni)
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
SELECT NumerEwidencyjny, TypSprzetu, DataKoncaGwarancji
FROM dbo.Sprzety
WHERE DataKoncaGwarancji IS NOT NULL
  AND DataKoncaGwarancji <= DATEADD(DAY, @dni, CAST(GETDATE() AS DATE))
ORDER BY DataKoncaGwarancji;";
        cmd.Parameters.AddWithValue("@dni", dni);

        using var r = cmd.ExecuteReader();
        var list = new List<(string, string, DateTime)>();
        while (r.Read())
            list.Add((r.GetString(0), r.GetString(1), r.GetDateTime(2)));

        return list;
    }

    public List<(int SprzetId, decimal SumaKosztow)> KosztySerwisowNaSprzet()
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
SELECT SprzetId, SUM(ISNULL(Koszt,0)) AS Suma
FROM dbo.Serwisy
GROUP BY SprzetId
ORDER BY Suma DESC;";

        using var r = cmd.ExecuteReader();
        var list = new List<(int, decimal)>();
        while (r.Read())
            list.Add((r.GetInt32(0), r.GetDecimal(1)));

        return list;
    }
}
