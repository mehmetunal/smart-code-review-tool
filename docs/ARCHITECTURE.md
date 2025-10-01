# Smart Code Review Tool - Mimari Dokümantasyonu

## 🏗️ Genel Mimari

```
┌─────────────────────────────────────────────────────────────────┐
│                     Smart Code Review Tool                      │
│                         Tam Workflow                            │
└─────────────────────────────────────────────────────────────────┘

┌──────────────┐      ┌──────────────┐      ┌──────────────┐
│   GitHub     │      │   Webhook    │      │    Redis     │
│  Pull Request│─────▶│  Controller  │─────▶│    Queue     │
└──────────────┘      └──────────────┘      └──────────────┘
                                                     │
                                                     ▼
                                            ┌──────────────┐
                                            │  Background  │
                                            │   Worker     │
                                            └──────────────┘
                                                     │
                          ┌──────────────────────────┼──────────────────────────┐
                          ▼                          ▼                          ▼
                  ┌──────────────┐          ┌──────────────┐          ┌──────────────┐
                  │   GitHub     │          │   Ollama AI  │          │   Database   │
                  │   Service    │          │   Service    │          │   Service    │
                  └──────────────┘          └──────────────┘          └──────────────┘
                          │                          │                          │
                          │                          │                          │
                  Get PR Diff              Analyze Code              Save Results
                  Get Files                Find Issues               Update Status
                          │                          │                          │
                          ▼                          ▼                          ▼
                  ┌──────────────────────────────────────────────────────────────┐
                  │               Post Comment to GitHub PR                      │
                  └──────────────────────────────────────────────────────────────┘
```

---

## 📊 Katman Mimarisi

### 1. Presentation Layer (API)
```
SmartCodeReview.Api/
├── Controllers/
│   ├── AuthController.cs           # Kullanıcı girişi/kaydı
│   ├── WebhookController.cs        # GitHub/GitLab webhook receiver
│   └── CodeReviewController.cs     # Kod inceleme CRUD
├── Middleware/
│   ├── GlobalExceptionHandler      # Hata yönetimi
│   └── RequestLogging              # İstek loglama
├── BackgroundServices/
│   └── WebhookProcessorWorker      # Webhook işleme worker'ı
├── Extensions/
│   └── SeedDataExtensions          # Seed data
└── Program.cs                       # Startup configuration
```

### 2. Service Layer
```
SmartCodeReview.Mssql.Services/
├── Services/
│   ├── CodeReviewService.cs        # İnceleme CRUD
│   ├── GitHubService.cs            # GitHub API
│   ├── OllamaAIService.cs          # AI kod analizi
│   └── WebhookQueueService.cs      # Redis kuyruk
├── Interfaces/
│   ├── ICodeReviewService
│   ├── IGitHubService
│   ├── IAIService
│   └── IWebhookQueueService
└── Models/
    ├── ServiceResult<T>             # Standart sonuç
    ├── PagedResult<T>               # Sayfalama
    └── WebhookQueueMessage          # Kuyruk mesajı
```

### 3. Data Layer
```
SmartCodeReview.Data.Mssql/
├── User.cs                          # Kullanıcı (Identity)
├── Role.cs                          # Rol (Identity)
├── Project.cs                       # GitHub/GitLab repo
├── CodeReview.cs                    # PR inceleme
├── Analysis.cs                      # Tespit edilen sorunlar
├── FileAnalysis.cs                  # Dosya analizi
├── BaseEntity.cs                    # Temel entity
└── Enums/
    ├── ReviewStatus
    ├── SecurityLevel
    ├── IssueCategory
    ├── CodeQualityScore
    └── ProgrammingLanguage
```

### 4. Database Layer
```
SmartCodeReview.Mssql/
├── SmartCodeReviewDbContext.cs     # EF Core context
└── Migrations/
    └── InitialDatabaseMigration.cs # FluentMigrator
```

---

## 🔄 İş Akışı Detayları

### Webhook İşleme Workflow

