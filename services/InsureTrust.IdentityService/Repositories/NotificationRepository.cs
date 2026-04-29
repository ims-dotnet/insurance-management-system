using InsureTrust.IdentityService.Data;
using InsureTrust.IdentityService.Models;
using Microsoft.EntityFrameworkCore;

namespace InsureTrust.IdentityService.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly IdentityDbContext _context;

    public NotificationRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(int userId)
    {
        return await _context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead);
    }

    public async Task<Notification?> GetByIdAsync(int id)
    {
        return await _context.Notifications.FindAsync(id);
    }

    public async Task AddAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
    }

    public async Task UpdateAsync(Notification notification)
    {
        _context.Notifications.Update(notification);
        await Task.CompletedTask;
    }

    public async Task UpdateRangeAsync(IEnumerable<Notification> notifications)
    {
        _context.Notifications.UpdateRange(notifications);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
