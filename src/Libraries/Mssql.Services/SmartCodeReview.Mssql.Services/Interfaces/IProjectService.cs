using SmartCodeReview.Data.Mssql;
using SmartCodeReview.Mssql.Services.Models;

namespace SmartCodeReview.Mssql.Services.Interfaces;

/// <summary>
/// Proje servisi interface'i
/// </summary>
public interface IProjectService
{
    /// <summary>
    /// Proje oluşturur
    /// </summary>
    Task<ServiceResult<Project>> CreateAsync(Project project);

    /// <summary>
    /// Proje getirir
    /// </summary>
    Task<ServiceResult<Project>> GetByIdAsync(Guid id);

    /// <summary>
    /// Kullanıcının projelerini listeler
    /// </summary>
    Task<ServiceResult<PagedResult<Project>>> GetUserProjectsAsync(Guid userId, int page = 1, int pageSize = 10);

    /// <summary>
    /// Repository full name ile proje bulur
    /// </summary>
    Task<ServiceResult<Project>> GetByFullNameAsync(string fullName);

    /// <summary>
    /// Projeyi günceller
    /// </summary>
    Task<ServiceResult<Project>> UpdateAsync(Project project);

    /// <summary>
    /// Projeyi siler (soft delete)
    /// </summary>
    Task<ServiceResult> DeleteAsync(Guid id);

    /// <summary>
    /// Proje aktiflik durumunu değiştirir
    /// </summary>
    Task<ServiceResult> ToggleActiveAsync(Guid id);
}

