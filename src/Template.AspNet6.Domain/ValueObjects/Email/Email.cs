using System.Globalization;
using System.Text.RegularExpressions;

namespace Template.AspNet6.Domain.ValueObjects.Email;

public class Email : IEquatable<Email>
{
    public const int EmailMaxLength = 255;

    public Email(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new EmailException();

        Value = email.Trim();

        if (Value.Length >= EmailMaxLength || !IsValidEmail(Value))
            throw new EmailException();
    }

    public string Value { get; }

    public bool Equals(Email? other)
    {
        return Value == other?.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is Email o && Equals(o);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Value);
    }

    public static bool operator ==(Email? left, Email? right)
    {
        return (left is null && right is null) || (left is not null && left.Equals(right));
    }

    public static bool operator !=(Email left, Email right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return Value;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));

            string DomainMapper(Match match)
            {
                var idn = new IdnMapping();

                var domainName = idn.GetAscii(match.Groups[2].Value);

                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }

        try
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }
}