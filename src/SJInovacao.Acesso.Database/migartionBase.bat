@echo off
setlocal EnableExtensions EnableDelayedExpansion
chcp 65001 >nul

REM ===================================================
REM CONFIGURACAO DE CORES (ANSI)
REM ===================================================
for /f %%A in ('echo prompt $E ^| cmd') do set "ESC=%%A"

set "GREEN=%ESC%[92m"
set "RED=%ESC%[91m"
set "YELLOW=%ESC%[93m"
set "CYAN=%ESC%[96m"
set "WHITE=%ESC%[97m"
set "RESET=%ESC%[0m"

REM ===================================================
REM CABECALHO
REM ===================================================
cls
echo.
echo %CYAN%=========================================%RESET%
echo %WHITE%S J I N O V A C A O - A C E S S O%RESET%
echo %CYAN%=========================================%RESET%
echo.
echo %WHITE%Gerenciamento de Banco de Dados%RESET%
echo.

:MENU
echo %CYAN%Selecione uma opcao:%RESET%
echo.
echo %WHITE%[1]%RESET% Criar nova Migration
echo %WHITE%[2]%RESET% Atualizar Banco de Dados
echo %WHITE%[3]%RESET% Remover ultima Migration
echo %WHITE%[4]%RESET% Apagar Banco de Dados
echo %WHITE%[5]%RESET% Mostrar status das Migrations
echo %WHITE%[6]%RESET% Criar Migration e Atualizar Banco (Completo)
echo %WHITE%[7]%RESET% Sair
echo.
set /p "OPCAO=Digite sua opcao: "

if "%OPCAO%"=="1" goto CREATE_MIGRATION
if "%OPCAO%"=="2" goto UPDATE_DATABASE
if "%OPCAO%"=="3" goto REMOVE_MIGRATION
if "%OPCAO%"=="4" goto DROP_DATABASE
if "%OPCAO%"=="5" goto MIGRATION_STATUS
if "%OPCAO%"=="6" goto FULL_RESET
if "%OPCAO%"=="7" goto EXIT

echo.
echo %YELLOW%[AVISO] Opcao invalida!%RESET%
pause
cls
goto MENU

:CREATE_MIGRATION
cls
echo.
echo %CYAN%=========================================%RESET%
echo %WHITE%Criando nova Migration...%RESET%
echo %CYAN%=========================================%RESET%
echo.
set /p "MIGRATION_NAME=Nome da Migration (ex: AddUserTable): "

if "%MIGRATION_NAME%"=="" (
  echo.
  echo %RED%[ERRO] Nome da migration nao pode ser vazio!%RESET%
  pause
  cls
  goto MENU
)

dotnet ef migrations add "%MIGRATION_NAME%" --output-dir "Migrations" --verbose

if errorlevel 1 (
  echo.
  echo %RED%[ERRO] Falha ao criar migration!%RESET%
) else (
  echo.
  echo %GREEN%[SUCESSO] Migration criada com sucesso!%RESET%
)

pause
cls
goto MENU

:UPDATE_DATABASE
cls
echo.
echo %CYAN%=========================================%RESET%
echo %WHITE%Atualizando Banco de Dados...%RESET%
echo %CYAN%=========================================%RESET%
echo.

dotnet ef database update --verbose

if errorlevel 1 (
  echo.
  echo %RED%[ERRO] Falha ao atualizar banco de dados!%RESET%
) else (
  echo.
  echo %GREEN%[SUCESSO] Banco de dados atualizado com sucesso!%RESET%
)

pause
cls
goto MENU

:REMOVE_MIGRATION
cls
echo.
echo %CYAN%=========================================%RESET%
echo %WHITE%Removendo ultima Migration...%RESET%
echo %CYAN%=========================================%RESET%
echo.

dotnet ef migrations remove --force --verbose

if errorlevel 1 (
  echo.
  echo %RED%[ERRO] Falha ao remover migration!%RESET%
) else (
  echo.
  echo %GREEN%[SUCESSO] Migration removida com sucesso!%RESET%
)

pause
cls
goto MENU

