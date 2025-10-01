namespace SmartCodeReview.Mssql.Services.Models;

/// <summary>
/// Webhook kuyruk mesajı
/// </summary>
public class WebhookQueueMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Source { get; set; } = string.Empty; // GitHub, GitLab
    public string Owner { get; set; } = string.Empty;
    public string Repository { get; set; } = string.Empty;
    public int PullRequestNumber { get; set; }
    public string Action { get; set; } = string.Empty; // opened, synchronize, closed
    public string BranchName { get; set; } = string.Empty;
    public DateTime EnqueuedAt { get; set; } = DateTime.UtcNow;
    public string Payload { get; set; } = string.Empty; // JSON payload
}

