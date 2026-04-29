using Moq;
using InsureTrust.IdentityService.Services;
using InsureTrust.IdentityService.Repositories;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using InsureTrust.IdentityService.Models;
using InsureTrust.IdentityService.DTOs;
using Xunit;

namespace InsureTrust.IdentityService.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _configurationMock = new Mock<IConfiguration>();
            _mapperMock = new Mock<IMapper>();
            
            // Initialize AuthService with mocked dependencies
            _authService = new AuthService(_userRepositoryMock.Object, _configurationMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task LoginAsync_UserDoesNotExist_ThrowsUnauthorizedAccessException()
        {
            // Arrange: Setup the mock to return null for any email
            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            var loginDto = new LoginDto { Email = "nonexistent@example.com", Password = "password123" };

            // Act & Assert: Verify that an UnauthorizedAccessException is thrown
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginAsync(loginDto));
        }

        [Fact]
        public async Task GetProfileAsync_UserExists_ReturnsUserDto()
        {
            // Arrange: Setup the mock to return a user
            var userId = 1;
            var user = new User { Id = userId, Name = "Test User" };
            var userDto = new UserDto { Id = userId, Name = "Test User" };

            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
                .ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<UserDto>(user))
                .Returns(userDto);

            // Act
            var result = await _authService.GetProfileAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("Test User", result.Name);
        }
    }
}
