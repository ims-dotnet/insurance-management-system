using AutoMapper;
using InsureTrust.IdentityService.DTOs;
using InsureTrust.IdentityService.Models;
using InsureTrust.IdentityService.Repositories;

namespace InsureTrust.IdentityService.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repository;
    private readonly IMapper _mapper;

    public NotificationService(INotificationRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId)
    {
        var notifications = await _repository.GetUserNotificationsAsync(userId);
        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<int> GetUnreadCountAsync(int userId)
    {
        return await _repository.GetUnreadCountAsync(userId);
    }

    public async Task MarkReadAsync(int notifId, int userId)
    {
        var notification = await _repository.GetByIdAsync(notifId);
        
        if (notification == null)
            throw new KeyNotFoundException("Notification not found.");
            
        if (notification.UserId != userId)
            throw new UnauthorizedAccessException("You are not authorized to update this notification.");

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            await _repository.UpdateAsync(notification);
            await _repository.SaveChangesAsync();
        }
    }

    public async Task MarkAllReadAsync(int userId)
    {
        var notifications = await _repository.GetUserNotificationsAsync(userId);
        var unreadNotifications = notifications.Where(n => !n.IsRead).ToList();

        if (unreadNotifications.Any())
        {
            foreach (var notif in unreadNotifications)
            {
                notif.IsRead = true;
            }
            await _repository.UpdateRangeAsync(unreadNotifications);
            await _repository.SaveChangesAsync();
        }
    }

    public async Task SendAsync(int userId, string title, string message, string color, string feature)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            ColorCode = color,
            RelatedFeature = feature,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        await _repository.AddAsync(notification);
        await _repository.SaveChangesAsync();
    }
}
