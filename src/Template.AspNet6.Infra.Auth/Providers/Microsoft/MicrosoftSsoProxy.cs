using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using RestSharp;
using Template.AspNet6.Infra.Auth.Providers.Microsoft.Models;

namespace Template.AspNet6.Infra.Auth.Providers.Microsoft;

public class MicrosoftSsoProxy
{
    private readonly string _backOfficeBaseUrl;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _codeVerifier;
    private readonly string _graphBaseUrl;
    private readonly string _oAuthBaseUrl;
    private readonly string _scope;
    private readonly TelemetryClient _telemetry;

    private readonly string _tenantId;
    private RestClient? _restClient;

    public MicrosoftSsoProxy(IConfiguration config, TelemetryClient telemetry)
    {
        _backOfficeBaseUrl = config["Application:Backend:Url"] ?? throw new ArgumentNullException(nameof(_backOfficeBaseUrl));

        _oAuthBaseUrl = config["ExternalProviders:Microsoft:OAuthBaseUrl"] ?? throw new ArgumentNullException(nameof(_oAuthBaseUrl));
        _graphBaseUrl = config["ExternalProviders:Microsoft:GraphBaseUrl"] ?? throw new ArgumentNullException(nameof(_graphBaseUrl));
        _codeVerifier = config["ExternalProviders:Microsoft:Verifier"] ?? throw new ArgumentNullException(nameof(_codeVerifier));
        _clientId = config["ExternalProviders:Microsoft:ClientId"] ?? throw new ArgumentNullException(nameof(_clientId));
        _clientSecret = config["ExternalProviders:Microsoft:ClientSecret"] ?? throw new ArgumentNullException(nameof(_clientSecret));
        _scope = config["ExternalProviders:Microsoft:Scope"] ?? throw new ArgumentNullException(nameof(_scope));
        _tenantId = config["ExternalProviders:Microsoft:TenantId"] ?? throw new ArgumentNullException(nameof(_tenantId));

        _telemetry = telemetry;
    }

    public async Task<string> GetAccessTokenAsync(string code)
    {
        _restClient = new RestClient(_oAuthBaseUrl);

        var requestEndpoint = $"{_tenantId}/oauth2/v2.0/token";
        var request = new RestRequest(requestEndpoint, Method.Post);
        request.AddParameter("grant_type", "authorization_code", ParameterType.GetOrPost);
        request.AddParameter("scope", "https://graph.microsoft.com/user.read", ParameterType.GetOrPost);
        request.AddParameter("client_id", _clientId, ParameterType.GetOrPost);
        request.AddParameter("redirect_uri", $"{_backOfficeBaseUrl}/connexion", ParameterType.GetOrPost);
        request.AddParameter("code", code, ParameterType.GetOrPost);
        request.AddParameter("code_verifier", _codeVerifier, ParameterType.GetOrPost);

        request.AddHeader("Origin", $"{_backOfficeBaseUrl}");

        var response = await _restClient.ExecuteAsync<AccessToken>(request);
        if (!response.IsSuccessful)
        {
            var exception = new Exception("error occured retrieving user access token");
            _telemetry.TrackException(response.ErrorException ?? exception, new Dictionary<string, string>
            {
                {nameof(response.Content), $"{response.Content}"},
                {nameof(response.StatusCode), $"{response.StatusCode}"}
            });

            throw response.ErrorException ?? exception;
        }

        return response.Data!.access_token;
    }

    public async Task<UserProviderModel> GetIdentityInformationsAsync(string accessToken)
    {
        _restClient = new RestClient(_graphBaseUrl);

        var requestEndpoint = "v1.0/me";
        var request = new RestRequest(requestEndpoint);
        request.AddHeader("Authorization", $"Bearer {accessToken}");

        var response = await _restClient.ExecuteAsync<UserProviderModel>(request);
        if (!response.IsSuccessful)
        {
            var exception = new Exception("error occured retrieving user access token");
            _telemetry.TrackException(response.ErrorException ?? exception, new Dictionary<string, string>
            {
                {nameof(response.Content), $"{response.Content}"},
                {nameof(response.StatusCode), $"{response.StatusCode}"}
            });

            throw response.ErrorException ?? exception;
        }

        _telemetry.TrackTrace("Identity infos response.",
            new Dictionary<string, string>
            {
                {nameof(response.Content), $"{response.Content}"}
            });

        return response.Data!;
    }
}
