namespace KppBlazor;

public static class StringExtensions
{
    /// <summary>Возвращает null если строка пустая или whitespace.</summary>
    public static string? NullIfEmpty(this string? s) =>
        string.IsNullOrWhiteSpace(s) ? null : s;
}