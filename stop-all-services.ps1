###############################################################################
# Script para detener todos los microservicios de CafeSanJuan
# Uso: .\stop-all-services.ps1
###############################################################################

Write-Host "===========================================================" -ForegroundColor Cyan
Write-Host "  Deteniendo Microservicios de CafeSanJuan" -ForegroundColor Cyan
Write-Host "===========================================================" -ForegroundColor Cyan
Write-Host ""

# Función para detener procesos de dotnet en puertos específicos
function Stop-ServiceOnPort {
    param([int]$Port)
    
    try {
        $process = Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue | Select-Object -ExpandProperty OwningProcess -First 1
        
     if ($process) {
  $processName = (Get-Process -Id $process -ErrorAction SilentlyContinue).ProcessName
       Write-Host "Deteniendo proceso en puerto $Port (PID: $process, Nombre: $processName)..." -ForegroundColor Yellow
        Stop-Process -Id $process -Force -ErrorAction SilentlyContinue
            Write-Host "? Proceso detenido en puerto $Port" -ForegroundColor Green
    }
        else {
  Write-Host "? No hay proceso en puerto $Port" -ForegroundColor Gray
        }
    }
    catch {
        Write-Host "? No se pudo detener proceso en puerto $Port" -ForegroundColor Gray
    }
}

# Detener servicios por puerto
Write-Host "Deteniendo servicios..." -ForegroundColor Cyan
Write-Host ""

Stop-ServiceOnPort -Port 5001  # SeguridadService
Stop-ServiceOnPort -Port 5002  # MenuService
Stop-ServiceOnPort -Port 5003  # ReservasService
Stop-ServiceOnPort -Port 5004  # FacturacionService

Write-Host ""

# También detener todos los procesos de dotnet relacionados con los servicios
Write-Host "Deteniendo procesos adicionales de dotnet..." -ForegroundColor Cyan

$dotnetProcesses = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | 
    Where-Object { $_.MainWindowTitle -match "SeguridadService|MenuService|ReservasService|FacturacionService" }

if ($dotnetProcesses) {
    foreach ($proc in $dotnetProcesses) {
        Write-Host "Deteniendo: $($proc.MainWindowTitle) (PID: $($proc.Id))" -ForegroundColor Yellow
        Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue
  }
    Write-Host "? Procesos adicionales detenidos" -ForegroundColor Green
}
else {
    Write-Host "? No hay procesos adicionales para detener" -ForegroundColor Gray
}

Write-Host ""
Write-Host "===========================================================" -ForegroundColor Green
Write-Host "  Todos los servicios han sido detenidos" -ForegroundColor Green
Write-Host "===========================================================" -ForegroundColor Green
Write-Host ""

Read-Host "Presiona Enter para cerrar"
