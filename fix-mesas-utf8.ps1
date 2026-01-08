# Script para forzar codificación UTF-8 en mesas +Page.vue
$ErrorActionPreference = "Stop"

Write-Host "?? Forzando codificación UTF-8 en mesas +Page.vue..." -ForegroundColor Cyan

$filePath = "CafeSanJuanVue\pages\mesas\+Page.vue"

# Leer el contenido actual
$content = Get-Content $filePath -Raw -Encoding UTF8

# Reemplazar caracteres problemáticos si existen
$content = $content -replace 'n?mero', 'número'
$content = $content -replace 'Buscar por n.*mero de mesa', 'Buscar por número de mesa...'

# Guardar con UTF-8 sin BOM
$Utf8NoBomEncoding = New-Object System.Text.UTF8Encoding $False
[System.IO.File]::WriteAllText((Resolve-Path $filePath), $content, $Utf8NoBomEncoding)

Write-Host "? Archivo corregido con UTF-8 sin BOM" -ForegroundColor Green
Write-Host ""
Write-Host "Por favor recarga el navegador (Ctrl+F5) para ver los cambios." -ForegroundColor Yellow
