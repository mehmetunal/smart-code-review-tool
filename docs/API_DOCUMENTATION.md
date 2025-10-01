# Smart Code Review Tool - API Dokümantasyonu

## 🚀 Genel Bilgiler

- **Base URL**: `https://localhost:7001`
- **API Version**: `v1.0`
- **Authentication**: JWT Bearer Token
- **Content-Type**: `application/json`

---

## 📋 İçindekiler

1. [Authentication API](#authentication-api)
2. [Webhook API](#webhook-api)
3. [Code Review API](#code-review-api)
4. [Project API](#project-api)
5. [API Configuration API](#api-configuration-api)
6. [Statistics API](#statistics-api)
7. [Health Check API](#health-check-api)

---

## 🔐 Authentication API

### 1. Kullanıcı Kaydı

**Endpoint:** `POST /api/auth/register`

**Request:**
```json
{
  "email": "user@example.com",
  "password": "SecurePass123!",
  "firstName": "John",
  "lastName": "Doe"
}
```

**Response (Success - 200 OK):**
```json
{
  "message": "Kullanıcı başarıyla oluşturuldu"
}
```

**Response (Error - 400 Bad Request):**
```json
{
  "errors": [
    {
      "code": "DuplicateUserName",
      "description": "Bu email adresi zaten kullanılıyor"
    }
  ]
}
```

---

### 2. Kullanıcı Girişi

**Endpoint:** `POST /api/auth/login`

**Request:**
```json
{
  "email": "admin@gmail.com",
  "password": "Super123!",
  "rememberMe": true
}
```

**Response (Success - 200 OK):**
```json
{
  "message": "Giriş başarılı"
}
```

**Response (Error - 401 Unauthorized):**
```json
{
  "message": "Email veya şifre hatalı"
}
```

**Response (Locked - 400 Bad Request):**
```json
{
  "message": "Hesap kilitlendi"
}
```

---

## 🔔 Webhook API

### 1. GitHub Webhook

**Endpoint:** `POST /api/webhook/github`

**Headers:**
```
X-GitHub-Event: pull_request
X-Hub-Signature-256: sha256=xxx
```

**Request (Pull Request Opened):**
```json
{
  "action": "opened",
  "number": 123,
  "pull_request": {
    "id": 1234567890,
    "number": 123,
    "title": "Add new feature",
    "body": "This PR adds a new feature",
    "state": "open",
    "user": {
      "login": "johndoe",
      "id": 12345
    },
    "head": {
      "ref": "feature/new-feature",
      "sha": "abc123def456"
    },
    "base": {
      "ref": "main",
      "sha": "xyz789uvw012"
    }
  },
  "repository": {
    "id": 987654321,
    "name": "my-repo",
    "full_name": "johndoe/my-repo",
    "owner": {
      "login": "johndoe"
    }
  }
}
```

**Response (Success - 200 OK):**
```json
{
  "message": "Webhook alındı"
}
```

**Response (Error - 500 Internal Server Error):**
```json
{
  "message": "Webhook işlenemedi"
}
```

---

### 2. GitLab Webhook

**Endpoint:** `POST /api/webhook/gitlab`

**Headers:**
```
X-Gitlab-Event: Merge Request Hook
X-Gitlab-Token: secret-token
```

**Request (Merge Request Opened):**
```json
{
  "object_kind": "merge_request",
  "event_type": "merge_request",
  "user": {
    "name": "John Doe",
    "username": "johndoe",
    "email": "johndoe@example.com"
  },
  "project": {
    "id": 15,
    "name": "My Project",
    "path_with_namespace": "johndoe/my-project"
  },
  "object_attributes": {
    "id": 99,
    "iid": 1,
    "title": "Add new feature",
    "description": "This MR adds a new feature",
    "state": "opened",
    "source_branch": "feature/new-feature",
    "target_branch": "main"
  }
}
```

**Response (Success - 200 OK):**
```json
{
  "message": "Webhook alındı"
}
```

---

## 📝 Code Review API

### 1. Kod İncelemeleri Listesi

**Endpoint:** `GET /api/reviews`

**Headers:**
```
Authorization: Bearer {token}
```

**Query Parameters:**
```
?page=1&pageSize=10&status=Completed&projectId={guid}
```

**Response (Success - 200 OK):**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "pullRequestNumber": 123,
        "title": "Add new feature",
        "description": "This PR adds authentication",
        "pullRequestUrl": "https://github.com/owner/repo/pull/123",
        "branchName": "feature/auth",
        "status": "Completed",
        "qualityScore": 85,
        "totalIssuesCount": 5,
        "criticalIssuesCount": 0,
        "highIssuesCount": 1,
        "mediumIssuesCount": 2,
        "lowIssuesCount": 2,
        "analysisStartTime": "2025-01-01T10:00:00Z",
        "analysisEndTime": "2025-01-01T10:05:30Z",
        "projectId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
        "userId": "9b1deb4d-3b7d-4bad-9bdd-2b0d7b3dcb6d",
        "createdDate": "2025-01-01T10:00:00Z"
      }
    ],
    "totalCount": 45,
    "page": 1,
    "pageSize": 10,
    "totalPages": 5
  },
  "statusCode": 200
}
```

---

### 2. Kod İnceleme Detayı

**Endpoint:** `GET /api/reviews/{id}`

**Headers:**
```
Authorization: Bearer {token}
```

**Response (Success - 200 OK):**
```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "pullRequestNumber": 123,
    "title": "Add new feature",
    "description": "This PR adds authentication functionality",
    "pullRequestUrl": "https://github.com/owner/repo/pull/123",
    "branchName": "feature/auth",
    "status": "Completed",
    "qualityScore": 85,
    "totalIssuesCount": 5,
    "criticalIssuesCount": 0,
    "highIssuesCount": 1,
    "mediumIssuesCount": 2,
    "lowIssuesCount": 2,
    "analysisStartTime": "2025-01-01T10:00:00Z",
    "analysisEndTime": "2025-01-01T10:05:30Z",
    "projectId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    "analyses": [
      {
        "id": "8f1e4d2a-3b7d-4bad-9bdd-2b0d7b3dcb6d",
        "title": "Potential SQL Injection",
        "description": "String concatenation in SQL query can lead to SQL injection",
        "category": "Security",
        "severity": "High",
        "filePath": "src/Services/UserService.cs",
        "lineNumber": 45,
        "codeSnippet": "var query = \"SELECT * FROM Users WHERE Id = \" + userId;",
        "suggestion": "Use parameterized queries or ORM instead",
        "isAIGenerated": true
      },
      {
        "id": "1a2b3c4d-5e6f-7g8h-9i0j-k1l2m3n4o5p6",
        "title": "Missing null check",
        "description": "Potential NullReferenceException",
        "category": "Bug",
        "severity": "Medium",
        "filePath": "src/Controllers/AuthController.cs",
        "lineNumber": 78,
        "codeSnippet": "var userName = user.Name.ToLower();",
        "suggestion": "Add null check: if (user?.Name != null)",
        "isAIGenerated": true
      }
    ],
    "fileAnalyses": [
      {
        "id": "9c8e7d6f-5a4b-3c2d-1e0f-a1b2c3d4e5f6",
        "filePath": "src/Services/UserService.cs",
        "fileName": "UserService.cs",
        "language": "CSharp",
        "addedLines": 45,
        "deletedLines": 12,
        "totalChanges": 57,
        "qualityScore": 75,
        "issuesCount": 2
      }
    ],
    "createdDate": "2025-01-01T10:00:00Z"
  },
  "statusCode": 200
}
```

**Response (Not Found - 404):**
```json
{
  "success": false,
  "message": "Kod incelemesi bulunamadı",
  "statusCode": 404
}
```

---

## 📊 Project API

### 1. Proje Oluşturma

**Endpoint:** `POST /api/projects`

**Headers:**
```
Authorization: Bearer {token}
```

**Request:**
```json
{
  "name": "My Awesome Project",
  "description": "A great project for code reviews",
  "repositoryUrl": "https://github.com/johndoe/awesome-project",
  "fullName": "johndoe/awesome-project",
  "repositoryId": 123456789,
  "webhookSecret": "my-webhook-secret-key"
}
```

**Response (Success - 201 Created):**
```json
{
  "success": true,
  "data": {
    "id": "a1b2c3d4-e5f6-7g8h-9i0j-k1l2m3n4o5p6",
    "name": "My Awesome Project",
    "description": "A great project for code reviews",
    "repositoryUrl": "https://github.com/johndoe/awesome-project",
    "fullName": "johndoe/awesome-project",
    "repositoryId": 123456789,
    "isActive": true,
    "createdDate": "2025-01-01T12:00:00Z"
  },
  "message": "Proje başarıyla oluşturuldu",
  "statusCode": 201
}
```

---

### 2. Proje Listesi

**Endpoint:** `GET /api/projects`

**Headers:**
```
Authorization: Bearer {token}
```

**Query Parameters:**
```
?page=1&pageSize=20&isActive=true
```

**Response (Success - 200 OK):**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "a1b2c3d4-e5f6-7g8h-9i0j-k1l2m3n4o5p6",
        "name": "My Awesome Project",
        "description": "A great project",
        "repositoryUrl": "https://github.com/johndoe/awesome-project",
        "fullName": "johndoe/awesome-project",
        "isActive": true,
        "totalReviews": 45,
        "lastReviewDate": "2025-01-01T10:00:00Z",
        "createdDate": "2024-12-01T08:00:00Z"
      }
    ],
    "totalCount": 5,
    "page": 1,
    "pageSize": 20,
    "totalPages": 1
  },
  "statusCode": 200
}
```

