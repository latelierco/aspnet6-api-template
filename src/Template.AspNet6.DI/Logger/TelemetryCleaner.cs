using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Template.AspNet6.DI.Logger;

public class TelemetryCleaner : ITelemetryProcessor
{
    private readonly List<string> _unwantedCategoryNameTelemetry = new()
    {
        "Microsoft.AspNetCore.Hosting.Internal.WebHost",
        "Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker",
        "Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor",
        "Microsoft.AspNetCore.Mvc.StatusCodeResult",
        "Microsoft.AspNetCore.Cors.Infrastructure.CorsService"
    };

    public TelemetryCleaner(ITelemetryProcessor next) => Next = next;

    private ITelemetryProcessor Next { get; }

    public void Process(ITelemetry item)
    {
        if (ShouldIgnoreRequest(item) || ShouldIgnoreTrace(item)) return;

        Next.Process(item);
    }

    private bool ShouldIgnoreRequest(ITelemetry item)
    {
        if (!string.IsNullOrWhiteSpace(item.Context.Operation.Name))
        {
            var operationName = item.Context.Operation.Name.ToLower();

            if (operationName.Equals("GET /", StringComparison.InvariantCultureIgnoreCase) ||
                operationName.Equals("GET /swagger/index.html", StringComparison.InvariantCultureIgnoreCase) ||
                operationName.Contains("GET /robots", StringComparison.InvariantCultureIgnoreCase) ||
                operationName.Contains("OPTIONS", StringComparison.InvariantCultureIgnoreCase))
                return item is RequestTelemetry req &&
                       (req.ResponseCode.Equals("200", StringComparison.OrdinalIgnoreCase) ||
                        req.ResponseCode.Equals("301", StringComparison.OrdinalIgnoreCase) ||
                        req.ResponseCode.Equals("404", StringComparison.OrdinalIgnoreCase) ||
                        req.ResponseCode.Equals("307", StringComparison.OrdinalIgnoreCase));

            if (item is TraceTelemetry trace && trace.Properties.Keys.Contains("CategoryName"))
            {
                if (TelemetryContainsUnwantedCategoryNameProperty(trace))
                    return true;

                if (TelemetryContainsDbCommands(trace))
                    return true;
            }

            return false;
        }

        return false;
    }

    private bool ShouldIgnoreTrace(ITelemetry item)
    {
        if (item is TraceTelemetry {SeverityLevel: SeverityLevel.Verbose})
            return true;

        return false;
    }

    private bool TelemetryContainsUnwantedCategoryNameProperty(TraceTelemetry trace)
    {
        if (_unwantedCategoryNameTelemetry.Contains(trace.Properties["CategoryName"]))
            return true;

        return false;
    }

    private bool TelemetryContainsDbCommands(TraceTelemetry trace)
    {
        if (!trace.Properties.Keys.Contains("AspNetCoreEnvironment"))
            return false;

        if (trace.Properties["AspNetCoreEnvironment"] == "Production"
            && trace.Properties["CategoryName"] == "Microsoft.EntityFrameworkCore.Database.Command")
            return true;

        return false;
    }
}
