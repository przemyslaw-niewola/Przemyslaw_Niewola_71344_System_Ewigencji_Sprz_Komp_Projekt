using EwidencjaSprzetuOOP.Domain.Entities;

namespace EwidencjaSprzetuOOP.Data.Repositories;

public interface IPracownikRepository
{
    List<Pracownik> GetAll();
    int Add(Pracownik p);
    void Update(Pracownik p);
    void SetActive(int pracownikId, bool aktywny);
    Pracownik? GetById(int id);
}

public sealed class PracownikRepository : IPracownikRepository
{
    private readonly ISqlConnectionFactory _factory;
    public PracownikRepository(ISqlConnectionFactory factory) => _factory = factory;

    public List<Pracownik> GetAll()
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
SELECT PracownikId, DzialId, Imie, Nazwisko, Email, Telefon, Aktywny
FROM dbo.Pracownicy
ORDER BY Nazwisko, Imie;";

        using var r = cmd.ExecuteReader();
        var list = new List<Pracownik>();
        while (r.Read())
        {
            list.Add(new Pracownik
            {
                PracownikId = r.GetInt32(0),
                DzialId = r.GetInt32(1),
                Imie = r.GetString(2),
                Nazwisko = r.GetString(3),
                Email = r.GetString(4),
                Telefon = r.IsDBNull(5) ? null : r.GetString(5),
                Aktywny = r.GetBoolean(6),
            });
        }
        return list;
    }

    public Pracownik? GetById(int id)
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
SELECT PracownikId, DzialId, Imie, Nazwisko, Email, Telefon, Aktywny
FROM dbo.Pracownicy WHERE PracownikId=@id";
        cmd.Parameters.AddWithValue("@id", id);

        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;

        return new Pracownik
        {
            PracownikId = r.GetInt32(0),
            DzialId = r.GetInt32(1),
            Imie = r.GetString(2),
            Nazwisko = r.GetString(3),
            Email = r.GetString(4),
            Telefon = r.IsDBNull(5) ? null : r.GetString(5),
            Aktywny = r.GetBoolean(6),
        };
    }

    public int Add(Pracownik p)
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
INSERT INTO dbo.Pracownicy(DzialId,Imie,Nazwisko,Email,Telefon,Aktywny)
VALUES(@d,@i,@n,@e,@t,@a);
SELECT SCOPE_IDENTITY();";
        cmd.Parameters.AddWithValue("@d", p.DzialId);
        cmd.Parameters.AddWithValue("@i", p.Imie);
        cmd.Parameters.AddWithValue("@n", p.Nazwisko);
        cmd.Parameters.AddWithValue("@e", p.Email);
        cmd.Parameters.AddWithValue("@t", (object?)p.Telefon ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@a", p.Aktywny);

        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void Update(Pracownik p)
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
UPDATE dbo.Pracownicy
SET DzialId=@d, Imie=@i, Nazwisko=@n, Email=@e, Telefon=@t, Aktywny=@a
WHERE PracownikId=@id";
        cmd.Parameters.AddWithValue("@d", p.DzialId);
        cmd.Parameters.AddWithValue("@i", p.Imie);
        cmd.Parameters.AddWithValue("@n", p.Nazwisko);
        cmd.Parameters.AddWithValue("@e", p.Email);
        cmd.Parameters.AddWithValue("@t", (object?)p.Telefon ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@a", p.Aktywny);
        cmd.Parameters.AddWithValue("@id", p.PracownikId);

        cmd.ExecuteNonQuery();
    }

    public void SetActive(int pracownikId, bool aktywny)
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE dbo.Pracownicy SET Aktywny=@a WHERE PracownikId=@id";
        cmd.Parameters.AddWithValue("@a", aktywny);
        cmd.Parameters.AddWithValue("@id", pracownikId);
        cmd.ExecuteNonQuery();
    }
}
