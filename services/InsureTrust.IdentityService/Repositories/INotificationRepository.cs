using InsureTrust.IdentityService.Models;

namespace InsureTrust.IdentityService.Repositories;

public interface INotificationRepository
{
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId);
    Task<int> GetUnreadCountAsync(int userId);
    Task<Notification?> GetByIdAsync(int id);
    Task AddAsync(Notification notification);
    Task UpdateAsync(Notification notification);
    Task UpdateRangeAsync(IEnumerable<Notification> notifications);
    Task SaveChangesAsync();
}
