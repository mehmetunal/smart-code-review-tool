# API Provider Kurulum Kılavuzu

## 🔑 API Provider'ları Nasıl Kayıt Edilir?

Smart Code Review Tool artık tüm API key'leri database'de güvenli şekilde saklar. Bu kılavuz, farklı AI provider'ları ve GitHub API'sini nasıl kayıt edeceğinizi gösterir.

---

## 📋 Desteklenen Provider'lar

| Provider | Tür | Ücretsiz Limit | Kurulum Zorluğu |
|----------|-----|----------------|-----------------|
| **Ollama** | Local AI | Sınırsız | ⭐ Kolay |
| **GitHub** | Git Platform | Sınırsız (Public) | ⭐⭐ Orta |
| **Google Gemini** | Cloud AI | 60 req/dakika | ⭐⭐⭐ Zor |
| **Hugging Face** | Cloud AI | Rate limit | ⭐⭐⭐ Zor |
| **OpenAI** | Cloud AI | $5 kredi | ⭐⭐⭐ Zor |

---

## 🚀 Hızlı Başlangıç

### 1. **Ollama (Önerilen - Ücretsiz)**

Ollama otomatik olarak eklenir, ancak manuel olarak da ekleyebilirsiniz:

```bash
POST /api/apiconfiguration
Content-Type: application/json

{
  "apiType": "Ollama",
  "apiName": "Ollama AI",
  "apiKey": "local",
  "baseUrl": "http://localhost:11434",
  "model": "deepseek-coder",
  "isActive": true,
  "isDefault": true,
  "description": "Yerel Ollama AI servisi (tamamen ücretsiz)"
}
```

### 2. **GitHub API (Zorunlu)**

```bash
POST /api/apiconfiguration
Content-Type: application/json

{
  "apiType": "GitHub",
  "apiName": "GitHub API",
  "apiKey": "ghp_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
  "webhookSecret": "my-super-secret-webhook-key-12345",
  "isActive": true,
  "description": "GitHub API entegrasyonu"
}
```

---

## 🤖 AI Provider'ları

### **1. Google Gemini (Ücretsiz Tier)**

#### API Key Alma:
1. https://aistudio.google.com/app/apikey adresine gidin
2. Google hesabınızla giriş yapın
3. "Create API Key" tıklayın
4. API key'i kopyalayın

#### Kayıt:
```bash
POST /api/apiconfiguration
Content-Type: application/json

{
  "apiType": "Gemini",
  "apiName": "Google Gemini",
  "apiKey": "AIzaSyxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
  "model": "gemini-pro",
  "baseUrl": "https://generativelanguage.googleapis.com/v1beta",
  "isActive": true,
  "description": "Google Gemini AI (60 req/dakika ücretsiz)"
}
```

### **2. Hugging Face (Ücretsiz)**

#### API Key Alma:
1. https://huggingface.co/join adresine kaydolun
2. https://huggingface.co/settings/tokens adresine gidin
3. "New token" tıklayın
4. "Write" permission verin
5. Token'ı kopyalayın

#### Kayıt:
```bash
POST /api/apiconfiguration
Content-Type: application/json

{
  "apiType": "HuggingFace",
  "apiName": "Hugging Face",
  "apiKey": "hf_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
  "model": "bigcode/starcoder",
  "baseUrl": "https://api-inference.huggingface.co",
  "isActive": true,
  "description": "Hugging Face AI (ücretsiz tier)"
}
```

### **3. OpenAI (Ücretli)**

#### API Key Alma:
1. https://platform.openai.com/api-keys adresine gidin
2. OpenAI hesabınızla giriş yapın
3. "Create new secret key" tıklayın
4. API key'i kopyalayın

#### Kayıt:
```bash
POST /api/apiconfiguration
Content-Type: application/json

{
  "apiType": "OpenAI",
  "apiName": "OpenAI GPT",
  "apiKey": "sk-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
  "model": "gpt-4",
  "baseUrl": "https://api.openai.com/v1",
  "isActive": true,
  "description": "OpenAI GPT-4 (ücretli)"
}
```

---

## 🔧 GitHub API Kurulumu

