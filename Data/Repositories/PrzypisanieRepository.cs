using EwidencjaSprzetuOOP.Domain.Entities;

namespace EwidencjaSprzetuOOP.Data.Repositories;

public interface IPrzypisanieRepository
{
    List<Przypisanie> GetActive();
    int Add(Przypisanie p);
    void CloseAssignment(int przypisanieId);
    Przypisanie? GetActiveBySprzetId(int sprzetId);
}

public sealed class PrzypisanieRepository : IPrzypisanieRepository
{
    private readonly ISqlConnectionFactory _factory;
    public PrzypisanieRepository(ISqlConnectionFactory factory) => _factory = factory;

    public List<Przypisanie> GetActive()
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
SELECT PrzypisanieId, SprzetId, PracownikId, PrzypisanoDnia, ZwroconoDnia, Uwagi
FROM dbo.Przypisania
WHERE ZwroconoDnia IS NULL
ORDER BY PrzypisanoDnia DESC;";

        using var r = cmd.ExecuteReader();
        var list = new List<Przypisanie>();
        while (r.Read())
        {
            list.Add(new Przypisanie
            {
                PrzypisanieId = r.GetInt32(0),
                SprzetId = r.GetInt32(1),
                PracownikId = r.GetInt32(2),
                PrzypisanoDnia = r.GetDateTime(3),
                ZwroconoDnia = r.IsDBNull(4) ? null : r.GetDateTime(4),
                Uwagi = r.IsDBNull(5) ? null : r.GetString(5),
            });
        }
        return list;
    }

    public Przypisanie? GetActiveBySprzetId(int sprzetId)
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
SELECT TOP 1 PrzypisanieId, SprzetId, PracownikId, PrzypisanoDnia, ZwroconoDnia, Uwagi
FROM dbo.Przypisania
WHERE SprzetId=@sid AND ZwroconoDnia IS NULL
ORDER BY PrzypisanoDnia DESC;";
        cmd.Parameters.AddWithValue("@sid", sprzetId);

        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;

        return new Przypisanie
        {
            PrzypisanieId = r.GetInt32(0),
            SprzetId = r.GetInt32(1),
            PracownikId = r.GetInt32(2),
            PrzypisanoDnia = r.GetDateTime(3),
            ZwroconoDnia = r.IsDBNull(4) ? null : r.GetDateTime(4),
            Uwagi = r.IsDBNull(5) ? null : r.GetString(5),
        };
    }

    public int Add(Przypisanie p)
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
INSERT INTO dbo.Przypisania(SprzetId,PracownikId,PrzypisanoDnia,ZwroconoDnia,Uwagi)
VALUES(@s,@p,SYSDATETIME(),NULL,@u);
SELECT SCOPE_IDENTITY();";
        cmd.Parameters.AddWithValue("@s", p.SprzetId);
        cmd.Parameters.AddWithValue("@p", p.PracownikId);
        cmd.Parameters.AddWithValue("@u", (object?)p.Uwagi ?? DBNull.Value);

        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void CloseAssignment(int przypisanieId)
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE dbo.Przypisania SET ZwroconoDnia=SYSDATETIME() WHERE PrzypisanieId=@id";
        cmd.Parameters.AddWithValue("@id", przypisanieId);
        cmd.ExecuteNonQuery();
    }
}
