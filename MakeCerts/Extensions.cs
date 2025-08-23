using System.Text.RegularExpressions;

namespace MakeCerts;

internal static class Extensions
{
    /// <summary>
    ///  The original specification of hostnames in RFC 952, mandated that labels could not start with a digit or with a hyphen, and must not end with a hyphen. However, a subsequent specification (RFC 1123) permitted hostname labels to start with digits.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsHostName(this string value)
    {
        return Regex.IsMatch(value, @"^(([a-zA-Z]|[a-zA-Z][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z]|[A-Za-z][A-Za-z0-9\-]*[A-Za-z0-9])$");
    }

    public static bool IsIpV4(this string value)
    {
        return Regex.IsMatch(value, @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
    }

    public static string ToCertName(this string subject)
    {
        if (subject.StartsWith("*"))
            subject = subject.Replace("*", "_wildcard");

        return subject.ToLowerInvariant();
    }
}
