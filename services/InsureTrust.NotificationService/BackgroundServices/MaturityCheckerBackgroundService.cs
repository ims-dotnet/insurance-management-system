using InsureTrust.NotificationService.Services;

namespace InsureTrust.NotificationService.BackgroundServices;

public class MaturityCheckerBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MaturityCheckerBackgroundService> _logger;
    private readonly IConfiguration _configuration;

    public MaturityCheckerBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<MaturityCheckerBackgroundService> logger,
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var enabled = _configuration.GetValue<bool>("BackgroundJobs:MaturityChecker:Enabled");
        if (!enabled)
        {
            _logger.LogInformation("Maturity checker background service is disabled.");
            return;
        }

        var intervalMinutes = _configuration.GetValue("BackgroundJobs:MaturityChecker:IntervalMinutes", 180);

        while (!stoppingToken.IsCancellationRequested)
        {
            await RunCycleAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromMinutes(intervalMinutes), stoppingToken);
        }
    }

    private async Task RunCycleAsync(CancellationToken cancellationToken)
    {
        var demoUserId = _configuration.GetValue("BackgroundJobs:MaturityChecker:DemoUserId", 0);
        if (demoUserId <= 0)
        {
            _logger.LogDebug("Maturity checker cycle skipped because DemoUserId is not configured.");
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        await notificationService.SendAsync(
            demoUserId,
            "Policy Maturity Update",
            "A policy has reached maturity. Please review your maturity amount details.",
            "success",
            "Maturity");

        _logger.LogInformation("Maturity notification sent for user {UserId}.", demoUserId);

        cancellationToken.ThrowIfCancellationRequested();
    }
}
