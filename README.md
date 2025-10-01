# Proje Analizi: Akıllı Kod İnceleme Aracı (Smart Code Review Tool)

## 1. Proje Tanımı
Bu proje, yazılım geliştirme süreçlerinde kod kalitesini artırmak ve insan hatasını azaltmak için tasarlanmış bir yapay zekâ destekli kod inceleme aracıdır. Araç, GitHub veya diğer Git platformlarında açılan Pull Request (PR) veya push sonrası otomatik olarak çalışır, kodu analiz eder ve öneriler sunar.

### Amaçlar:
- Kodun okunabilirliğini ve standartlara uygunluğunu artırmak
- Güvenlik açıklarını tespit etmek
- Performans sorunlarını belirlemek
- Kod inceleme sürecini hızlandırmak

---

## 2. Hedef Kitle
- Yazılım geliştirme ekipleri
- Open-source projeler
- Kurumsal yazılım şirketleri

---

## 3. Ana Özellikler
1. **Kod Analizi:**
   - Statik analiz araçları kullanılarak (ESLint, Pylint, SonarQube) temel hatalar tespit edilir.
   - Yapay zekâ ile kodun okunabilirliği, best practices ve performans değerlendirilir.

2. **Otomatik Geri Bildirim:**
   - Pull Request veya commit üzerine yorum bırakır.
   - Öneriler AI tarafından üretilir.

3. **Dashboard (Opsiyonel):**
   - Kod inceleme sonuçlarını görsel olarak sunar.
   - Hataları kategoriye göre sıralar: güvenlik, performans, okunabilirlik.

4. **Çoklu Dil Desteği:**
   - Başlangıçta JavaScript ve Python, ileride C#, Java, TypeScript vb.

5. **CI/CD Entegrasyonu:**
   - GitHub Actions veya GitLab CI ile PR bazlı otomatik kontrol.

---

## 4. Teknoloji Yığını
| Katman | Teknoloji |
|--------|-----------|
| Backend | ASP.NET Core 8 |
| Frontend | ASP.NET Core MVC + Bootstrap 5 (Opsiyonel) |
| AI & Kod Analizi | **Ollama (Ücretsiz, Local AI)**, Gemini (Ücretsiz Tier), HuggingFace (Ücretsiz) |
| Authentication | ASP.NET Core Identity |
| Veritabanı | Microsoft SQL Server (MSSQL) |
| ORM & Migration | Entity Framework Core + FluentMigrator |
| Validation | FluentValidation |
| Caching | IMemoryCache / IDistributedCache |
| Logging | Serilog |
| API Documentation | Swagger (OpenAPI) |
| Package Management | Central Package Management (Directory.Packages.props) |
| Background Jobs | Quartz.NET |
| CI/CD | GitHub Actions / GitLab CI |

---

## 5. Backend Mimarisi (Katmanlı Mimari)
Proje, **Clean Architecture** prensiplerine göre katmanlı mimari ile geliştirilecektir:

### Proje Yapısı:
```
SmartCodeReview/
├── src/
│   ├── Libraries/                              # Business Logic ve Data Katmanı
│   │   ├── Data/
│   │   │   └── SmartCodeReview.Data.Mssql/    # Entity'ler (CodeReview, PullRequest, User, vb.)
│   │   ├── Dto/
│   │   │   └── SmartCodeReview.Dto.Mssql/     # DTO'lar (Data Transfer Objects)
│   │   ├── Mssql/
│   │   │   └── SmartCodeReview.Mssql/         # DbContext ve Migration
│   │   └── Mssql.Services/
│   │       └── SmartCodeReview.Mssql.Services/ # Business Logic Services
│   └── Presentation/
│       └── Api/
│           └── SmartCodeReview.Api/            # API Controllers
├── docs/                                        # Dokümantasyonlar
├── Directory.Packages.props                     # Central Package Management
└── SmartCodeReview.sln
```

### Katmanlar:
1. **Data Layer (Veri Katmanı) - `Libraries/Data/`**
   - Entity'ler (CodeReview, PullRequest, Analysis, User, vb.)
   - BaseEntity (Id, CreatedDate, UpdatedDate, IsDeleted, vb.)
   - Enum'lar (CodeQualityScore, ReviewStatus, SecurityLevel)
   - Domain modelleri

2. **DTO Layer (Transfer Katmanı) - `Libraries/Dto/`**
   - DTO'lar (Data Transfer Objects)
   - CreateCodeReviewDto, UpdateCodeReviewDto, AnalysisResultDto
   - FluentValidation ile validasyon kuralları
   - Filter ve List DTO'ları

