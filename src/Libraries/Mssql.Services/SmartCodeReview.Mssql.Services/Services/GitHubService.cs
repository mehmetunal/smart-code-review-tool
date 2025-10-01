using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Octokit;
using SmartCodeReview.Mssql.Services.Interfaces;
using SmartCodeReview.Mssql.Services.Models;

namespace SmartCodeReview.Mssql.Services.Services;

/// <summary>
/// GitHub API servisi
/// </summary>
public class GitHubService : IGitHubService
{
    private readonly GitHubClient _client;
    private readonly ILogger<GitHubService> _logger;

    public GitHubService(IConfiguration configuration, ILogger<GitHubService> logger)
    {
        _logger = logger;
        
        // GitHub client oluştur
        _client = new GitHubClient(new ProductHeaderValue("SmartCodeReview"));
        
        // Token varsa ayarla
        var githubToken = configuration["GitHub:Token"];
        if (!string.IsNullOrEmpty(githubToken))
        {
            _client.Credentials = new Credentials(githubToken);
        }
    }

    public async Task<ServiceResult<string>> GetPullRequestDiffAsync(string owner, string repo, int pullRequestNumber)
    {
        try
        {
            var pullRequest = await _client.PullRequest.Get(owner, repo, pullRequestNumber);
            
            // Diff URL'den diff içeriğini al
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3.diff");
            
            var response = await httpClient.GetAsync(pullRequest.DiffUrl);
            if (response.IsSuccessStatusCode)
            {
                var diff = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("PR diff alındı: {Owner}/{Repo}#{Number}, Boyut: {Size} bytes", 
                    owner, repo, pullRequestNumber, diff.Length);
                
                return ServiceResult<string>.Success(diff);
            }

            return ServiceResult<string>.Fail("Diff alınamadı", 500);
        }
        catch (NotFoundException)
        {
            _logger.LogWarning("PR bulunamadı: {Owner}/{Repo}#{Number}", owner, repo, pullRequestNumber);
            return ServiceResult<string>.NotFound("Pull request bulunamadı");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PR diff alınırken hata: {Owner}/{Repo}#{Number}", owner, repo, pullRequestNumber);
            return ServiceResult<string>.Fail("Diff alınamadı", 500);
        }
    }

    public async Task<ServiceResult<PullRequestInfo>> GetPullRequestInfoAsync(string owner, string repo, int pullRequestNumber)
    {
        try
        {
            var pr = await _client.PullRequest.Get(owner, repo, pullRequestNumber);
            
            var info = new PullRequestInfo
            {
                Number = pr.Number,
                Title = pr.Title,
                Body = pr.Body,
                HeadBranch = pr.Head.Ref,
                BaseBranch = pr.Base.Ref,
                HeadSha = pr.Head.Sha,
                BaseSha = pr.Base.Sha,
                HtmlUrl = pr.HtmlUrl,
                State = pr.State.ToString(),
                CreatedAt = pr.CreatedAt.UtcDateTime,
                UpdatedAt = pr.UpdatedAt.UtcDateTime
            };

            _logger.LogInformation("PR bilgileri alındı: {Owner}/{Repo}#{Number}", owner, repo, pullRequestNumber);
            return ServiceResult<PullRequestInfo>.Success(info);
        }
        catch (NotFoundException)
        {
            return ServiceResult<PullRequestInfo>.NotFound("Pull request bulunamadı");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PR bilgileri alınırken hata: {Owner}/{Repo}#{Number}", owner, repo, pullRequestNumber);
            return ServiceResult<PullRequestInfo>.Fail("PR bilgileri alınamadı", 500);
        }
    }

    public async Task<ServiceResult> PostReviewCommentAsync(
        string owner, 
        string repo, 
        int pullRequestNumber, 
        string body,
        string? path = null,
        int? line = null)
    {
        try
        {
            // Genel yorum (issue comment olarak)
            await _client.Issue.Comment.Create(owner, repo, pullRequestNumber, body);

            _logger.LogInformation("PR'a yorum bırakıldı: {Owner}/{Repo}#{Number}", owner, repo, pullRequestNumber);
            return ServiceResult.Success("Yorum başarıyla bırakıldı");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PR'a yorum bırakılırken hata: {Owner}/{Repo}#{Number}", owner, repo, pullRequestNumber);
            return ServiceResult.Fail("Yorum bırakılamadı", 500);
        }
    }

    public async Task<ServiceResult<List<FileChange>>> GetChangedFilesAsync(string owner, string repo, int pullRequestNumber)
    {
        try
        {
            var files = await _client.PullRequest.Files(owner, repo, pullRequestNumber);
            
            var fileChanges = files.Select(f => new FileChange
            {
                FileName = f.FileName,
                Status = f.Status,
                Additions = f.Additions,
                Deletions = f.Deletions,
                Changes = f.Changes,
                Patch = f.Patch
            }).ToList();

            _logger.LogInformation("PR'da değiştirilen dosyalar alındı: {Owner}/{Repo}#{Number}, Dosya Sayısı: {Count}", 
                owner, repo, pullRequestNumber, fileChanges.Count);

            return ServiceResult<List<FileChange>>.Success(fileChanges);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Değiştirilen dosyalar alınırken hata: {Owner}/{Repo}#{Number}", owner, repo, pullRequestNumber);
            return ServiceResult<List<FileChange>>.Fail("Dosyalar alınamadı", 500);
        }
    }
}