---

## 📈 Statistics API

### 1. Dashboard İstatistikleri

**Endpoint:** `GET /api/statistics/dashboard`

**Headers:**
```
Authorization: Bearer {token}
```

**Response (Success - 200 OK):**
```json
{
  "success": true,
  "data": {
    "totalReviews": 1234,
    "completedReviews": 1100,
    "failedReviews": 34,
    "pendingReviews": 100,
    "totalIssuesFound": 5678,
    "criticalIssues": 45,
    "highIssues": 234,
    "mediumIssues": 789,
    "lowIssues": 4610,
    "averageQualityScore": 78.5,
    "totalProjects": 15,
    "activeProjects": 12,
    "averageReviewTime": "00:05:30",
    "last30DaysReviews": [
      {
        "date": "2025-01-01",
        "count": 25,
        "averageScore": 82.3
      },
      {
        "date": "2024-12-31",
        "count": 18,
        "averageScore": 79.1
      }
    ],
    "topIssueCategories": [
      {
        "category": "CodeQuality",
        "count": 2500,
        "percentage": 44.0
      },
      {
        "category": "Performance",
        "count": 1500,
        "percentage": 26.4
      },
      {
        "category": "Security",
        "count": 800,
        "percentage": 14.1
      }
    ]
  },
  "statusCode": 200
}
```

