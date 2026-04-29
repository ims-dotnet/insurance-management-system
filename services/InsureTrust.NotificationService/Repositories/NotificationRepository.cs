using InsureTrust.NotificationService.Data;
using InsureTrust.NotificationService.Models;
using Microsoft.EntityFrameworkCore;

namespace InsureTrust.NotificationService.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext _db;
    private readonly DbSet<Notification> _notifications;

    public NotificationRepository(NotificationDbContext db)
    {
        _db = db;
        _notifications = _db.Set<Notification>();
    }

    public async Task<IReadOnlyList<Notification>> GetByUserIdAsync(int userId) =>
        await _notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

    public async Task<int> GetUnreadCountAsync(int userId) =>
        await _notifications.CountAsync(n => n.UserId == userId && !n.IsRead);

    public async Task<Notification?> GetByIdAsync(int id) =>
        await _notifications.FirstOrDefaultAsync(n => n.Id == id);

    public async Task AddAsync(Notification notification) =>
        await _notifications.AddAsync(notification);

    public Task UpdateAsync(Notification notification)
    {
        _notifications.Update(notification);
        return Task.CompletedTask;
    }

    public async Task MarkAllReadAsync(int userId)
    {
        await _notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
    }

    public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
}