using Template.AspNet6.Infra.Persistence.Blob;
using Microsoft.Extensions.DependencyInjection;
using Template.AspNet6.Application.Services.Persistence;

namespace Template.AspNet6.DI.Persistence;

public static class StorageConfiguration
{
    public static IServiceCollection AddStorage(this IServiceCollection services)
    {
        services.AddScoped<IStoreService, BlobStorageService>();

        return services;
    }
}
