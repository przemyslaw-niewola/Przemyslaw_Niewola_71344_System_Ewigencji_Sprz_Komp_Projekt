using EwidencjaSprzetuOOP.Domain.Entities;

namespace EwidencjaSprzetuOOP.Data.Repositories;

public interface IDostawcaRepository
{
    List<Dostawca> GetAll();
    int Add(Dostawca d);
    void Update(Dostawca d);
    void Delete(int id);
}

public sealed class DostawcaRepository : IDostawcaRepository
{
    private readonly ISqlConnectionFactory _factory;
    public DostawcaRepository(ISqlConnectionFactory factory) => _factory = factory;

    public List<Dostawca> GetAll()
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT DostawcaId, Nazwa, Email, Telefon FROM dbo.Dostawcy ORDER BY Nazwa";

        using var r = cmd.ExecuteReader();
        var list = new List<Dostawca>();
        while (r.Read())
        {
            list.Add(new Dostawca
            {
                DostawcaId = r.GetInt32(0),
                Nazwa = r.GetString(1),
                Email = r.IsDBNull(2) ? null : r.GetString(2),
                Telefon = r.IsDBNull(3) ? null : r.GetString(3),
            });
        }
        return list;
    }

    public int Add(Dostawca d)
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
INSERT INTO dbo.Dostawcy(Nazwa,Email,Telefon) VALUES(@n,@e,@t);
SELECT SCOPE_IDENTITY();";
        cmd.Parameters.AddWithValue("@n", d.Nazwa);
        cmd.Parameters.AddWithValue("@e", (object?)d.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@t", (object?)d.Telefon ?? DBNull.Value);

        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void Update(Dostawca d)
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE dbo.Dostawcy SET Nazwa=@n, Email=@e, Telefon=@t WHERE DostawcaId=@id";
        cmd.Parameters.AddWithValue("@n", d.Nazwa);
        cmd.Parameters.AddWithValue("@e", (object?)d.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@t", (object?)d.Telefon ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@id", d.DostawcaId);

        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM dbo.Dostawcy WHERE DostawcaId=@id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }
}
