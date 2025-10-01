using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCodeReview.Data.Mssql;
using SmartCodeReview.Dto.Mssql.Project;
using SmartCodeReview.Mssql.Services.Interfaces;
using System.Security.Claims;

namespace SmartCodeReview.Api.Controllers;

/// <summary>
/// Proje yönetimi controller'ı
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly ILogger<ProjectController> _logger;

    public ProjectController(
        IProjectService projectService,
        ILogger<ProjectController> logger)
    {
        _projectService = projectService;
        _logger = logger;
    }

    /// <summary>
    /// Kullanıcının projelerini listeler
    /// </summary>
    [HttpGet("my-projects")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyProjects(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized(new { Message = "Kullanıcı kimliği bulunamadı" });
            }

            var result = await _projectService.GetUserProjectsAsync(userId, page, pageSize);

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
            _logger.LogError(ex, "Kullanıcı projeleri listelenirken hata");
            return StatusCode(500, new { Message = "Sunucu hatası" });
        }
    }

    /// <summary>
    /// Proje detayını getirir
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProject(Guid id)
    {
        try
        {
            var result = await _projectService.GetByIdAsync(id);

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
            _logger.LogError(ex, "Proje getirilirken hata: {Id}", id);
            return StatusCode(500, new { Message = "Sunucu hatası" });
        }
    }

    /// <summary>
    /// Yeni proje oluşturur
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized(new { Message = "Kullanıcı kimliği bulunamadı" });
            }

            var project = new Project
            {
                Name = dto.Name,
                Description = dto.Description,
                RepositoryUrl = dto.RepositoryUrl,
                FullName = dto.FullName,
                RepositoryId = dto.RepositoryId,
                WebhookSecret = dto.WebhookSecret,
                UserId = userId
            };

            var result = await _projectService.CreateAsync(project);

            if (result.IsSuccess)
            {
                return CreatedAtAction(
                    nameof(GetProject),
                    new { id = result.Data!.Id },
                    new
                    {
                        success = true,
                        data = result.Data,
                        message = result.Message,
                        statusCode = 201
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
            _logger.LogError(ex, "Proje oluşturulurken hata");
            return StatusCode(500, new { Message = "Sunucu hatası" });
        }
    }

    /// <summary>
    /// Projeyi aktif/pasif yapar
    /// </summary>
    [HttpPut("{id}/toggle-active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        try
        {
            var result = await _projectService.ToggleActiveAsync(id);

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
            _logger.LogError(ex, "Proje durumu değiştirilirken hata: {Id}", id);
            return StatusCode(500, new { Message = "Sunucu hatası" });
        }
    }

    /// <summary>
    /// Projeyi siler
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        try
        {
            var result = await _projectService.DeleteAsync(id);

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
            _logger.LogError(ex, "Proje silinirken hata: {Id}", id);
            return StatusCode(500, new { Message = "Sunucu hatası" });
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}

