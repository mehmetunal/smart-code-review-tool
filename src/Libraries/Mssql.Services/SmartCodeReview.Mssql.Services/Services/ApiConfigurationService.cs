using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartCodeReview.Data.Mssql;
using SmartCodeReview.Mssql.Services.Interfaces;
using SmartCodeReview.Mssql.Services.Models;
using System.Security.Cryptography;
using System.Text;

namespace SmartCodeReview.Mssql.Services.Services;

/// <summary>
/// API konfigürasyon servisi
/// </summary>
public class ApiConfigurationService : IApiConfigurationService
{
    private readonly SmartCodeReviewDbContext _context;
    private readonly ILogger<ApiConfigurationService> _logger;
    private readonly string _encryptionKey;

    public ApiConfigurationService(
        SmartCodeReviewDbContext context,
        ILogger<ApiConfigurationService> logger,
        IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _encryptionKey = configuration["Encryption:Key"] ?? "SmartCodeReview2025!SecretKey123";
    }

    public async Task<ServiceResult<ApiConfiguration>> CreateAsync(ApiConfiguration apiConfig)
    {
        try
        {
            // API key'i şifrele
            apiConfig.ApiKey = EncryptApiKey(apiConfig.ApiKey);
            
            if (!string.IsNullOrEmpty(apiConfig.WebhookSecret))
            {
                apiConfig.WebhookSecret = EncryptApiKey(apiConfig.WebhookSecret);
            }

            _context.ApiConfigurations.Add(apiConfig);
            await _context.SaveChangesAsync();

            _logger.LogInformation("API konfigürasyonu oluşturuldu: {ApiType}", apiConfig.ApiType);
            return ServiceResult<ApiConfiguration>.Success(apiConfig);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API konfigürasyonu oluşturulamadı");
            return ServiceResult<ApiConfiguration>.Fail("API konfigürasyonu oluşturulamadı", 500);
        }
    }

    public async Task<ServiceResult<ApiConfiguration>> UpdateAsync(ApiConfiguration apiConfig)
    {
        try
        {
            var existing = await _context.ApiConfigurations.FindAsync(apiConfig.Id);
            if (existing == null)
            {
                return ServiceResult<ApiConfiguration>.Fail("API konfigürasyonu bulunamadı", 404);
            }

            // API key'i şifrele (eğer değiştiyse)
            if (apiConfig.ApiKey != existing.ApiKey)
            {
                apiConfig.ApiKey = EncryptApiKey(apiConfig.ApiKey);
            }

            if (!string.IsNullOrEmpty(apiConfig.WebhookSecret) && apiConfig.WebhookSecret != existing.WebhookSecret)
            {
                apiConfig.WebhookSecret = EncryptApiKey(apiConfig.WebhookSecret);
            }

            _context.Entry(existing).CurrentValues.SetValues(apiConfig);
            await _context.SaveChangesAsync();

            _logger.LogInformation("API konfigürasyonu güncellendi: {ApiType}", apiConfig.ApiType);
            return ServiceResult<ApiConfiguration>.Success(apiConfig);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API konfigürasyonu güncellenemedi");
            return ServiceResult<ApiConfiguration>.Fail("API konfigürasyonu güncellenemedi", 500);
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var apiConfig = await _context.ApiConfigurations.FindAsync(id);
            if (apiConfig == null)
            {
                return ServiceResult<bool>.Fail("API konfigürasyonu bulunamadı", 404);
            }

            _context.ApiConfigurations.Remove(apiConfig);
            await _context.SaveChangesAsync();

            _logger.LogInformation("API konfigürasyonu silindi: {ApiType}", apiConfig.ApiType);
            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API konfigürasyonu silinemedi");
            return ServiceResult<bool>.Fail("API konfigürasyonu silinemedi", 500);
        }
    }

    public async Task<ServiceResult<ApiConfiguration>> GetByIdAsync(Guid id)
    {
        try
        {
            var apiConfig = await _context.ApiConfigurations
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (apiConfig == null)
            {
                return ServiceResult<ApiConfiguration>.Fail("API konfigürasyonu bulunamadı", 404);
            }

            // API key'i çöz
            apiConfig.ApiKey = DecryptApiKey(apiConfig.ApiKey);
            if (!string.IsNullOrEmpty(apiConfig.WebhookSecret))
            {
                apiConfig.WebhookSecret = DecryptApiKey(apiConfig.WebhookSecret);
            }

            return ServiceResult<ApiConfiguration>.Success(apiConfig);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API konfigürasyonu getirilemedi");
            return ServiceResult<ApiConfiguration>.Fail("API konfigürasyonu getirilemedi", 500);
        }
    }

