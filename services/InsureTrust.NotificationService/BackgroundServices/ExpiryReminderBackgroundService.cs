using InsureTrust.NotificationService.Services;

namespace InsureTrust.NotificationService.BackgroundServices;

public class ExpiryReminderBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ExpiryReminderBackgroundService> _logger;
    private readonly IConfiguration _configuration;

    public ExpiryReminderBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<ExpiryReminderBackgroundService> logger,
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var enabled = _configuration.GetValue<bool>("BackgroundJobs:ExpiryReminder:Enabled");
        if (!enabled)
        {
            _logger.LogInformation("Expiry reminder background service is disabled.");
            return;
        }

        var intervalMinutes = _configuration.GetValue("BackgroundJobs:ExpiryReminder:IntervalMinutes", 60);

        while (!stoppingToken.IsCancellationRequested)
        {
            await RunCycleAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromMinutes(intervalMinutes), stoppingToken);
        }
    }

    private async Task RunCycleAsync(CancellationToken cancellationToken)
    {
        var demoUserId = _configuration.GetValue("BackgroundJobs:ExpiryReminder:DemoUserId", 0);
        if (demoUserId <= 0)
        {
            _logger.LogDebug("Expiry reminder cycle skipped because DemoUserId is not configured.");
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        await notificationService.SendAsync(
            demoUserId,
            "Policy Expiry Reminder",
            "Your policy is nearing expiry. Please renew to keep coverage active.",
            "warning",
            "Policy Renewal");

        _logger.LogInformation("Expiry reminder sent for user {UserId}.", demoUserId);

        cancellationToken.ThrowIfCancellationRequested();
    }
}
