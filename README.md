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
| Backend | ASP.NET Core 8 (Clean Architecture) |
| Frontend | ASP.NET Core MVC + Bootstrap 5 |
| AI & Kod Analizi | OpenAI GPT-4/5 API, Tree-sitter, ESLint, Pylint, SonarQube |
| Authentication | ASP.NET Core Identity |
| Veritabanı | Microsoft SQL Server (MSSQL) |
| Migration | FluentMigrator |
| Validation | FluentValidation |
| Caching | IMemoryCache / IDistributedCache |
| Logging | Serilog / ILogger |
| API Documentation | Swagger (OpenAPI) |
| CI/CD | GitHub Actions / GitLab CI |

---

## 5. Backend Mimarisi (Clean Architecture)
Proje, ASP.NET Core 8 ile Clean Architecture prensiplerine göre geliştirilecektir:

### Katmanlar:
1. **Domain Layer (Alan Katmanı)**
   - Entity'ler ve Domain modelleri
   - Business logic ve domain kuralları
   - Interface tanımlamaları

2. **Application Layer (Uygulama Katmanı)**
   - Use case'ler ve business logic
   - DTO'lar (Data Transfer Objects)
   - FluentValidation ile validasyon kuralları
   - Service interface'leri

3. **Infrastructure Layer (Altyapı Katmanı)**
   - Database context ve repository implementasyonları
   - FluentMigrator ile veritabanı migration'ları
   - External API entegrasyonları (GitHub, OpenAI)
   - Caching, logging ve diğer teknik servisler

4. **API/Presentation Layer (Sunum Katmanı)**
   - RESTful API endpoint'leri
   - Global Exception Handler middleware
   - Global Response Wrapper
   - Maintenance Mode middleware
   - Authentication ve Authorization
   - Swagger documentation

### Proje Yapısı:
- **Public Site**: Kullanıcı arayüzü ve genel erişim
- **Admin Panel**: Ayrı bir web projesi olarak yönetim paneli

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
- FluentValidation
- FluentMigrator
- Microsoft.AspNetCore.Identity
- Serilog
- Swashbuckle (Swagger)
- Bogus (seed data)
- OpenAI API Client
- Octokit (GitHub API)

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