---

### 2. Proje İstatistikleri

**Endpoint:** `GET /api/statistics/projects/{projectId}`

**Headers:**
```
Authorization: Bearer {token}
```

**Response (Success - 200 OK):**
```json
{
  "success": true,
  "data": {
    "projectId": "a1b2c3d4-e5f6-7g8h-9i0j-k1l2m3n4o5p6",
    "projectName": "My Awesome Project",
    "totalReviews": 156,
    "completedReviews": 145,
    "failedReviews": 5,
    "pendingReviews": 6,
    "averageQualityScore": 81.2,
    "totalIssuesFound": 678,
    "criticalIssues": 3,
    "highIssues": 45,
    "mediumIssues": 120,
    "lowIssues": 510,
    "mostActiveContributors": [
      {
        "userId": "9b1deb4d-3b7d-4bad-9bdd-2b0d7b3dcb6d",
        "userName": "John Doe",
        "reviewCount": 45,
        "averageScore": 85.3
      }
    ],
    "languageDistribution": [
      {
        "language": "CSharp",
        "fileCount": 250,
        "percentage": 65.0
      },
      {
        "language": "JavaScript",
        "fileCount": 100,
        "percentage": 26.0
      }
    ]
  },
  "statusCode": 200
}
```

---

## ❤️ Health Check API

