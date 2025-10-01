using SmartCodeReview.Data.Mssql.Enums;

namespace SmartCodeReview.Data.Mssql;

/// <summary>
/// API konfigürasyonları için entity
/// </summary>
public class ApiConfiguration : BaseEntity
{
    /// <summary>
    /// API türü (GitHub, Gemini, HuggingFace, vb.)
    /// </summary>
    public string ApiType { get; set; } = string.Empty;

    /// <summary>
    /// API adı (GitHub, Google Gemini, Hugging Face)
    /// </summary>
    public string ApiName { get; set; } = string.Empty;

    /// <summary>
    /// API key (şifrelenmiş)
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Webhook secret (GitHub için)
    /// </summary>
    public string? WebhookSecret { get; set; }

    /// <summary>
    /// Model adı (AI servisleri için)
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Base URL (API endpoint)
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Aktif mi?
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Varsayılan mı?
    /// </summary>
    public bool IsDefault { get; set; } = false;

    /// <summary>
    /// Açıklama
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Kullanıcı ID (hangi kullanıcıya ait)
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Kullanıcı navigation property
    /// </summary>
    public virtual User? User { get; set; }
}
