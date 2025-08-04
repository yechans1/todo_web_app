#!/bin/bash

# Azure 배포 스크립트
echo "🚀 TodoWeb Azure 배포 시작"

# 변수 설정
RESOURCE_GROUP="TodoWeb-RG"
LOCATION="koreacentral"
APP_NAME="todoweb-app-$(date +%s)"
ACR_NAME="todowebacr$(date +%s)"
SQL_SERVER="todoweb-sql-$(date +%s)"
SQL_DATABASE="TodoWebDb"
SQL_ADMIN="todowebadmin"
SQL_PASSWORD="TodoWeb123!"

echo "📋 배포 정보:"
echo "  리소스 그룹: $RESOURCE_GROUP"
echo "  위치: $LOCATION"
echo "  앱 이름: $APP_NAME"
echo "  컨테이너 레지스트리: $ACR_NAME"
echo "  SQL 서버: $SQL_SERVER"

# 1. 리소스 그룹 생성
echo "1️⃣ 리소스 그룹 생성 중..."
az group create --name $RESOURCE_GROUP --location $LOCATION

# 2. Azure Container Registry 생성
echo "2️⃣ Container Registry 생성 중..."
az acr create --resource-group $RESOURCE_GROUP --name $ACR_NAME --sku Basic --admin-enabled true

# 3. SQL Server 및 데이터베이스 생성
echo "3️⃣ SQL Server 생성 중..."
az sql server create \
  --name $SQL_SERVER \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --admin-user $SQL_ADMIN \
  --admin-password $SQL_PASSWORD

echo "4️⃣ SQL Database 생성 중..."
az sql db create \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER \
  --name $SQL_DATABASE \
  --service-objective Basic

# 5. SQL Server 방화벽 규칙 (Azure 서비스 허용)
echo "5️⃣ SQL Server 방화벽 설정 중..."
az sql server firewall-rule create \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0

# 6. Docker 이미지 빌드 및 푸시
echo "6️⃣ Docker 이미지 빌드 중..."
az acr build --registry $ACR_NAME --image todoweb:latest .

# 7. Container App Environment 생성
echo "7️⃣ Container App Environment 생성 중..."
az containerapp env create \
  --name todowebenv \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION

# 8. 연결 문자열 생성
CONNECTION_STRING="Server=tcp:${SQL_SERVER}.database.windows.net,1433;Initial Catalog=${SQL_DATABASE};Persist Security Info=False;User ID=${SQL_ADMIN};Password=${SQL_PASSWORD};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

# 9. Container App 배포
echo "8️⃣ Container App 배포 중..."
az containerapp create \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --environment todowebenv \
  --image ${ACR_NAME}.azurecr.io/todoweb:latest \
  --registry-server ${ACR_NAME}.azurecr.io \
  --target-port 8080 \
  --ingress external \
  --env-vars "ConnectionStrings__DefaultConnection=${CONNECTION_STRING}" \
  --cpu 0.5 \
  --memory 1Gi

# 10. 앱 URL 가져오기
echo "9️⃣ 배포 완료!"
APP_URL=$(az containerapp show --name $APP_NAME --resource-group $RESOURCE_GROUP --query properties.configuration.ingress.fqdn -o tsv)

echo "✅ 배포 성공!"
echo "🌐 앱 URL: https://$APP_URL"
echo "📚 Swagger: https://$APP_URL/swagger"
echo "🔐 로그인: admin / admin"
echo ""
echo "📋 리소스 정보:"
echo "  리소스 그룹: $RESOURCE_GROUP"
echo "  앱 이름: $APP_NAME"
echo "  SQL 서버: $SQL_SERVER.database.windows.net"
echo "  데이터베이스: $SQL_DATABASE"