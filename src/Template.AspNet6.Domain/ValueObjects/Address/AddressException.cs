namespace Template.AspNet6.Domain.ValueObjects.Address;

public class AddressException : Exception
{
    private const string PublicMessage = "Address is not valid.";

    public AddressException(string message = PublicMessage) : base(message) { }

    public AddressException(Exception innerException, string message = PublicMessage) : base(message, innerException) { }
}