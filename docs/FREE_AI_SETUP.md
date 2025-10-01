# Ücretsiz AI Servisleri Kurulum Kılavuzu

## 🆓 Desteklenen Ücretsiz AI Servisleri

Bu proje **tamamen ücretsiz** AI servisleri kullanır. Hiçbir ücret ödemeden kod inceleme yapabilirsiniz!

---

## 1️⃣ Ollama (Önerilen - Tamamen Ücretsiz, Local)

### ✅ Avantajları:
- **100% Ücretsiz** - Hiçbir maliyet yok
- **Tamamen Offline** - İnternet bağlantısı gerektirmez
- **Gizlilik** - Kodunuz asla dışarı çıkmaz
- **Sınırsız Kullanım** - Rate limit yok
- **Hızlı** - Local çalıştığı için çok hızlı

### 📦 Kurulum

#### Linux:
```bash
curl -fsSL https://ollama.ai/install.sh | sh
```

#### macOS:
```bash
# Homebrew ile
brew install ollama

# Veya direct download
# https://ollama.ai/download/mac
```

#### Windows:
```bash
# Download: https://ollama.ai/download/windows
# Installer'ı çalıştırın
```

### 🚀 Model İndirme

```bash
# DeepSeek Coder (Önerilen - Kod için optimize edilmiş)
ollama pull deepseek-coder

# Alternatifler:
ollama pull codellama        # Meta'nın kod modeli
ollama pull mistral          # Genel amaçlı güçlü model
ollama pull llama3           # Son teknoloji model
```

### ⚙️ Konfigürasyon (appsettings.json)

```json
{
  "AIProvider": {
    "Provider": "Ollama"
  },
  "Ollama": {
    "BaseUrl": "http://localhost:11434",
    "Model": "deepseek-coder"
  }
}
```

### 🧪 Test

```bash
# Ollama'nın çalıştığını test et
curl http://localhost:11434/api/tags

# Model test et
ollama run deepseek-coder "What is a SQL injection?"
```

---

## 2️⃣ Google Gemini (Ücretsiz Tier)

### ✅ Avantajları:
- **Ücretsiz Tier** - Günde 60 istek/dakika
- **Güçlü Model** - Google'ın AI'ı
- **Kolay Entegrasyon** - API basit

### 🔑 API Key Alma

1. https://ai.google.dev adresine gidin
2. "Get API Key" butonuna tıklayın
3. Google hesabınızla giriş yapın
4. API key'i kopyalayın

### ⚙️ Konfigürasyon (appsettings.json)

```json
{
  "AIProvider": {
    "Provider": "Gemini"
  },
  "Gemini": {
    "ApiKey": "YOUR_GEMINI_API_KEY_HERE",
    "Model": "gemini-pro"
  }
}
```

### 📊 Limitler
- **Ücretsiz**: 60 request/dakika
- **Maksimum Token**: 30,000 input + 2,000 output
- **Ücretli geçiş**: Gerekirse

---

## 3️⃣ Hugging Face (Ücretsiz API)

### ✅ Avantajları:
- **Ücretsiz API** - Sınırlı ama yeterli
- **Çok Model Seçeneği** - Binlerce model
- **Açık Kaynak** - Community driven

### 🔑 API Key Alma

1. https://huggingface.co adresine gidin
2. Hesap oluşturun (ücretsiz)
3. Settings → Access Tokens
4. "New token" oluşturun

### ⚙️ Konfigürasyon (appsettings.json)

```json
{
  "AIProvider": {
    "Provider": "HuggingFace"
  },
  "HuggingFace": {
    "ApiKey": "YOUR_HUGGINGFACE_API_KEY_HERE",
    "Model": "bigcode/starcoder"
  }
}
```

### 📊 Önerilen Modeller
- `bigcode/starcoder` - Kod için
- `codellama/CodeLlama-7b-hf` - Meta'nın modeli
- `WizardLM/WizardCoder-15B-V1.0` - Güçlü kod modeli

---

## 📊 AI Provider Karşılaştırması

| Özellik | Ollama | Gemini | Hugging Face |
|---------|--------|--------|--------------|
| **Maliyet** | 🟢 Ücretsiz | 🟢 Ücretsiz Tier | 🟢 Ücretsiz |
| **Kurulum** | Lokal kurulum | API Key | API Key |
| **Gizlilik** | 🟢 Mükemmel | 🟡 Orta | 🟡 Orta |
| **Hız** | 🟢 Çok Hızlı | 🟡 Orta | 🔴 Yavaş |
| **Kalite** | 🟢 İyi | 🟢 Mükemmel | 🟡 Orta |
| **Rate Limit** | 🟢 Yok | 🟡 60/dakika | 🔴 Düşük |
| **İnternet** | 🟢 Gerekmez | 🔴 Gerekli | 🔴 Gerekli |

