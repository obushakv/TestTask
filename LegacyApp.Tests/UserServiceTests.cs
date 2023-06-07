using LegacyApp.Abstraction;
using LegacyApp.Tests.Helper;
using Moq;

namespace LegacyApp.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserValidationService> _userValidationServiceMock;
        private readonly Mock<IClientRepository> _clientRepositoryMock;
        private readonly Mock<IUserCreditLimitService> _userCreditLimitServiceMock;
        private readonly Mock<IUserDataAccessService> _userDataAccessServiceMock;

        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userValidationServiceMock = new Mock<IUserValidationService>();
            _clientRepositoryMock = new Mock<IClientRepository>();
            _userCreditLimitServiceMock = new Mock<IUserCreditLimitService>();
            _userDataAccessServiceMock = new Mock<IUserDataAccessService>();

            _userService = new UserService(
                _userValidationServiceMock.Object,
                _userCreditLimitServiceMock.Object,
                _clientRepositoryMock.Object, 
                _userDataAccessServiceMock.Object);
        }

        [Fact]
        public void AddUser_WithInvalidUser_ShouldReturnFalse()
        {
            // Arrange
            var user = TestHelper.GetValidUser();
            var clientId = 1;
            _userValidationServiceMock.Setup(u => u.IsValidUser(It.IsAny<User>())).Returns(false);

            // Act
            var result = _userService.AddUser(user.Firstname, user.Surname, user.EmailAddress, user.DateOfBirth, 1);

            // Assert
            Assert.False(result);
            _userValidationServiceMock.Verify(u => u.IsValidUser(It.IsAny<User>()), Times.Once);
            _clientRepositoryMock.Verify(c => c.GetById(clientId), Times.Never);
            _userCreditLimitServiceMock.Verify(u => u.UpdateCreditLimit(It.IsAny<User>()), Times.Never);
            _userDataAccessServiceMock.Verify(d => d.AddUser(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public void AddUser_UserWithInvalidCreditLimit_ShouldReturnFalse()
        {
            // Arrange
            var user = TestHelper.GetValidUser();
            var clientId = 1;
            _userValidationServiceMock.Setup(u => u.IsValidUser(It.IsAny<User>())).Returns(true);
            _userValidationServiceMock.Setup(u => u.IsValidCreditLimit(It.IsAny<User>())).Returns(false);
            _clientRepositoryMock.Setup(c => c.GetById(clientId)).Returns(new Client { Name = "ClientWithCreditLimit" });
            // Act
            var result = _userService.AddUser(user.Firstname, user.Surname, user.EmailAddress, user.DateOfBirth, 1);

            // Assert
            Assert.False(result);
            _userValidationServiceMock.Verify(u => u.IsValidUser(It.IsAny<User>()), Times.Once);
            _clientRepositoryMock.Verify(c => c.GetById(clientId), Times.Once);
            _userCreditLimitServiceMock.Verify(u => u.UpdateCreditLimit(It.IsAny<User>()), Times.Once);
            _userDataAccessServiceMock.Verify(d => d.AddUser(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public void AddUser_WithClientWithoutCreditLimit_ShouldAddUserWithoutUpdatingCreditLimit()
        {
            // Arrange
            var user = TestHelper.GetValidUser();
            var clientId = 1;
            var userClient = new Client { Name = "VeryImportantClient" };

            _userValidationServiceMock.Setup(u => 
                u.IsValidUser(It.Is<User>(s => 
                    s.EmailAddress == user.EmailAddress 
                    && s.DateOfBirth == user.DateOfBirth
                    && s.Firstname == user.Firstname
                    && s.Surname == user.Surname))).Returns(true);

            _clientRepositoryMock.Setup(c => c.GetById(clientId)).Returns(userClient);
            _userValidationServiceMock.Setup(u => u.IsValidCreditLimit(It.IsAny<User>())).Returns(true);

            // Act
            var result = _userService.AddUser(user.Firstname, user.Surname, user.EmailAddress, user.DateOfBirth, clientId);

            // Assert
            Assert.True(result);
            _userValidationServiceMock.Verify(u => u.IsValidUser(It.IsAny<User>()), Times.Once);
            _clientRepositoryMock.Verify(c => c.GetById(clientId), Times.Once);
            _userCreditLimitServiceMock.Verify(u => u.UpdateCreditLimit(It.IsAny<User>()), Times.Never);
            _userDataAccessServiceMock.Verify(d => d.AddUser(It.Is<User>(s =>
                s.Surname == user.Surname
                && s.Firstname == user.Firstname
                && s.EmailAddress == user.EmailAddress
                && s.DateOfBirth == user.DateOfBirth
                && s.Client == userClient)), Times.Once);
        }

        [Fact]
        public void AddUser_WithClientWithCreditLimit_ShouldAddUserAndUpdateCreditLimit()
        {
            // Arrange
              var user = TestHelper.GetValidUser();

              var clientId = 1;
              var userClient = new Client { Name = "ClientWithCreditLimit" };
              
              _userValidationServiceMock.Setup(u => u.IsValidUser(It.Is<User>(s =>
                s.EmailAddress == user.EmailAddress
                && s.DateOfBirth == user.DateOfBirth
                && s.Firstname == user.Firstname
                && s.Surname == user.Surname))).Returns(true);

            _clientRepositoryMock.Setup(c => c.GetById(clientId)).Returns(userClient);
            _userValidationServiceMock.Setup(u => u.IsValidCreditLimit(It.IsAny<User>())).Returns(true);

            // Act
            var result = _userService.AddUser(user.Firstname, user.Surname, user.EmailAddress, user.DateOfBirth, 1);

            // Assert
            Assert.True(result);
            _userValidationServiceMock.Verify(u => u.IsValidUser(It.IsAny<User>()), Times.Once);
            _clientRepositoryMock.Verify(c => c.GetById(clientId), Times.Once);
            _userCreditLimitServiceMock.Verify(u => u.UpdateCreditLimit(It.IsAny<User>()), Times.Once);
            _userValidationServiceMock.Verify(u => u.IsValidCreditLimit(It.IsAny<User>()), Times.Once);
            _userDataAccessServiceMock.Verify(d => d.AddUser(It.Is<User>(s => 
                s.Surname == user.Surname 
                && s.Firstname == user.Firstname
                && s.EmailAddress == user.EmailAddress
                && s.DateOfBirth == user.DateOfBirth
                && s.Client == userClient)), Times.Once);
        }
    }
}