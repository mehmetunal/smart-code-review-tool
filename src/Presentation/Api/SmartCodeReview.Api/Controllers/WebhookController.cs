using Microsoft.AspNetCore.Mvc;

namespace SmartCodeReview.Api.Controllers;

/// <summary>
/// GitHub/GitLab webhook controller'ı
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly ILogger<WebhookController> _logger;

    public WebhookController(ILogger<WebhookController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// GitHub webhook endpoint
    /// </summary>
    [HttpPost("github")]
    public async Task<IActionResult> HandleGitHubWebhook([FromBody] object payload)
    {
        try
        {
            _logger.LogInformation("GitHub webhook alındı");
            
            // TODO: Webhook'u kuyruğa ekle (HostedService + Redis)
            // TODO: AI analizi için background job başlat
            
            return Ok(new { Message = "Webhook alındı" });
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
    public async Task<IActionResult> HandleGitLabWebhook([FromBody] object payload)
    {
        try
        {
            _logger.LogInformation("GitLab webhook alındı");
            
            // TODO: Webhook'u kuyruğa ekle (HostedService + Redis)
            // TODO: AI analizi için background job başlat
            
            return Ok(new { Message = "Webhook alındı" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GitLab webhook işleme hatası");
            return StatusCode(500, new { Message = "Webhook işlenemedi" });
        }
    }
}

