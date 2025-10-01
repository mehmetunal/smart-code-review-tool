namespace SmartCodeReview.Dto.Mssql.Project;

/// <summary>
/// Proje oluşturma DTO
/// </summary>
public class CreateProjectDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string RepositoryUrl { get; set; }
    public required string FullName { get; set; } // owner/repo
    public long? RepositoryId { get; set; }
    public string? WebhookSecret { get; set; }
}

