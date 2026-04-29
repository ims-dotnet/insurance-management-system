using Moq;
using InsureTrust.ProductService.Services;
using InsureTrust.ProductService.Repository;
using InsureTrust.ProductService.DTOs;
using Xunit;

namespace InsureTrust.ProductService.Tests
{
    public class PolicyServiceTests
    {
        private readonly Mock<IPolicyRepository> _repoMock;
        private readonly PolicyService _service;

        public PolicyServiceTests()
        {
            _repoMock = new Mock<IPolicyRepository>();
            _service = new PolicyService(_repoMock.Object);
        }

        [Fact]
        public async Task GetPolicyTypeByIdAsync_InvalidId_ReturnsNewPolicyTypeDto()
        {
            // Act: Call the service with an invalid ID
            var result = await _service.GetPolicyTypeByIdAsync(0);

            // Assert: Verify it returns a new (empty) DTO instead of null or error
            Assert.NotNull(result);
            Assert.Equal(0, result.Id);
        }

        [Fact]
        public async Task GetMyPoliciesAsync_ValidUserId_ReturnsPoliciesFromRepo()
        {
            // Arrange
            var userId = 1;
            var mockPolicies = new List<PolicyDto>
            {
                new PolicyDto { Id = 101, Status = "Active" },
                new PolicyDto { Id = 102, Status = "Pending" }
            };

            _repoMock.Setup(repo => repo.GetMyPoliciesAsync(userId))
                .ReturnsAsync(mockPolicies);

            // Act
            var result = await _service.GetMyPoliciesAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(102, result.First().Id); // Should be ordered by descending ID
        }
    }
}
