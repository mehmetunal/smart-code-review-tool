# Smart Code Review Tool - Hızlı Başlangıç Kılavuzu

## 🚀 Kurulum Adımları

### 1️⃣ Gereksinimleri Kontrol Edin

```bash
# .NET 8 SDK kontrolü
dotnet --version
# Beklenen: 8.0.x

# Docker kontrolü
docker --version
# Beklenen: 24.x veya üzeri

# Docker Compose kontrolü
docker compose version
# Beklenen: v2.x veya üzeri
```

### 2️⃣ Projeyi Klonlayın

```bash
git clone https://github.com/your-username/smart-code-review-tool.git
cd smart-code-review-tool
```

### 3️⃣ Docker Servislerini Başlatın

```bash
# Script ile (Önerilen)
./run-docker.sh

# Veya manuel olarak
docker compose up -d

# Servislerin durumunu kontrol et
docker compose ps
```

**Beklenen Çıktı:**
```
NAME                      IMAGE                                        STATUS
smartcodereview-mssql     mcr.microsoft.com/mssql/server:2022-latest   Up
smartcodereview-redis     redis:7-alpine                               Up
```

### 4️⃣ Veritabanını Oluşturun

```bash
# Veritabanı oluştur (otomatik)
docker exec -it smartcodereview-mssql /opt/mssql-tools18/bin/sqlcmd \
    -S localhost \
    -U sa \
    -P "YourStrong!Passw0rd" \
    -C \
    -Q "CREATE DATABASE SmartCodeReviewDb"

# Veritabanını kontrol et
docker exec -it smartcodereview-mssql /opt/mssql-tools18/bin/sqlcmd \
    -S localhost \
    -U sa \
    -P "YourStrong!Passw0rd" \
    -C \
    -Q "SELECT name FROM sys.databases"
```

### 5️⃣ NuGet Paketlerini Yükleyin

```bash
dotnet restore
```

### 6️⃣ Projeyi Build Edin

```bash
dotnet build
```

**Beklenen Çıktı:**
```
Build succeeded.
    2 Warning(s)
    0 Error(s)
```

### 7️⃣ API'yi Çalıştırın

```bash
cd src/Presentation/Api/SmartCodeReview.Api
dotnet run
```

**Beklenen Çıktı:**
```
info: SmartCodeReview.Api.Extensions.SeedDataExtensions[0]
      Veritabanı oluşturuldu/kontrol edildi
info: SmartCodeReview.Api.Extensions.SeedDataExtensions[0]
      Rol oluşturuldu: Admin
info: SmartCodeReview.Api.Extensions.SeedDataExtensions[0]
      Rol oluşturuldu: User
info: SmartCodeReview.Api.Extensions.SeedDataExtensions[0]
      Admin kullanıcısı oluşturuldu: admin@gmail.com
info: SmartCodeReview.Api.Extensions.SeedDataExtensions[0]
      Seed data başarıyla oluşturuldu
Now listening on: https://localhost:7001
Now listening on: http://localhost:5000
```

### 8️⃣ Swagger'ı Açın

Tarayıcınızda şu adresi açın:
```
https://localhost:7001/swagger
```

### 9️⃣ Health Check Test

```bash
curl http://localhost:5000/health
```

**Beklenen Çıktı:**
```json
{
  "status": "Healthy",
  "timestamp": "2025-01-01T12:34:56.789Z",
  "version": "1.0.0"
}
```

---

## 🧪 İlk API Testleri

### 1. Admin ile Giriş Yapma

```bash
curl -X POST https://localhost:7001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@gmail.com",
    "password": "Super123!",
    "rememberMe": true
  }' \
  -k
```

**Beklenen Response:**
```json
{
  "message": "Giriş başarılı"
}
```

### 2. Yeni Kullanıcı Kaydı

```bash
curl -X POST https://localhost:7001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123!",
    "firstName": "Test",
    "lastName": "User"
  }' \
  -k
```

### 3. Health Check

```bash
curl http://localhost:5000/health
```

---

## 🔍 Sorun Giderme

### MSSQL Bağlantı Hatası

**Sorun:** `Cannot open database 'SmartCodeReviewDb'`

**Çözüm:**
```bash
# Container'ı kontrol et
docker ps | grep mssql

# Veritabanını oluştur
docker exec -it smartcodereview-mssql /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P "YourStrong!Passw0rd" -C \
    -Q "CREATE DATABASE SmartCodeReviewDb"
```

### Port Zaten Kullanımda

**Sorun:** `Port 1433 is already in use`

**Çözüm:**
```bash
# Çalışan MSSQL container'ı durdur
docker stop smartcodereview-mssql

# Veya farklı port kullan (docker-compose.yml'de değiştir)
# "1434:1433"
```

### Redis Bağlantı Hatası

**Sorun:** `Unable to connect to Redis`

**Çözüm:**
```bash
# Redis container'ını kontrol et
docker exec smartcodereview-redis redis-cli ping

# Beklenen çıktı: PONG
```

### Seed Data Çalışmadı

**Sorun:** Admin kullanıcısı oluşmadı

**Çözüm:**
```bash
# Veritabanını sıfırlayın
docker exec -it smartcodereview-mssql /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P "YourStrong!Passw0rd" -C \
    -Q "DROP DATABASE SmartCodeReviewDb; CREATE DATABASE SmartCodeReviewDb"

# API'yi tekrar çalıştırın
dotnet run
```

---

## 🛑 Servisleri Durdurma

### Docker Servislerini Durdur

```bash
# Servisleri durdur
docker compose down

# Servisleri durdur ve volume'ları sil
docker compose down -v
```

### API'yi Durdur

```bash
# Ctrl+C ile durdurun
# Veya
pkill -f SmartCodeReview.Api
```

---

## 📊 Varsayılan Kullanıcı Bilgileri

| Rol | Email | Şifre | Açıklama |
|-----|-------|-------|----------|
| **Admin** | admin@gmail.com | Super123! | Tam yetki |
| **User** | - | - | Seed data ile oluşturulmadı (manuel kayıt gerekli) |

---

## 🔗 Önemli URL'ler

| Servis | URL | Açıklama |
|--------|-----|----------|
| **Swagger UI** | https://localhost:7001/swagger | API dokümantasyonu |
| **Health Check** | http://localhost:5000/health | Sistem sağlık kontrolü |
| **API Base** | https://localhost:7001 | API base URL |
| **MSSQL** | localhost,1433 | Database bağlantısı |
| **Redis** | localhost:6379 | Cache bağlantısı |

---

## 📝 Sonraki Adımlar

1. ✅ **Proje Kuruldu** - API çalışıyor
2. ⏭️ **GitHub Webhook Kurulumu** - Repository'nize webhook ekleyin
3. ⏭️ **OpenAI API Key** - AI analizi için API key ekleyin
4. ⏭️ **İlk PR İncelemesi** - Test PR'ı webhook ile gönderin

---

## 🎯 Test Checklist

- [ ] Docker servisleri çalışıyor
- [ ] MSSQL bağlantısı başarılı
- [ ] Redis bağlantısı başarılı
- [ ] API başarıyla başladı
- [ ] Swagger UI açılıyor
- [ ] Health check başarılı
- [ ] Admin kullanıcısı ile giriş yapabiliyorum
- [ ] Yeni kullanıcı kaydedebiliyorum

---

**Hazırladı:** AI Assistant  
**Tarih:** 1 Ocak 2025  
**Versiyon:** 1.0.0

