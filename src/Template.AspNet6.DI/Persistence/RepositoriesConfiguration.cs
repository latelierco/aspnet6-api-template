using Template.AspNet6.Domain.Entities.Users;
using Template.AspNet6.Domain.Entities.Users.Claims;
using Microsoft.Extensions.DependencyInjection;
using Template.AspNet6.Infra.Persistence.SqlServer.Users;
using Template.AspNet6.Infra.Persistence.SqlServer.Users.Claims;

namespace Template.AspNet6.DI.Persistence;

public static class RepositoriesConfiguration
{
    public static IServiceCollection ConfigureRepositories(this IServiceCollection services)
    {
        //USERS
        services.AddScoped<IUserFactory, UserFactory>();
        services.AddScoped<IReadUserRepository, UserRepository>();
        services.AddScoped<IWriteUserRepository, UserRepository>();

        //CLAIMS
        services.AddScoped<IClaimReadRepository, ClaimRepository>();
        services.AddScoped<IClaimWriteRepository, ClaimRepository>();

        return services;
    }
}
