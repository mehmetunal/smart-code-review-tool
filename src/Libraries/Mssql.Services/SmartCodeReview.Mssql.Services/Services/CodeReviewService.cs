using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartCodeReview.Data.Mssql;
using SmartCodeReview.Data.Mssql.Enums;
using SmartCodeReview.Mssql;
using SmartCodeReview.Mssql.Services.Interfaces;
using SmartCodeReview.Mssql.Services.Models;

namespace SmartCodeReview.Mssql.Services.Services;

/// <summary>
/// Kod inceleme servisi
/// </summary>
public class CodeReviewService : ICodeReviewService
{
    private readonly SmartCodeReviewDbContext _context;
    private readonly ILogger<CodeReviewService> _logger;

    public CodeReviewService(
        SmartCodeReviewDbContext context,
        ILogger<CodeReviewService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResult<CodeReview>> CreateAsync(CodeReview codeReview)
    {
        try
        {
            codeReview.Id = Guid.NewGuid();
            codeReview.CreatedDate = DateTime.UtcNow;
            codeReview.Status = ReviewStatus.Pending;

            await _context.CodeReviews.AddAsync(codeReview);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Kod incelemesi oluşturuldu: {Id}, PR: {PullRequestNumber}", 
                codeReview.Id, codeReview.PullRequestNumber);

            return ServiceResult<CodeReview>.Created(codeReview, "Kod incelemesi oluşturuldu");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kod incelemesi oluşturulurken hata: {Message}", ex.Message);
            return ServiceResult<CodeReview>.Fail("Kod incelemesi oluşturulamadı", 500);
        }
    }

    public async Task<ServiceResult<CodeReview>> GetByIdAsync(Guid id)
    {
        try
        {
            var codeReview = await _context.CodeReviews
                .Include(c => c.Project)
                .Include(c => c.User)
                .Include(c => c.Analyses)
                .Include(c => c.FileAnalyses)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (codeReview == null)
            {
                return ServiceResult<CodeReview>.NotFound("Kod incelemesi bulunamadı");
            }

            return ServiceResult<CodeReview>.Success(codeReview);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kod incelemesi getirilirken hata: {Id}", id);
            return ServiceResult<CodeReview>.Fail("Kod incelemesi getirilemedi", 500);
        }
    }

