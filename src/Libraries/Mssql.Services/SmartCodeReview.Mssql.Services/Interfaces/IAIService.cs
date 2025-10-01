using SmartCodeReview.Data.Mssql;
using SmartCodeReview.Mssql.Services.Models;

namespace SmartCodeReview.Mssql.Services.Interfaces;

/// <summary>
/// AI kod analiz servisi interface'i
/// Birden fazla AI provider destekler (Ollama, Gemini, vb.)
/// </summary>
public interface IAIService
{
    /// <summary>
    /// Kod değişikliklerini analiz eder
    /// </summary>
    Task<ServiceResult<List<Analysis>>> AnalyzeCodeChangesAsync(string diff, string fileName, string language);

    /// <summary>
    /// Kod kalitesini değerlendirir
    /// </summary>
    Task<ServiceResult<CodeQualityResult>> EvaluateCodeQualityAsync(string code, string language);

    /// <summary>
    /// Güvenlik açıklarını tespit eder
    /// </summary>
    Task<ServiceResult<List<Analysis>>> DetectSecurityIssuesAsync(string code, string language);

    /// <summary>
    /// Performans önerileri sunar
    /// </summary>
    Task<ServiceResult<List<Analysis>>> SuggestPerformanceImprovementsAsync(string code, string language);
}

/// <summary>
/// Kod kalite sonucu
/// </summary>
public class CodeQualityResult
{
    public int Score { get; set; } // 0-100
    public string Summary { get; set; } = string.Empty;
    public List<string> Strengths { get; set; } = new();
    public List<string> Weaknesses { get; set; } = new();
    public List<string> Suggestions { get; set; } = new();
}

