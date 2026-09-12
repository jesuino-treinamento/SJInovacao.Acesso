@echo off
setlocal enabledelayedexpansion

REM ===================================================
REM CONFIGURACAO - ajuste aqui conforme seu ambiente
REM ===================================================
set "URL_API=http://localhost:8080"
set "EMAIL_LOGIN=TI@gmail.com"
set "PASSWORD_LOGIN=EquipeTi@1"

REM ===================================================
REM 1️⃣ Apagar e recriar o Banco de Dados (opcional)
REM ===================================================
echo.
echo =========================================
echo 1 - Apaga Banco de Dados
echo =========================================
dotnet ef database drop --force --verbose
echo =========================================
echo 2 - Remove Migrations
echo =========================================
dotnet ef migrations remove --force
echo =========================================
echo 4 - Criar Migrations
echo =========================================
dotnet ef migrations add InitialCreate --output-dir "Migrations" --verbose
echo =========================================
echo 5 - Criar banco de dados
echo =========================================
dotnet ef database update --verbose
echo =========================================
echo TODAS AS EPATAS CONCLUIDAS COM SUCESSO!
echo =========================================
echo.

REM REM ===================================================
REM REM 2️⃣ Autenticar e capturar o Token JWT
REM REM ===================================================
REM echo Realizando login para capturar o Token JWT...

REM REM curl -X POST "%URL_API%/api/Auth" ^
 REM REM -H "Content-Type: application/json" ^
 REM REM -d "{\"email\": \"%EMAIL_LOGIN%\", \"password\": \"%PASSWORD_LOGIN%\"}" -o login-response.json

REM REM echo Resultado do login:
REM REM type login-response.json
REM REM echo.

REM REM set "TOKEN="
REM REM for /f "usebackq delims=" %%i in (`powershell -NoProfile -Command ^
    REM REM "try { $json = Get-Content -Raw 'login-response.json' | ConvertFrom-Json; if ($json.data.token) { $json.data.token } elseif ($json.data.data.token) { $json.data.data.token } else { '' } } catch { '' }"`) do (
    REM REM set "TOKEN=%%i"
REM REM )

REM REM if "%TOKEN%"=="" (
    REM REM echo [ERRO] Nao foi possivel capturar o token. Verifique suas credenciais.
    REM REM pause
    REM REM exit /b 1
REM REM )

REM REM echo Token JWT capturado com sucesso!
REM REM echo Token: %TOKEN%
REM REM echo.

REM ===================================================
REM 3️⃣ Inserir o Usuario Admin (sem token)
REM ===================================================
@echo off
setlocal

set "URL_API=http://localhost:8080"
echo =========================================
echo Inserindo usuario Admin...
echo =========================================
curl -X POST "%URL_API%/api/users" -H "Content-Type: application/json" -d "{\"email\":\"TI@gmail.com\",\"username\":\"Jesuino\",\"password\":\"J@suin00\",\"name\":{\"firstName\":\"Marcelo\",\"lastName\":\"Jesuino\"},\"document\":{\"number\":\"06275451068\",\"personType\":1},\"addresses\":[{\"city\":\"Nova Iguaçu\",\"street\":\"Rua 1\",\"number\":\"10\",\"neighborhood\":\"Prados Verdes\",\"state\":\"Rio de Janeiro\",\"zipCode\":\"26299-051\",\"geolocation\":{\"lat\":\"1\",\"long\":\"1\"}}],\"phones\":[{\"number\":\"+5521261110454\",\"type\":1},{\"number\":\"+5521991110455\",\"type\":2}],\"status\":1,\"role\":3}"

echo.
eecho =========================================
echo  Operacao concluida sem usar token!
echo =========================================
pause
