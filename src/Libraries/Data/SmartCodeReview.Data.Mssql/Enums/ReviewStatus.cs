namespace SmartCodeReview.Data.Mssql.Enums;

/// <summary>
/// Kod inceleme durumu
/// </summary>
public enum ReviewStatus
{
    /// <summary>
    /// Beklemede - Webhook alındı, henüz işleme alınmadı
    /// </summary>
    Pending = 0,

    /// <summary>
    /// İşleniyor - AI analizi yapılıyor
    /// </summary>
    Processing = 1,

    /// <summary>
    /// Tamamlandı - Analiz tamamlandı ve yorumlar bırakıldı
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Başarısız - Analiz sırasında hata oluştu
    /// </summary>
    Failed = 3,

    /// <summary>
    /// İptal edildi - Kullanıcı tarafından iptal edildi
    /// </summary>
    Cancelled = 4
}