### **1. GitHub Token Alma:**

#### Adım 1: GitHub'a giriş
- https://github.com adresine gidin
- Hesabınızla giriş yapın

#### Adım 2: Settings'e gidin
- Sağ üst köşedeki profil fotoğrafınıza tıklayın
- **"Settings"** seçin

#### Adım 3: Developer settings
- Sol menüden en alta kaydırın
- **"Developer settings"** tıklayın

#### Adım 4: Personal Access Token
- **"Personal access tokens"** → **"Tokens (classic)"**
- **"Generate new token"** → **"Generate new token (classic)"**

#### Adım 5: Token ayarları
```
Note: Smart Code Review Tool
Expiration: 1 year (veya istediğiniz süre)
Scopes: ✅ repo (Full control of private repositories)
```

#### Adım 6: Token'ı kopyalayın
- **"Generate token"** tıklayın
- **Token'ı kopyalayın** (bir daha göremezsiniz!)

### **2. Webhook Secret (Opsiyonel)**

#### Adım 1: Repository'ye gidin
- Webhook ekleyeceğiniz repository'ye gidin

#### Adım 2: Settings → Webhooks
- **"Settings"** → **"Webhooks"** → **"Add webhook"**

#### Adım 3: Secret oluşturun
- **"Secret"** alanına rastgele bir string yazın
- Örnek: `my-super-secret-webhook-key-12345`

---

## 📝 Request Body Modelleri

### **ApiConfiguration Model:**

```json
{
  "apiType": "string",           // Zorunlu: "GitHub", "Gemini", "HuggingFace", "OpenAI", "Ollama"
  "apiName": "string",           // Zorunlu: "GitHub API", "Google Gemini", vb.
  "apiKey": "string",            // Zorunlu: API key (şifrelenerek saklanır)
  "webhookSecret": "string",     // Opsiyonel: GitHub webhook secret
  "model": "string",             // Opsiyonel: AI model adı
  "baseUrl": "string",           // Opsiyonel: API base URL
  "isActive": "boolean",         // Opsiyonel: true/false (varsayılan: true)
  "isDefault": "boolean",        // Opsiyonel: true/false (varsayılan: false)
  "description": "string",       // Opsiyonel: Açıklama
  "userId": "guid"               // Opsiyonel: Kullanıcı ID (JWT'den alınır)
}
```

### **Validation Kuralları:**

```json
{
  "apiType": {
    "required": true,
    "maxLength": 50,
    "enum": ["GitHub", "Gemini", "HuggingFace", "OpenAI", "Ollama", "GitLab"]
  },
  "apiName": {
    "required": true,
    "maxLength": 100
  },
  "apiKey": {
    "required": true,
    "maxLength": 500
  },
  "webhookSecret": {
    "maxLength": 500
  },
  "model": {
    "maxLength": 100
  },
  "baseUrl": {
    "maxLength": 500,
    "format": "uri"
  },
  "description": {
    "maxLength": 1000
  }
}
```

---

## 🔄 API Endpoint'leri

### **1. API Konfigürasyonu Oluştur**
```bash
POST /api/apiconfiguration
Authorization: Bearer {jwt_token}
Content-Type: application/json

{
  "apiType": "GitHub",
  "apiName": "GitHub API",
  "apiKey": "ghp_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
  "webhookSecret": "my-secret-key",
  "isActive": true,
  "description": "GitHub API entegrasyonu"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "apiType": "GitHub",
    "apiName": "GitHub API",
    "apiKey": "ghp_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
    "webhookSecret": "my-secret-key",
    "isActive": true,
    "createdDate": "2025-01-01T10:00:00Z"
  }
}
```

### **2. API Konfigürasyonu Getir**
```bash
GET /api/apiconfiguration/{id}
Authorization: Bearer {jwt_token}
```

### **3. API Türüne Göre Getir**
```bash
GET /api/apiconfiguration/by-type/GitHub
Authorization: Bearer {jwt_token}
```

### **4. Kullanıcının Konfigürasyonları**
```bash
GET /api/apiconfiguration/my-configurations?page=1&pageSize=10
Authorization: Bearer {jwt_token}
```

