using LegacyApp.Abstraction;
using LegacyApp.Tests.Helper;
using Moq;
using Xunit;

namespace LegacyApp.Tests
{
    public class UserCreditServiceTests
    {
        private readonly Mock<IUserCreditClient> _userCreditClientMock;
        private readonly Mock<IClientRepository> _clientRepositoryMock;
        private readonly UserCreditService _userCreditService;

        public UserCreditServiceTests()
        {
            _userCreditClientMock = new Mock<IUserCreditClient>();
            _clientRepositoryMock = new Mock<IClientRepository>();
            _userCreditService = new UserCreditService(_userCreditClientMock.Object, _clientRepositoryMock.Object);
        }

        [Fact]
        public void SetUserCreditLimit_ClientWithoutCreditLimit_NoCreditLimitSet()
        {
            // Arrange
            var user = TestHelper.GetValidUser();

            var clientId = 1;
            var client = new Client { Name = "VeryImportantClient" };

            _clientRepositoryMock.Setup(r => r.GetById(clientId)).Returns(client);

            // Act
            _userCreditService.SetUserCreditLimit(user, clientId);

            // Assert
            Assert.False(user.HasCreditLimit);
            Assert.Equal(0, user.CreditLimit);
            _userCreditClientMock.Verify(c => c.GetCreditLimit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
        }

        [Fact]
        public void SetUserCreditLimit_CustomCreditLimitMultiplier_CreditLimitSetWithMultiplier()
        {
            // Arrange
            var user = TestHelper.GetValidUser();

            var clientId = 2;
            var client = new Client { Name = "ImportantClient" };

            _clientRepositoryMock.Setup(r => r.GetById(clientId)).Returns(client);
            _userCreditClientMock.Setup(c => c.GetCreditLimit(user.Firstname, user.Surname, user.DateOfBirth)).Returns(1000);

            // Act
            _userCreditService.SetUserCreditLimit(user, clientId);

            // Assert
            Assert.True(user.HasCreditLimit);
            Assert.Equal(2000, user.CreditLimit);
            _userCreditClientMock.Verify(c => c.GetCreditLimit(user.Firstname, user.Surname, user.DateOfBirth), Times.Once);
        }

        [Fact]
        public void SetUserCreditLimit_NoCustomCreditLimitMultiplier_CreditLimitSetWithoutMultiplier()
        {
            // Arrange
            var user = TestHelper.GetValidUser();

            var clientId = 3;
            var client = new Client { Name = "RegularClient" };

            _clientRepositoryMock.Setup(r => r.GetById(clientId)).Returns(client);
            _userCreditClientMock.Setup(c => c.GetCreditLimit(user.Firstname, user.Surname, user.DateOfBirth)).Returns(1000);

            // Act
            _userCreditService.SetUserCreditLimit(user, clientId);

            // Assert
            Assert.True(user.HasCreditLimit);
            Assert.Equal(1000, user.CreditLimit);
            _userCreditClientMock.Verify(c => c.GetCreditLimit(user.Firstname, user.Surname, user.DateOfBirth), Times.Once);
        }
    }
}