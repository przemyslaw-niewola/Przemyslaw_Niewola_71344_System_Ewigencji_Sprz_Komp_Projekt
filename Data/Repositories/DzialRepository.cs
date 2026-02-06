using EwidencjaSprzetuOOP.Domain.Entities;
using Microsoft.Data.SqlClient;

namespace EwidencjaSprzetuOOP.Data.Repositories;

public interface IDzialRepository
{
    List<Dzial> GetAll();
    int Add(string nazwa);
    void Update(int dzialId, string nazwa);
    void Delete(int dzialId);
}

public sealed class DzialRepository : IDzialRepository
{
    private readonly ISqlConnectionFactory _factory;
    public DzialRepository(ISqlConnectionFactory factory) => _factory = factory;

    public List<Dzial> GetAll()
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT DzialId, Nazwa FROM dbo.Dzialy ORDER BY Nazwa";

        using var r = cmd.ExecuteReader();
        var list = new List<Dzial>();
        while (r.Read())
        {
            list.Add(new Dzial
            {
                DzialId = r.GetInt32(0),
                Nazwa = r.GetString(1)
            });
        }
        return list;
    }

    public int Add(string nazwa)
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO dbo.Dzialy(Nazwa) VALUES (@n); SELECT SCOPE_IDENTITY();";
        cmd.Parameters.AddWithValue("@n", nazwa);

        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void Update(int dzialId, string nazwa)
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE dbo.Dzialy SET Nazwa=@n WHERE DzialId=@id";
        cmd.Parameters.AddWithValue("@n", nazwa);
        cmd.Parameters.AddWithValue("@id", dzialId);

        cmd.ExecuteNonQuery();
    }

    public void Delete(int dzialId)
    {
        using var conn = _factory.Create();
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM dbo.Dzialy WHERE DzialId=@id";
        cmd.Parameters.AddWithValue("@id", dzialId);

        cmd.ExecuteNonQuery();
    }
}