### **5. Tüm Aktif Konfigürasyonlar**
```bash
GET /api/apiconfiguration/all-active
Authorization: Bearer {jwt_token}
```

### **6. Konfigürasyon Güncelle**
```bash
PUT /api/apiconfiguration/{id}
Authorization: Bearer {jwt_token}
Content-Type: application/json

{
  "apiType": "GitHub",
  "apiName": "GitHub API Updated",
  "apiKey": "ghp_new_token_xxxxxxxxxxxxxxxxxxxxxxxxxxxx",
  "isActive": true,
  "description": "Updated GitHub API"
}
```

### **7. Konfigürasyon Sil**
```bash
DELETE /api/apiconfiguration/{id}
Authorization: Bearer {jwt_token}
```

### **8. Varsayılan Konfigürasyonları Oluştur (Admin)**
```bash
POST /api/apiconfiguration/create-defaults
Authorization: Bearer {admin_jwt_token}
```

---

## 🔐 Güvenlik Özellikleri

### **1. Şifreleme:**
- Tüm API key'ler AES-256 ile şifrelenir
- Şifreleme anahtarı `appsettings.json`'da tanımlı
- Database'de sadece şifrelenmiş veriler saklanır

### **2. Kullanıcı İzolasyonu:**
- Her kullanıcı sadece kendi API key'lerini görebilir
- Admin tüm konfigürasyonları yönetebilir
- API key'ler kullanıcı bazlı saklanır

### **3. Aktif/Pasif Yönetimi:**
- API key'leri aktif/pasif yapabilirsiniz
- Pasif key'ler kullanılmaz
- Varsayılan provider'lar otomatik aktif

---

## 🚨 Hata Kodları

| Kod | Açıklama | Çözüm |
|-----|----------|-------|
| 400 | Geçersiz request body | Validation kurallarını kontrol edin |
| 401 | Yetkisiz erişim | JWT token'ınızı kontrol edin |
| 403 | Yetki yok | Admin yetkisi gerekli |
| 404 | Konfigürasyon bulunamadı | ID'yi kontrol edin |
| 409 | Konfigürasyon zaten mevcut | Farklı bir isim kullanın |
| 500 | Sunucu hatası | Log'ları kontrol edin |

---

## 💡 İpuçları

### **1. En İyi Pratikler:**
- ✅ API key'leri düzenli olarak yenileyin
- ✅ Güçlü webhook secret'ları kullanın
- ✅ Sadece gerekli permission'ları verin
- ✅ API key'leri environment variable'larda saklamayın

### **2. Test Etme:**
```bash
# Health check
curl http://localhost:5000/health

# API konfigürasyonu test
curl -X GET https://localhost:7001/api/apiconfiguration/all-active \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -k
```

### **3. Sorun Giderme:**
- API key'lerin doğru olduğundan emin olun
- Network bağlantısını kontrol edin
- Log'ları inceleyin
- Swagger UI'da test edin

---

## 📊 Maliyet Karşılaştırması

| Provider | Aylık Maliyet | Kullanım Limiti | Kalite |
|----------|---------------|-----------------|--------|
| **Ollama** | $0 | Sınırsız | ⭐⭐⭐⭐ |
| **GitHub** | $0 | Sınırsız (Public) | ⭐⭐⭐⭐⭐ |
| **Gemini** | $0 | 60 req/dakika | ⭐⭐⭐⭐ |
| **HuggingFace** | $0 | Rate limit | ⭐⭐⭐ |
| **OpenAI** | $20+ | Kredi bazlı | ⭐⭐⭐⭐⭐ |

**Önerilen Kombinasyon:** Ollama + GitHub (Tamamen ücretsiz!)

---

## 🎯 Sonraki Adımlar

1. **Ollama'yı kurun** (otomatik eklenir)
2. **GitHub token alın** ve kaydedin
3. **Webhook'ları ayarlayın**
4. **İlk PR'ı test edin**

**Artık AI kod inceleme sisteminiz hazır!** 🚀

---

**Son Güncelleme:** 1 Ocak 2025  
**Versiyon:** 1.0.0
