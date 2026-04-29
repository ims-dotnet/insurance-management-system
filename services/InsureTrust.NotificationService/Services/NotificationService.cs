using InsureTrust.NotificationService.DTOs;
using InsureTrust.NotificationService.Models;
using InsureTrust.NotificationService.Repositories;

namespace InsureTrust.NotificationService.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repo;

    public NotificationService(INotificationRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId)
    {
        var list = await _repo.GetByUserIdAsync(userId);
        return list.Select(Map);
    }

    public async Task<int> GetUnreadCountAsync(int userId) =>
        await _repo.GetUnreadCountAsync(userId);

    public async Task MarkReadAsync(int notifId, int userId)
    {
        var notif = await _repo.GetByIdAsync(notifId);
        if (notif is null || notif.UserId != userId) return;

        notif.IsRead = true;
        await _repo.UpdateAsync(notif);
        await _repo.SaveChangesAsync();
    }

    public async Task MarkAllReadAsync(int userId)
    {
        await _repo.MarkAllReadAsync(userId);
        await _repo.SaveChangesAsync();
    }

    public async Task SendAsync(int userId, string title, string message, string color, string feature)
    {
        var notif = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            ColorCode = color,
            RelatedFeature = feature,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(notif);
        await _repo.SaveChangesAsync();
    }

    private static NotificationDto Map(Notification n) => new()
    {
        Id = n.Id,
        Title = n.Title,
        Message = n.Message,
        ColorCode = n.ColorCode,
        IsRead = n.IsRead,
        CreatedAt = n.CreatedAt
    };
}