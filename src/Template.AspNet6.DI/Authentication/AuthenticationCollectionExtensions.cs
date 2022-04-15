using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Template.AspNet6.Application.Services.Authentication;
using Template.AspNet6.DI.Authentication.Policies;
using Template.AspNet6.Domain.Entities.Users.Plans;
using Template.AspNet6.Infra.Auth;

namespace Template.AspNet6.DI.Authentication;

public static class AuthenticationCollectionExtensions
{
    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddTransient<IAuthenticator, Authenticator>();
        services.AddTransient<IIdentityProvider, IdentityProvider>();
        services.AddTransient<ITokenGenerator, TokenGenerator>();

        services.AddSingleton<IAuthorizationHandler, FreePlanRequirementHandler>();
        services.AddSingleton<IAuthorizationHandler, DashboardPlanRequirementHandler>();
        services.AddSingleton<IAuthorizationHandler, DematerializationPlanRequirementHandler>();

        services.AddAuthorization(opt =>
        {
            opt.AddPolicy(CPlan.Free, policy => policy.Requirements.Add(new FreePlanRequirement()));
            opt.AddPolicy(CPlan.Dematerialization, policy => policy.Requirements.Add(new DematerializationPlanRequirement()));
            opt.AddPolicy(CPlan.Dashboard, policy => policy.Requirements.Add(new DashboardPlanRequirement()));
        });

        var builder = services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        });

        builder.AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["OAuth:BuiltIn:ClientSecret"])),
                ValidIssuer = configuration["OAuth:BuiltIn:Issuer"],
                ValidAudience = configuration["OAuth:BuiltIn:Audience"]
            };
        });

        return services;
    }
}