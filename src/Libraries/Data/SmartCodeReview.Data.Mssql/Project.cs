namespace SmartCodeReview.Data.Mssql;

/// <summary>
/// Proje entity'si (GitHub/GitLab repository)
/// </summary>
public class Project : BaseEntity
{
    /// <summary>
    /// Proje adı
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Proje açıklaması
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Repository URL (GitHub/GitLab)
    /// </summary>
    public required string RepositoryUrl { get; set; }

    /// <summary>
    /// Repository tam adı (örn: owner/repo)
    /// </summary>
    public required string FullName { get; set; }

    /// <summary>
    /// GitHub/GitLab Repository ID
    /// </summary>
    public long? RepositoryId { get; set; }

    /// <summary>
    /// Webhook secret key
    /// </summary>
    public string? WebhookSecret { get; set; }

    /// <summary>
    /// Aktif mi?
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Proje sahibi kullanıcı ID
    /// </summary>
    public Guid UserId { get; set; }

    // Navigation properties
    public virtual User? User { get; set; }
    public virtual ICollection<CodeReview> CodeReviews { get; set; } = new List<CodeReview>();
}

