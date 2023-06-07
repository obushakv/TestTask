using Moq;

namespace LegacyApp.Tests
{
    public class UserCreditLimitServiceTests
    {
        [Fact]
        public void UpdateCreditLimit_WithCustomMultiplier_ShouldMultiplyCreditLimit()
        {
            // Arrange
            var user = new User
            {
                Firstname = "John",
                Surname = "Doe",
                DateOfBirth = new DateTime(1990, 1, 1),
                Client = new Client { Name = "ImportantClient" }
            };

            var userCreditServiceMock = new Mock<UserCreditServiceClient>();
            userCreditServiceMock.Setup(c => c.GetCreditLimit(user.Firstname, user.Surname, user.DateOfBirth)).Returns(1000);

            var userCreditLimitService = new UserCreditLimitService();

            // Act
            userCreditLimitService.UpdateCreditLimit(user);

            // Assert
            Assert.True(user.HasCreditLimit);
            Assert.Equal(2000, user.CreditLimit);

            userCreditServiceMock.Verify(c => c.GetCreditLimit(user.Firstname, user.Surname, user.DateOfBirth), Times.Once);
        }
    }
}