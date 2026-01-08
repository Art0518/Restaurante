###############################################################################
# Script para iniciar todos los microservicios de CafeSanJuan
# Uso: .\start-all-services.ps1
###############################################################################

Write-Host "===========================================================" -ForegroundColor Cyan
Write-Host "  Iniciando Microservicios de CafeSanJuan" -ForegroundColor Cyan
Write-Host "===========================================================" -ForegroundColor Cyan
Write-Host ""

# Función para iniciar un servicio en una nueva ventana de PowerShell
function Start-Service {
    param(
     [string]$ServiceName,
     [string]$ServicePath,
     [int]$Port
    )
    
    Write-Host "Iniciando $ServiceName en puerto $Port..." -ForegroundColor Yellow
    
    $scriptBlock = {
        param($path, $name, $port)
        $Host.UI.RawUI.WindowTitle = "$name - Puerto $port"
        Set-Location $path
        Write-Host "=== $name ===" -ForegroundColor Green
      Write-Host "Puerto: $port" -ForegroundColor Green
        Write-Host "Ruta: $path" -ForegroundColor Gray
 Write-Host ""
    dotnet run
    }
    
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "& {$scriptBlock} '$ServicePath' '$ServiceName' $Port"
  Start-Sleep -Seconds 1
}

# Verificar que estamos en el directorio correcto
if (-not (Test-Path "SeguridadService")) {
    Write-Host "ERROR: No se encuentra la carpeta SeguridadService" -ForegroundColor Red
    Write-Host "Asegúrate de ejecutar este script desde la raíz del proyecto CafeSanJuan" -ForegroundColor Red
    Write-Host ""
  Read-Host "Presiona Enter para salir"
    exit
}

# Iniciar cada servicio en su propia ventana
Start-Service -ServiceName "SeguridadService" -ServicePath "$PWD\SeguridadService" -Port 5001
Start-Service -ServiceName "MenuService" -ServicePath "$PWD\MenuService" -Port 5002
Start-Service -ServiceName "ReservasService" -ServicePath "$PWD\ReservasService" -Port 5003
Start-Service -ServiceName "FacturacionService" -ServicePath "$PWD\FacturacionService" -Port 5004

Write-Host ""
Write-Host "===========================================================" -ForegroundColor Green
Write-Host "  Todos los servicios se están iniciando..." -ForegroundColor Green
Write-Host "===========================================================" -ForegroundColor Green
Write-Host ""
Write-Host "Servicios disponibles:" -ForegroundColor Cyan
Write-Host "  - SeguridadService:    http://localhost:5001" -ForegroundColor White
Write-Host "  - MenuService:         http://localhost:5002" -ForegroundColor White
Write-Host "  - ReservasService:     http://localhost:5003" -ForegroundColor White
Write-Host "  - FacturacionService:  http://localhost:5004" -ForegroundColor White
Write-Host ""
Write-Host "Espera unos segundos a que todos los servicios terminen de iniciar..." -ForegroundColor Yellow
Write-Host ""
Write-Host "Puedes probar los servicios usando el archivo 'test-apis.http'" -ForegroundColor Green
Write-Host ""
Write-Host "Para detener los servicios, cierra las ventanas de PowerShell o presiona Ctrl+C en cada una" -ForegroundColor Gray
Write-Host ""

# Esperar 5 segundos para que los servicios inicien
Write-Host "Esperando 5 segundos para que los servicios inicien completamente..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# Verificar que los servicios estén corriendo
Write-Host ""
Write-Host "Verificando servicios..." -ForegroundColor Cyan

$services = @(
    @{Name="SeguridadService"; Url="http://localhost:5001"},
    @{Name="MenuService"; Url="http://localhost:5002"},
    @{Name="ReservasService"; Url="http://localhost:5003"},
    @{Name="FacturacionService"; Url="http://localhost:5004"}
)

foreach ($service in $services) {
    try {
        $response = Invoke-WebRequest -Uri $service.Url -Method GET -TimeoutSec 2 -ErrorAction Stop
        Write-Host "? $($service.Name) - OK (Status: $($response.StatusCode))" -ForegroundColor Green
    }
    catch {
   Write-Host "? $($service.Name) - No responde (aún iniciando...)" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "Script completado. Las ventanas de los servicios permanecerán abiertas." -ForegroundColor Green
Write-Host ""
Read-Host "Presiona Enter para cerrar esta ventana"
