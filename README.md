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
| Framework | Maggsoft Framework |
| Frontend | ASP.NET Core MVC + Bootstrap 5 |
| AI & Kod Analizi | OpenAI GPT-4/5 API, Tree-sitter, ESLint, Pylint, SonarQube |
| Authentication | ASP.NET Core Identity + Maggsoft.Core |
| Veritabanı | Microsoft SQL Server (MSSQL) |
| ORM & Migration | Maggsoft.Data + Maggsoft.Mssql + FluentMigrator |
| Validation | FluentValidation |
| Caching | Maggsoft.Cache + Maggsoft.Cache.MemoryCache |
| Logging | Serilog + Maggsoft.Logging |
| API Documentation | Swagger (OpenAPI) |
| Services | Maggsoft.Services + Maggsoft.Mssql.Services |
| Endpoints | Maggsoft.Endpoints |
| Package Management | Central Package Management (Directory.Packages.props) |
| CI/CD | GitHub Actions / GitLab CI |

---

## 5. Backend Mimarisi (Maggsoft Framework ile Katmanlı Mimari)
Proje, **Maggsoft Framework** kullanarak katmanlı mimari prensiplerine göre geliştirilecektir (TrinkEmlak yapısına benzer):

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
├── Directory.Packages.props                     # Central Package Management (Maggsoft paketleri)
└── SmartCodeReview.sln
```

### Katmanlar:
1. **Data Layer (Veri Katmanı) - `Libraries/Data/`**
   - Entity'ler (CodeReview, PullRequest, Analysis, User, vb.)
   - BaseEntity inheritance (Maggsoft.Data'dan)
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

### Maggsoft Framework Kullanımı:
- **Maggsoft.Data**: Repository pattern ve base entity
- **Maggsoft.Mssql**: Database connection ve context
- **Maggsoft.Services**: Service layer base class'ları
- **Maggsoft.Cache**: Caching infrastructure
- **Maggsoft.Endpoints**: API endpoint helpers
- **Maggsoft.Logging**: Structured logging

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

#### Maggsoft Framework Paketleri:
- **Maggsoft.Core** - Framework core
- **Maggsoft.Data** - Data access layer
- **Maggsoft.Framework** - Main framework
- **Maggsoft.Data.Mssql** - MSSQL data provider
- **Maggsoft.Mssql** - MSSQL infrastructure
- **Maggsoft.Cache** - Cache infrastructure
- **Maggsoft.Cache.MemoryCache** - Memory cache implementation
- **Maggsoft.Services** - Service layer base
- **Maggsoft.Aspect.Core** - AOP support
- **Maggsoft.Mssql.Services** - MSSQL services
- **Maggsoft.Endpoints** - API endpoints
- **Maggsoft.Dto.Mssql** - DTO infrastructure
- **Maggsoft.Logging** - Logging infrastructure

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
- **FluentMigrator** - Database migration
- **FluentMigrator.Runner** - Migration runner
- **FluentMigrator.Runner.SqlServer** - SQL Server support
- **AutoMapper** - Object mapping
- **Swashbuckle.AspNetCore** - Swagger/OpenAPI
- **StackExchange.Redis** - Redis client
- **Bogus** - Test data generation
- **OpenAI API Client** - AI integration
- **Octokit** - GitHub API client
- **Azure.Storage.Blobs** - Azure Blob Storage

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
    <!-- Maggsoft Framework Paketleri -->
    <PackageVersion Include="Maggsoft.Core" Version="2.1.6" />
    <PackageVersion Include="Maggsoft.Data" Version="2.1.7" />
    <PackageVersion Include="Maggsoft.Framework" Version="2.5.8" />
    <PackageVersion Include="Maggsoft.Data.Mssql" Version="2.1.0" />
    <PackageVersion Include="Maggsoft.Mssql" Version="2.0.12" />
    <PackageVersion Include="Maggsoft.Cache" Version="2.0.22" />
    
    <!-- Microsoft Paketleri -->
    <PackageVersion Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.0" />
    <PackageVersion Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
    
    <!-- Diğer Paketler -->
    <PackageVersion Include="FluentValidation.AspNetCore" Version="11.3.0" />
    <PackageVersion Include="FluentMigrator" Version="6.2.0" />
    <PackageVersion Include="Serilog.AspNetCore" Version="8.0.1" />
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
  <PackageReference Include="Maggsoft.Core" />
  <PackageReference Include="Maggsoft.Data" />
  <!-- Version belirtmeye gerek yok! -->
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

---

## 16. Proje Özellikleri ve Avantajları

### Maggsoft Framework Kullanımının Avantajları:
✅ **Hazır Altyapı**: Repository pattern, base entity, service layer hazır
✅ **Hızlı Geliştirme**: Boilerplate kod yazmaya gerek yok
✅ **Tutarlılık**: Standart mimari ve pattern'ler
✅ **Cache Desteği**: Built-in cache infrastructure
✅ **Logging**: Structured logging desteği
✅ **Migration**: FluentMigrator entegrasyonu

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

## 17. Sonuç

Smart Code Review Tool projesi, **Maggsoft Framework** ve **TrinkEmlak benzeri proje yapısı** kullanarak:

- ✅ Modern ve ölçeklenebilir bir mimari
- ✅ Katmanlı ve bakımı kolay kod yapısı
- ✅ Merkezi paket yönetimi
- ✅ AI destekli kod inceleme
- ✅ GitHub/GitLab entegrasyonu
- ✅ Güvenli ve performanslı altyapı

ile geliştirilecektir. 🚀
