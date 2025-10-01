using SmartCodeReview.Data.Mssql.Enums;

namespace SmartCodeReview.Dto.Mssql.CodeReview;

/// <summary>
/// Kod inceleme liste DTO
/// </summary>
public class CodeReviewListDto
{
    public Guid Id { get; set; }
    public int PullRequestNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public ReviewStatus Status { get; set; }
    public int? QualityScore { get; set; }
    public int TotalIssuesCount { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}

