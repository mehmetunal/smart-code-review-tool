using SmartCodeReview.Data.Mssql;
using SmartCodeReview.Data.Mssql.Enums;
using SmartCodeReview.Mssql.Services.Interfaces;
using System.Text.Json;

namespace SmartCodeReview.Api.BackgroundServices;

/// <summary>
/// Webhook işleme background worker'ı
/// Redis kuyruğundan webhook'ları alır ve işler
/// </summary>
public class WebhookProcessorWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<WebhookProcessorWorker> _logger;
    private readonly IConfiguration _configuration;

    public WebhookProcessorWorker(
        IServiceProvider serviceProvider,
        ILogger<WebhookProcessorWorker> logger,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var isEnabled = _configuration.GetValue<bool>("BackgroundJobs:EnableWebhookQueue", true);
        if (!isEnabled)
        {
            _logger.LogInformation("Webhook queue worker devre dışı");
            return;
        }

        _logger.LogInformation("🚀 Webhook processor worker başlatıldı");

        var intervalSeconds = _configuration.GetValue<int>("BackgroundJobs:QueueCheckIntervalSeconds", 5);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var queueService = scope.ServiceProvider.GetRequiredService<IWebhookQueueService>();
                var codeReviewService = scope.ServiceProvider.GetRequiredService<ICodeReviewService>();
                var githubService = scope.ServiceProvider.GetRequiredService<IGitHubService>();
                var aiService = scope.ServiceProvider.GetRequiredService<IAIService>();
                var projectService = scope.ServiceProvider.GetRequiredService<IProjectService>();

                // Kuyruktan webhook al
                var message = await queueService.DequeueAsync();
                
                if (message != null)
                {
                    await ProcessWebhookAsync(message, codeReviewService, githubService, aiService, projectService);
                }
                else
                {
                    // Kuyruk boş, biraz bekle
                    await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Webhook işlenirken hata oluştu");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        _logger.LogInformation("🛑 Webhook processor worker durduruluyor");
    }

    private async Task ProcessWebhookAsync(
        SmartCodeReview.Mssql.Services.Models.WebhookQueueMessage message,
        ICodeReviewService codeReviewService,
        IGitHubService githubService,
        IAIService aiService,
        IProjectService projectService)
    {
        try
        {
            _logger.LogInformation(
                "📝 Webhook işleniyor: {Source} - {Owner}/{Repo}#{Number}", 
                message.Source, message.Owner, message.Repository, message.PullRequestNumber);

            // 1. PR bilgilerini GitHub'dan al
            var prInfoResult = await githubService.GetPullRequestInfoAsync(
                message.Owner, 
                message.Repository, 
                message.PullRequestNumber);

            if (!prInfoResult.IsSuccess || prInfoResult.Data == null)
            {
                _logger.LogWarning("PR bilgileri alınamadı: {Message}", prInfoResult.Message);
                return;
            }

            var prInfo = prInfoResult.Data;

            // 2. Project'i bul veya oluştur
            var fullName = $"{message.Owner}/{message.Repository}";
            var projectResult = await projectService.GetByFullNameAsync(fullName);
            
            Project project;
            if (!projectResult.IsSuccess)
            {
                // Proje yoksa oluştur
                project = new Project
                {
                    Name = message.Repository,
                    RepositoryUrl = $"https://github.com/{fullName}",
                    FullName = fullName,
                    UserId = Guid.Empty, // System user (admin olabilir)
                    CreatedDate = DateTime.UtcNow
                };

                var createProjectResult = await projectService.CreateAsync(project);
                if (!createProjectResult.IsSuccess)
                {
                    _logger.LogError("Proje oluşturulamadı: {FullName}", fullName);
                    return;
                }
                project = createProjectResult.Data!;
                _logger.LogInformation("✅ Yeni proje oluşturuldu: {FullName}", fullName);
            }
            else
            {
                project = projectResult.Data!;
            }

            // 3. CodeReview entity oluştur
            var codeReview = new CodeReview
            {
                PullRequestNumber = prInfo.Number,
                Title = prInfo.Title,
                Description = prInfo.Body,
                PullRequestUrl = prInfo.HtmlUrl,
                BranchName = prInfo.HeadBranch,
                Status = ReviewStatus.Pending,
                ProjectId = project.Id,
                CreatedDate = DateTime.UtcNow
            };

            var createResult = await codeReviewService.CreateAsync(codeReview);
            
            if (!createResult.IsSuccess)
            {
                _logger.LogError("CodeReview oluşturulamadı: {Message}", createResult.Message);
                return;
            }

            _logger.LogInformation("✅ CodeReview oluşturuldu: {Id}", createResult.Data!.Id);

            // 4. Durumu Processing olarak güncelle
            await codeReviewService.UpdateStatusAsync(createResult.Data.Id, ReviewStatus.Processing);

            // 5. Değiştirilen dosyaları al
            var filesResult = await githubService.GetChangedFilesAsync(
                message.Owner, 
                message.Repository, 
                message.PullRequestNumber);

            if (!filesResult.IsSuccess || filesResult.Data == null)
            {
                await codeReviewService.UpdateStatusAsync(createResult.Data.Id, ReviewStatus.Failed, "Dosya değişiklikleri alınamadı");
                return;
            }

            _logger.LogInformation("📁 {Count} dosya değişikliği tespit edildi", filesResult.Data.Count);

            // 6. Her dosya için AI analizi yap
            var allAnalyses = new List<Analysis>();
            var allFileAnalyses = new List<FileAnalysis>();

            foreach (var file in filesResult.Data)
            {
                if (string.IsNullOrEmpty(file.Patch))
                    continue;

                _logger.LogInformation("🤖 AI analizi yapılıyor: {FileName}", file.FileName);

                // AI ile analiz yap
                var analysisResult = await aiService.AnalyzeCodeChangesAsync(
                    file.Patch, 
                    file.FileName, 
                    GetLanguageFromFileName(file.FileName));

                if (analysisResult.IsSuccess && analysisResult.Data != null)
                {
                    allAnalyses.AddRange(analysisResult.Data);
                    _logger.LogInformation("✅ {Count} sorun tespit edildi: {FileName}", 
                        analysisResult.Data.Count, file.FileName);
                }

                // FileAnalysis oluştur
                var fileAnalysis = new FileAnalysis
                {
                    FilePath = file.FileName,
                    FileName = Path.GetFileName(file.FileName),
                    Language = GetProgrammingLanguage(file.FileName),
                    AddedLines = file.Additions,
                    DeletedLines = file.Deletions,
                    TotalChanges = file.Changes,
                    IssuesCount = analysisResult.Data?.Count ?? 0,
                    DiffContent = file.Patch
                };

                allFileAnalyses.Add(fileAnalysis);
            }

            // 7. Analiz sonuçlarını kaydet
            if (allAnalyses.Any() || allFileAnalyses.Any())
            {
                await codeReviewService.SaveAnalysisResultsAsync(
                    createResult.Data.Id, 
                    allAnalyses, 
                    allFileAnalyses);

                // Kalite skorunu hesapla
                await codeReviewService.CalculateQualityScoreAsync(createResult.Data.Id);
            }

            // 8. PR'a özet yorum bırak
            var summary = $@"## 🤖 AI Kod İnceleme Sonuçları

**Toplam Sorun:** {allAnalyses.Count}
- 🔴 Kritik: {allAnalyses.Count(a => a.Severity == SecurityLevel.Critical)}
- 🟠 Yüksek: {allAnalyses.Count(a => a.Severity == SecurityLevel.High)}
- 🟡 Orta: {allAnalyses.Count(a => a.Severity == SecurityLevel.Medium)}
- 🔵 Düşük: {allAnalyses.Count(a => a.Severity == SecurityLevel.Low)}

**İncelenen Dosya:** {allFileAnalyses.Count}

{(allAnalyses.Any() ? "Detaylar için kod satırlarına bırakılan yorumlara bakın." : "✅ Sorun tespit edilmedi!")}";

            await githubService.PostReviewCommentAsync(
                message.Owner,
                message.Repository,
                message.PullRequestNumber,
                summary);

            // 9. Her sorun için satıra özel yorum bırak (ilk 10 sorun)
            var topIssues = allAnalyses
                .Where(a => a.Severity >= SecurityLevel.Medium)
                .OrderByDescending(a => a.Severity)
                .Take(10);

            foreach (var issue in topIssues)
            {
                var comment = $@"**{GetSeverityEmoji(issue.Severity)} {issue.Title}**

{issue.Description}

**Kategori:** {issue.Category}
**Öneri:** {issue.Suggestion ?? "N/A"}";

                await githubService.PostReviewCommentAsync(
                    message.Owner,
                    message.Repository,
                    message.PullRequestNumber,
                    comment,
                    issue.FilePath,
                    issue.LineNumber);

                await Task.Delay(500); // Rate limit için
            }

            // 10. Durumu Completed olarak güncelle
            await codeReviewService.UpdateStatusAsync(createResult.Data.Id, ReviewStatus.Completed);

            _logger.LogInformation("✅ Webhook işleme tamamlandı: {Id}", createResult.Data.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Webhook işleme hatası: {Owner}/{Repo}#{Number}", 
                message.Owner, message.Repository, message.PullRequestNumber);
        }
    }

    private string GetLanguageFromFileName(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLower();
        return extension switch
        {
            ".cs" => "C#",
            ".js" => "JavaScript",
            ".ts" => "TypeScript",
            ".py" => "Python",
            ".java" => "Java",
            ".go" => "Go",
            ".rs" => "Rust",
            ".php" => "PHP",
            ".rb" => "Ruby",
            _ => "Unknown"
        };
    }

    private ProgrammingLanguage GetProgrammingLanguage(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLower();
        return extension switch
        {
            ".cs" => ProgrammingLanguage.CSharp,
            ".js" => ProgrammingLanguage.JavaScript,
            ".ts" => ProgrammingLanguage.TypeScript,
            ".py" => ProgrammingLanguage.Python,
            ".java" => ProgrammingLanguage.Java,
            ".go" => ProgrammingLanguage.Go,
            ".rs" => ProgrammingLanguage.Rust,
            ".php" => ProgrammingLanguage.PHP,
            ".rb" => ProgrammingLanguage.Ruby,
            _ => ProgrammingLanguage.Other
        };
    }

    private string GetSeverityEmoji(SecurityLevel severity)
    {
        return severity switch
        {
            SecurityLevel.Critical => "🔴",
            SecurityLevel.High => "🟠",
            SecurityLevel.Medium => "🟡",
            SecurityLevel.Low => "🔵",
            _ => "ℹ️"
        };
    }
}