3. **Database Layer (Veritabanı Katmanı) - `Libraries/Mssql/`**
   - DbContext (SmartCodeReviewDbContext)
   - FluentMigrator ile migration'lar
   - Entity Framework Core konfigürasyonu
   - Seed data

4. **Service Layer (Servis Katmanı) - `Libraries/Mssql.Services/`**
   - Business logic servisler (CodeReviewService, AnalysisService, AIService)
   - Repository implementasyonları
   - External API entegrasyonları (GitHub, OpenAI)
   - Caching, logging ve diğer teknik servisler
   - Background job'lar

5. **Presentation Layer (Sunum Katmanı) - `Presentation/Api/`**
   - RESTful API endpoint'leri
   - Controllers (CodeReviewController, PullRequestController, vb.)
   - Global Exception Handler middleware
   - Global Response Wrapper
   - Maintenance Mode middleware
   - Authentication ve Authorization
   - Swagger documentation

### Kullanılan Pattern'ler:
- **Repository Pattern**: Veri erişim katmanı soyutlaması
- **Unit of Work Pattern**: Transaction yönetimi
- **Dependency Injection**: Loose coupling
- **SOLID Principles**: Clean code prensipler
- **BaseEntity**: Ortak property'ler için temel sınıf

### Admin Panel:
- Ayrı bir web projesi olarak yönetim paneli (opsiyonel)
- API üzerinden kod incelemeleri yönetimi
- Kullanıcı ve proje yönetimi

---

## 6. Güvenlik ve Kimlik Doğrulama
- **ASP.NET Core Identity**: Kullanıcı kimlik doğrulama ve yetkilendirme sistemi
- **IP Kısıtlama**: Hassas endpoint'ler için IP adresi filtreleme
- **Admin Seed Data**: Varsayılan admin kullanıcısı (admin@gmail.com / Super123!)
- **Bogus NuGet**: Test verisi üretimi için kullanılacak
- **Güvenlik Analizi**: AI ile kod güvenlik açıklarının tespiti

---

## 7. Mimari Akış
1. PR açıldığında webhook tetiklenir.
2. Backend servis, PR diff'ini GitHub API'den çeker.
3. Statik analiz ve AI analizi yapılır.
4. AI tarafından üretilen yorumlar PR üzerine otomatik bırakılır.
5. Dashboard üzerinde sonuçlar görüntülenebilir.

---

## 8. MVP (Minimum Viable Product) Planı
1. GitHub webhook ile PR tetikleme
2. Diff alımı
3. Basit AI prompt ile analiz ve öneri üretme
4. PR üzerine yorum bırakma

---

## 9. İleri Seviye Özellikler
- AI tarafından önerilen otomatik düzeltmeler (Auto-fix)
- Kod kalite skoru (0-100)
- Takım analizi ve istatistik raporları
- Çoklu programlama dili desteği
- Gerçek zamanlı bildirimler
- Webhook entegrasyonları
- Custom kural setleri tanımlama

---

## 10. Middleware ve Servisler
- **Global Exception Handler**: Merkezi hata yönetimi ve standart hata yanıtları
- **Global Response Wrapper**: API yanıtlarının standartlaştırılması
- **Maintenance Mode Middleware**: Bakım modu kontrolü ve yönlendirmesi
- **Logging Middleware**: Tüm isteklerin ve hataların loglanması
- **IP Filtering Middleware**: IP bazlı erişim kontrolü

---

## 11. Proje Avantajları
- Kod inceleme sürecini hızlandırır
- İnsan hatasını azaltır
- Yazılım standartlarına uyumu artırır
- Kurumsal ve bireysel projeler için değerli bir araçtır
- Clean Architecture ile sürdürülebilir kod yapısı
- Modern teknoloji stack ile gelecek odaklı mimari

---

## 12. Geliştirme Gereksinimleri
### Gerekli Araçlar:
- .NET 8 SDK
- Microsoft SQL Server (MSSQL)
- Visual Studio 2022 / JetBrains Rider / VS Code
- Git

### NuGet Paketleri:

#### Microsoft Paketleri:
- **Microsoft.AspNetCore.Identity.EntityFrameworkCore** - Identity sistemi
- **Microsoft.EntityFrameworkCore.SqlServer** - EF Core SQL Server provider
- **Microsoft.EntityFrameworkCore.Tools** - EF Core tools

#### Serilog Paketleri:
- **Serilog.AspNetCore** - ASP.NET Core entegrasyonu
- **Serilog.Sinks.MSSqlServer** - SQL Server sink
- **Serilog.Settings.Configuration** - Configuration support

