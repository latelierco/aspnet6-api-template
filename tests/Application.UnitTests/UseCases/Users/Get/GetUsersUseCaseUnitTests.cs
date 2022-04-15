using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Template.AspNet6.Application.UseCases.Users.Get;
using Template.AspNet6.Domain.Entities.Users;
using Template.AspNet6.Domain.Entities.Users.Roles;
using Template.AspNet6.Domain.ValueObjects.Email;
using Template.AspNet6.Infra.Persistence.SqlServer;
using Template.AspNet6.Infra.Persistence.SqlServer.Users;

namespace Application.UnitTests.UseCases.Users.Get;

public class GetUsersUseCaseUnitTests : IDisposable
{
    private const string InMemoryConnectionString = "DataSource=:memory:";
    private readonly SqliteConnection _connection = new(InMemoryConnectionString);

    private IReadUserRepository _readUsers = null!;
    private GetUsersUseCase _useCase = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        _connection.Open();

        var options = new DbContextOptionsBuilder<Context>().UseSqlite(_connection).Options;

        var context = new Context(options);
        context.Database.EnsureCreated();

        context.Users.Add(new User {Id = Guid.NewGuid(), Email = new Email("test@test.fr"), FirstName = "Spider", LastName = "Man", Roles = CRoles.User, Plans = "Free", IsActivated = false});
        context.SaveChanges();

        _readUsers = new UserRepository(context);
        _useCase = new GetUsersUseCase(_readUsers);
    }

    [Test]
    public async Task ShouldHaveOneResultIfMailMatch()
    {
        var outputPort = new FakeOutputPort();

        _useCase.SetOutputPort(outputPort);

        await _useCase.ExecuteAsync("test@test.fr", Array.Empty<string>(), null, null);

        Assert.AreEqual(1, outputPort.Users.Count());
    }

    [Test]
    public async Task ShouldHaveOneResultIfFirstnameMatch()
    {
        var outputPort = new FakeOutputPort();

        _useCase.SetOutputPort(outputPort);

        await _useCase.ExecuteAsync("Spider", Array.Empty<string>(), null, null);

        Assert.AreEqual(1, outputPort.Users.Count());
    }

    [Test]
    public async Task ShouldHaveOneResultIfRolesMatch()
    {
        var outputPort = new FakeOutputPort();

        _useCase.SetOutputPort(outputPort);

        await _useCase.ExecuteAsync(null, new[] {CRoles.User}, null, null);

        Assert.AreEqual(1, outputPort.Users.Count());
    }

    [Test]
    public async Task ShouldHaveZeroResultIfSearchingByActivated()
    {
        var outputPort = new FakeOutputPort();

        _useCase.SetOutputPort(outputPort);

        await _useCase.ExecuteAsync(null, Array.Empty<string>(), true, null);

        Assert.Zero(outputPort.Users.Count());
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
