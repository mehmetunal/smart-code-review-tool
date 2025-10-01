using SmartCodeReview.Data.Mssql.Enums;

namespace SmartCodeReview.Data.Mssql;

/// <summary>
/// Dosya bazlı analiz entity'si
/// Her değiştirilen dosya için bir kayıt
/// </summary>
public class FileAnalysis : BaseEntity
{
    /// <summary>
    /// Dosya yolu
    /// </summary>
    public required string FilePath { get; set; }

    /// <summary>
    /// Dosya adı
    /// </summary>
    public required string FileName { get; set; }

    /// <summary>
    /// Programlama dili
    /// </summary>
    public ProgrammingLanguage Language { get; set; }

    /// <summary>
    /// Eklenen satır sayısı
    /// </summary>
    public int AddedLines { get; set; } = 0;

    /// <summary>
    /// Silinen satır sayısı
    /// </summary>
    public int DeletedLines { get; set; } = 0;

    /// <summary>
    /// Toplam değişiklik sayısı
    /// </summary>
    public int TotalChanges { get; set; } = 0;

    /// <summary>
    /// Dosya kalite skoru (0-100)
    /// </summary>
    public int? QualityScore { get; set; }

    /// <summary>
    /// Dosyada tespit edilen sorun sayısı
    /// </summary>
    public int IssuesCount { get; set; } = 0;

    /// <summary>
    /// Diff içeriği
    /// </summary>
    public string? DiffContent { get; set; }

    /// <summary>
    /// CodeReview ID
    /// </summary>
    public Guid CodeReviewId { get; set; }

    // Navigation property
    public virtual CodeReview? CodeReview { get; set; }
}

