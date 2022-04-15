namespace Template.AspNet6.Domain.ValueObjects.Address;

public class Address : IEquatable<Address>
{
    public const int CountryMaxLength = 64;
    public const int PostalCodeMaxLength = 8;
    public const int CityMaxLength = 64;
    public const int StreetMaxLength = 64;
    public const int ComplementMaxLength = 64;

    public Address(string country = "", string postalCode = "", string city = "", string street = "", string complement = "")
    {
        country = country?.Trim() ?? "";
        postalCode = postalCode?.Trim() ?? "";
        city = city?.Trim() ?? "";
        street = street?.Trim() ?? "";
        complement = complement?.Trim() ?? "";

        if (country.Length > CountryMaxLength)
            throw new AddressException($"Country code must not exceed {CountryMaxLength} characters.");
        if (postalCode.Length > PostalCodeMaxLength)
            throw new AddressException($"Postal code must not exceed {PostalCodeMaxLength} characters.");
        if (city.Length > CityMaxLength)
            throw new AddressException($"City must not exceed {CityMaxLength} characters.");
        if (street.Length > StreetMaxLength)
            throw new AddressException($"Street must not exceed {StreetMaxLength} characters.");
        if (complement.Length > ComplementMaxLength)
            throw new AddressException($"Complement must not exceed {ComplementMaxLength} characters.");

        CountryCodeFullname.TryGetValue(country, out var countryFullname);
        Country = countryFullname ?? country;
        PostalCode = postalCode;
        City = city;
        Street = street;
        Complement = complement;
    }

    public string Country { get; }
    public string PostalCode { get; }
    public string City { get; }
    public string Street { get; }
    public string Complement { get; }

    public bool IsEmpty()
        =>
            Country == string.Empty
            && PostalCode == string.Empty
            && City == string.Empty
            && Street == string.Empty
            && Complement == string.Empty;

    public bool Equals(Address? other)
    {
        return
            other is not null
            && Country == other.Country
            && PostalCode == other.PostalCode
            && City == other.City
            && Street == other.Street
            && Complement == other.Complement;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == this.GetType() && Equals((Address) obj);
    }

    public override int GetHashCode() => HashCode.Combine(Country, PostalCode, City, Street, Complement);

    private static readonly Dictionary<string, string> CountryCodeFullname = new() {{"FR", "FRANCE"}};
}
