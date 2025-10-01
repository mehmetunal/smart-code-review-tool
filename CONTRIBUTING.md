# Katkıda Bulunma Kılavuzu

## 🤝 Smart Code Review Tool Projesine Katkı

Projeye katkıda bulunmak istediğiniz için teşekkürler! Bu kılavuz, katkı sürecini kolaylaştırmak için hazırlanmıştır.

---

## 🚀 Başlamadan Önce

### Gereksinimler
- .NET 8 SDK
- Docker Desktop
- Git
- Visual Studio / Rider / VS Code

### Proje Kurulumu
```bash
# 1. Fork edin
# GitHub'da "Fork" butonuna tıklayın

# 2. Clone edin
git clone https://github.com/YOUR_USERNAME/smart-code-review-tool.git
cd smart-code-review-tool

# 3. Docker servislerini başlatın
./run-docker.sh

# 4. Çalıştırın
cd src/Presentation/Api/SmartCodeReview.Api
dotnet run
```

---

## 📝 Geliştirme Süreci

### 1. Branch Oluşturun
```bash
git checkout -b feature/amazing-feature
```

**Branch isimlendirme:**
- `feature/` - Yeni özellik
- `bugfix/` - Hata düzeltme
- `docs/` - Dokümantasyon
- `refactor/` - Kod iyileştirme

### 2. Kodunuzu Yazın

**Kod Standartları:**
- ✅ Türkçe yorumlar
- ✅ XML documentation
- ✅ Clean code principles
- ✅ SOLID principles
- ✅ Async/await kullanımı

**Örnek:**
```csharp
/// <summary>
/// Kod incelemesi oluşturur
/// </summary>
/// <param name="codeReview">Kod inceleme entity'si</param>
/// <returns>Servis sonucu</returns>
public async Task<ServiceResult<CodeReview>> CreateAsync(CodeReview codeReview)
{
    try
    {
        // Implementation
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Hata mesajı");
        return ServiceResult<CodeReview>.Fail("Hata", 500);
    }
}
```

### 3. Test Edin
```bash
# Build
dotnet build

# Run
dotnet run

# Test endpoints
curl http://localhost:5000/health
```

### 4. Commit Edin
```bash
git add .
git commit -m "feat: Add amazing feature"
```

**Commit mesaj formatı:**
- `feat:` - Yeni özellik
- `fix:` - Hata düzeltme
- `docs:` - Dokümantasyon
- `refactor:` - Kod iyileştirme
- `test:` - Test ekleme

### 5. Push Edin
```bash
git push origin feature/amazing-feature
```

### 6. Pull Request Açın
- GitHub'da Pull Request açın
- Değişikliklerinizi açıklayın
- Reviewers atayın

---

## 🎯 Katkı Alanları

### Öncelikli İhtiyaçlar

1. **JWT Authentication**
   - Token-based auth
   - Refresh token
   - Token validation

2. **Statistics API**
   - Dashboard endpoint'leri
   - Kod kalitesi grafikleri
   - Kullanıcı istatistikleri

3. **Alternatif AI Provider'lar**
   - Gemini implementation
   - HuggingFace implementation
   - Provider seçim mekanizması

4. **Frontend (Opsiyonel)**
   - Bootstrap 5 admin panel
   - Dashboard görünümü
   - İstatistik grafikleri

5. **Unit Tests**
   - Service layer tests
   - Controller tests
   - Integration tests

---

## 📚 Kod Örnekleri

### Yeni Service Ekleme

```csharp
// 1. Interface oluştur
public interface IMyService
{
    Task<ServiceResult<T>> DoSomethingAsync();
}

// 2. Implementation
public class MyService : IMyService
{
    private readonly SmartCodeReviewDbContext _context;
    private readonly ILogger<MyService> _logger;

    public MyService(
        SmartCodeReviewDbContext context,
        ILogger<MyService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResult<T>> DoSomethingAsync()
    {
        try
        {
            // Business logic
            return ServiceResult<T>.Success(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hata");
            return ServiceResult<T>.Fail("Hata mesajı", 500);
        }
    }
}

// 3. Program.cs'e ekle
builder.Services.AddScoped<IMyService, MyService>();
```

### Yeni Controller Ekleme

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MyController : ControllerBase
{
    private readonly IMyService _myService;

    public MyController(IMyService myService)
    {
        _myService = myService;
    }

    [HttpGet]
    public async Task<IActionResult> GetData()
    {
        var result = await _myService.DoSomethingAsync();
        
        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Data });
        }
        
        return StatusCode(result.StatusCode, new { 
            success = false, 
            message = result.Message 
        });
    }
}
```

### Yeni DTO ve Validator Ekleme

```csharp
// DTO
public class MyDto
{
    public required string Name { get; set; }
    public int Value { get; set; }
}

// Validator
public class MyDtoValidator : AbstractValidator<MyDto>
{
    public MyDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ad gereklidir")
            .MaximumLength(100).WithMessage("En fazla 100 karakter");

        RuleFor(x => x.Value)
            .GreaterThan(0).WithMessage("Değer 0'dan büyük olmalıdır");
    }
}
```

---

## 🐛 Hata Bildirimi

### Issue Açma

1. [GitHub Issues](https://github.com/your-repo/issues) sayfasına gidin
2. "New Issue" tıklayın
3. Şablonu doldurun:

```markdown
**Hata Açıklaması:**
Kısa ve net açıklama

**Adımlar:**
1. Adım 1
2. Adım 2
3. Hata oluştu

**Beklenen Davranış:**
Ne olmasını bekliyordunuz

**Gerçek Davranış:**
Ne oldu

**Ortam:**
- OS: Linux/Windows/Mac
- .NET: 8.0
- Docker: 24.0

**Loglar:**
```
Hata logları buraya
```
```

---

## ✅ Pull Request Checklist

PR açmadan önce kontrol edin:

- [ ] Kod derleniyor (`dotnet build` başarılı)
- [ ] Tüm yeni metodlar XML documentation içeriyor
- [ ] Değişiklikler test edildi
- [ ] Commit mesajları anlamlı
- [ ] README güncel (gerekiyorsa)
- [ ] Breaking change yoksa (veya belirtildi)

---

## 🎨 Kod Stili

### C# Kod Stili
- PascalCase: Class, Method, Property
- camelCase: Local variable, parameter
- _camelCase: Private field
- UPPER_CASE: Constant

### Dosya Organizasyonu
```
Controller/
├── FooController.cs
Service/
├── Interfaces/
│   └── IFooService.cs
└── Services/
    └── FooService.cs
```

---

## 💡 İpuçları

1. **Küçük PR'lar:** Büyük değişiklikleri parçalara bölün
2. **Tek Sorumluluk:** Her PR bir şey yapsın
3. **Test Edin:** Değişikliklerinizi manuel test edin
4. **Dokümante Edin:** Yeni özellikler için döküman ekleyin

---

## 📞 İletişim

- **Issues:** GitHub Issues
- **Discussions:** GitHub Discussions
- **Email:** (eklenecek)

---

## 🏆 Katkıda Bulunanlar

Katkıda bulunan herkese teşekkürler! 🎉

(Contributors listesi otomatik eklenecek)

---

**Teşekkürler!** ❤️

