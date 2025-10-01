namespace SmartCodeReview.Data.Mssql.Enums;

/// <summary>
/// API türleri
/// </summary>
public enum ApiType
{
    /// <summary>
    /// GitHub API
    /// </summary>
    GitHub = 1,

    /// <summary>
    /// Google Gemini
    /// </summary>
    Gemini = 2,

    /// <summary>
    /// Hugging Face
    /// </summary>
    HuggingFace = 3,

    /// <summary>
    /// OpenAI
    /// </summary>
    OpenAI = 4,

    /// <summary>
    /// Ollama (local)
    /// </summary>
    Ollama = 5,

    /// <summary>
    /// GitLab
    /// </summary>
    GitLab = 6
}
