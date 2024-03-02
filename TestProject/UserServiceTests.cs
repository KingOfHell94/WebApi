using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WebApi.Controllers.Users.V1.Models;
using WebApi.Controllers.Users.V1.Repositories;
using WebApi.Controllers.Users.V1.Services;
using WebApi.Database.Entities;
using Xunit;

namespace TestProject
{
    public class UserServiceTests
    {
        [Fact]
        public async Task Register_User_ShouldReturnSuccess_WhenUserDoesNotExist()
        {
            var userConfig = new UserConfig
            {
                Key = "your_secret_key_here_that_is_long_enough_to_make_256_bits_test_version",
                ExpiryDurationHours = 1,
                StartingCoins = 1000
            };
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c["PasswordHashing:Salt"]).Returns("YourUniqueSaltHereTest");
            var mockLogger = new Mock<ILogger<UserService>>();
            // Arrange
            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo.Setup(repo => repo.FindByUsernameAsync(It.IsAny<string>())).ReturnsAsync(() => null);
            var userService = new UserService(mockUserRepo.Object, new PasswordHasher(configuration.Object), userConfig, mockLogger.Object);

            // Act
            var result = await userService.Register(new UserRegisterModel { Username = "newuser", Password = "password", Email = "newuser@example.com" });

            // Assert
            Xunit.Assert.True(result.Success);
        }
        [Fact]
        public async Task Authenticate_User_ShouldReturnSuccess_WhenCredentialsAreValid()
        {
            var userConfig = new UserConfig
            {
                Key = "your_secret_key_here_that_is_long_enough_to_make_256_bits_test_version",
                ExpiryDurationHours = 1,
                StartingCoins = 1000
            };
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c["PasswordHashing:Salt"]).Returns("YourUniqueSaltHereTest");
            var mockLogger = new Mock<ILogger<UserService>>();
            // Arrange
            var testUser = new User { Username = "testuser", PasswordHash = new PasswordHasher(configuration.Object).HashPassword("password") };
            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo.Setup(repo => repo.FindByUsernameAsync("testuser")).ReturnsAsync(testUser);
            var userService = new UserService(mockUserRepo.Object, new PasswordHasher(configuration.Object), userConfig, mockLogger.Object);

            // Act
            var result = await userService.Authenticate(new UserAuthenticateModel { Username = "testuser", Password = "password" });

            // Assert
            Xunit.Assert.True(result.Success);
        }
        [Fact]
        public async Task GetUserProfile_ShouldReturnUserProfile_WhenUserExists()
        {
            var userConfig = new UserConfig
            {
                Key = "your_secret_key_here_that_is_long_enough_to_make_256_bits_test_version",
                ExpiryDurationHours = 1,
                StartingCoins = 1000
            };
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c["PasswordHashing:Salt"]).Returns("YourUniqueSaltHereTest");
            var mockLogger = new Mock<ILogger<UserService>>();
            // Arrange
            var username = "existinguser";
            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo.Setup(repo => repo.FindByUsernameAsync(username)).ReturnsAsync(new User { Username = username, Email = "user@example.com", Balance = 1000 });
            var userService = new UserService(mockUserRepo.Object, new PasswordHasher(configuration.Object), userConfig, mockLogger.Object);

            // Act
            var result = await userService.GetUserProfile(username);

            // Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal(username, result.Profile.Username);
        }
        [Fact]
        public async Task Register_User_ShouldReturnError_WhenUserAlreadyExists()
        {
            var userConfig = new UserConfig
            {
                Key = "your_secret_key_here_that_is_long_enough_to_make_256_bits_test_version",
                ExpiryDurationHours = 1,
                StartingCoins = 1000
            };
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c["PasswordHashing:Salt"]).Returns("YourUniqueSaltHereTest");
            var mockLogger = new Mock<ILogger<UserService>>();
            // Arrange
            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo.Setup(repo => repo.FindByUsernameAsync(It.IsAny<string>())).ReturnsAsync(new User());
            var userService = new UserService(mockUserRepo.Object, new PasswordHasher(configuration.Object), userConfig, mockLogger.Object);

            // Act
            var result = await userService.Register(new UserRegisterModel { Username = "existinguser", Password = "password", Email = "user@example.com" });

            // Assert
            Xunit.Assert.False(result.Success);
            Xunit.Assert.Equal("Username or Email already exists.", result.Message);
        }
        [Fact]
        public async Task Authenticate_ShouldFail_WhenPasswordIsIncorrect()
        {
            var userConfig = new UserConfig
            {
                Key = "your_secret_key_here_that_is_long_enough_to_make_256_bits_test_version",
                ExpiryDurationHours = 1,
                StartingCoins = 1000
            };
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c["PasswordHashing:Salt"]).Returns("YourUniqueSaltHereTest");
            var mockLogger = new Mock<ILogger<UserService>>();
            // Arrange
            var testUser = new User { Username = "user", PasswordHash = "hashed_password" };
            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo.Setup(repo => repo.FindByUsernameAsync("user")).ReturnsAsync(testUser);
            var userService = new UserService(mockUserRepo.Object, new PasswordHasher(configuration.Object), userConfig, mockLogger.Object);

            // Act
            var result = await userService.Authenticate(new UserAuthenticateModel { Username = "user", Password = "wrong_password" });

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Username or password is incorrect.", result.Message);
        }
        [Fact]
        public async Task GetUserProfile_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            var userConfig = new UserConfig
            {
                Key = "your_secret_key_here_that_is_long_enough_to_make_256_bits_test_version",
                ExpiryDurationHours = 1,
                StartingCoins = 1000
            };
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c["PasswordHashing:Salt"]).Returns("YourUniqueSaltHereTest");
            var mockLogger = new Mock<ILogger<UserService>>();
            // Arrange
            var username = "nonexistentuser";
            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo.Setup(repo => repo.FindByUsernameAsync(username)).ReturnsAsync(() => null);
            var userService = new UserService(mockUserRepo.Object, new PasswordHasher(configuration.Object), userConfig, mockLogger.Object);

            // Act
            var result = await userService.GetUserProfile(username);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("User not found", result.Message);
        }
    }
}