#### Diğer Paketler:
- **FluentValidation.AspNetCore** - Request validation
- **FluentMigrator** - Database migration (5.2.0)
- **FluentMigrator.Runner** - Migration runner
- **FluentMigrator.Runner.SqlServer** - SQL Server support
- **AutoMapper** - Object mapping
- **Swashbuckle.AspNetCore** - Swagger/OpenAPI
- **StackExchange.Redis** - Redis client (caching)
- **Bogus** - Test data generation
- **Octokit** - GitHub API client
- **Azure.Storage.Blobs** - Azure Blob Storage
- **Quartz** - Background job scheduler
- **Quartz.Extensions.Hosting** - Quartz .NET hosting

### Varsayılan Admin Bilgileri:
- **Email**: admin@gmail.com
- **Şifre**: Super123!

---

## 13. API Response Standartları
Tüm API yanıtları standart bir wrapper ile dönecektir:

```json
{
  "success": true,
  "message": "İşlem başarılı",
  "data": { },
  "errors": [],
  "statusCode": 200
}
```

### HTTP Durum Kodları:
- **200 OK**: İşlem başarılı
- **400 Bad Request**: Geçersiz istek veya validasyon hatası
- **401 Unauthorized**: Kimlik doğrulama gerekli
- **403 Forbidden**: Yetki yetersiz
- **404 Not Found**: Kaynak bulunamadı
- **500 Internal Server Error**: Sunucu hatası

---

## 14. Central Package Management

Proje, **Central Package Management (CPM)** kullanarak tüm NuGet paketlerini merkezi olarak yönetir.

### Directory.Packages.props Yapısı:

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <!-- Microsoft Paketleri -->
    <PackageVersion Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.0" />
    <PackageVersion Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
    <PackageVersion Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />
    
    <!-- Serilog Paketleri -->
    <PackageVersion Include="Serilog.AspNetCore" Version="8.0.1" />
    <PackageVersion Include="Serilog.Sinks.MSSqlServer" Version="6.5.0" />
    
    <!-- Diğer Paketler -->
    <PackageVersion Include="FluentValidation.AspNetCore" Version="11.3.0" />
    <PackageVersion Include="FluentMigrator" Version="5.2.0" />
    <PackageVersion Include="AutoMapper" Version="13.0.1" />
    <PackageVersion Include="Octokit" Version="13.0.1" />
    <PackageVersion Include="Quartz" Version="3.8.0" />
    <!-- ... -->
  </ItemGroup>
</Project>
```

### Avantajları:
- **Merkezi Yönetim**: Tüm paket versiyonları tek bir dosyada
- **Tutarlılık**: Tüm projeler aynı paket versiyonlarını kullanır
- **Kolay Güncelleme**: Tek bir yerden tüm paketleri güncelleyebilirsiniz
- **Version Conflict Önleme**: Versiyon çakışmaları önlenir

### Kullanım:
```xml
<!-- Proje dosyasında sadece paket adı belirtilir -->
<ItemGroup>
  <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" />
  <PackageReference Include="Serilog.AspNetCore" />
  <!-- Version belirtmeye gerek yok! Directory.Packages.props'tan alınır -->
</ItemGroup>
```

---

## 15. Proje Kurulum ve Başlatma

### 1. Gereksinimler:
- .NET 8 SDK
- Microsoft SQL Server 2022 (veya Docker ile)
- Visual Studio 2022 / JetBrains Rider / VS Code
- Git

### 2. Projeyi İndirin:
```bash
git clone https://github.com/your-repo/smart-code-review-tool.git
cd smart-code-review-tool
```

### 3. Directory.Packages.props Oluşturun:
```bash
# Root dizinde Directory.Packages.props dosyası oluşturun
# Yukarıdaki örnek yapıyı kullanın
```

### 4. NuGet Paketlerini Yükleyin:
```bash
dotnet restore
```

### 5. Veritabanı Bağlantısını Yapılandırın:
```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SmartCodeReviewDb;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### 6. Migration'ları Çalıştırın:
```bash
# Migrations otomatik olarak çalışacak (FluentMigrator)
# İlk çalıştırmada tablolar oluşturulur
dotnet run --project src/Presentation/Api/SmartCodeReview.Api
```

### 7. Seed Data Oluşturun:
- Uygulama ilk çalıştığında admin kullanıcısı ve temel veriler otomatik oluşturulur
- Admin: admin@gmail.com / Super123!

