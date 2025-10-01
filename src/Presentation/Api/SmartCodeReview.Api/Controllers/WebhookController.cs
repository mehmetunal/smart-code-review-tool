using Microsoft.AspNetCore.Mvc;
using SmartCodeReview.Mssql.Services.Interfaces;
using SmartCodeReview.Mssql.Services.Models;
using System.Text.Json;

namespace SmartCodeReview.Api.Controllers;

/// <summary>
/// GitHub/GitLab webhook controller'ı
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly IWebhookQueueService _webhookQueue;
    private readonly ILogger<WebhookController> _logger;

    public WebhookController(
        IWebhookQueueService webhookQueue,
        ILogger<WebhookController> logger)
    {
        _webhookQueue = webhookQueue;
        _logger = logger;
    }

    /// <summary>
    /// GitHub webhook endpoint
    /// </summary>
    [HttpPost("github")]
    public async Task<IActionResult> HandleGitHubWebhook([FromBody] JsonElement payload)
    {
        try
        {
            _logger.LogInformation("GitHub webhook alındı");
            
            // Webhook payload'dan bilgileri çıkar
            var action = payload.GetProperty("action").GetString() ?? "";
            var prNumber = payload.GetProperty("number").GetInt32();
            var repository = payload.GetProperty("repository");
            var owner = repository.GetProperty("owner").GetProperty("login").GetString() ?? "";
            var repo = repository.GetProperty("name").GetString() ?? "";
            var pullRequest = payload.GetProperty("pull_request");
            var branchName = pullRequest.GetProperty("head").GetProperty("ref").GetString() ?? "";

            // Webhook mesajını oluştur
            var message = new WebhookQueueMessage
            {
                Source = "GitHub",
                Owner = owner,
                Repository = repo,
                PullRequestNumber = prNumber,
                Action = action,
                BranchName = branchName,
                Payload = payload.ToString()
            };

            // Kuyruğa ekle
            var enqueued = await _webhookQueue.EnqueueAsync(message);
            
            if (!enqueued)
            {
                return StatusCode(500, new { Message = "Webhook kuyruğa eklenemedi" });
            }

            _logger.LogInformation("✅ GitHub webhook kuyruğa eklendi: {Owner}/{Repo}#{Number}", 
                owner, repo, prNumber);
            
            return Ok(new { Message = "Webhook alındı ve kuyruğa eklendi" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GitHub webhook işleme hatası");
            return StatusCode(500, new { Message = "Webhook işlenemedi" });
        }
    }

    /// <summary>
    /// GitLab webhook endpoint
    /// </summary>
    [HttpPost("gitlab")]
    public async Task<IActionResult> HandleGitLabWebhook([FromBody] JsonElement payload)
    {
        try
        {
            _logger.LogInformation("GitLab webhook alındı");
            
            // Webhook payload'dan bilgileri çıkar
            var objectKind = payload.GetProperty("object_kind").GetString() ?? "";
            var objectAttributes = payload.GetProperty("object_attributes");
            var prNumber = objectAttributes.GetProperty("iid").GetInt32();
            var project = payload.GetProperty("project");
            var pathWithNamespace = project.GetProperty("path_with_namespace").GetString() ?? "";
            var parts = pathWithNamespace.Split('/');
            var owner = parts.Length > 0 ? parts[0] : "";
            var repo = parts.Length > 1 ? parts[1] : "";
            var branchName = objectAttributes.GetProperty("source_branch").GetString() ?? "";
            var action = objectAttributes.GetProperty("state").GetString() ?? "";

            // Webhook mesajını oluştur
            var message = new WebhookQueueMessage
            {
                Source = "GitLab",
                Owner = owner,
                Repository = repo,
                PullRequestNumber = prNumber,
                Action = action,
                BranchName = branchName,
                Payload = payload.ToString()
            };

            // Kuyruğa ekle
            var enqueued = await _webhookQueue.EnqueueAsync(message);
            
            if (!enqueued)
            {
                return StatusCode(500, new { Message = "Webhook kuyruğa eklenemedi" });
            }

            _logger.LogInformation("✅ GitLab webhook kuyruğa eklendi: {Owner}/{Repo}#{Number}", 
                owner, repo, prNumber);
            
            return Ok(new { Message = "Webhook alındı ve kuyruğa eklendi" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GitLab webhook işleme hatası");
            return StatusCode(500, new { Message = "Webhook işlenemedi" });
        }
    }

    /// <summary>
    /// Kuyruk durumunu getirir (Admin)
    /// </summary>
    [HttpGet("queue/status")]
    public async Task<IActionResult> GetQueueStatus()
    {
        try
        {
            var queueLength = await _webhookQueue.GetQueueLengthAsync();
            
            return Ok(new
            {
                QueueLength = queueLength,
                Status = queueLength > 0 ? "Active" : "Empty",
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kuyruk durumu kontrolü hatası");
            return StatusCode(500, new { Message = "Kuyruk durumu alınamadı" });
        }
    }
}

