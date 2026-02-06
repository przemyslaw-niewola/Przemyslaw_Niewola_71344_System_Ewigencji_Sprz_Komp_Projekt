using System.Text;

namespace EwidencjaSprzetuOOP.Utils;

public static class CsvImporter
{
    public sealed record CsvData(IReadOnlyList<string> Headers, IReadOnlyList<IReadOnlyList<string>> Rows);

    public static CsvData Read(string filePath, char separator = ';', bool skipEmptyLines = true)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Nie znaleziono pliku CSV: " + filePath);

        using var sr = new StreamReader(filePath, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), detectEncodingFromByteOrderMarks: true);

        string? headerLine = sr.ReadLine();
        if (headerLine is null)
            throw new InvalidOperationException("Plik CSV jest pusty.");

        var headers = ParseLine(headerLine, separator);

        var rows = new List<IReadOnlyList<string>>();
        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine();
            if (line is null) break;

            if (skipEmptyLines && string.IsNullOrWhiteSpace(line))
                continue;

            var row = ParseLine(line, separator);
            rows.Add(row);
        }

        return new CsvData(headers, rows);
    }

    private static List<string> ParseLine(string line, char separator)
    {
        var result = new List<string>();
        var sb = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    sb.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
                continue;
            }

            if (c == separator && !inQuotes)
            {
                result.Add(sb.ToString());
                sb.Clear();
                continue;
            }

            sb.Append(c);
        }

        result.Add(sb.ToString());
        return result;
    }
}
