using System.Reflection;
using Template.AspNet6.DI.Authentication;
using Template.AspNet6.DI.Errors;
using Template.AspNet6.DI.Logger;
using Template.AspNet6.DI.Persistence;
using Template.AspNet6.DI.Swagger;
using Template.AspNet6.DI.UseCases;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions {Args = args, EnvironmentName = "Development"});

var assemblyName = Assembly.GetExecutingAssembly().GetName().Name ?? string.Empty;

builder.Services
    .ConfigureLogger(builder.Configuration)
    .AddAuth(builder.Configuration)
    .ConfigureDatabase(builder.Configuration)
    .AddVersioning()
    .AddSwagger(assemblyName)
    .AddRouting(options => options.LowercaseUrls = true)
    .AddStorage()
    .AddMemoryCache()
    .AddUseCases()
    .AddControllers();

// builder.Services.AddCors(options => { options.AddPolicy("all", b => { b.WithOrigins("*").AllowAnyHeader().AllowAnyMethod(); }); });

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseVersionedSwagger();

app.UseHttpsRedirection()
    .UseAuthentication()
    .UseRouting()
    // .UseCors("all")
    .UseMiddleware(typeof(ErrorHandlingMiddleware))
    .UseAuthorization()
    // .UpdateDatabase()
    .UseEndpoints(endpoints => { endpoints.MapControllers(); })
    .UseApiVersioning();

app.MapGet("", () => "up!");

app.Run();
