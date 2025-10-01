using Microsoft.AspNetCore.Identity;

namespace SmartCodeReview.Data.Mssql;

/// <summary>
/// Rol entity'si
/// Microsoft.AspNetCore.Identity.IdentityRole'dan türetilmiştir
/// </summary>
public class Role : IdentityRole<Guid>
{
    /// <summary>
    /// Oluşturulma tarihi
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}

