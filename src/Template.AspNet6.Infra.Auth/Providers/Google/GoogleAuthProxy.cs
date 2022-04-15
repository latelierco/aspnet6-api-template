using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using RestSharp;
using Template.AspNet6.Infra.Auth.Providers.Google.Models;

namespace Template.AspNet6.Infra.Auth.Providers.Google;

public class GoogleAuthProxy
{
    private readonly RestClient _restClient;
    private readonly TelemetryClient _telemetry;

    public GoogleAuthProxy(IConfiguration configuration, TelemetryClient telemetry)
    {
        _telemetry = telemetry;
        var baseUrl = configuration["OAuth:ExternalProviders:Google:BaseUrl"] ?? throw new ArgumentNullException();
        _restClient = new RestClient(baseUrl);
    }

    public async Task<UserInfos> GetUserInfoAsync(string accessToken)
    {
        var request = new RestRequest();
        request.AddHeader("Authorization", $"Bearer {accessToken}");
        request.Resource = "userinfo";

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
