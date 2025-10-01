using SmartCodeReview.Data.Mssql;
using SmartCodeReview.Data.Mssql.Enums;
using SmartCodeReview.Mssql.Services.Models;

namespace SmartCodeReview.Mssql.Services.Interfaces;

/// <summary>
/// Kod inceleme servisi interface'i
/// </summary>
public interface ICodeReviewService
{
    /// <summary>
    /// Kod incelemesi oluşturur
    /// </summary>
    Task<ServiceResult<CodeReview>> CreateAsync(CodeReview codeReview);

    /// <summary>
    /// Kod incelemesi getirir
    /// </summary>
    Task<ServiceResult<CodeReview>> GetByIdAsync(Guid id);

    /// <summary>
    /// Kod incelemelerini listeler
    /// </summary>
    Task<ServiceResult<PagedResult<CodeReview>>> GetAllAsync(
        int page = 1, 
        int pageSize = 10, 
        ReviewStatus? status = null,
        Guid? projectId = null);

    /// <summary>
    /// Kod incelemesini günceller
    /// </summary>
    Task<ServiceResult<CodeReview>> UpdateAsync(CodeReview codeReview);

    /// <summary>
    /// Kod inceleme durumunu günceller
    /// </summary>
    Task<ServiceResult> UpdateStatusAsync(Guid id, ReviewStatus status, string? errorMessage = null);

    /// <summary>
    /// Analiz sonuçlarını kaydeder
    /// </summary>
    Task<ServiceResult> SaveAnalysisResultsAsync(Guid codeReviewId, List<Analysis> analyses, List<FileAnalysis> fileAnalyses);

    /// <summary>
    /// Kalite skorunu hesaplar ve günceller
    /// </summary>
    Task<ServiceResult> CalculateQualityScoreAsync(Guid codeReviewId);
}

