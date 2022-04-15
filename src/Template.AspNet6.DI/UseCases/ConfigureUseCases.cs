using Microsoft.Extensions.DependencyInjection;
using Template.AspNet6.Application.UseCases.OAuth.LogAs;
using Template.AspNet6.Application.UseCases.OAuth.RefreshToken;
using Template.AspNet6.Application.UseCases.OAuth.SignIn;
using Template.AspNet6.Application.UseCases.Users.Add;
using Template.AspNet6.Application.UseCases.Users.Get;
using Template.AspNet6.Application.UseCases.Users.Update;
using Template.AspNet6.Application.UseCases.Users.Update.ProfilePicture;

namespace Template.AspNet6.DI.UseCases;

public static class ConfigureUseCases
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        //OAUTH
        services.AddScoped<IRefreshTokenUseCase, RefreshTokenUseCase>();
        services.AddScoped<ISignInUseCase, SignInUseCase>();
        services.AddScoped<IAdminLogAsUseCase, AdminLogAsUseCase>();

        //USERS
        services.AddScoped<IGetUsersUseCase, GetUsersUseCase>();
        services.AddScoped<IAddUserUseCase, AddUserUseCase>();
        services.AddScoped<IUpdateUserUseCase, UpdateUserUseCase>();
        services.AddScoped<IUpdateProfilePictureUseCase, UpdateProfilePictureUseCase>();

        return services;
    }
}
