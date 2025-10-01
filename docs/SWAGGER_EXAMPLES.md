# Swagger UI Kullanım Örnekleri

## 🎯 Swagger'a Erişim

API çalıştıktan sonra Swagger UI'ya şu adresten erişebilirsiniz:

```
https://localhost:7001/swagger
```

---

## 🔐 Authentication Workflow

### 1. Admin ile Giriş Yapma

**Endpoint:** `POST /api/auth/login`

**Swagger'da:**
1. `Auth` bölümünü genişletin
2. `POST /api/auth/login` endpoint'ini tıklayın
3. "Try it out" butonuna tıklayın
4. Request body'yi doldurun:

```json
{
  "email": "admin@gmail.com",
  "password": "Super123!",
  "rememberMe": true
}
```

5. "Execute" butonuna tıklayın
6. Response'da gelen token'ı kopyalayın (ileride eklenecek)

### 2. Yeni Kullanıcı Kaydı

**Endpoint:** `POST /api/auth/register`

```json
{
  "email": "newuser@example.com",
  "password": "SecurePass123!",
  "firstName": "Test",
  "lastName": "User"
}
```

**Beklenen Response:**
```json
{
  "message": "Kullanıcı başarıyla oluşturuldu"
}
```

---

## 🔔 Webhook Test Etme

### GitHub Webhook Simülasyonu

**Endpoint:** `POST /api/webhook/github`

**Test Payload:**
```json
{
  "action": "opened",
  "number": 123,
  "pull_request": {
    "id": 1234567890,
    "number": 123,
    "title": "Add authentication feature",
    "body": "This PR adds JWT authentication",
    "state": "open",
    "user": {
      "login": "johndoe",
      "id": 12345,
      "avatar_url": "https://avatars.githubusercontent.com/u/12345"
    },
    "head": {
      "ref": "feature/auth",
      "sha": "abc123def456789"
    },
    "base": {
      "ref": "main",
      "sha": "xyz789uvw012345"
    },
    "diff_url": "https://github.com/owner/repo/pull/123.diff",
    "created_at": "2025-01-01T10:00:00Z"
  },
  "repository": {
    "id": 987654321,
    "name": "awesome-project",
    "full_name": "johndoe/awesome-project",
    "owner": {
      "login": "johndoe",
      "id": 12345
    },
    "html_url": "https://github.com/johndoe/awesome-project"
  }
}
```

### GitLab Webhook Simülasyonu

**Endpoint:** `POST /api/webhook/gitlab`

**Test Payload:**
```json
{
  "object_kind": "merge_request",
  "event_type": "merge_request",
  "user": {
    "name": "John Doe",
    "username": "johndoe",
    "email": "johndoe@example.com",
    "avatar_url": "https://gitlab.com/uploads/-/system/user/avatar/12345/avatar.png"
  },
  "project": {
    "id": 15,
    "name": "Awesome Project",
    "path_with_namespace": "johndoe/awesome-project",
    "web_url": "https://gitlab.com/johndoe/awesome-project"
  },
  "object_attributes": {
    "id": 99,
    "iid": 1,
    "title": "Add authentication feature",
    "description": "This MR adds JWT authentication",
    "state": "opened",
    "source_branch": "feature/auth",
    "target_branch": "main",
    "created_at": "2025-01-01T10:00:00Z",
    "updated_at": "2025-01-01T10:00:00Z"
  }
}
```

---

## 📊 Response Örnekleri

### Başarılı Webhook Response
```json
{
  "message": "Webhook alındı"
}
```

### Hata Response
```json
{
  "message": "Webhook işlenemedi"
}
```

---

## 🧪 Test Senaryoları

### Senaryo 1: Tam Workflow Test

1. **Admin Girişi:**
   - `POST /api/auth/login` ile admin olarak giriş yap
   - Token'ı al (ileride JWT eklenecek)

2. **Webhook Gönder:**
   - `POST /api/webhook/github` ile test webhook'u gönder
   - Response: `{"message": "Webhook alındı"}`

3. **Sonuçları Kontrol Et (İleride eklenecek):**
   - `GET /api/reviews` ile inceleme listesini al
   - `GET /api/reviews/{id}` ile detayları gör

### Senaryo 2: Hata Durumları

1. **Geçersiz Email ile Kayıt:**
```json
{
  "email": "invalid-email",
  "password": "Test123!",
  "firstName": "Test",
  "lastName": "User"
}
```

**Beklenen Response (400):**
```json
{
  "errors": [
    {
      "code": "InvalidEmail",
      "description": "Email formatı geçersiz"
    }
  ]
}
```

2. **Zayıf Şifre:**
```json
{
  "email": "test@example.com",
  "password": "123",
  "firstName": "Test",
  "lastName": "User"
}
```

**Beklenen Response (400):**
```json
{
  "errors": [
    {
      "code": "PasswordTooShort",
      "description": "Şifre en az 8 karakter olmalıdır"
    }
  ]
}
```

---

## 🎨 Swagger UI Özellikleri

### Authorization Header (İleride eklenecek)

1. Swagger UI'da sağ üstteki **"Authorize"** butonuna tıklayın
2. Value alanına token'ı yapıştırın:
```
Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```
3. "Authorize" butonuna tıklayın
4. Artık tüm korumalı endpoint'leri test edebilirsiniz

### Schema Görüntüleme

1. Her endpoint'in altında **"Schemas"** bölümü var
2. Request/Response modellerini burada görebilirsiniz
3. Örnek değerleri kopyalayabilirsiniz

### Response İndirme

1. Response geldiğinde **"Download"** butonuna tıklayın
2. JSON dosyasını indirebilirsiniz

---

## 📝 Swagger Annotations (İleride eklenecek)

Controller'lara şu annotation'ları ekleyeceğiz:

```csharp
/// <summary>
/// Kullanıcı girişi
/// </summary>
/// <param name="loginDto">Giriş bilgileri</param>
/// <returns>Giriş sonucu</returns>
/// <response code="200">Giriş başarılı</response>
/// <response code="401">Email veya şifre hatalı</response>
[HttpPost("login")]
[ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
{
    // Implementation
}
```

---

## 🔍 Debugging İpuçları

### 1. Request Logging

Swagger UI'daki "curl" komutunu kopyalayıp terminal'de çalıştırabilirsiniz:

```bash
curl -X 'POST' \
  'https://localhost:7001/api/auth/login' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "email": "admin@gmail.com",
  "password": "Super123!",
  "rememberMe": true
}'
```

### 2. Response Headers İnceleme

Response'da "Headers" sekmesine tıklayarak HTTP header'ları görebilirsiniz.

### 3. Validation Hataları

FluentValidation hataları detaylı olarak gösterilir:

```json
{
  "errors": {
    "Email": [
      "Email adresi gereklidir"
    ],
    "Password": [
      "Şifre en az 8 karakter olmalıdır",
      "Şifre en az bir büyük harf içermelidir"
    ]
  }
}
```

---

**Son Güncelleme:** 1 Ocak 2025

