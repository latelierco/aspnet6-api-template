using System.Diagnostics;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Template.AspNet6.Application.Services.Authentication;

namespace Template.AspNet6.DI.Logger;

public class ContextUserInjector : ITelemetryInitializer
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityProvider _identity;

    public ContextUserInjector(IHttpContextAccessor httpContextAccessor, IIdentityProvider identity)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _identity = identity;
    }

    public void Initialize(ITelemetry telemetry)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        telemetry.Context.Operation.Id = Activity.Current?.Id ?? httpContext?.TraceIdentifier;

        var userInfo = _identity.GetCurrentIdentity();

        if (userInfo is null) return;

        telemetry.Context.User.AuthenticatedUserId = $"{userInfo.Email}";
        telemetry.Context.User.Id = $"{userInfo.UserId}";
        telemetry.Context.User.AccountId = $"{userInfo.FirstName} {userInfo.LastName}";

        telemetry.Context.GlobalProperties["RequestClientIp"] = httpContext?.Connection?.RemoteIpAddress?.ToString();
    }
}
