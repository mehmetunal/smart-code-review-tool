using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCodeReview.Data.Mssql.Enums;
using SmartCodeReview.Mssql.Services.Interfaces;

namespace SmartCodeReview.Api.Controllers;

/// <summary>
/// Kod inceleme controller'ı
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CodeReviewController : ControllerBase
{
    private readonly ICodeReviewService _codeReviewService;
    private readonly ILogger<CodeReviewController> _logger;

    public CodeReviewController(
        ICodeReviewService codeReviewService,
        ILogger<CodeReviewController> logger)
    {
        _codeReviewService = codeReviewService;
        _logger = logger;
    }

    /// <summary>
    /// Kod incelemelerini listeler
    /// </summary>
    /// <param name="page">Sayfa numarası (default: 1)</param>
    /// <param name="pageSize">Sayfa boyutu (default: 10, max: 100)</param>
    /// <param name="status">Durum filtresi (opsiyonel)</param>
    /// <param name="projectId">Proje ID filtresi (opsiyonel)</param>
    /// <returns>Sayfalanmış kod inceleme listesi</returns>
    /// <response code="200">Başarılı</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllReviews(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] ReviewStatus? status = null,
        [FromQuery] Guid? projectId = null)
    {
        try
        {
            // Sayfa boyutu kontrolü
            if (pageSize > 100)
                pageSize = 100;

            var result = await _codeReviewService.GetAllAsync(page, pageSize, status, projectId);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    data = result.Data,
                    statusCode = 200
                });
            }

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors,
                statusCode = result.StatusCode
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kod incelemeleri listelenirken hata");
            return StatusCode(500, new
            {
                success = false,
                message = "Sunucu hatası",
                statusCode = 500
            });
        }
    }

    /// <summary>
    /// Kod inceleme detayını getirir
    /// </summary>
    /// <param name="id">Kod inceleme ID</param>
    /// <returns>Kod inceleme detayı</returns>
    /// <response code="200">Başarılı</response>
    /// <response code="404">Bulunamadı</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReview(Guid id)
    {
        try
        {
            var result = await _codeReviewService.GetByIdAsync(id);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    data = result.Data,
                    statusCode = 200
                });
            }

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                statusCode = result.StatusCode
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kod incelemesi getirilirken hata: {Id}", id);
            return StatusCode(500, new
            {
                success = false,
                message = "Sunucu hatası",
                statusCode = 500
            });
        }
    }

    /// <summary>
    /// Kod inceleme durumunu günceller (Admin)
    /// </summary>
    /// <param name="id">Kod inceleme ID</param>
    /// <param name="request">Durum güncelleme isteği</param>
    /// <returns>Güncelleme sonucu</returns>
    /// <response code="200">Başarılı</response>
    /// <response code="404">Bulunamadı</response>
    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateStatusRequest request)
    {
        try
        {
            var result = await _codeReviewService.UpdateStatusAsync(id, request.Status, request.ErrorMessage);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    message = result.Message,
                    statusCode = 200
                });
            }

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                statusCode = result.StatusCode
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kod inceleme durumu güncellenirken hata: {Id}", id);
            return StatusCode(500, new
            {
                success = false,
                message = "Sunucu hatası",
                statusCode = 500
            });
        }
    }
}

/// <summary>
/// Durum güncelleme request model
/// </summary>
public record UpdateStatusRequest(ReviewStatus Status, string? ErrorMessage);