```
1. PR Açıldı (GitHub)
   └─▶ Webhook gönderildi

2. WebhookController.HandleGitHubWebhook()
   ├─▶ Payload parse edildi
   ├─▶ WebhookQueueMessage oluşturuldu
   └─▶ Redis kuyruğa eklendi
   └─▶ HTTP 200 OK (hızlı yanıt)

3. WebhookProcessorWorker (Background)
   ├─▶ Kuyruktan mesaj alındı
   ├─▶ ProcessWebhookAsync() çağrıldı
   │
   ├─▶ 4. GitHubService.GetPullRequestInfoAsync()
   │   └─▶ PR bilgileri alındı
   │
   ├─▶ 5. CodeReviewService.CreateAsync()
   │   └─▶ Database'e kayıt oluşturuldu
   │
   ├─▶ 6. GitHubService.GetChangedFilesAsync()
   │   └─▶ Değiştirilen dosyalar listelendi
   │
   ├─▶ 7. Her dosya için:
   │   ├─▶ OllamaAIService.AnalyzeCodeChangesAsync()
   │   │   └─▶ AI ile kod analizi yapıldı
   │   └─▶ FileAnalysis oluşturuldu
   │
   ├─▶ 8. CodeReviewService.SaveAnalysisResultsAsync()
   │   └─▶ Tüm sonuçlar database'e kaydedildi
   │
   ├─▶ 9. CodeReviewService.CalculateQualityScoreAsync()
   │   └─▶ Kod kalite skoru hesaplandı (0-100)
   │
   ├─▶ 10. GitHubService.PostReviewCommentAsync()
   │    ├─▶ Özet yorum bırakıldı
   │    └─▶ Her sorun için satır yorumu bırakıldı
   │
   └─▶ 11. Status: Completed
```

---

## 🤖 AI Servisi Mimarisi

### Ollama AI Service

```
OllamaAIService
├── AnalyzeCodeChangesAsync()
│   ├── Prompt oluştur
│   ├── Ollama API'ye istek gönder
│   ├── JSON response parse et
│   └── Analysis listesi döndür
│
├── EvaluateCodeQualityAsync()
│   ├── Kalite değerlendirme prompt'u
│   ├── AI'dan skor al (0-100)
│   └── Güçlü/zayıf yönleri listele
│
├── DetectSecurityIssuesAsync()
│   ├── Güvenlik odaklı prompt
│   ├── SQL Injection, XSS, vb. kontrol
│   └── Güvenlik sorunları listesi
│
└── SuggestPerformanceImprovementsAsync()
    ├── Performans odaklı prompt
    ├── N+1 query, memory leak kontrol
    └── Performans önerileri
```

### AI Prompt Yapısı

```markdown
Sen bir kod inceleme uzmanısın. Aşağıdaki kod değişikliklerini analiz et.

Dosya: UserService.cs
Dil: C#

Kod Değişiklikleri:
```diff
+ var query = "SELECT * FROM Users WHERE Id = " + userId;
+ var result = db.Execute(query);
```

Kategoriler:
1. Güvenlik açıkları
2. Performans sorunları
3. Kod kalitesi
4. Best practices
5. Potansiyel bug'lar

JSON formatında döndür:
{
  "issues": [...]
}
```

---

## 🗄️ Veritabanı Şeması

```
Users (Identity)
├── Id (Guid, PK)
├── Email
├── PasswordHash
├── FirstName
├── LastName
├── GitHubUsername
└── GitLabUsername

Projects
├── Id (Guid, PK)
├── Name
├── RepositoryUrl
├── FullName (owner/repo)
├── WebhookSecret
├── UserId (FK → Users)
└── IsActive

CodeReviews
├── Id (Guid, PK)
├── PullRequestNumber
├── Title
├── Status (Enum)
├── QualityScore (0-100)
├── TotalIssuesCount
├── ProjectId (FK → Projects)
└── UserId (FK → Users)

Analyses
├── Id (Guid, PK)
├── Title
├── Description
├── Category (Enum)
├── Severity (Enum)
├── FilePath
├── LineNumber
├── Suggestion
└── CodeReviewId (FK → CodeReviews)

FileAnalyses
├── Id (Guid, PK)
├── FilePath
├── Language (Enum)
├── AddedLines
├── DeletedLines
├── QualityScore
└── CodeReviewId (FK → CodeReviews)
```

---

## 🔐 Güvenlik Mimarisi

### Authentication Flow

