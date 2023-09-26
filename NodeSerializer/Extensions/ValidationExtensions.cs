namespace NodeSerializer.Extensions;

public static class ValidationExtensions
{
    public static string EnsureNotNull(this string? str) =>
        string.IsNullOrEmpty(str) ? throw new ArgumentNullException(nameof(str)) : str;
}