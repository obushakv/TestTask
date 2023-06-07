using LegacyApp.Tests.Helper;

namespace LegacyApp.Tests;

public class UserValidationServiceTests
{
    private readonly UserValidationService _validationService;

    public UserValidationServiceTests()
    {
        _validationService = new UserValidationService();
    }

    [Fact]
    public void IsValidUser_ValidUser_ReturnsTrue()
    {
        // Arrange
        var user = TestHelper.GetValidUser();

        // Act
        bool isValid = _validationService.IsValidUser(user);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void IsValidUser_NullUser_ReturnsFalse()
    {
        // Arrange
        User? user = null;

        // Act
        bool isValid = _validationService.IsValidUser(user);

        // Assert
        Assert.False(isValid);
    }

    [Theory]
    [InlineData("John", "Doe", true)] 
    [InlineData("", "Doe", false)] 
    [InlineData(null, "Doe", false)]
    [InlineData("John", "", false)]
    [InlineData("John", null, false)] 
    [InlineData("", "", false)]
    [InlineData(null, null, false)]
    public void IsValidUser_NameValidationTests(string firstName, string surname, bool expectedIsValid)
    {
        // Arrange
        var user = TestHelper.GetValidUser();
        user.Firstname = firstName;
        user.Surname = surname;

        // Act
        bool isValid = _validationService.IsValidUser(user);

        // Assert
        Assert.Equal(expectedIsValid, isValid);
    }

    [Theory]
    [InlineData("test@example.com", true)]
    [InlineData("te.st@exa.mple.com", true)]
    [InlineData("test-._+!#$%&'*/=123@example.com", true)]
    [InlineData("test@123@example.com", false)]
    [InlineData("test", false)] 
    [InlineData("test@", false)] 
    [InlineData("test@example", false)] 
    [InlineData("test@.example", false)] 
    [InlineData("te.st@example", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValidUser_EmailValidationTests(string email, bool expectedIsValid)
    {
        // Arrange
        var user = TestHelper.GetValidUser();
        user.EmailAddress = email;

        // Act
        bool isValid = _validationService.IsValidUser(user);

        // Assert
        Assert.Equal(expectedIsValid, isValid);
    }

    [Fact]
    public void IsValidUser_UnderageUser_ReturnsFalse()
    {
        // Arrange
        var user = TestHelper.GetValidUser();
        user.DateOfBirth = DateTime.Today.AddYears(-20);// Underage user

        // Act
        bool isValid = _validationService.IsValidUser(user);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void IsValidCreditLimit_UserWithoutCreditLimit_ReturnsTrue()
    {
        // Arrange
        var user = TestHelper.GetValidUser();

        // Act
        bool isValid = _validationService.IsValidCreditLimit(user);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void IsValidCreditLimit_UserWithCreditLimitAboveMin_ReturnsTrue()
    {
        // Arrange
        var user = TestHelper.GetValidUser();
        user.HasCreditLimit = true;
        user.CreditLimit = 1000;
        // Act
        bool isValid = _validationService.IsValidCreditLimit(user);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void IsValidCreditLimit_UserWithCreditLimitBelowMin_ReturnsFalse()
    {
        // Arrange
        var user = TestHelper.GetValidUser();
        user.HasCreditLimit = true;
        user.CreditLimit = 499;
        // Act
        bool isValid = _validationService.IsValidCreditLimit(user);

        // Assert
        Assert.False(isValid);
    }
}