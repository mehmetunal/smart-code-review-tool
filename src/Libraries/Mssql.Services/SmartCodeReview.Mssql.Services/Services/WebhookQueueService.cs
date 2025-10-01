using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartCodeReview.Mssql.Services.Interfaces;
using SmartCodeReview.Mssql.Services.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace SmartCodeReview.Mssql.Services.Services;

/// <summary>
/// Webhook kuyruğu servisi (Redis tabanlı)
/// </summary>
public class WebhookQueueService : IWebhookQueueService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<WebhookQueueService> _logger;
    private const string QueueKey = "webhook:queue";

    public WebhookQueueService(
        IConnectionMultiplexer redis,
        ILogger<WebhookQueueService> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<bool> EnqueueAsync(WebhookQueueMessage message)
    {
        try
        {
            var db = _redis.GetDatabase();
            var json = JsonSerializer.Serialize(message);
            
            await db.ListRightPushAsync(QueueKey, json);
            
            _logger.LogInformation(
                "Webhook kuyruğa eklendi: {Source} - {Owner}/{Repo}#{Number}", 
                message.Source, message.Owner, message.Repository, message.PullRequestNumber);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Webhook kuyruğa eklenirken hata");
            return false;
        }
    }

    public async Task<WebhookQueueMessage?> DequeueAsync()
    {
        try
        {
            var db = _redis.GetDatabase();
            var json = await db.ListLeftPopAsync(QueueKey);
            
            if (json.IsNullOrEmpty)
                return null;

            var message = JsonSerializer.Deserialize<WebhookQueueMessage>(json!);
            
            _logger.LogInformation(
                "Webhook kuyruktan alındı: {Source} - {Owner}/{Repo}#{Number}", 
                message?.Source, message?.Owner, message?.Repository, message?.PullRequestNumber);
            
            return message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Webhook kuyruktan alınırken hata");
            return null;
        }
    }

    public async Task<long> GetQueueLengthAsync()
    {
        try
        {
            var db = _redis.GetDatabase();
            return await db.ListLengthAsync(QueueKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kuyruk uzunluğu alınırken hata");
            return 0;
        }
    }

    public async Task<bool> ClearQueueAsync()
    {
        try
        {
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync(QueueKey);
            
            _logger.LogWarning("Webhook kuyruğu temizlendi");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kuyruk temizlenirken hata");
            return false;
        }
    }
}