### 8. Swagger'a Erişin:
```
https://localhost:7001/swagger
```

### 9. Ücretsiz AI Kurulumu:
- **Ollama kurulumu:** [FREE_AI_SETUP.md](docs/FREE_AI_SETUP.md) - Tamamen ücretsiz AI servisleri
- Model indirme: `ollama pull deepseek-coder`

### 10. API Dokümantasyonu:
Detaylı API dokümantasyonu için:
- [API Dokümantasyonu](docs/API_DOCUMENTATION.md) - Tüm endpoint'ler ve response örnekleri
- [API Provider Kurulum](docs/API_PROVIDER_SETUP.md) - GitHub, AI servisleri kurulum kılavuzu
- [Swagger Örnekleri](docs/SWAGGER_EXAMPLES.md) - Swagger UI kullanım kılavuzu
- [Proje Durumu](docs/PROJECT_STATUS.md) - Güncel proje durumu
- [Hızlı Başlangıç](docs/QUICK_START.md) - Adım adım kurulum

---

## 16. Proje Özellikleri ve Avantajları

### Clean Architecture Avantajları:
✅ **Bağımlılık Yönetimi**: Katmanlar arası doğru bağımlılık yönü
✅ **Test Edilebilirlik**: Her katman izole test edilebilir
✅ **Esneklik**: Framework bağımsız iş mantığı
✅ **Bakım Kolaylığı**: Değişikliklerin etki alanı sınırlı
✅ **Genişletilebilirlik**: Yeni özellikler kolay eklenir

### Katmanlı Mimari Avantajları:
✅ **Separation of Concerns**: Her katman kendi sorumluluğuna odaklanır
✅ **Testability**: Her katman bağımsız test edilebilir
✅ **Maintainability**: Kod bakımı ve geliştirme kolay
✅ **Scalability**: Kolayca ölçeklendirilebilir
✅ **Reusability**: Kod tekrarı minimalize edilir

### Central Package Management Avantajları:
✅ **Merkezi Kontrol**: Tüm paket versiyonları tek yerden
✅ **Tutarlılık**: Version conflict önlenir
✅ **Kolay Güncelleme**: Tek bir dosyadan güncelleme
✅ **Güvenlik**: Güvenlik yamaları merkezi uygulanır

---

## 17. Ücretsiz AI Servisleri 🆓

Bu proje **tamamen ücretsiz** AI servisleri kullanır:

### 🏆 Ollama (Önerilen - Tamamen Ücretsiz)
- **Maliyet:** 🟢 Tamamen ücretsiz, sınırsız kullanım
- **Gizlilik:** 🟢 Kodunuz asla dışarı çıkmaz (local AI)
- **Hız:** 🟢 Çok hızlı (local çalışır)
- **Kurulum:** `curl -fsSL https://ollama.ai/install.sh | sh`
- **Model:** `ollama pull deepseek-coder`
- **Kullanım:** Docker Compose ile otomatik başlar

### 🌟 Alternatif Ücretsiz AI'lar
- **Google Gemini:** Ücretsiz tier (60 req/dakika)
- **Hugging Face:** Ücretsiz API (sınırlı)

**Detaylı kurulum:** [FREE_AI_SETUP.md](docs/FREE_AI_SETUP.md)

---

## 18. Sonuç

Smart Code Review Tool projesi, **Clean Architecture** ve **ASP.NET Core 8** kullanarak:

- ✅ Modern ve ölçeklenebilir bir mimari
- ✅ Katmanlı ve bakımı kolay kod yapısı
- ✅ Merkezi paket yönetimi (Central Package Management)
- ✅ **Tamamen ücretsiz AI** destekli kod inceleme (**Ollama**)
- ✅ GitHub/GitLab entegrasyonu (Octokit)
- ✅ Güvenli ve performanslı altyapı
- ✅ FluentMigrator ile database migration
- ✅ Redis Queue + Background Worker sistemi
- ✅ Docker Compose ile kolay kurulum
- ✅ Serilog ile structured logging

ile geliştirilmiştir. 🚀

## 🎯 Öne Çıkan Özellikler

1. **💰 %100 Ücretsiz AI:** Ollama ile sınırsız kod analizi
2. **🔒 Gizlilik:** Kodunuz asla dışarı çıkmaz
3. **⚡ Hızlı:** Local AI, saniyeler içinde analiz
4. **🔄 Otomatik:** Webhook → Queue → AI Analysis → PR Comment
5. **📊 Dashboard:** Kod kalitesi istatistikleri
6. **🐳 Docker:** Tek komutla tüm servisler hazır
