namespace InsureTrust.SupportService.Services
{
    public interface INotificationClient
    {
        Task SendSupportStatusChangedAsync(int userId, string ticketNumber, string status);
    }
}
