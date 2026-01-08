# Script para iniciar TODOS los microservicios de forma individual y controlada
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "  INICIANDO MICROSERVICIOS CAFESANJUAN" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host ""

# Función para iniciar un servicio
function Start-Microservice {
    param(
  [string]$ServiceName,
  [string]$Path,
      [int]$Port
    )
    
    Write-Host "Iniciando $ServiceName en puerto $Port..." -ForegroundColor Yellow
    
    $scriptBlock = {
        param($servicePath, $serviceName, $servicePort)
  $Host.UI.RawUI.WindowTitle = "$serviceName - Puerto $servicePort"
        Set-Location $servicePath
        Write-Host "=== $serviceName ===" -ForegroundColor Green
     Write-Host "Puerto: $servicePort" -ForegroundColor Green
        Write-Host "Ruta: $servicePath" -ForegroundColor Gray
  Write-Host ""
        Write-Host "Iniciando servicio..." -ForegroundColor Yellow
        dotnet run
    }
    
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "& {$scriptBlock} '$Path' '$ServiceName' $Port"
    Start-Sleep -Seconds 3
Write-Host "? Ventana abierta para $ServiceName" -ForegroundColor Green
    Write-Host ""
}

# Iniciar cada servicio
Write-Host "Iniciando servicios..." -ForegroundColor Cyan
Write-Host ""

Start-Microservice -ServiceName "SeguridadService" -Path "$PWD\SeguridadService" -Port 5001
Start-Microservice -ServiceName "MenuService" -Path "$PWD\MenuService" -Port 5002
Start-Microservice -ServiceName "ReservasService" -Path "$PWD\ReservasService" -Port 5003
Start-Microservice -ServiceName "FacturacionService" -Path "$PWD\FacturacionService" -Port 5004

Write-Host "===============================================" -ForegroundColor Green
Write-Host "  SERVICIOS INICIÁNDOSE" -ForegroundColor Green
Write-Host "===============================================" -ForegroundColor Green
Write-Host ""
Write-Host "Se han abierto 4 ventanas de PowerShell:" -ForegroundColor Cyan
Write-Host "  • SeguridadService    - http://localhost:5001" -ForegroundColor White
Write-Host "  • MenuService         - http://localhost:5002" -ForegroundColor White
Write-Host "  • ReservasService     - http://localhost:5003" -ForegroundColor White
Write-Host "  • FacturacionService  - http://localhost:5004" -ForegroundColor White
Write-Host ""
Write-Host "Espera 15-20 segundos a que terminen de cargar..." -ForegroundColor Yellow
Write-Host ""
Write-Host "Presiona cualquier tecla para verificar el estado..." -ForegroundColor Green
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

# Verificar servicios
Write-Host ""
Write-Host "=== VERIFICANDO SERVICIOS ===" -ForegroundColor Cyan
Write-Host ""

$servicios = @(
    @{Nombre="SeguridadService"; Puerto=5001},
    @{Nombre="MenuService"; Puerto=5002},
    @{Nombre="ReservasService"; Puerto=5003},
    @{Nombre="FacturacionService"; Puerto=5004}
)

foreach ($s in $servicios) {
    $conn = Get-NetTCPConnection -LocalPort $s.Puerto -State Listen -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($conn) {
        Write-Host "? $($s.Nombre) - ACTIVO en puerto $($s.Puerto)" -ForegroundColor Green
    } else {
        Write-Host "? $($s.Nombre) - NO DISPONIBLE en puerto $($s.Puerto)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "Listo! Ahora puedes usar test-apis.http" -ForegroundColor Cyan
Write-Host ""
