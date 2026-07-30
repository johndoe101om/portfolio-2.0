using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Portfolio.Infrastructure.Services;

/// <summary>
/// Background service that executes a periodic heartbeat pulse every 10 minutes
/// to log server telemetry and perform self-ping requests to keep cloud hosting instances active.
/// </summary>
public class HeartbeatService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<HeartbeatService> _logger;

    public HeartbeatService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<HeartbeatService> logger)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var enabled = _configuration.GetValue("Heartbeat:Enabled", true);
        if (!enabled)
        {
            _logger.LogInformation("HeartbeatService is disabled by configuration.");
            return;
        }

        var intervalMinutes = _configuration.GetValue("Heartbeat:IntervalMinutes", 10);
        if (intervalMinutes <= 0)
        {
            intervalMinutes = 10;
        }

        var interval = TimeSpan.FromMinutes(intervalMinutes);
        _logger.LogInformation("HeartbeatService started. Heartbeat interval: {IntervalMinutes} minutes.", intervalMinutes);

        using var timer = new PeriodicTimer(interval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await timer.WaitForNextTickAsync(stoppingToken);
                await PerformHeartbeatAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Normal shutdown, exit gracefully
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during HeartbeatService execution cycle.");
            }
        }

        _logger.LogInformation("HeartbeatService has stopped.");
    }

    public async Task PerformHeartbeatAsync(CancellationToken cancellationToken = default)
    {
        var timestamp = DateTimeOffset.UtcNow;
        _logger.LogInformation("Heartbeat pulse triggered at {Timestamp}", timestamp);

        var selfUrl = GetSelfUrl();
        if (string.IsNullOrWhiteSpace(selfUrl))
        {
            _logger.LogInformation("Heartbeat internal tick logged successfully (No external SelfUrl configured).");
            return;
        }

        var targetUri = selfUrl.TrimEnd('/') + "/health";
        try
        {
            using var client = _httpClientFactory.CreateClient("HeartbeatClient");
            client.Timeout = TimeSpan.FromSeconds(15);

            _logger.LogInformation("Sending heartbeat keep-alive ping to {TargetUri}", targetUri);
            var response = await client.GetAsync(targetUri, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Heartbeat ping to {TargetUri} succeeded with status code {StatusCode}.", targetUri, (int)response.StatusCode);
            }
            else
            {
                _logger.LogWarning("Heartbeat ping to {TargetUri} returned non-success status code {StatusCode}.", targetUri, (int)response.StatusCode);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Heartbeat ping to {TargetUri} failed to complete.", targetUri);
        }
    }

    private string? GetSelfUrl()
    {
        var selfUrl = _configuration["Heartbeat:SelfUrl"];
        if (!string.IsNullOrWhiteSpace(selfUrl))
            return selfUrl;

        selfUrl = Environment.GetEnvironmentVariable("SELF_URL");
        if (!string.IsNullOrWhiteSpace(selfUrl))
            return selfUrl;

        selfUrl = Environment.GetEnvironmentVariable("RENDER_EXTERNAL_URL");
        if (!string.IsNullOrWhiteSpace(selfUrl))
            return selfUrl;

        return null;
    }
}
