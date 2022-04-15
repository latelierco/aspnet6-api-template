using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using RestSharp;
using Template.AspNet6.Infra.Auth.Providers.Custom.Models;

namespace Template.AspNet6.Infra.Auth.Providers.Custom;

public class CustomOpenIdProxy
{
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _frontUrl;

    private readonly RestClient _restClient;
    private readonly TelemetryClient _telemetry;

    public CustomOpenIdProxy(IConfiguration config, TelemetryClient telemetry)
    {
        _telemetry = telemetry;

        _frontUrl = config["Application:Frontend:Url"] ?? throw new ArgumentNullException(nameof(_frontUrl));
        _clientId = config["OAuth:ExternalProviders:CustomOpenId:ClientId"] ?? throw new ArgumentNullException(nameof(_clientId));
        _clientSecret = config["OAuth:ExternalProviders:CustomOpenId:ClientSecret"] ?? throw new ArgumentNullException(nameof(_clientSecret));

        var baseUrl = config["OAuth:ExternalProviders:CustomOpenId:BaseUrl"];
        ArgumentNullException.ThrowIfNull(baseUrl);
        _restClient = new RestClient(baseUrl);
    }

    public async Task<string> GetAccessTokenAsync(string code)
    {
        var requestEndpoint = "access_token";
        var request = new RestRequest(requestEndpoint, Method.Post);
        request.AddParameter("grant_type", "authorization_code", ParameterType.GetOrPost);
        request.AddParameter("client_id", _clientId, ParameterType.GetOrPost);
        request.AddParameter("client_secret", _clientSecret, ParameterType.GetOrPost);
        request.AddParameter("redirect_uri", $"{_frontUrl}/login", ParameterType.GetOrPost);
        request.AddParameter("code", code, ParameterType.GetOrPost);

        var response = await _restClient.ExecuteAsync<AccessToken>(request);
        if (!response.IsSuccessful || response.Data is null)
        {
            var exception = new Exception("error occured retrieving user access token");
            _telemetry.TrackException(response.ErrorException ?? exception, new Dictionary<string, string>
            {
                {nameof(response.Content), $"{response.Content}"},
                {nameof(response.StatusCode), $"{response.StatusCode}"}
            });

            throw response.ErrorException ?? exception;
        }

        return response.Data.access_token;
    }

    public async Task<UserInfos> GetIdentityInformationsAsync(string accessToken)
    {
        var requestEndpoint = "userinfo";
        var request = new RestRequest(requestEndpoint);
        request.AddHeader("Authorization", $"Bearer {accessToken}");

        var response = await _restClient.ExecuteAsync<UserInfos>(request);
        if (!response.IsSuccessful || response.Data is null)
        {
            var exception = new Exception("error occured retrieving user info");
            _telemetry.TrackException(response.ErrorException ?? exception, new Dictionary<string, string>
            {
                {nameof(response.Content), $"{response.Content}"},
                {nameof(response.StatusCode), $"{response.StatusCode}"}
            });

            throw response.ErrorException ?? exception;
        }

        return response.Data;
    }
}
