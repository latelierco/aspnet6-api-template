using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.AspNet6.Application.Services.Persistence;
using Template.AspNet6.Infra.Persistence.SqlServer;

namespace Template.AspNet6.DI.Persistence;

public static class DatabaseConfiguration
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<Context>(options =>
            options.UseLazyLoadingProxies().UseSqlServer(config.GetConnectionString("Database"),
                o => { o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery); }
            ));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.ConfigureRepositories();

        return services;
    }

    public static IApplicationBuilder UpdateDatabase(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope();
        using var context = serviceScope.ServiceProvider.GetService<Context>();
        context?.Database.Migrate();

        return app;
    }
}
