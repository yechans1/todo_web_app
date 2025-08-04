#!/bin/bash

# 간단한 Azure Web App 배포 스크립트
echo "🚀 TodoWeb 간단 배포 시작"

# 변수 설정
RESOURCE_GROUP="TodoWeb-RG"
LOCATION="koreacentral"
APP_NAME="todoweb-$(date +%s)"
APP_SERVICE_PLAN="TodoWeb-Plan"

echo "📋 배포 정보:"
echo "  앱 이름: $APP_NAME"
echo "  리소스 그룹: $RESOURCE_GROUP"

# 1. App Service Plan 생성 (Linux, 무료 티어)
echo "1️⃣ App Service Plan 생성 중..."
az appservice plan create \
  --name $APP_SERVICE_PLAN \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --sku F1 \
  --is-linux

# 2. Web App 생성 (SQLite 사용)
echo "2️⃣ Web App 생성 중..."
az webapp create \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --plan $APP_SERVICE_PLAN \
  --runtime "DOTNETCORE:8.0"

# 3. SQLite를 위한 환경 변수 설정
echo "3️⃣ 환경 변수 설정 중..."
az webapp config appsettings set \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --settings \
    ASPNETCORE_ENVIRONMENT=Production \
    ConnectionStrings__DefaultConnection="Data Source=/home/site/wwwroot/TodoWeb.db" \
    JwtSettings__SecretKey="TodoWebApp-Production-SuperSecretKey-32CharactersLong!" \
    JwtSettings__Issuer="TodoWebApp-Production" \
    JwtSettings__Audience="TodoWebApp-Users" \
    JwtSettings__ExpirationHours="8" \
    AdminAccount__Username="admin" \
    AdminAccount__Password="admin"

# 4. 앱 배포 (ZIP 방식)
echo "4️⃣ 앱 빌드 및 배포 중..."
dotnet publish -c Release -o ./publish
cd publish
zip -r ../app.zip .
cd ..

az webapp deployment source config-zip \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --src app.zip

# 5. 앱 URL 가져오기
echo "5️⃣ 배포 완료!"
APP_URL="https://${APP_NAME}.azurewebsites.net"

echo "✅ 배포 성공!"
echo "🌐 앱 URL: $APP_URL"
echo "📚 Swagger: $APP_URL/swagger"
echo "🔐 로그인: admin / admin"
echo ""
echo "📋 리소스 정보:"
echo "  리소스 그룹: $RESOURCE_GROUP"
echo "  앱 이름: $APP_NAME"
echo "  앱 서비스 플랜: $APP_SERVICE_PLAN"

# 정리
rm -f app.zip
rm -rf publish