:DROP_DATABASE
cls
echo.
echo %CYAN%=========================================%RESET%
echo %WHITE%Apagando Banco de Dados...%RESET%
echo %CYAN%=========================================%RESET%
echo.
echo %YELLOW%[ATENCAO] Isso apagara TODOS os dados!%RESET%
set /p "CONFIRM=Tem certeza? (S/N): "

if /i not "%CONFIRM%"=="S" (
  echo.
  echo %YELLOW%[AVISO] Operacao cancelada.%RESET%
  pause
  cls
  goto MENU
)

dotnet ef database drop --force --verbose

if errorlevel 1 (
  echo.
  echo %RED%[ERRO] Falha ao apagar banco de dados!%RESET%
) else (
  echo.
  echo %GREEN%[SUCESSO] Banco de dados apagado com sucesso!%RESET%
)

pause
cls
goto MENU

:MIGRATION_STATUS
cls
echo.
echo %CYAN%=========================================%RESET%
echo %WHITE%Status das Migrations...%RESET%
echo %CYAN%=========================================%RESET%
echo.

dotnet ef migrations list

echo.
echo %WHITE%Pressione qualquer tecla para voltar ao menu...%RESET%
pause >nul
cls
goto MENU

:FULL_RESET
cls
echo.
echo %CYAN%=========================================%RESET%
echo %WHITE%RESET COMPLETO DO BANCO DE DADOS%RESET%
echo %CYAN%=========================================%RESET%
echo.
echo %WHITE%Isso ira:%RESET%
echo %WHITE%1.%RESET% Apagar o banco atual
echo %WHITE%2.%RESET% Remover a ultima migration (repetir ate nao haver mais)
echo %WHITE%3.%RESET% Criar nova migration
echo %WHITE%4.%RESET% Criar banco novamente
echo.
echo %YELLOW%[ATENCAO] Esta operacao pode exigir rodar mais de uma vez o "remove".%RESET%
set /p "CONFIRM=Confirmar reset completo? (S/N): "

if /i not "%CONFIRM%"=="S" (
  echo.
  echo %YELLOW%[AVISO] Operacao cancelada.%RESET%
  pause
  cls
  goto MENU
)

echo.
echo %CYAN%1/4 - Apagando banco de dados...%RESET%
dotnet ef database drop --force --verbose
if errorlevel 1 (
  echo %RED%[ERRO] Falha ao apagar banco de dados (talvez ele nao exista).%RESET%
) else (
  echo %GREEN%[SUCESSO] Banco removido (ou ja estava ausente).%RESET%
)

echo.
echo %CYAN%2/4 - Removendo migrations...%RESET%
REM Observacao: dotnet ef migrations remove remove uma por vez.
:REMOVE_LOOP
dotnet ef migrations remove --force --verbose
if errorlevel 1 (
  echo %YELLOW%[AVISO] Nao foi possivel remover mais migrations (provavelmente nao ha mais).%RESET%
) else (
  echo %GREEN%[SUCESSO] Uma migration foi removida. Tentando remover a proxima...%RESET%
  goto REMOVE_LOOP
)

echo.
echo %CYAN%3/4 - Criando nova migration...%RESET%
set "TIMESTAMP=%date:~6,4%%date:~3,2%%date:~0,2%_%time:~0,2%%time:~3,2%%time:~6,2%"
set "TIMESTAMP=%TIMESTAMP: =0%"

dotnet ef migrations add "InitialCreate_%TIMESTAMP%" --output-dir "Migrations" --verbose

if errorlevel 1 (
  echo.
  echo %RED%[ERRO] Falha ao criar migration!%RESET%
  pause
  cls
  goto MENU
) else (
  echo %GREEN%[SUCESSO] Migration criada: InitialCreate_%TIMESTAMP%%RESET%
)

echo.
echo %CYAN%4/4 - Criando banco de dados...%RESET%
dotnet ef database update --verbose

if errorlevel 1 (
  echo.
  echo %RED%[ERRO] Falha ao criar/atualizar banco de dados!%RESET%
) else (
  echo.
  echo %CYAN%=========================================%RESET%
  echo %GREEN%RESET CONCLUIDO COM SUCESSO!%RESET%
  echo %CYAN%=========================================%RESET%
)

pause
cls
goto MENU

:EXIT
echo.
echo %WHITE%Saindo...%RESET%
endlocal
exit /b 0