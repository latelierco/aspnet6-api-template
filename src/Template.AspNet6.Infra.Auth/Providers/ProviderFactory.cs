using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Template.AspNet6.Domain.Entities.Users;
using Template.AspNet6.Domain.ValueObjects.Email;
using Template.AspNet6.Infra.Auth.Providers.Custom;
using Template.AspNet6.Infra.Auth.Providers.Google;
using Template.AspNet6.Infra.Auth.Providers.Microsoft;

namespace Template.AspNet6.Infra.Auth.Providers;

public class ProviderFactory
{
    private readonly IConfiguration _config;
    private readonly TelemetryClient _telemetry;
    private readonly IUserFactory _userFactory;

    public ProviderFactory(IConfiguration config, IUserFactory userFactory, TelemetryClient telemetry)
    {
        _config = config;
        _userFactory = userFactory;
        _telemetry = telemetry;
    }

    public IProvider Create(ProviderType providerType)
    {
        return providerType switch
        {
            ProviderType.Custom => new CustomSsoProvider(_config, _userFactory, _telemetry),
            ProviderType.Google => new GoogleSsoProvider(_config, _userFactory, _telemetry),
            ProviderType.Microsoft => new MicrosoftSsoProvider(_config, _userFactory, _telemetry),
            _ => throw new ArgumentOutOfRangeException(nameof(providerType), providerType, null)
        };
    }
}

public interface IProvider
{
    Task<User> GetUserAsync(string code);
}

public class CustomSsoProvider : IProvider
{
    private readonly CustomOpenIdProxy _proxy;
    private readonly IUserFactory _userFactory;

    public CustomSsoProvider(IConfiguration config, IUserFactory userFactory, TelemetryClient telemetry)
    {
        _userFactory = userFactory;
        _proxy = new CustomOpenIdProxy(config, telemetry);
    }

    public async Task<User> GetUserAsync(string code)
    {
        var accessToken = await _proxy.GetAccessTokenAsync(code);
        var userInfo = await _proxy.GetIdentityInformationsAsync(accessToken);

        var user = _userFactory.NewActivatedUser(userInfo.firstname, userInfo.lastname, new Email(userInfo.emailcontact));
        return user;
    }
}

public class GoogleSsoProvider : IProvider
{
    private readonly GoogleAuthProxy _proxy;
    private readonly IUserFactory _userFactory;

    public GoogleSsoProvider(IConfiguration config, IUserFactory userFactory, TelemetryClient telemetry)
    {
        _userFactory = userFactory;
        _proxy = new GoogleAuthProxy(config, telemetry);
    }

    public async Task<User> GetUserAsync(string code)
    {
        var userInfo = await _proxy.GetUserInfoAsync(code);

        var user = _userFactory.NewActivatedUser(userInfo.given_name, userInfo.family_name, new Email(userInfo.email), userInfo.picture);
        return user;
    }
}

public class MicrosoftSsoProvider : IProvider
{
    private readonly MicrosoftSsoProxy _proxy;
    private readonly IUserFactory _userFactory;

    public MicrosoftSsoProvider(IConfiguration config, IUserFactory userFactory, TelemetryClient telemetry)
    {
        _userFactory = userFactory;
        _proxy = new MicrosoftSsoProxy(config, telemetry);
    }

    public async Task<User> GetUserAsync(string code)
    {
        var acessToken = await _proxy.GetAccessTokenAsync(code);
        var userInfo = await _proxy.GetIdentityInformationsAsync(acessToken);

        var user = _userFactory.NewActivatedUser(userInfo.Surname, userInfo.GivenName, new Email(userInfo.Mail));
        return user;
    }
}
