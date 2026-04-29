using InsureTrust.NotificationService.Models;

namespace InsureTrust.NotificationService.Repositories;

public interface INotificationRepository
{
    Task<IReadOnlyList<Notification>> GetByUserIdAsync(int userId);
    Task<int> GetUnreadCountAsync(int userId);
    Task<Notification?> GetByIdAsync(int id);
    Task AddAsync(Notification notification);
    Task UpdateAsync(Notification notification);
    Task MarkAllReadAsync(int userId);
    Task SaveChangesAsync();
}