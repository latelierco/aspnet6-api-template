using NUnit.Framework;
using Template.AspNet6.Domain.ValueObjects.Email;
// ReSharper disable ObjectCreationAsStatement

namespace Domain.UnitTests.ValueObjects;

public class EmailTests
{
    // [SetUp]
    // public void Setup()
    // {
    // }
    
    [Test]
    public void ValidEmailShouldPass()
    {
        const string emailTest = "test@test.fr";

        var email = new Email(emailTest);
        
        Assert.Pass();
    }
    
    [Test]
    public void InvalidEmailWithoutTopLevelDomainShouldThrow()
    {
        const string emailTest = "test@testfr";

        Assert.Throws<EmailException>(() => new Email(emailTest));
    }
    
    [Test]
    public void InvalidEmailWithoutNameShouldThrow()
    {
        const string emailTest = "@test.fr";

        Assert.Throws<EmailException>(() => new Email(emailTest));
    }
    
    [Test]
    public void InvalidEmailWithoutMailboxIdentifierShouldThrow()
    {
        const string emailTest = "test@.fr";

        Assert.Throws<EmailException>(() => new Email(emailTest));
    }
}
