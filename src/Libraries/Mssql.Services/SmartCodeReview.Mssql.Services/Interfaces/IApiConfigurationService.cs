using SmartCodeReview.Data.Mssql;
using SmartCodeReview.Mssql.Services.Models;

namespace SmartCodeReview.Mssql.Services.Interfaces;

/// <summary>
/// API konfigürasyon servisi interface'i
/// </summary>
public interface IApiConfigurationService
{
    /// <summary>
    /// API konfigürasyonu oluşturur
    /// </summary>
    Task<ServiceResult<ApiConfiguration>> CreateAsync(ApiConfiguration apiConfig);

    /// <summary>
    /// API konfigürasyonu günceller
    /// </summary>
    Task<ServiceResult<ApiConfiguration>> UpdateAsync(ApiConfiguration apiConfig);

    /// <summary>
    /// API konfigürasyonu siler
    /// </summary>
    Task<ServiceResult<bool>> DeleteAsync(Guid id);

    /// <summary>
    /// API konfigürasyonu getirir
    /// </summary>
    Task<ServiceResult<ApiConfiguration>> GetByIdAsync(Guid id);

    /// <summary>
    /// API türüne göre aktif konfigürasyonu getirir
    /// </summary>
    Task<ServiceResult<ApiConfiguration>> GetActiveByTypeAsync(string apiType);

    /// <summary>
    /// Kullanıcının API konfigürasyonlarını listeler
    /// </summary>
    Task<ServiceResult<PagedResult<ApiConfiguration>>> GetUserConfigurationsAsync(
        Guid userId, int page = 1, int pageSize = 10);

    /// <summary>
    /// Tüm aktif API konfigürasyonlarını getirir
    /// </summary>
    Task<ServiceResult<List<ApiConfiguration>>> GetAllActiveAsync();

    /// <summary>
    /// API key'i şifreler
    /// </summary>
    string EncryptApiKey(string apiKey);

    /// <summary>
    /// API key'i çözer
    /// </summary>
    string DecryptApiKey(string encryptedApiKey);

    /// <summary>
    /// Varsayılan konfigürasyonları oluşturur
    /// </summary>
    Task<ServiceResult<bool>> CreateDefaultConfigurationsAsync();
}
