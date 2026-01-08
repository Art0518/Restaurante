# Script para corregir la codificación del componente de carrito
Write-Host "?? Corrigiendo codificación del carrito..." -ForegroundColor Cyan

$file = "C:\Users\luisa\Documents\CafeSanJuan\CafeSanJuanVue\pages\carrito\+Page.vue"

if (Test-Path $file) {
    Write-Host "?? Procesando: $file"
    
    # Leer el contenido del archivo
    $content = Get-Content -Path $file -Raw -Encoding UTF8
    
  # Reemplazar caracteres mal codificados
    $content = $content -replace '?', 'ó'
    $content = $content -replace '?', 'é'
    $content = $content -replace '?', 'á'
    $content = $content -replace '?', 'í'
  $content = $content -replace '?', 'ú'
    $content = $content -replace '?', 'ñ'
    $content = $content -replace '?', 'ü'
    $content = $content -replace '?', '¿'
    $content = $content -replace '?', '¡'
    
    # Guardar el archivo con codificación UTF-8 sin BOM
    $utf8NoBom = New-Object System.Text.UTF8Encoding $false
    [System.IO.File]::WriteAllText($file, $content, $utf8NoBom)
    
    Write-Host "  ? Corregido" -ForegroundColor Green
} else {
    Write-Host "  ? Archivo no encontrado" -ForegroundColor Red
}

Write-Host "`n? Corrección completada!`n" -ForegroundColor Green
