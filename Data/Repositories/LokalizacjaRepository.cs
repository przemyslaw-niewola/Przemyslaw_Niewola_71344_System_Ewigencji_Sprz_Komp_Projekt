using EwidencjaSprzetuOOP.Domain.Entities;

namespace EwidencjaSprzetuOOP.Data.Repositories;

public interface ILokalizacjaRepository
{
    List<Lokalizacja> GetAll();
    int Add(Lokalizacja l);
    void Update(Lokalizacja l);
    void Delete(int id);
}

public sealed class LokalizacjaRepository : ILokalizacjaRepository
{
    private readonly ISqlConnectionFactory _factory;
    public LokalizacjaRepository(ISqlConnectionFactory factory) => _factory = factory;

    public List<Lokalizacja> GetAll()
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT LokalizacjaId, Nazwa, Adres FROM dbo.Lokalizacje ORDER BY Nazwa";

        using var r = cmd.ExecuteReader();
        var list = new List<Lokalizacja>();
        while (r.Read())
        {
            list.Add(new Lokalizacja
            {
                LokalizacjaId = r.GetInt32(0),
                Nazwa = r.GetString(1),
                Adres = r.IsDBNull(2) ? null : r.GetString(2)
            });
        }
        return list;
    }

    public int Add(Lokalizacja l)
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
INSERT INTO dbo.Lokalizacje(Nazwa,Adres) VALUES(@n,@a);
SELECT SCOPE_IDENTITY();";
        cmd.Parameters.AddWithValue("@n", l.Nazwa);
        cmd.Parameters.AddWithValue("@a", (object?)l.Adres ?? DBNull.Value);

        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void Update(Lokalizacja l)
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE dbo.Lokalizacje SET Nazwa=@n, Adres=@a WHERE LokalizacjaId=@id";
        cmd.Parameters.AddWithValue("@n", l.Nazwa);
        cmd.Parameters.AddWithValue("@a", (object?)l.Adres ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@id", l.LokalizacjaId);

        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM dbo.Lokalizacje WHERE LokalizacjaId=@id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }
}