```
1. Kullanıcı Kaydı
   POST /api/auth/register
   └─▶ UserManager.CreateAsync()
       └─▶ Şifre hash'lenir
           └─▶ Database'e kaydedilir
               └─▶ "User" rolü atanır

2. Kullanıcı Girişi
   POST /api/auth/login
   └─▶ SignInManager.PasswordSignInAsync()
       └─▶ Şifre doğrulanır
           └─▶ Cookie/Session oluşturulur
               └─▶ [İleride JWT token dönülecek]

3. Korumalı Endpoint Erişimi
   [Authorize]
   └─▶ Middleware cookie/token kontrol eder
       └─▶ User.Identity.IsAuthenticated
           └─▶ Endpoint'e erişim sağlanır
```

### Authorization Levels

| Level | Attribute | Erişim |
|-------|-----------|--------|
| **Public** | - | Herkes |
| **Authenticated** | `[Authorize]` | Giriş yapmış kullanıcılar |
| **Admin** | `[Authorize(Roles = "Admin")]` | Sadece admin |

---

## 🚀 Background Job Mimarisi

### Redis Queue + HostedService

```
┌─────────────────┐
│  Webhook Geldi  │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  Redis Queue    │  ◀── FIFO Queue (First In, First Out)
│  (webhook:queue)│
└────────┬────────┘
         │
         ▼
┌─────────────────────────────────┐
│  WebhookProcessorWorker         │
│  (Background HostedService)     │
│                                 │
│  while (true):                  │
│    message = DequeueAsync()     │
│    ProcessWebhookAsync(message) │
│    Delay(5 seconds)             │
└─────────────────────────────────┘
         │
         ▼
┌─────────────────┐
│  Process Steps: │
│  1. Get PR Info │
│  2. Create      │
│  3. Get Files   │
│  4. AI Analyze  │
│  5. Save        │
│  6. Post Comment│
└─────────────────┘
```

### Avantajları

✅ **Webhook Timeout Önleme:** 30 saniyelik GitHub timeout sınırını aşmıyoruz
✅ **Asenkron İşleme:** AI analizi background'da yapılır
✅ **Güvenilirlik:** Hata durumunda mesaj kaybolmaz
✅ **Ölçeklenebilirlik:** Paralel worker eklenebilir
✅ **Monitoring:** Queue length takibi

---

## 🔄 Veri Akışı

### 1. Webhook Alımı → Kuyruk

```csharp
// WebhookController.cs
public async Task<IActionResult> HandleGitHubWebhook(JsonElement payload)
{
    // Parse payload
    var message = new WebhookQueueMessage {
        Source = "GitHub",
        Owner = "johndoe",
        Repository = "my-repo",
        PullRequestNumber = 123,
        Action = "opened"
    };
    
    // Kuyruğa ekle (Redis)
    await _webhookQueue.EnqueueAsync(message);
    
    // Hızlı yanıt ver (< 1 saniye)
    return Ok("Webhook alındı");
}
```

### 2. Kuyruktan İşleme

```csharp
// WebhookProcessorWorker.cs
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    while (!stoppingToken.IsCancellationRequested)
    {
        var message = await _queueService.DequeueAsync();
        if (message != null)
        {
            await ProcessWebhookAsync(message);
        }
        await Task.Delay(5000); // 5 saniye bekle
    }
}
```

### 3. AI Analizi

```csharp
// OllamaAIService.cs
public async Task<List<Analysis>> AnalyzeCodeChangesAsync(string diff)
{
    var prompt = "Kod değişikliklerini analiz et...";
    var response = await CallOllamaAsync(prompt);
    return ParseAIResponse(response);
}
```

### 4. Sonuçları Kaydet

```csharp
// CodeReviewService.cs
public async Task SaveAnalysisResultsAsync(
    Guid codeReviewId,
    List<Analysis> analyses,
    List<FileAnalysis> fileAnalyses)
{
    // Database'e kaydet
    await _context.Analyses.AddRangeAsync(analyses);
    await _context.FileAnalyses.AddRangeAsync(fileAnalyses);
    await _context.SaveChangesAsync();
}
```

### 5. GitHub'a Yorum Bırak

```csharp
// GitHubService.cs
public async Task PostReviewCommentAsync(string owner, string repo, int number, string comment)
{
    await _client.Issue.Comment.Create(owner, repo, number, comment);
}
```

---

## 🐳 Docker Servisleri

### Container'lar

| Container | Image | Port | Açıklama |
|-----------|-------|------|----------|
| **smartcodereview-mssql** | mcr.microsoft.com/mssql/server:2022 | 1433 | Database |
| **smartcodereview-redis** | redis:7-alpine | 6379 | Queue & Cache |
| **smartcodereview-ollama** | ollama/ollama:latest | 11434 | AI Service |

