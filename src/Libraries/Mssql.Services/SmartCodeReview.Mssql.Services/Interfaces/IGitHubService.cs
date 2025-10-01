using SmartCodeReview.Mssql.Services.Models;

namespace SmartCodeReview.Mssql.Services.Interfaces;

/// <summary>
/// GitHub API servisi interface'i
/// </summary>
public interface IGitHubService
{
    /// <summary>
    /// Pull request diff'ini alır
    /// </summary>
    Task<ServiceResult<string>> GetPullRequestDiffAsync(string owner, string repo, int pullRequestNumber);

    /// <summary>
    /// Pull request bilgilerini alır
    /// </summary>
    Task<ServiceResult<PullRequestInfo>> GetPullRequestInfoAsync(string owner, string repo, int pullRequestNumber);

    /// <summary>
    /// Pull request'e yorum bırakır
    /// </summary>
    Task<ServiceResult> PostReviewCommentAsync(
        string owner, 
        string repo, 
        int pullRequestNumber, 
        string body,
        string? path = null,
        int? line = null);

    /// <summary>
    /// Değiştirilen dosyaları listeler
    /// </summary>
    Task<ServiceResult<List<FileChange>>> GetChangedFilesAsync(string owner, string repo, int pullRequestNumber);
}

/// <summary>
/// Pull request bilgileri
/// </summary>
public class PullRequestInfo
{
    public int Number { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Body { get; set; }
    public string HeadBranch { get; set; } = string.Empty;
    public string BaseBranch { get; set; } = string.Empty;
    public string HeadSha { get; set; } = string.Empty;
    public string BaseSha { get; set; } = string.Empty;
    public string HtmlUrl { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Değiştirilen dosya bilgisi
/// </summary>
public class FileChange
{
    public string FileName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // added, modified, removed
    public int Additions { get; set; }
    public int Deletions { get; set; }
    public int Changes { get; set; }
    public string? Patch { get; set; }
}

