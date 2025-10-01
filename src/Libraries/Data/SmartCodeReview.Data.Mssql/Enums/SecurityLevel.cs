namespace SmartCodeReview.Data.Mssql.Enums;

/// <summary>
/// Güvenlik seviyesi
/// </summary>
public enum SecurityLevel
{
    /// <summary>
    /// Bilgi - Güvenlik sorunu yok
    /// </summary>
    Info = 0,

    /// <summary>
    /// Düşük - Küçük güvenlik riski
    /// </summary>
    Low = 1,

    /// <summary>
    /// Orta - Orta düzeyde güvenlik riski
    /// </summary>
    Medium = 2,

    /// <summary>
    /// Yüksek - Yüksek güvenlik riski
    /// </summary>
    High = 3,

    /// <summary>
    /// Kritik - Kritik güvenlik açığı
    /// </summary>
    Critical = 4
}