### Health Check

**Endpoint:** `GET /health`

**Response (Success - 200 OK):**
```json
{
  "status": "Healthy",
  "timestamp": "2025-01-01T12:34:56.789Z",
  "version": "1.0.0"
}
```

---

## 🔒 Authentication

Tüm korumalı endpoint'ler için JWT Bearer token gereklidir:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Token alma örneği:**
```bash
curl -X POST https://localhost:7001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@gmail.com",
    "password": "Super123!"
  }'
```

---

## 📊 Standart Response Formatı

Tüm API yanıtları aşağıdaki standart formatı kullanır:

### Başarılı Yanıt:
```json
{
  "success": true,
  "data": { },
  "message": "İşlem başarılı",
  "statusCode": 200
}
```

### Hata Yanıtı:
```json
{
  "success": false,
  "message": "Hata mesajı",
  "errors": [
    {
      "field": "email",
      "message": "Email adresi geçersiz"
    }
  ],
  "statusCode": 400
}
```

### Validasyon Hatası:
```json
{
  "success": false,
  "message": "Validasyon hatası",
  "errors": [
    {
      "field": "password",
      "message": "Şifre en az 8 karakter olmalıdır"
    },
    {
      "field": "email",
      "message": "Email adresi gereklidir"
    }
  ],
  "statusCode": 400
}
```

---

## 🚦 HTTP Status Kodları

| Kod | Açıklama |
|-----|----------|
| **200** | OK - İşlem başarılı |
| **201** | Created - Kayıt başarıyla oluşturuldu |
| **400** | Bad Request - Geçersiz istek veya validasyon hatası |
| **401** | Unauthorized - Kimlik doğrulama gerekli |
| **403** | Forbidden - Yetki yetersiz |
| **404** | Not Found - Kaynak bulunamadı |
| **500** | Internal Server Error - Sunucu hatası |

---

## ⚙️ API Configuration API

API provider'ları (GitHub, AI servisleri) yönetmek için kullanılır.

### **POST** `/api/apiconfiguration` - API Konfigürasyonu Oluştur

Yeni bir API provider konfigürasyonu oluşturur.

**Headers:**
```
Authorization: Bearer {jwt_token}
Content-Type: application/json
```

**Request Body:**
```json
{
  "apiType": "GitHub",
  "apiName": "GitHub API",
  "apiKey": "ghp_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
  "webhookSecret": "my-super-secret-webhook-key-12345",
  "model": "gpt-4",
  "baseUrl": "https://api.github.com",
  "isActive": true,
  "isDefault": false,
  "description": "GitHub API entegrasyonu"
}
```

**Response (201):**
```json
{
  "success": true,
  "data": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "apiType": "GitHub",
    "apiName": "GitHub API",
    "apiKey": "ghp_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
    "webhookSecret": "my-super-secret-webhook-key-12345",
    "model": "gpt-4",
    "baseUrl": "https://api.github.com",
    "isActive": true,
    "isDefault": false,
    "description": "GitHub API entegrasyonu",
    "userId": "456e7890-e89b-12d3-a456-426614174001",
    "createdDate": "2025-01-01T10:00:00Z",
    "updatedDate": null
  }
}
```

### **GET** `/api/apiconfiguration/{id}` - Konfigürasyon Getir

Belirli bir API konfigürasyonunu getirir.

**Headers:**
```
Authorization: Bearer {jwt_token}
```

**Response (200):**
```json
{
  "success": true,
  "data": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "apiType": "GitHub",
    "apiName": "GitHub API",
    "apiKey": "ghp_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
    "webhookSecret": "my-super-secret-webhook-key-12345",
    "isActive": true,
    "createdDate": "2025-01-01T10:00:00Z"
  }
}
```

