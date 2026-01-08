# Script para reconstruir Vue con encoding UTF-8 correcto
Write-Host "?? Reconstruyendo aplicación Vue con UTF-8..." -ForegroundColor Cyan

# Navegar al directorio de Vue
Set-Location "CafeSanJuanVue"

# Limpiar builds anteriores
Write-Host "?? Limpiando builds anteriores..." -ForegroundColor Yellow
if (Test-Path "dist") {
    Remove-Item -Path "dist" -Recurse -Force
    Write-Host "? Directorio dist eliminado" -ForegroundColor Green
}

if (Test-Path ".vite") {
    Remove-Item -Path ".vite" -Recurse -Force
    Write-Host "? Cache de Vite eliminado" -ForegroundColor Green
}

if (Test-Path "node_modules\.vite") {
    Remove-Item -Path "node_modules\.vite" -Recurse -Force
    Write-Host "? Cache de node_modules eliminado" -ForegroundColor Green
}

# Establecer encoding UTF-8 para el proceso actual
Write-Host "?? Configurando encoding UTF-8..." -ForegroundColor Yellow
$OutputEncoding = [System.Text.Encoding]::UTF8
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$env:CHCP = "65001"

# Ejecutar build
Write-Host "??? Ejecutando build de Vite..." -ForegroundColor Cyan
npm run build

if ($LASTEXITCODE -eq 0) {
    Write-Host "? Build completado exitosamente" -ForegroundColor Green
    
    # Verificar archivos generados
    if (Test-Path "dist/index.html") {
 Write-Host "? Archivos generados correctamente en dist/" -ForegroundColor Green
        
        # Mostrar primeras líneas del index.html para verificar encoding
        Write-Host "`n?? Verificando encoding del index.html generado:" -ForegroundColor Cyan
        Get-Content "dist/index.html" -First 10 -Encoding UTF8
    } else {
        Write-Host "?? Advertencia: No se encontró dist/index.html" -ForegroundColor Yellow
    }
} else {
    Write-Host "? Error en el build" -ForegroundColor Red
    exit 1
}

# Volver al directorio raíz
Set-Location ..

Write-Host "`n? Proceso completado!" -ForegroundColor Green
Write-Host "?? Ahora puedes:" -ForegroundColor Cyan
Write-Host "   1. Ejecutar: npm run preview (en CafeSanJuanVue)" -ForegroundColor White
Write-Host "   2. O copiar los archivos al servidor con: .\publish-vue-monster.ps1" -ForegroundColor White
