#!/bin/bash

# Smart Code Review Tool - Docker Compose Başlatma Script'i

echo "🚀 Smart Code Review Tool - Docker servisleri başlatılıyor..."

# Docker Compose ile servisleri başlat
docker compose up -d

# Servislerin başlamasını bekle
echo "⏳ Servisler başlatılıyor (30 saniye bekleniyor)..."
sleep 30

# Servislerin durumunu kontrol et
echo "📊 Servis durumları:"
docker compose ps

# MSSQL container'ın çalıştığını kontrol et
if docker ps | grep -q smartcodereview-mssql; then
    echo "✅ MSSQL Server çalışıyor"
    
    # Veritabanını oluştur
    echo "📦 Veritabanı oluşturuluyor..."
    docker exec -it smartcodereview-mssql /opt/mssql-tools18/bin/sqlcmd \
        -S localhost \
        -U sa \
        -P "YourStrong!Passw0rd" \
        -C \
        -Q "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'SmartCodeReviewDb') CREATE DATABASE SmartCodeReviewDb"
    
    echo "✅ Veritabanı kontrol edildi/oluşturuldu"
    
    # Veritabanlarını listele
    echo "📋 Mevcut veritabanları:"
    docker exec -it smartcodereview-mssql /opt/mssql-tools18/bin/sqlcmd \
        -S localhost \
        -U sa \
        -P "YourStrong!Passw0rd" \
        -C \
        -Q "SELECT name FROM sys.databases"
else
    echo "❌ MSSQL Server başlatılamadı"
fi

# Redis container'ın çalıştığını kontrol et
if docker ps | grep -q smartcodereview-redis; then
    echo "✅ Redis Server çalışıyor"
    
    # Redis ping testi
    docker exec smartcodereview-redis redis-cli ping
else
    echo "❌ Redis Server başlatılamadı"
fi

# Ollama container'ın çalıştığını kontrol et
if docker ps | grep -q smartcodereview-ollama; then
    echo "✅ Ollama AI Server çalışıyor"
    
    echo ""
    echo "📥 AI Model indiriliyor (deepseek-coder)..."
    echo "Bu işlem 3-5 dakika sürebilir (model boyutu: ~3.8GB)"
    docker exec smartcodereview-ollama ollama pull deepseek-coder
    
    echo "✅ Model indirildi ve hazır!"
else
    echo "❌ Ollama AI Server başlatılamadı"
fi

echo ""
echo "🎯 Bağlantı Bilgileri:"
echo "======================================"
echo "MSSQL Server: localhost,1433"
echo "Database: SmartCodeReviewDb"
echo "Username: sa"
echo "Password: YourStrong!Passw0rd"
echo "======================================"
echo "Redis: localhost:6379"
echo "======================================"
echo "Ollama AI: http://localhost:11434"
echo "Model: deepseek-coder"
echo "======================================"
echo ""
echo "📝 Sonraki Adımlar:"
echo "1. API projesini çalıştırın:"
echo "   cd src/Presentation/Api/SmartCodeReview.Api"
echo "   dotnet run"
echo ""
echo "2. Swagger'a erişin:"
echo "   https://localhost:7001/swagger"
echo ""
echo "3. Health check:"
echo "   curl http://localhost:5000/health"
echo ""
echo "🛑 Servisleri durdurmak için:"
echo "   docker compose down"
echo ""

