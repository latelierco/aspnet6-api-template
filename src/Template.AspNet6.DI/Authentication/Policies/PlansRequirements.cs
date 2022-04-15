using Template.AspNet6.Domain.Entities.Users.Plans;
using Microsoft.AspNetCore.Authorization;
using Template.AspNet6.Application.Services.Authentication;

namespace Template.AspNet6.DI.Authentication.Policies;

public class FreePlanRequirement : IAuthorizationRequirement { }
public class FreePlanRequirementHandler : AuthorizationHandler<FreePlanRequirement>
{
    private readonly IIdentityProvider _identity;

    public FreePlanRequirementHandler(IIdentityProvider identity)
    {
        _identity = identity;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, FreePlanRequirement requirement)
    {
        try
        {
            var identity = _identity.GetCurrentIdentity();
            if (identity != null && identity.IsActivated!.Value && (identity.HasPlan(CPlan.Free) || identity.HasPlan(CPlan.Dematerialization) || identity.HasPlan(CPlan.Dashboard)))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail(new AuthorizationFailureReason(this, "You have not paid the registration fee"));
            return Task.CompletedTask;
        }
        catch
        {
            context.Fail();
            return Task.CompletedTask;
        }
    }
}

public class DashboardPlanRequirement : IAuthorizationRequirement { }
public class DashboardPlanRequirementHandler : AuthorizationHandler<DashboardPlanRequirement>, IAuthorizationRequirement
{
    private readonly IIdentityProvider _identity;

    public DashboardPlanRequirementHandler(IIdentityProvider identity)
    {
        _identity = identity;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DashboardPlanRequirement requirement)
    {
        try
        {
            var identity = _identity.GetCurrentIdentity();
            if (identity != null && identity.IsActivated!.Value && identity.HasPlan(CPlan.Dashboard))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail(new AuthorizationFailureReason(this, "You do not have a valid dashboard plan"));
            return Task.CompletedTask;
        }
        catch
        {
            context.Fail();
            return Task.CompletedTask;
        }
    }
}

public class DematerializationPlanRequirement : IAuthorizationRequirement { }
public class DematerializationPlanRequirementHandler : AuthorizationHandler<DematerializationPlanRequirement>
{
    private readonly IIdentityProvider _identity;

    public DematerializationPlanRequirementHandler(IIdentityProvider identity)
    {
        _identity = identity;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DematerializationPlanRequirement requirement)
    {
        try
        {
            var identity = _identity.GetCurrentIdentity();
            if (identity != null && identity.IsActivated!.Value && identity.HasPlan(CPlan.Dematerialization))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail(new AuthorizationFailureReason(this, "You do not have a valid dematerialization plan"));
            return Task.CompletedTask;
        }
        catch
        {
            context.Fail();
            return Task.CompletedTask;
        }
    }
}