    public async Task<ServiceResult<ApiConfiguration>> GetActiveByTypeAsync(string apiType)
    {
        try
        {
            var apiConfig = await _context.ApiConfigurations
                .FirstOrDefaultAsync(x => x.ApiType == apiType && x.IsActive && !x.IsDeleted);

            if (apiConfig == null)
            {
                return ServiceResult<ApiConfiguration>.Fail($"{apiType} API konfigürasyonu bulunamadı", 404);
            }

            // API key'i çöz
            apiConfig.ApiKey = DecryptApiKey(apiConfig.ApiKey);
            if (!string.IsNullOrEmpty(apiConfig.WebhookSecret))
            {
                apiConfig.WebhookSecret = DecryptApiKey(apiConfig.WebhookSecret);
            }

            return ServiceResult<ApiConfiguration>.Success(apiConfig);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API konfigürasyonu getirilemedi: {ApiType}", apiType);
            return ServiceResult<ApiConfiguration>.Fail("API konfigürasyonu getirilemedi", 500);
        }
    }

    public async Task<ServiceResult<PagedResult<ApiConfiguration>>> GetUserConfigurationsAsync(
        Guid userId, int page = 1, int pageSize = 10)
    {
        try
        {
            var query = _context.ApiConfigurations
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .OrderByDescending(x => x.CreatedDate);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // API key'leri çöz
            foreach (var item in items)
            {
                item.ApiKey = DecryptApiKey(item.ApiKey);
                if (!string.IsNullOrEmpty(item.WebhookSecret))
                {
                    item.WebhookSecret = DecryptApiKey(item.WebhookSecret);
                }
            }

            var result = new PagedResult<ApiConfiguration>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return ServiceResult<PagedResult<ApiConfiguration>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kullanıcı API konfigürasyonları getirilemedi");
            return ServiceResult<PagedResult<ApiConfiguration>>.Fail("API konfigürasyonları getirilemedi", 500);
        }
    }

    public async Task<ServiceResult<List<ApiConfiguration>>> GetAllActiveAsync()
    {
        try
        {
            var apiConfigs = await _context.ApiConfigurations
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.ApiType)
                .ToListAsync();

            // API key'leri çöz
            foreach (var item in apiConfigs)
            {
                item.ApiKey = DecryptApiKey(item.ApiKey);
                if (!string.IsNullOrEmpty(item.WebhookSecret))
                {
                    item.WebhookSecret = DecryptApiKey(item.WebhookSecret);
                }
            }

            return ServiceResult<List<ApiConfiguration>>.Success(apiConfigs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Aktif API konfigürasyonları getirilemedi");
            return ServiceResult<List<ApiConfiguration>>.Fail("API konfigürasyonları getirilemedi", 500);
        }
    }

    public string EncryptApiKey(string apiKey)
    {
        try
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_encryptionKey.PadRight(32).Substring(0, 32));
            aes.IV = Encoding.UTF8.GetBytes(_encryptionKey.PadRight(16).Substring(0, 16));

            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(apiKey);
            var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            return Convert.ToBase64String(encryptedBytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API key şifrelenemedi");
            return apiKey; // Şifreleme başarısız olursa orijinal key'i döndür
        }
    }

    public string DecryptApiKey(string encryptedApiKey)
    {
        try
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_encryptionKey.PadRight(32).Substring(0, 32));
            aes.IV = Encoding.UTF8.GetBytes(_encryptionKey.PadRight(16).Substring(0, 16));

            using var decryptor = aes.CreateDecryptor();
            var encryptedBytes = Convert.FromBase64String(encryptedApiKey);
            var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API key çözülemedi");
            return encryptedApiKey; // Çözme başarısız olursa şifreli key'i döndür
        }
    }

    public async Task<ServiceResult<bool>> CreateDefaultConfigurationsAsync()
    {
        try
        {
            // Ollama varsayılan konfigürasyonu (her zaman mevcut)
            var ollamaConfig = new ApiConfiguration
            {
                ApiType = "Ollama",
                ApiName = "Ollama AI",
                ApiKey = "local", // Ollama local çalışır
                BaseUrl = "http://localhost:11434",
                Model = "deepseek-coder",
                IsActive = true,
                IsDefault = true,
                Description = "Yerel Ollama AI servisi (tamamen ücretsiz)",
                CreatedDate = DateTime.UtcNow
            };

            _context.ApiConfigurations.Add(ollamaConfig);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Varsayılan API konfigürasyonları oluşturuldu");
            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Varsayılan API konfigürasyonları oluşturulamadı");
            return ServiceResult<bool>.Fail("Varsayılan konfigürasyonlar oluşturulamadı", 500);
        }
    }
}
