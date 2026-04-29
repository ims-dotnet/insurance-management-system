using Moq;
using InsureTrust.ClaimService.Services;
using InsureTrust.ClaimService.Repositories;
using InsureTrust.ClaimService.DTOs;
using InsureTrust.ClaimService.Models;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Xunit;

namespace InsureTrust.ClaimService.Tests
{
    public class ClaimServiceTests
    {
        private readonly Mock<IClaimRepository> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<InsureTrust.ClaimService.Services.ClaimService>> _loggerMock;
        private readonly InsureTrust.ClaimService.Services.ClaimService _service;

        public ClaimServiceTests()
        {
            _repoMock = new Mock<IClaimRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<InsureTrust.ClaimService.Services.ClaimService>>();
            _service = new InsureTrust.ClaimService.Services.ClaimService(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetMyClaimsAsync_ValidUserId_ReturnsClaims()
        {
            // Arrange
            var userId = 1;
            var claims = new List<Claim> { new Claim { Id = 1, UserId = userId } };
            var claimDtos = new List<ClaimDto> { new ClaimDto { Id = 1 } };

            _repoMock.Setup(repo => repo.GetByUserIdAsync(userId))
                .ReturnsAsync(claims);
            _mapperMock.Setup(m => m.Map<IEnumerable<ClaimDto>>(claims))
                .Returns(claimDtos);

            // Act
            var result = await _service.GetMyClaimsAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task UpdateClaimAsync_ClaimDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            _repoMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Claim?)null);

            var updateDto = new UpdateClaimDto { Action = "Approved", AdminRemarks = "Ok" };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateClaimAsync(999, updateDto));
        }
    }
}
