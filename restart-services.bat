@echo off
cls
echo ===============================================
echo   LIMPIANDO PUERTOS Y REINICIANDO SERVICIOS
echo ===============================================
echo.

echo Paso 1: Deteniendo procesos en puertos 5001-5004...
echo.

REM Detener procesos en puerto 5001
for /f "tokens=5" %%a in ('netstat -aon ^| findstr :5001 ^| findstr LISTENING') do (
    taskkill /F /PID %%a 2>nul
)

REM Detener procesos en puerto 5002
for /f "tokens=5" %%a in ('netstat -aon ^| findstr :5002 ^| findstr LISTENING') do (
    taskkill /F /PID %%a 2>nul
)

REM Detener procesos en puerto 5003
for /f "tokens=5" %%a in ('netstat -aon ^| findstr :5003 ^| findstr LISTENING') do (
    taskkill /F /PID %%a 2>nul
)

REM Detener procesos en puerto 5004
for /f "tokens=5" %%a in ('netstat -aon ^| findstr :5004 ^| findstr LISTENING') do (
    taskkill /F /PID %%a 2>nul
)

echo.
echo Puertos liberados. Esperando 3 segundos...
timeout /t 3 /nobreak >nul

echo.
echo ===============================================
echo Paso 2: Iniciando servicios...
echo ===============================================
echo.

echo Iniciando SeguridadService en puerto 5001...
start "SeguridadService-5001" /D "%~dp0SeguridadService" cmd /k "echo === SEGURIDAD SERVICE === && echo Puerto: 5001 && echo. && dotnet run"
timeout /t 3 /nobreak >nul

echo Iniciando MenuService en puerto 5002...
start "MenuService-5002" /D "%~dp0MenuService" cmd /k "echo === MENU SERVICE === && echo Puerto: 5002 && echo. && dotnet run"
timeout /t 3 /nobreak >nul

echo Iniciando ReservasService en puerto 5003...
start "ReservasService-5003" /D "%~dp0ReservasService" cmd /k "echo === RESERVAS SERVICE === && echo Puerto: 5003 && echo. && dotnet run"
timeout /t 3 /nobreak >nul

echo Iniciando FacturacionService en puerto 5004...
start "FacturacionService-5004" /D "%~dp0FacturacionService" cmd /k "echo === FACTURACION SERVICE === && echo Puerto: 5004 && echo. && dotnet run"
timeout /t 3 /nobreak >nul

echo.
echo ===============================================
echo   SERVICIOS INICIADOS CORRECTAMENTE
echo ===============================================
echo.
echo Se han abierto 4 ventanas de CMD con los servicios:
echo   - SeguridadService    http://localhost:5001
echo   - MenuService         http://localhost:5002
echo - ReservasService     http://localhost:5003
echo   - FacturacionService  http://localhost:5004
echo.
echo Espera 15-20 segundos a que todos los servicios terminen de cargar
echo.
echo Luego puedes usar el archivo test-apis.http para probar las APIs
echo.
pause
