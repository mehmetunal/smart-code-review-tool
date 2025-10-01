using SmartCodeReview.Mssql.Services.Models;

namespace SmartCodeReview.Mssql.Services.Interfaces;

/// <summary>
/// Webhook kuyruğu servisi interface'i
/// </summary>
public interface IWebhookQueueService
{
    /// <summary>
    /// Webhook'u kuyruğa ekler
    /// </summary>
    Task<bool> EnqueueAsync(WebhookQueueMessage message);

    /// <summary>
    /// Kuyruktan webhook alır
    /// </summary>
    Task<WebhookQueueMessage?> DequeueAsync();

    /// <summary>
    /// Kuyruk uzunluğunu getirir
    /// </summary>
    Task<long> GetQueueLengthAsync();

    /// <summary>
    /// Kuyruğu temizler
    /// </summary>
    Task<bool> ClearQueueAsync();
}

