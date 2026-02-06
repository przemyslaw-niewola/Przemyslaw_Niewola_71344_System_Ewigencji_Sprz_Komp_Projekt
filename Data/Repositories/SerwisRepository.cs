using EwidencjaSprzetuOOP.Domain.Entities;

namespace EwidencjaSprzetuOOP.Data.Repositories;

public interface ISerwisRepository
{
    List<Serwis> GetLatest(int top = 30);
    int Add(Serwis s);
}

public sealed class SerwisRepository : ISerwisRepository
{
    private readonly ISqlConnectionFactory _factory;
    public SerwisRepository(ISqlConnectionFactory factory) => _factory = factory;

    public List<Serwis> GetLatest(int top = 30)
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = $@"
SELECT TOP ({top})
    SerwisId, SprzetId, DostawcaId, WykonanoDnia, RodzajSerwisu, Opis, Koszt
FROM dbo.Serwisy
ORDER BY WykonanoDnia DESC;";

        using var r = cmd.ExecuteReader();
        var list = new List<Serwis>();
        while (r.Read())
        {
            list.Add(new Serwis
            {
                SerwisId = r.GetInt32(0),
                SprzetId = r.GetInt32(1),
                DostawcaId = r.IsDBNull(2) ? null : r.GetInt32(2),
                WykonanoDnia = r.GetDateTime(3),
                RodzajSerwisu = r.GetString(4),
                Opis = r.IsDBNull(5) ? null : r.GetString(5),
                Koszt = r.IsDBNull(6) ? null : r.GetDecimal(6)
            });
        }
        return list;
    }

    public int Add(Serwis s)
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
INSERT INTO dbo.Serwisy(SprzetId,DostawcaId,WykonanoDnia,RodzajSerwisu,Opis,Koszt)
VALUES(@sid,@did,SYSDATETIME(),@rodz,@opis,@koszt);
SELECT SCOPE_IDENTITY();";
        cmd.Parameters.AddWithValue("@sid", s.SprzetId);
        cmd.Parameters.AddWithValue("@did", (object?)s.DostawcaId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@rodz", s.RodzajSerwisu);
        cmd.Parameters.AddWithValue("@opis", (object?)s.Opis ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@koszt", (object?)s.Koszt ?? DBNull.Value);

        return Convert.ToInt32(cmd.ExecuteScalar());
    }
}
