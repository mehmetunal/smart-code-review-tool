namespace SmartCodeReview.Data.Mssql.Enums;

/// <summary>
/// Kod kalite skoru (0-100 arası)
/// </summary>
public enum CodeQualityScore
{
    /// <summary>
    /// Çok Kötü (0-20)
    /// </summary>
    VeryPoor = 0,

    /// <summary>
    /// Kötü (21-40)
    /// </summary>
    Poor = 1,

    /// <summary>
    /// Orta (41-60)
    /// </summary>
    Fair = 2,

    /// <summary>
    /// İyi (61-80)
    /// </summary>
    Good = 3,

    /// <summary>
    /// Mükemmel (81-100)
    /// </summary>
    Excellent = 4
}

