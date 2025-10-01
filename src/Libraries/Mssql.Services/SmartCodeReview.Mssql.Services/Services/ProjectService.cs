using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartCodeReview.Data.Mssql;
using SmartCodeReview.Mssql;
using SmartCodeReview.Mssql.Services.Interfaces;
using SmartCodeReview.Mssql.Services.Models;

namespace SmartCodeReview.Mssql.Services.Services;

/// <summary>
/// Proje servisi
/// </summary>
public class ProjectService : IProjectService
{
    private readonly SmartCodeReviewDbContext _context;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(
        SmartCodeReviewDbContext context,
        ILogger<ProjectService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResult<Project>> CreateAsync(Project project)
    {
        try
        {
            // Aynı full name var mı kontrol et
            var exists = await _context.Projects
                .AnyAsync(p => p.FullName == project.FullName && !p.IsDeleted);

            if (exists)
            {
                return ServiceResult<Project>.Fail("Bu repository zaten eklenmiş", 400);
            }

            project.Id = Guid.NewGuid();
            project.CreatedDate = DateTime.UtcNow;
            project.IsActive = true;

            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Proje oluşturuldu: {Name} ({FullName})", project.Name, project.FullName);
            return ServiceResult<Project>.Created(project, "Proje başarıyla oluşturuldu");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Proje oluşturulurken hata");
            return ServiceResult<Project>.Fail("Proje oluşturulamadı", 500);
        }
    }

    public async Task<ServiceResult<Project>> GetByIdAsync(Guid id)
    {
        try
        {
            var project = await _context.Projects
                .Include(p => p.User)
                .Include(p => p.CodeReviews)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (project == null)
            {
                return ServiceResult<Project>.NotFound("Proje bulunamadı");
            }

            return ServiceResult<Project>.Success(project);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Proje getirilirken hata: {Id}", id);
            return ServiceResult<Project>.Fail("Proje getirilemedi", 500);
        }
    }

    public async Task<ServiceResult<PagedResult<Project>>> GetUserProjectsAsync(
        Guid userId, 
        int page = 1, 
        int pageSize = 10)
    {
        try
        {
            var query = _context.Projects
                .Include(p => p.User)
                .Where(p => p.UserId == userId && !p.IsDeleted);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedResult<Project>(items, totalCount, page, pageSize);
            return ServiceResult<PagedResult<Project>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kullanıcı projeleri listelenirken hata: {UserId}", userId);
            return ServiceResult<PagedResult<Project>>.Fail("Projeler listelenemedi", 500);
        }
    }

    public async Task<ServiceResult<Project>> GetByFullNameAsync(string fullName)
    {
        try
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.FullName == fullName && !p.IsDeleted && p.IsActive);

            if (project == null)
            {
                return ServiceResult<Project>.NotFound("Proje bulunamadı");
            }

            return ServiceResult<Project>.Success(project);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Proje getirilirken hata: {FullName}", fullName);
            return ServiceResult<Project>.Fail("Proje getirilemedi", 500);
        }
    }

    public async Task<ServiceResult<Project>> UpdateAsync(Project project)
    {
        try
        {
            project.UpdatedDate = DateTime.UtcNow;
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Proje güncellendi: {Id}", project.Id);
            return ServiceResult<Project>.Success(project, "Proje güncellendi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Proje güncellenirken hata: {Id}", project.Id);
            return ServiceResult<Project>.Fail("Proje güncellenemedi", 500);
        }
    }

    public async Task<ServiceResult> DeleteAsync(Guid id)
    {
        try
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return ServiceResult.Fail("Proje bulunamadı", 404);
            }

            project.IsDeleted = true;
            project.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Proje silindi (soft delete): {Id}", id);
            return ServiceResult.Success("Proje silindi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Proje silinirken hata: {Id}", id);
            return ServiceResult.Fail("Proje silinemedi", 500);
        }
    }

    public async Task<ServiceResult> ToggleActiveAsync(Guid id)
    {
        try
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return ServiceResult.Fail("Proje bulunamadı", 404);
            }

            project.IsActive = !project.IsActive;
            project.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Proje aktiflik durumu değişti: {Id}, Aktif: {IsActive}", id, project.IsActive);
            return ServiceResult.Success($"Proje {(project.IsActive ? "aktif" : "pasif")} edildi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Proje aktiflik durumu değiştirilirken hata: {Id}", id);
            return ServiceResult.Fail("Durum değiştirilemedi", 500);
        }
    }
}

