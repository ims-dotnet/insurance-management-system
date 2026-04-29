using Moq;
using InsureTrust.QueryService.Services;
using InsureTrust.QueryService.Repositories;
using InsureTrust.QueryService.DTOs;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Xunit;

namespace InsureTrust.QueryService.Tests
{
    public class QueryServiceTests
    {
        private readonly Mock<ISupportRepository> _repoMock;
        private readonly Mock<INotificationClient> _notificationMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<InsureTrust.QueryService.Services.QueryService>> _loggerMock;
        private readonly InsureTrust.QueryService.Services.QueryService _service;

        public QueryServiceTests()
        {
            _repoMock = new Mock<ISupportRepository>();
            _notificationMock = new Mock<INotificationClient>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<InsureTrust.QueryService.Services.QueryService>>();

            _service = new InsureTrust.QueryService.Services.QueryService(
                _repoMock.Object, 
                _notificationMock.Object, 
                _mapperMock.Object, 
                _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllQueriesAsync_CallsRepository()
        {
            // Arrange
            var queries = new List<InsureTrust.QueryService.Models.SupportQuery>();
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(queries);
            _mapperMock.Setup(m => m.Map<List<SupportQueryDto>>(queries)).Returns(new List<SupportQueryDto>());

            // Act
            var result = await _service.GetAllQueriesAsync();

            // Assert
            Assert.NotNull(result);
            _repoMock.Verify(r => r.GetAllAsync(), Times.Once);
        }
    }
}
