using EwidencjaSprzetuOOP.Domain;
using EwidencjaSprzetuOOP.Domain.Entities;

namespace EwidencjaSprzetuOOP.Data.Repositories;

public interface ISprzetRepository
{
    List<Sprzet> GetAll();
    Sprzet? GetById(int id);
    int Add(Sprzet s);
    void Update(Sprzet s);
    void SetStatus(int sprzetId, StatusSprzetu status);
    void Delete(int sprzetId);
}

public sealed class SprzetRepository : ISprzetRepository
{
    private readonly ISqlConnectionFactory _factory;
    public SprzetRepository(ISqlConnectionFactory factory) => _factory = factory;

    public List<Sprzet> GetAll()
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
SELECT SprzetId, TypSprzetu, NumerEwidencyjny, NumerSeryjny, Status,
       DataZakupu, DataKoncaGwarancji, LokalizacjaId, DostawcaId, Uwagi,
       Procesor, RamGb, DyskGb, SystemOperacyjny, PrzekatnaCala,
       Obudowa, Rozdzielczosc, Kolorowa, AdresIp, AdresMac
FROM dbo.Sprzety
ORDER BY NumerEwidencyjny;";

        using var r = cmd.ExecuteReader();
        var list = new List<Sprzet>();
        while (r.Read())
        {
            list.Add(Map(r));
        }
        return list;
    }

    public Sprzet? GetById(int id)
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
SELECT SprzetId, TypSprzetu, NumerEwidencyjny, NumerSeryjny, Status,
       DataZakupu, DataKoncaGwarancji, LokalizacjaId, DostawcaId, Uwagi,
       Procesor, RamGb, DyskGb, SystemOperacyjny, PrzekatnaCala,
       Obudowa, Rozdzielczosc, Kolorowa, AdresIp, AdresMac
FROM dbo.Sprzety
WHERE SprzetId=@id;";
        cmd.Parameters.AddWithValue("@id", id);

        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;
        return Map(r);
    }

    public int Add(Sprzet s)
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
INSERT INTO dbo.Sprzety
(TypSprzetu, NumerEwidencyjny, NumerSeryjny, Status,
 DataZakupu, DataKoncaGwarancji, LokalizacjaId, DostawcaId, Uwagi,
 Procesor, RamGb, DyskGb, SystemOperacyjny, PrzekatnaCala,
 Obudowa, Rozdzielczosc, Kolorowa, AdresIp, AdresMac)
VALUES
(@Typ, @NE, @NS, @Status,
 @DZ, @DG, @LID, @DID, @Uw,
 @Proc, @Ram, @Dysk, @SO, @Prz,
 @Ob, @Roz, @Kol, @IP, @MAC);
SELECT SCOPE_IDENTITY();";

        FillParams(cmd, s);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void Update(Sprzet s)
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
UPDATE dbo.Sprzety
SET TypSprzetu=@Typ, NumerEwidencyjny=@NE, NumerSeryjny=@NS, Status=@Status,
    DataZakupu=@DZ, DataKoncaGwarancji=@DG, LokalizacjaId=@LID, DostawcaId=@DID, Uwagi=@Uw,
    Procesor=@Proc, RamGb=@Ram, DyskGb=@Dysk, SystemOperacyjny=@SO, PrzekatnaCala=@Prz,
    Obudowa=@Ob, Rozdzielczosc=@Roz, Kolorowa=@Kol, AdresIp=@IP, AdresMac=@MAC
WHERE SprzetId=@Id;";

        FillParams(cmd, s);
        cmd.Parameters.AddWithValue("@Id", s.SprzetId);
        cmd.ExecuteNonQuery();
    }

    public void SetStatus(int sprzetId, StatusSprzetu status)
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE dbo.Sprzety SET Status=@s WHERE SprzetId=@id";
        cmd.Parameters.AddWithValue("@s", (int)status);
        cmd.Parameters.AddWithValue("@id", sprzetId);
        cmd.ExecuteNonQuery();
    }

    public void Delete(int sprzetId)
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM dbo.Sprzety WHERE SprzetId=@id";
        cmd.Parameters.AddWithValue("@id", sprzetId);
        cmd.ExecuteNonQuery();
    }

    private static Sprzet Map(Microsoft.Data.SqlClient.SqlDataReader r)
    {
        return new Sprzet
        {
            SprzetId = r.GetInt32(0),
            TypSprzetu = r.GetString(1),
            NumerEwidencyjny = r.GetString(2),
            NumerSeryjny = r.IsDBNull(3) ? null : r.GetString(3),
            Status = (StatusSprzetu)r.GetInt32(4),

            DataZakupu = r.IsDBNull(5) ? null : r.GetDateTime(5),
            DataKoncaGwarancji = r.IsDBNull(6) ? null : r.GetDateTime(6),

            LokalizacjaId = r.IsDBNull(7) ? null : r.GetInt32(7),
            DostawcaId = r.IsDBNull(8) ? null : r.GetInt32(8),

            Uwagi = r.IsDBNull(9) ? null : r.GetString(9),

            Procesor = r.IsDBNull(10) ? null : r.GetString(10),
            RamGb = r.IsDBNull(11) ? null : r.GetInt32(11),
            DyskGb = r.IsDBNull(12) ? null : r.GetInt32(12),
            SystemOperacyjny = r.IsDBNull(13) ? null : r.GetString(13),

            PrzekatnaCala = r.IsDBNull(14) ? null : r.GetDecimal(14),
            Obudowa = r.IsDBNull(15) ? null : r.GetString(15),
            Rozdzielczosc = r.IsDBNull(16) ? null : r.GetString(16),
            Kolorowa = r.IsDBNull(17) ? null : r.GetBoolean(17),

            AdresIp = r.IsDBNull(18) ? null : r.GetString(18),
            AdresMac = r.IsDBNull(19) ? null : r.GetString(19),
        };
    }

    private static void FillParams(Microsoft.Data.SqlClient.SqlCommand cmd, Sprzet s)
    {
        cmd.Parameters.AddWithValue("@Typ", s.TypSprzetu);
        cmd.Parameters.AddWithValue("@NE", s.NumerEwidencyjny);
        cmd.Parameters.AddWithValue("@NS", (object?)s.NumerSeryjny ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Status", (int)s.Status);

        cmd.Parameters.AddWithValue("@DZ", (object?)s.DataZakupu ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DG", (object?)s.DataKoncaGwarancji ?? DBNull.Value);

        cmd.Parameters.AddWithValue("@LID", (object?)s.LokalizacjaId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DID", (object?)s.DostawcaId ?? DBNull.Value);

        cmd.Parameters.AddWithValue("@Uw", (object?)s.Uwagi ?? DBNull.Value);

        cmd.Parameters.AddWithValue("@Proc", (object?)s.Procesor ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Ram", (object?)s.RamGb ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Dysk", (object?)s.DyskGb ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@SO", (object?)s.SystemOperacyjny ?? DBNull.Value);

        cmd.Parameters.AddWithValue("@Prz", (object?)s.PrzekatnaCala ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Ob", (object?)s.Obudowa ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Roz", (object?)s.Rozdzielczosc ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Kol", (object?)s.Kolorowa ?? DBNull.Value);

        cmd.Parameters.AddWithValue("@IP", (object?)s.AdresIp ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@MAC", (object?)s.AdresMac ?? DBNull.Value);
    }
}
