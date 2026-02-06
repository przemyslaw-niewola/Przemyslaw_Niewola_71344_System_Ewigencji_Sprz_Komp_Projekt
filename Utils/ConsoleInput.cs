namespace EwidencjaSprzetuOOP.Utils;

public static class ConsoleInput
{
    public static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            if (int.TryParse(s, out var v)) return v;
            Console.WriteLine("Błąd: wpisz liczbę całkowitą.");
        }
    }

    public static int? ReadNullableInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(s)) return null;
            if (int.TryParse(s, out var v)) return v;
            Console.WriteLine("Błąd: wpisz liczbę całkowitą albo zostaw puste.");
        }
    }

    public static decimal? ReadNullableDecimal(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(s)) return null;
            if (decimal.TryParse(s, out var v)) return v;
            Console.WriteLine("Błąd: wpisz liczbę (np. 123.45) albo zostaw puste.");
        }
    }

    public static string ReadRequired(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(s)) return s.Trim();
            Console.WriteLine("Błąd: pole wymagane.");
        }
    }

    public static string? ReadOptional(string prompt)
    {
        Console.Write(prompt);
        var s = Console.ReadLine();
        return string.IsNullOrWhiteSpace(s) ? null : s.Trim();
    }

    public static bool ReadYesNo(string prompt)
    {
        while (true)
        {
            Console.Write(prompt + " (T/N): ");
            var s = Console.ReadLine()?.Trim().ToUpperInvariant();
            if (s == "T" || s == "TAK") return true;
            if (s == "N" || s == "NIE") return false;
            Console.WriteLine("Wpisz T albo N.");
        }
    }
}