---

## 💡 Hangisini Seçmeli?

### 🏆 En İyi Seçim: **Ollama**

**Neden Ollama?**
- ✅ Tamamen ücretsiz ve sınırsız
- ✅ Kodunuz asla dışarı çıkmaz (gizlilik)
- ✅ Çok hızlı (local çalışır)
- ✅ İnternet gerektirmez
- ✅ Rate limit yok
- ✅ Kurulumu kolay

**Dezavantajları:**
- ❌ Lokal kurulum gerekir
- ❌ GPU olması tercih edilir (CPU'da da çalışır ama yavaş)
- ❌ Model indirme gerekir (1-7 GB)

---

## 🚀 Hızlı Başlangıç (Ollama)

### 1. Ollama'yı Kurun
```bash
# Linux/Mac
curl -fsSL https://ollama.ai/install.sh | sh

# Servisin çalıştığını kontrol et
curl http://localhost:11434/api/tags
```

### 2. Model İndirin
```bash
ollama pull deepseek-coder
```

### 3. API'yi Çalıştırın
```bash
cd src/Presentation/Api/SmartCodeReview.Api
dotnet run
```

### 4. Test Edin
```bash
# Webhook gönder
curl -X POST http://localhost:5000/api/webhook/github \
  -H "Content-Type: application/json" \
  -d '{
    "action": "opened",
    "number": 1,
    "pull_request": {...},
    "repository": {...}
  }'
```

---

## 🔄 Provider Değiştirme

appsettings.json'da provider değiştirin:

```json
{
  "AIProvider": {
    "Provider": "Ollama"  // Ollama, Gemini, HuggingFace
  }
}
```

---

## 🐳 Docker Compose İle Ollama

Ollama'yı Docker ile çalıştırmak isterseniz:

```yaml
ollama:
  image: ollama/ollama:latest
  container_name: smartcodereview-ollama
  ports:
    - "11434:11434"
  volumes:
    - ollama-data:/root/.ollama
  networks:
    - smartcodereview-network
```

```bash
# Model indirme
docker exec -it smartcodereview-ollama ollama pull deepseek-coder
```

---

## 📈 Performans İpuçları

### Ollama Performansı

**CPU:**
- DeepSeek Coder 1.3B: ~5-10 saniye/analiz
- DeepSeek Coder 6.7B: ~20-30 saniye/analiz

**GPU (NVIDIA):**
- DeepSeek Coder 1.3B: ~1-2 saniye/analiz
- DeepSeek Coder 6.7B: ~3-5 saniye/analiz

**Öneri:**
- Hızlı sonuç için: `deepseek-coder:1.3b`
- Daha iyi kalite için: `deepseek-coder:6.7b`

---

## 🛠️ Sorun Giderme

### Ollama bağlantı hatası

**Sorun:** `Connection refused to localhost:11434`

**Çözüm:**
```bash
# Ollama servisini başlat
ollama serve

# Veya systemctl ile
sudo systemctl start ollama
```

### Model bulunamadı

**Sorun:** `model 'deepseek-coder' not found`

**Çözüm:**
```bash
# Modeli indirin
ollama pull deepseek-coder

# Mevcut modelleri listeleyin
ollama list
```

---

## 💰 Maliyet Karşılaştırması

| Provider | Ücretsiz Limit | Ücretli Başlangıç |
|----------|----------------|-------------------|
| **Ollama** | ∞ Sınırsız | - |
| **Gemini** | 60 req/dk | $0.001/1K token |
| **HuggingFace** | Düşük | $0.0002/1K token |
| **OpenAI** | - | $0.002/1K token |

---

## 🎯 Sonuç

**Smart Code Review Tool**, tamamen **ücretsiz AI servisleri** ile çalışır!

**Önerilen Setup:**
1. **Geliştirme**: Ollama (local, hızlı, ücretsiz)
2. **Production**: Ollama (kendi sunucunuzda) veya Gemini (cloud, güçlü)
3. **Yedek**: HuggingFace (alternatif)

---

**Son Güncelleme:** 1 Ocak 2025  
**Durum:** ✅ Tamamen Ücretsiz AI Desteği

