using SmartCodeReview.Data.Mssql.Enums;

namespace SmartCodeReview.Data.Mssql;

/// <summary>
/// Kod inceleme entity'si
/// </summary>
public class CodeReview : BaseEntity
{
    /// <summary>
    /// Pull Request numarası
    /// </summary>
    public int PullRequestNumber { get; set; }

    /// <summary>
    /// Pull Request başlığı
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Pull Request açıklaması
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Pull Request URL
    /// </summary>
    public required string PullRequestUrl { get; set; }

    /// <summary>
    /// Branch adı
    /// </summary>
    public required string BranchName { get; set; }

    /// <summary>
    /// İnceleme durumu
    /// </summary>
    public ReviewStatus Status { get; set; } = ReviewStatus.Pending;

    /// <summary>
    /// Kod kalite skoru (0-100)
    /// </summary>
    public int? QualityScore { get; set; }

    /// <summary>
    /// Toplam sorun sayısı
    /// </summary>
    public int TotalIssuesCount { get; set; } = 0;

    /// <summary>
    /// Kritik sorun sayısı
    /// </summary>
    public int CriticalIssuesCount { get; set; } = 0;

    /// <summary>
    /// Yüksek öncelikli sorun sayısı
    /// </summary>
    public int HighIssuesCount { get; set; } = 0;

    /// <summary>
    /// Orta öncelikli sorun sayısı
    /// </summary>
    public int MediumIssuesCount { get; set; } = 0;

    /// <summary>
    /// Düşük öncelikli sorun sayısı
    /// </summary>
    public int LowIssuesCount { get; set; } = 0;

    /// <summary>
    /// Analiz başlangıç zamanı
    /// </summary>
    public DateTime? AnalysisStartTime { get; set; }

    /// <summary>
    /// Analiz bitiş zamanı
    /// </summary>
    public DateTime? AnalysisEndTime { get; set; }

    /// <summary>
    /// Hata mesajı (varsa)
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Proje ID
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Kullanıcı ID (PR sahibi)
    /// </summary>
    public Guid? UserId { get; set; }

    // Navigation properties
    public virtual Project? Project { get; set; }
    public virtual User? User { get; set; }
    public virtual ICollection<Analysis> Analyses { get; set; } = new List<Analysis>();
    public virtual ICollection<FileAnalysis> FileAnalyses { get; set; } = new List<FileAnalysis>();
}

