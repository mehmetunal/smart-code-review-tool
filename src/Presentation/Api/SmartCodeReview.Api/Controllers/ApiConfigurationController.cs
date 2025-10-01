using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCodeReview.Data.Mssql;
using SmartCodeReview.Mssql.Services.Interfaces;
using SmartCodeReview.Mssql.Services.Models;

namespace SmartCodeReview.Api.Controllers;

/// <summary>
/// API konfigürasyon yönetimi
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ApiConfigurationController : ControllerBase
{
    private readonly IApiConfigurationService _apiConfigService;
    private readonly ILogger<ApiConfigurationController> _logger;

    public ApiConfigurationController(
        IApiConfigurationService apiConfigService,
        ILogger<ApiConfigurationController> logger)
    {
        _apiConfigService = apiConfigService;
        _logger = logger;
    }

    /// <summary>
    /// API konfigürasyonu oluşturur
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ApiConfiguration apiConfig)
    {
        try
        {
            var result = await _apiConfigService.CreateAsync(apiConfig);
            
            if (result.IsSuccess)
            {
                return Ok(new { success = true, data = result.Data });
            }
            
            return StatusCode(result.StatusCode, new { 
                success = false, 
                message = result.Message 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API konfigürasyonu oluşturulamadı");
            return StatusCode(500, new { 
                success = false, 
                message = "Sunucu hatası" 
            });
        }
    }

    /// <summary>
    /// API konfigürasyonu günceller
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ApiConfiguration apiConfig)
    {
        try
        {
            apiConfig.Id = id;
            var result = await _apiConfigService.UpdateAsync(apiConfig);
            
            if (result.IsSuccess)
            {
                return Ok(new { success = true, data = result.Data });
            }
            
            return StatusCode(result.StatusCode, new { 
                success = false, 
                message = result.Message 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API konfigürasyonu güncellenemedi");
            return StatusCode(500, new { 
                success = false, 
                message = "Sunucu hatası" 
            });
        }
    }

    /// <summary>
    /// API konfigürasyonu siler
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _apiConfigService.DeleteAsync(id);
            
            if (result.IsSuccess)
            {
                return Ok(new { success = true });
            }
            
            return StatusCode(result.StatusCode, new { 
                success = false, 
                message = result.Message 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API konfigürasyonu silinemedi");
            return StatusCode(500, new { 
                success = false, 
                message = "Sunucu hatası" 
            });
        }
    }

    /// <summary>
    /// API konfigürasyonu getirir
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _apiConfigService.GetByIdAsync(id);
            
            if (result.IsSuccess)
            {
                return Ok(new { success = true, data = result.Data });
            }
            
            return StatusCode(result.StatusCode, new { 
                success = false, 
                message = result.Message 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API konfigürasyonu getirilemedi");
            return StatusCode(500, new { 
                success = false, 
                message = "Sunucu hatası" 
            });
        }
    }

    /// <summary>
    /// API türüne göre aktif konfigürasyonu getirir
    /// </summary>
    [HttpGet("by-type/{apiType}")]
    public async Task<IActionResult> GetByType(string apiType)
    {
        try
        {
            var result = await _apiConfigService.GetActiveByTypeAsync(apiType);
            
            if (result.IsSuccess)
            {
                return Ok(new { success = true, data = result.Data });
            }
            
            return StatusCode(result.StatusCode, new { 
                success = false, 
                message = result.Message 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API konfigürasyonu getirilemedi: {ApiType}", apiType);
            return StatusCode(500, new { 
                success = false, 
                message = "Sunucu hatası" 
            });
        }
    }

    /// <summary>
    /// Kullanıcının API konfigürasyonlarını listeler
    /// </summary>
    [HttpGet("my-configurations")]
    public async Task<IActionResult> GetMyConfigurations(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        try
        {
            // TODO: User ID'yi JWT token'dan al
            var userId = Guid.Empty; // Geçici
            
            var result = await _apiConfigService.GetUserConfigurationsAsync(userId, page, pageSize);
            
            if (result.IsSuccess)
            {
                return Ok(new { success = true, data = result.Data });
            }
            
            return StatusCode(result.StatusCode, new { 
                success = false, 
                message = result.Message 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kullanıcı API konfigürasyonları getirilemedi");
            return StatusCode(500, new { 
                success = false, 
                message = "Sunucu hatası" 
            });
        }
    }

    /// <summary>
    /// Tüm aktif API konfigürasyonlarını getirir
    /// </summary>
    [HttpGet("all-active")]
    public async Task<IActionResult> GetAllActive()
    {
        try
        {
            var result = await _apiConfigService.GetAllActiveAsync();
            
            if (result.IsSuccess)
            {
                return Ok(new { success = true, data = result.Data });
            }
            
            return StatusCode(result.StatusCode, new { 
                success = false, 
                message = result.Message 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Aktif API konfigürasyonları getirilemedi");
            return StatusCode(500, new { 
                success = false, 
                message = "Sunucu hatası" 
            });
        }
    }

    /// <summary>
    /// Varsayılan konfigürasyonları oluşturur (Admin)
    /// </summary>
    [HttpPost("create-defaults")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateDefaults()
    {
        try
        {
            var result = await _apiConfigService.CreateDefaultConfigurationsAsync();
            
            if (result.IsSuccess)
            {
                return Ok(new { success = true, message = "Varsayılan konfigürasyonlar oluşturuldu" });
            }
            
            return StatusCode(result.StatusCode, new { 
                success = false, 
                message = result.Message 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Varsayılan konfigürasyonlar oluşturulamadı");
            return StatusCode(500, new { 
                success = false, 
                message = "Sunucu hatası" 
            });
        }
    }
}
