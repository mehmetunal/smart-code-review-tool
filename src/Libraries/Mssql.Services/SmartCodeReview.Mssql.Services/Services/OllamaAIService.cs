using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartCodeReview.Data.Mssql;
using SmartCodeReview.Data.Mssql.Enums;
using SmartCodeReview.Mssql.Services.Interfaces;
using SmartCodeReview.Mssql.Services.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace SmartCodeReview.Mssql.Services.Services;

/// <summary>
/// Ollama AI servisi (Tamamen ücretsiz, local AI)
/// Model: deepseek-coder, codellama, mistral
/// </summary>
public class OllamaAIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OllamaAIService> _logger;
    private readonly IApiConfigurationService _apiConfigService;
    private string _baseUrl = "http://localhost:11434";
    private string _model = "deepseek-coder";

    public OllamaAIService(
        IConfiguration configuration,
        ILogger<OllamaAIService> logger,
        IApiConfigurationService apiConfigService)
    {
        _logger = logger;
        _apiConfigService = apiConfigService;
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(_baseUrl);
        _httpClient.Timeout = TimeSpan.FromMinutes(5);
    }

    /// <summary>
    /// Ollama konfigürasyonunu database'den alır
    /// </summary>
    private async Task<bool> LoadOllamaConfigAsync()
    {
        try
        {
            var ollamaConfig = await _apiConfigService.GetActiveByTypeAsync("Ollama");
            if (ollamaConfig.IsSuccess)
            {
                _baseUrl = ollamaConfig.Data?.BaseUrl ?? "http://localhost:11434";
                _model = ollamaConfig.Data?.Model ?? "deepseek-coder";
                _httpClient.BaseAddress = new Uri(_baseUrl);
                return true;
            }
            
            _logger.LogWarning("Ollama konfigürasyonu bulunamadı, varsayılan değerler kullanılıyor");
            return true; // Varsayılan değerlerle devam et
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ollama konfigürasyonu alınamadı, varsayılan değerler kullanılıyor");
            return true; // Varsayılan değerlerle devam et
        }
    }

    public async Task<ServiceResult<List<Analysis>>> AnalyzeCodeChangesAsync(string diff, string fileName, string language)
    {
        try
        {
            // Ollama konfigürasyonunu yükle
            await LoadOllamaConfigAsync();
            var prompt = $@"Sen bir kod inceleme uzmanısın. Aşağıdaki kod değişikliklerini analiz et ve sorunları tespit et.

Dosya: {fileName}
Dil: {language}

Kod Değişiklikleri:
```
{diff}
```

Lütfen şu kategorilerde analiz yap:
1. Güvenlik açıkları (Security)
2. Performans sorunları (Performance)
3. Kod kalitesi (CodeQuality)
4. Best practices (BestPractices)
5. Potansiyel bug'lar (Bug)

Her sorun için şu formatta JSON döndür:
{{
  ""issues"": [
    {{
      ""title"": ""Sorun başlığı"",
      ""description"": ""Detaylı açıklama"",
      ""category"": ""Security|Performance|CodeQuality|BestPractices|Bug"",
      ""severity"": ""Critical|High|Medium|Low|Info"",
      ""lineNumber"": 10,
      ""suggestion"": ""Önerilen çözüm""
    }}
  ]
}}";

            var response = await CallOllamaAsync(prompt);
            
            if (string.IsNullOrEmpty(response))
            {
                return ServiceResult<List<Analysis>>.Fail("AI servisi yanıt vermedi", 500);
            }

            // JSON response'u parse et
            var analyses = ParseAIResponse(response, fileName);
            
            _logger.LogInformation("AI analizi tamamlandı: {Count} sorun tespit edildi", analyses.Count);
            return ServiceResult<List<Analysis>>.Success(analyses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI kod analizi sırasında hata");
            return ServiceResult<List<Analysis>>.Fail("AI analizi başarısız", 500);
        }
    }

    public async Task<ServiceResult<CodeQualityResult>> EvaluateCodeQualityAsync(string code, string language)
    {
        try
        {
            var prompt = $@"Sen bir kod kalite değerlendirme uzmanısın. Aşağıdaki kodu değerlendir.

Dil: {language}

Kod:
```
{code}
```

Lütfen şu formatta JSON döndür:
{{
  ""score"": 85,
  ""summary"": ""Genel değerlendirme"",
  ""strengths"": [""İyi yönler""],
  ""weaknesses"": [""Zayıf yönler""],
  ""suggestions"": [""Öneriler""]
}}";

            var response = await CallOllamaAsync(prompt);
            
            if (string.IsNullOrEmpty(response))
            {
                return ServiceResult<CodeQualityResult>.Fail("AI servisi yanıt vermedi", 500);
            }

            var result = JsonSerializer.Deserialize<CodeQualityResult>(response) ?? new CodeQualityResult();
            
            return ServiceResult<CodeQualityResult>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kod kalite değerlendirmesi sırasında hata");
            return ServiceResult<CodeQualityResult>.Fail("Kalite değerlendirmesi başarısız", 500);
        }
    }

    public async Task<ServiceResult<List<Analysis>>> DetectSecurityIssuesAsync(string code, string language)
    {
        try
        {
            var prompt = $@"Sen bir güvenlik uzmanısın. Aşağıdaki kodda güvenlik açıklarını tespit et.

Dil: {language}

Kod:
```
{code}
```

Güvenlik kontrolleri:
- SQL Injection
- XSS (Cross-Site Scripting)
- CSRF
- Authentication/Authorization sorunları
- Hassas veri sızıntısı
- Kriptografi hataları

JSON formatında döndür.";

            var response = await CallOllamaAsync(prompt);
            var analyses = ParseAIResponse(response, "security_check");
            
            return ServiceResult<List<Analysis>>.Success(analyses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Güvenlik analizi sırasında hata");
            return ServiceResult<List<Analysis>>.Fail("Güvenlik analizi başarısız", 500);
        }
    }

    public async Task<ServiceResult<List<Analysis>>> SuggestPerformanceImprovementsAsync(string code, string language)
    {
        try
        {
            var prompt = $@"Sen bir performans optimizasyon uzmanısın. Aşağıdaki kodda performans iyileştirmeleri öner.

Dil: {language}

Kod:
```
{code}
```

Performans kontrolleri:
- N+1 query problemi
- Gereksiz döngüler
- Memory leak'ler
- Async/await kullanımı
- Caching fırsatları

JSON formatında döndür.";

            var response = await CallOllamaAsync(prompt);
            var analyses = ParseAIResponse(response, "performance_check");
            
            return ServiceResult<List<Analysis>>.Success(analyses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Performans analizi sırasında hata");
            return ServiceResult<List<Analysis>>.Fail("Performans analizi başarısız", 500);
        }
    }

    /// <summary>
    /// Ollama API'ye istek gönderir
    /// </summary>
    private async Task<string> CallOllamaAsync(string prompt)
    {
        try
        {
            var request = new
            {
                model = _model,
                prompt = prompt,
                stream = false
            };

            var response = await _httpClient.PostAsJsonAsync("/api/generate", request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            return result.GetProperty("response").GetString() ?? string.Empty;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning("Ollama bağlantı hatası (Local AI çalışmıyor olabilir): {Message}", ex.Message);
            return string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ollama API çağrısı başarısız");
            return string.Empty;
        }
    }

    /// <summary>
    /// AI response'unu parse eder
    /// </summary>
    private List<Analysis> ParseAIResponse(string response, string filePath)
    {
        var analyses = new List<Analysis>();

        try
        {
            // JSON içinden issues array'ini bul
            var jsonStart = response.IndexOf('{');
            var jsonEnd = response.LastIndexOf('}') + 1;
            
            if (jsonStart < 0 || jsonEnd <= jsonStart)
            {
                _logger.LogWarning("AI response'da geçerli JSON bulunamadı");
                return analyses;
            }

            var jsonStr = response.Substring(jsonStart, jsonEnd - jsonStart);
            var doc = JsonDocument.Parse(jsonStr);
            
            if (doc.RootElement.TryGetProperty("issues", out var issues))
            {
                foreach (var issue in issues.EnumerateArray())
                {
                    var analysis = new Analysis
                    {
                        Title = issue.GetProperty("title").GetString() ?? "Bilinmeyen sorun",
                        Description = issue.GetProperty("description").GetString() ?? "",
                        Category = ParseCategory(issue.GetProperty("category").GetString()),
                        Severity = ParseSeverity(issue.GetProperty("severity").GetString()),
                        FilePath = filePath,
                        LineNumber = issue.TryGetProperty("lineNumber", out var line) ? line.GetInt32() : null,
                        Suggestion = issue.TryGetProperty("suggestion", out var sug) ? sug.GetString() : null,
                        IsAIGenerated = true
                    };

                    analyses.Add(analysis);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI response parse edilirken hata");
        }

        return analyses;
    }

    private IssueCategory ParseCategory(string? category)
    {
        return category?.ToLower() switch
        {
            "security" => IssueCategory.Security,
            "performance" => IssueCategory.Performance,
            "codequality" => IssueCategory.CodeQuality,
            "bestpractices" => IssueCategory.BestPractices,
            "bug" => IssueCategory.Bug,
            _ => IssueCategory.Style
        };
    }

    private SecurityLevel ParseSeverity(string? severity)
    {
        return severity?.ToLower() switch
        {
            "critical" => SecurityLevel.Critical,
            "high" => SecurityLevel.High,
            "medium" => SecurityLevel.Medium,
            "low" => SecurityLevel.Low,
            _ => SecurityLevel.Info
        };
    }
}

