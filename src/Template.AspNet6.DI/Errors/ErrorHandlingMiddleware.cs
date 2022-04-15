using System.Diagnostics;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Template.AspNet6.DI.Errors;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, TelemetryClient logger)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
            var errorObject = new ProblemDetails { Type = "Internal Server Error", Title = "Oops.. Something bad happened here.... 😈 mouahahah", Instance = traceId, Status = StatusCodes.Status500InternalServerError };
            logger.TrackException(ex);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(errorObject));
        }
    }
}
