using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace EwidencjaSprzetuOOP.Data;

public static class DbBootstrapper
{

    public static void EnsureDatabaseExists(string appConnectionString, string sqlScriptPath)
    {
        if (!File.Exists(sqlScriptPath))
            throw new FileNotFoundException("Brak pliku skryptu bazy: " + sqlScriptPath);

        var appSb = new SqlConnectionStringBuilder(appConnectionString);
        var dbName = appSb.InitialCatalog;

        if (string.IsNullOrWhiteSpace(dbName))
            throw new InvalidOperationException("Connection string musi mieć ustawione Database/Initial Catalog.");

        var masterSb = new SqlConnectionStringBuilder(appConnectionString)
        {
            InitialCatalog = "master"
        };

        using var conn = new SqlConnection(masterSb.ConnectionString);
        conn.Open();

        using (var check = conn.CreateCommand())
        {
            check.CommandText = "SELECT 1 FROM sys.databases WHERE name = @n";
            check.Parameters.AddWithValue("@n", dbName);
            var exists = check.ExecuteScalar() is not null;
            if (exists) return;
        }

        var sql = File.ReadAllText(sqlScriptPath, Encoding.UTF8);

        foreach (var batch in SplitOnGo(sql))
        {
            var b = batch.Trim();
            if (string.IsNullOrWhiteSpace(b)) continue;

            using var cmd = conn.CreateCommand();
            cmd.CommandText = b;
            cmd.CommandTimeout = 120;
            cmd.ExecuteNonQuery();
        }
    }

    private static IEnumerable<string> SplitOnGo(string sql)
    {
        var sb = new StringBuilder();
        using var sr = new StringReader(sql);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            if (Regex.IsMatch(line, @"^\s*GO\s*$", RegexOptions.IgnoreCase))
            {
                yield return sb.ToString();
                sb.Clear();
            }
            else
            {
                sb.AppendLine(line);
            }
        }

        if (sb.Length > 0)
            yield return sb.ToString();
    }
}
