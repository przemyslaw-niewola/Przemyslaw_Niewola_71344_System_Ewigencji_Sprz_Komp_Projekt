using System.Text;

namespace EwidencjaSprzetuOOP.Utils;

public static class CsvExporter
{
    public static string Export(
        string filePath,
        IReadOnlyList<string> headers,
        IEnumerable<IReadOnlyList<string?>> rows,
        char separator = ';')
    {
        var dir = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(dir))
            Directory.CreateDirectory(dir);

        using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
        using var writer = new StreamWriter(fs, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

        writer.WriteLine(string.Join(separator, headers.Select(h => Escape(h, separator))));

        foreach (var row in rows)
        {
            var line = string.Join(separator, row.Select(v => Escape(v ?? "", separator)));
            writer.WriteLine(line);
        }

        writer.Flush();
        return Path.GetFullPath(filePath);
    }

    private static string Escape(string value, char sep)
    {
        var mustQuote =
            value.Contains(sep) ||
            value.Contains('"') ||
            value.Contains('\n') ||
            value.Contains('\r');

        if (!mustQuote) return value;

        var escaped = value.Replace("\"", "\"\"");
        return $"\"{escaped}\"";
    }
}