### **GET** `/api/apiconfiguration/by-type/{apiType}` - Türüne Göre Getir

Belirli bir API türüne ait aktif konfigürasyonu getirir.

**Parameters:**
- `apiType` (string): API türü (GitHub, Gemini, HuggingFace, OpenAI, Ollama)

**Headers:**
```
Authorization: Bearer {jwt_token}
```

**Response (200):**
```json
{
  "success": true,
  "data": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "apiType": "GitHub",
    "apiName": "GitHub API",
    "apiKey": "ghp_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
    "isActive": true
  }
}
```

### **GET** `/api/apiconfiguration/my-configurations` - Kullanıcı Konfigürasyonları

Kullanıcının tüm API konfigürasyonlarını listeler.

**Query Parameters:**
- `page` (int, optional): Sayfa numarası (varsayılan: 1)
- `pageSize` (int, optional): Sayfa boyutu (varsayılan: 10)

**Headers:**
```
Authorization: Bearer {jwt_token}
```

**Response (200):**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "123e4567-e89b-12d3-a456-426614174000",
        "apiType": "GitHub",
        "apiName": "GitHub API",
        "isActive": true,
        "createdDate": "2025-01-01T10:00:00Z"
      }
    ],
    "totalCount": 1,
    "page": 1,
    "pageSize": 10,
    "totalPages": 1
  }
}
```

### **GET** `/api/apiconfiguration/all-active` - Tüm Aktif Konfigürasyonlar

Sistemdeki tüm aktif API konfigürasyonlarını getirir.

**Headers:**
```
Authorization: Bearer {jwt_token}
```

**Response (200):**
```json
{
  "success": true,
  "data": [
    {
      "id": "123e4567-e89b-12d3-a456-426614174000",
      "apiType": "GitHub",
      "apiName": "GitHub API",
      "isActive": true
    },
    {
      "id": "456e7890-e89b-12d3-a456-426614174001",
      "apiType": "Ollama",
      "apiName": "Ollama AI",
      "isActive": true
    }
  ]
}
```

### **PUT** `/api/apiconfiguration/{id}` - Konfigürasyon Güncelle

Mevcut bir API konfigürasyonunu günceller.

**Headers:**
```
Authorization: Bearer {jwt_token}
Content-Type: application/json
```

**Request Body:**
```json
{
  "apiType": "GitHub",
  "apiName": "GitHub API Updated",
  "apiKey": "ghp_new_token_xxxxxxxxxxxxxxxxxxxxxxxxxxxx",
  "webhookSecret": "new-secret-key",
  "isActive": true,
  "description": "Updated GitHub API configuration"
}
```

**Response (200):**
```json
{
  "success": true,
  "data": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "apiType": "GitHub",
    "apiName": "GitHub API Updated",
    "apiKey": "ghp_new_token_xxxxxxxxxxxxxxxxxxxxxxxxxxxx",
    "isActive": true,
    "updatedDate": "2025-01-01T11:00:00Z"
  }
}
```

### **DELETE** `/api/apiconfiguration/{id}` - Konfigürasyon Sil

Bir API konfigürasyonunu siler.

**Headers:**
```
Authorization: Bearer {jwt_token}
```

**Response (200):**
```json
{
  "success": true
}
```

### **POST** `/api/apiconfiguration/create-defaults` - Varsayılan Konfigürasyonlar (Admin)

Sistem için varsayılan API konfigürasyonlarını oluşturur.

**Headers:**
```
Authorization: Bearer {admin_jwt_token}
```

**Response (200):**
```json
{
  "success": true,
  "message": "Varsayılan konfigürasyonlar oluşturuldu"
}
```

---

## 📝 Notlar

1. Tüm tarih/saat değerleri **UTC** formatındadır
2. GUID'ler **lowercase** formatında döner
3. Sayfalama **1-based** index kullanır (ilk sayfa = 1)
4. Maksimum sayfa boyutu: **100**
5. Rate limiting: **100 request/dakika** (authentication hariç)

---

**Son Güncelleme:** 1 Ocak 2025  
**API Version:** 1.0.0

