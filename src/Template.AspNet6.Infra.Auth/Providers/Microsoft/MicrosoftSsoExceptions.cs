namespace Template.AspNet6.Infra.Auth.Providers.Microsoft;

public class MicrosoftSsoExceptions : Exception
{
    public MicrosoftSsoExceptions(string message) : base(message) { }
    public MicrosoftSsoExceptions(string message, Exception innerException) : base(message, innerException) { }
}

public class AccessTokenException : MicrosoftSsoExceptions
{
    private const string PublicMessage = "Retrieving access token failed.";

    public AccessTokenException(string message = PublicMessage) : base(message) { }
    public AccessTokenException(Exception innerException, string message = PublicMessage) : base(message, innerException) { }
}

public class AccessUserInfosException : MicrosoftSsoExceptions
{
    private const string PublicMessage = "Retrieving user informations failed.";

    public AccessUserInfosException(string message = PublicMessage) : base(message) { }
    public AccessUserInfosException(Exception innerException, string message = PublicMessage) : base(message, innerException) { }
}
