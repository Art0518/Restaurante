@echo off
cls
echo ===============================================
echo   INICIANDO MICROSERVICIOS CAFESANJUAN
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
echo   TODOS LOS SERVICIOS SE ESTAN INICIANDO
echo ===============================================
echo.
echo Se han abierto 4 ventanas de CMD:
echo   - SeguridadService  http://localhost:5001
echo   - MenuService         http://localhost:5002
echo   - ReservasService     http://localhost:5003
echo - FacturacionService  http://localhost:5004
echo.
echo Espera unos 15-20 segundos a que terminen de cargar
echo.
echo Para detener los servicios, cierra cada ventana de CMD
echo o presiona Ctrl+C en cada una
echo.
pause
