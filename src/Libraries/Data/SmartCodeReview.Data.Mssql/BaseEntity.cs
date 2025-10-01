namespace SmartCodeReview.Data.Mssql;

/// <summary>
/// Tüm entity'ler için temel sınıf
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Primary key
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Oluşturulma tarihi
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Güncellenme tarihi
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Silinmiş mi? (Soft delete)
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Oluşturan kullanıcı ID
    /// </summary>
    public Guid? CreatorUserId { get; set; }

    /// <summary>
    /// Güncelleyen kullanıcı ID
    /// </summary>
    public Guid? UpdatedByUserId { get; set; }
}

