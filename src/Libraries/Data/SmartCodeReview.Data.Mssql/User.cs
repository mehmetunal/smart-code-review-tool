using Microsoft.AspNetCore.Identity;

namespace SmartCodeReview.Data.Mssql;

/// <summary>
/// Kullanıcı entity'si
/// Microsoft.AspNetCore.Identity.IdentityUser'dan türetilmiştir
/// </summary>
public class User : IdentityUser<Guid>
{
    /// <summary>
    /// Kullanıcı adı
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Kullanıcı soyadı
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Oluşturulma tarihi
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Güncellenme tarihi
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Silinmiş mi?
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// GitHub kullanıcı adı
    /// </summary>
    public string? GitHubUsername { get; set; }

    /// <summary>
    /// GitLab kullanıcı adı
    /// </summary>
    public string? GitLabUsername { get; set; }

    // Navigation properties
    public virtual ICollection<CodeReview> CodeReviews { get; set; } = new List<CodeReview>();
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}

