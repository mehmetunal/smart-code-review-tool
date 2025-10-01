using SmartCodeReview.Data.Mssql.Enums;

namespace SmartCodeReview.Data.Mssql;

/// <summary>
/// Analiz sonucu entity'si
/// Her bir tespit edilen sorun için bir kayıt
/// </summary>
public class Analysis : BaseEntity
{
    /// <summary>
    /// Sorun başlığı
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Sorun açıklaması
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Sorun kategorisi
    /// </summary>
    public IssueCategory Category { get; set; }

    /// <summary>
    /// Güvenlik seviyesi
    /// </summary>
    public SecurityLevel Severity { get; set; }

    /// <summary>
    /// Dosya yolu
    /// </summary>
    public required string FilePath { get; set; }

    /// <summary>
    /// Satır numarası
    /// </summary>
    public int? LineNumber { get; set; }

    /// <summary>
    /// Kod snippet
    /// </summary>
    public string? CodeSnippet { get; set; }

    /// <summary>
    /// Önerilen çözüm
    /// </summary>
    public string? Suggestion { get; set; }

    /// <summary>
    /// AI tarafından üretildi mi?
    /// </summary>
    public bool IsAIGenerated { get; set; } = true;

    /// <summary>
    /// GitHub/GitLab yorum ID'si
    /// </summary>
    public long? CommentId { get; set; }

    /// <summary>
    /// CodeReview ID
    /// </summary>
    public Guid CodeReviewId { get; set; }

    // Navigation property
    public virtual CodeReview? CodeReview { get; set; }
}

