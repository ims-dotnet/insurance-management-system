using Moq;
using InsureTrust.PaymentService.Services;
using InsureTrust.PaymentService.Repositories;
using InsureTrust.PaymentService.DTOs;
using InsureTrust.PaymentService.Models;
using InsureTrust.PaymentService.Exceptions;
using AutoMapper;
using Xunit;

namespace InsureTrust.PaymentService.Tests
{
    public class PaymentServiceTests
    {
        private readonly Mock<IPaymentRepository> _repoMock;
        private readonly Mock<IPaymentGateway> _gatewayMock;
        private readonly Mock<IProductServiceClient> _productClientMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly InsureTrust.PaymentService.Services.PaymentService _service;

        public PaymentServiceTests()
        {
            _repoMock = new Mock<IPaymentRepository>();
            _gatewayMock = new Mock<IPaymentGateway>();
            _productClientMock = new Mock<IProductServiceClient>();
            _mapperMock = new Mock<IMapper>();

            _service = new InsureTrust.PaymentService.Services.PaymentService(
                _repoMock.Object,
                _gatewayMock.Object,
                _productClientMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task InitiateFirstPaymentAsync_PolicyNotFound_ThrowsNotFoundException()
        {
            // Arrange
            _productClientMock.Setup(c => c.GetPolicyByPolicyIdAsync(It.IsAny<int>()))
                .ReturnsAsync((ProductPolicyDto?)null);

            var dto = new InitiateFirstPaymentDto { PolicyId = 1 };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.InitiateFirstPaymentAsync(dto, 1));
        }

        [Fact]
        public async Task GetHistoryAsync_ValidUserId_ReturnsPaymentHistory()
        {
            // Arrange
            var userId = 1;
            var payments = new List<Payment> { new Payment { Id = 1, UserId = userId, PolicyId = 101 } };
            
            _repoMock.Setup(repo => repo.GetByUserIdAsync(userId))
                .ReturnsAsync(payments);
            _mapperMock.Setup(m => m.Map<PaymentDto>(It.IsAny<Payment>()))
                .Returns(new PaymentDto { Id = 1 });

            // Act
            var result = await _service.GetHistoryAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("POL-101", result.First().PolicyNumber);
        }
    }
}