    public async Task<ServiceResult<PagedResult<CodeReview>>> GetAllAsync(
        int page = 1, 
        int pageSize = 10, 
        ReviewStatus? status = null,
        Guid? projectId = null)
    {
        try
        {
            var query = _context.CodeReviews
                .Include(c => c.Project)
                .Include(c => c.User)
                .Where(c => !c.IsDeleted)
                .AsQueryable();

            // Filtreleme
            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            if (projectId.HasValue)
            {
                query = query.Where(c => c.ProjectId == projectId.Value);
            }

            // Toplam kayıt sayısı
            var totalCount = await query.CountAsync();

            // Sayfalama
            var items = await query
                .OrderByDescending(c => c.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedResult<CodeReview>(items, totalCount, page, pageSize);
            return ServiceResult<PagedResult<CodeReview>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kod incelemeleri listelenirken hata");
            return ServiceResult<PagedResult<CodeReview>>.Fail("Kod incelemeleri listelenemedi", 500);
        }
    }

    public async Task<ServiceResult<CodeReview>> UpdateAsync(CodeReview codeReview)
    {
        try
        {
            codeReview.UpdatedDate = DateTime.UtcNow;
            _context.CodeReviews.Update(codeReview);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Kod incelemesi güncellendi: {Id}", codeReview.Id);
            return ServiceResult<CodeReview>.Success(codeReview, "Kod incelemesi güncellendi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kod incelemesi güncellenirken hata: {Id}", codeReview.Id);
            return ServiceResult<CodeReview>.Fail("Kod incelemesi güncellenemedi", 500);
        }
    }

    public async Task<ServiceResult> UpdateStatusAsync(Guid id, ReviewStatus status, string? errorMessage = null)
    {
        try
        {
            var codeReview = await _context.CodeReviews.FindAsync(id);
            if (codeReview == null)
            {
                return ServiceResult.Fail("Kod incelemesi bulunamadı", 404);
            }

            codeReview.Status = status;
            codeReview.UpdatedDate = DateTime.UtcNow;

            if (status == ReviewStatus.Processing)
            {
                codeReview.AnalysisStartTime = DateTime.UtcNow;
            }
            else if (status == ReviewStatus.Completed || status == ReviewStatus.Failed)
            {
                codeReview.AnalysisEndTime = DateTime.UtcNow;
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                codeReview.ErrorMessage = errorMessage;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Kod inceleme durumu güncellendi: {Id}, Durum: {Status}", id, status);
            return ServiceResult.Success("Durum güncellendi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kod inceleme durumu güncellenirken hata: {Id}", id);
            return ServiceResult.Fail("Durum güncellenemedi", 500);
        }
    }

    public async Task<ServiceResult> SaveAnalysisResultsAsync(
        Guid codeReviewId, 
        List<Analysis> analyses, 
        List<FileAnalysis> fileAnalyses)
    {
        try
        {
            var codeReview = await _context.CodeReviews.FindAsync(codeReviewId);
            if (codeReview == null)
            {
                return ServiceResult.Fail("Kod incelemesi bulunamadı", 404);
            }

            // Analiz sonuçlarını ekle
            foreach (var analysis in analyses)
            {
                analysis.Id = Guid.NewGuid();
                analysis.CodeReviewId = codeReviewId;
                analysis.CreatedDate = DateTime.UtcNow;
                await _context.Analyses.AddAsync(analysis);
            }

            // Dosya analizlerini ekle
            foreach (var fileAnalysis in fileAnalyses)
            {
                fileAnalysis.Id = Guid.NewGuid();
                fileAnalysis.CodeReviewId = codeReviewId;
                fileAnalysis.CreatedDate = DateTime.UtcNow;
                await _context.FileAnalyses.AddAsync(fileAnalysis);
            }

            // İstatistikleri güncelle
            codeReview.TotalIssuesCount = analyses.Count;
            codeReview.CriticalIssuesCount = analyses.Count(a => a.Severity == SecurityLevel.Critical);
            codeReview.HighIssuesCount = analyses.Count(a => a.Severity == SecurityLevel.High);
            codeReview.MediumIssuesCount = analyses.Count(a => a.Severity == SecurityLevel.Medium);
            codeReview.LowIssuesCount = analyses.Count(a => a.Severity == SecurityLevel.Low);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Analiz sonuçları kaydedildi: {CodeReviewId}, Toplam Sorun: {Count}", 
                codeReviewId, analyses.Count);

            return ServiceResult.Success("Analiz sonuçları kaydedildi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Analiz sonuçları kaydedilirken hata: {CodeReviewId}", codeReviewId);
            return ServiceResult.Fail("Analiz sonuçları kaydedilemedi", 500);
        }
    }

    public async Task<ServiceResult> CalculateQualityScoreAsync(Guid codeReviewId)
    {
        try
        {
            var codeReview = await _context.CodeReviews
                .Include(c => c.Analyses)
                .FirstOrDefaultAsync(c => c.Id == codeReviewId);

            if (codeReview == null)
            {
                return ServiceResult.Fail("Kod incelemesi bulunamadı", 404);
            }

            // Basit bir kalite skoru hesaplama algoritması
            int baseScore = 100;
            int criticalPenalty = codeReview.CriticalIssuesCount * 20;
            int highPenalty = codeReview.HighIssuesCount * 10;
            int mediumPenalty = codeReview.MediumIssuesCount * 5;
            int lowPenalty = codeReview.LowIssuesCount * 2;

            int score = baseScore - criticalPenalty - highPenalty - mediumPenalty - lowPenalty;
            codeReview.QualityScore = Math.Max(0, Math.Min(100, score)); // 0-100 arası

            await _context.SaveChangesAsync();

            _logger.LogInformation("Kalite skoru hesaplandı: {CodeReviewId}, Skor: {Score}", 
                codeReviewId, codeReview.QualityScore);

            return ServiceResult.Success($"Kalite skoru: {codeReview.QualityScore}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kalite skoru hesaplanırken hata: {CodeReviewId}", codeReviewId);
            return ServiceResult.Fail("Kalite skoru hesaplanamadı", 500);
        }
    }
}

