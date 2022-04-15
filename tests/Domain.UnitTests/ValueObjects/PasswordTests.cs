using NUnit.Framework;
using Template.AspNet6.Domain.ValueObjects.Email;
using Template.AspNet6.Domain.ValueObjects.Password;

// ReSharper disable ObjectCreationAsStatement

namespace Domain.UnitTests.ValueObjects;

public class PasswordTests
{
    // [SetUp]
    // public void Setup()
    // {
    // }
    
    [Test]
    public void ValidPasswordShouldPass()
    {
        const string passwordTest = "eWfwo@#$123!";

        var password = new Password(passwordTest);
        
        Assert.Pass();
    }
    
    [Test]
    public void InvalidPasswordWithoutMinLengthRequirementShouldThrow()
    {
        const string passwordTest = "wo@#$123!";

        Assert.Throws<PasswordException>(() => new Password(passwordTest));
    }
    
    [Test]
    public void InvalidPasswordWithoutOneUpperCaseShouldThrow()
    {
        const string passwordTest = "ewfwo@#$123!";

        Assert.Throws<PasswordException>(() => new Password(passwordTest));
    }
    
    [Test]
    public void InvalidPasswordWithoutOneLowerCaseShouldThrow()
    {
        const string passwordTest = "EWFWO@#$123!";

        Assert.Throws<PasswordException>(() => new Password(passwordTest));
    }
     
    [Test]
    public void InvalidPasswordWithoutOneDigitShouldThrow()
    {
        const string passwordTest = "EWFWO@#$ABC!";

        Assert.Throws<PasswordException>(() => new Password(passwordTest));
    }
    
    [Test]
    public void InvalidPasswordWithoutOneSpecialCharShouldThrow()
    {
        const string passwordTest = "EWFWOQAAABCC";

        Assert.Throws<PasswordException>(() => new Password(passwordTest));
    }
}
