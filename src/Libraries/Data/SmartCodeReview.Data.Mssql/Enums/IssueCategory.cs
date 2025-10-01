namespace SmartCodeReview.Data.Mssql.Enums;

/// <summary>
/// Sorun kategorisi
/// </summary>
public enum IssueCategory
{
    /// <summary>
    /// Güvenlik - Güvenlik açıkları
    /// </summary>
    Security = 0,

    /// <summary>
    /// Performans - Performans sorunları
    /// </summary>
    Performance = 1,

    /// <summary>
    /// Kod Kalitesi - Kod okunabilirliği ve maintainability
    /// </summary>
    CodeQuality = 2,

    /// <summary>
    /// Best Practices - En iyi pratiklere uyumsuzluk
    /// </summary>
    BestPractices = 3,

    /// <summary>
    /// Hata - Potansiyel bug'lar
    /// </summary>
    Bug = 4,

    /// <summary>
    /// Stil - Kod stil kurallarına uyumsuzluk
    /// </summary>
    Style = 5
}

