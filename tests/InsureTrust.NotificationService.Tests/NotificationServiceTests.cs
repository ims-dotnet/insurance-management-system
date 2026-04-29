using Moq;
using InsureTrust.NotificationService.Services;
using InsureTrust.NotificationService.Repositories;
using InsureTrust.NotificationService.Models;
using Xunit;

namespace InsureTrust.NotificationService.Tests
{
    public class NotificationServiceTests
    {
        private readonly Mock<INotificationRepository> _repoMock;
        private readonly InsureTrust.NotificationService.Services.NotificationService _service;

        public NotificationServiceTests()
        {
            _repoMock = new Mock<INotificationRepository>();
            _service = new InsureTrust.NotificationService.Services.NotificationService(_repoMock.Object);
        }

        [Fact]
        public async Task MarkReadAsync_ValidId_CallsRepository()
        {
            // Arrange
            var notifId = 1;
            var userId = 1;
            var notif = new Notification { Id = notifId, UserId = userId };
            
            _repoMock.Setup(r => r.GetByIdAsync(notifId)).ReturnsAsync(notif);

            // Act
            await _service.MarkReadAsync(notifId, userId);

            // Assert
            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Notification>()), Times.Once);
        }
    }
}