### Network Topolojisi

```
smartcodereview-network (bridge)
├── smartcodereview-mssql     (1433)
├── smartcodereview-redis     (6379)
└── smartcodereview-ollama    (11434)
         ▲
         │
    [Host: API]
```

---

## 📈 Performans Optimizasyonları

### 1. Database
- **AsNoTracking:** Read-only queryler için
- **Indexes:** Sık sorgulanan kolonlar
- **Paging:** Büyük listeler için sayfalama

### 2. Redis Queue
- **FIFO:** Mesajlar sırayla işlenir
- **Async:** Webhook hemen yanıt verir
- **Persistent:** Uygulama yeniden başlasa bile kaybolmaz

### 3. AI Service
- **Local AI (Ollama):** İnternet gerektirmez, çok hızlı
- **GPU Support:** CUDA ile 10x daha hızlı
- **Model Caching:** Model memory'de kalır

### 4. Background Worker
- **Retry Logic:** Hata durumunda tekrar dener
- **Rate Limiting:** GitHub API limitlerine uyar
- **Batch Processing:** Paralel dosya analizi

---

## 🔍 Monitoring ve Logging

### Serilog Konfigürasyonu

```
Loglar:
├── Console (Development)
├── MSSqlServer (Production)
│   └── Logs tablosu
│       ├── Timestamp
│       ├── Level (Info, Warning, Error)
│       ├── Message
│       └── Exception

Log Seviyeleri:
├── Information: Genel akış
├── Warning: Dikkat gerektiren
└── Error: Hatalar
```

### Loglanan Olaylar

| Olay | Seviye | Örnek |
|------|--------|-------|
| Webhook alındı | Info | "GitHub webhook alındı" |
| Kuyruk işlemi | Info | "Webhook kuyruğa eklendi" |
| AI analizi | Info | "AI analizi tamamlandı: 5 sorun" |
| Hata | Error | "PR bilgileri alınamadı" |

---

## 🎯 Scalability (Ölçeklenebilirlik)

### Horizontal Scaling

**Şu anki yapı:**
- Single API instance
- Single background worker
- Redis queue (shared)

**İleride (Yüksek trafik için):**
```
Load Balancer
├── API Instance 1  ─┐
├── API Instance 2  ─┼─▶ Redis Queue
└── API Instance 3  ─┘
         │
    Background Workers
    ├── Worker 1
    ├── Worker 2
    └── Worker 3
```

### Vertical Scaling

- **Database:** MSSQL clustering
- **Redis:** Redis Cluster
- **AI:** GPU sunucu (10x hız artışı)

---

## 📦 Deployment Mimarisi

### Development
```
Developer Machine
├── Docker Desktop
│   ├── MSSQL (local)
│   ├── Redis (local)
│   └── Ollama (local)
└── dotnet run
```

### Production
```
Cloud Server (Azure/AWS/DigitalOcean)
├── Docker Compose
│   ├── MSSQL (container)
│   ├── Redis (container)
│   └── Ollama (container + GPU)
└── API (container)
```

---

## 🔒 Güvenlik Katmanları

1. **Transport:** HTTPS/TLS
2. **Authentication:** ASP.NET Core Identity
3. **Authorization:** Role-based access
4. **Input Validation:** FluentValidation (ileride)
5. **Exception Handling:** Global middleware
6. **SQL Injection:** EF Core parameterized queries
7. **Webhook Secret:** Signature verification (ileride)

---

## 📊 Teknoloji Stack Özeti

| Katman | Teknoloji | Versiyon |
|--------|-----------|----------|
| **Runtime** | .NET | 8.0 |
| **Web Framework** | ASP.NET Core | 8.0 |
| **ORM** | Entity Framework Core | 8.0 |
| **Migration** | FluentMigrator | 5.2.0 |
| **Database** | MS SQL Server | 2022 |
| **Cache/Queue** | Redis | 7.0 |
| **AI** | Ollama | Latest |
| **GitHub API** | Octokit | 13.0.1 |
| **Logging** | Serilog | 8.0.1 |
| **Validation** | FluentValidation | 11.3.0 |
| **Mapping** | AutoMapper | 13.0.1 |
| **Jobs** | Quartz.NET | 3.8.0 |

---

**Hazırladı:** AI Assistant  
**Tarih:** 1 Ocak 2025  
**Versiyon:** 1.0.0

