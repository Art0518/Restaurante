# Script para corregir la codificación UTF-8 en las páginas admin de Vue
# Corrige caracteres mal codificados (? por ó, ñ, í, etc.)

Write-Host "=== Iniciando corrección de codificación UTF-8 en páginas admin ===" -ForegroundColor Cyan

# Definir las páginas admin a corregir
$paginasAdmin = @(
  "CafeSanJuanVue\pages\admin-promociones\+Page.vue",
    "CafeSanJuanVue\pages\admin-mesas\+Page.vue",
    "CafeSanJuanVue\pages\admin-menu\+Page.vue",
    "CafeSanJuanVue\pages\admin-clientes\+Page.vue",
    "CafeSanJuanVue\pages\admin-facturacion\+Page.vue"
)

# Mapa de reemplazos de caracteres mal codificados
$reemplazos = @{
    # Vocales con tilde
    'Gestión' = 'Gestión'
    'Administración' = 'Administración'
    'Edición' = 'Edición'
    'descripción' = 'descripción'
    'categoría' = 'categoría'
    'información' = 'información'
    'teléfono' = 'teléfono'
    'dirección' = 'dirección'
    'número' = 'número'
    'página' = 'página'
    'búsqueda' = 'búsqueda'
    'único' = 'único'
    'están' = 'están'
    'más' = 'más'
    'será' = 'será'
    'está' = 'está'
 'código' = 'código'
    'métodos' = 'métodos'
    'también' = 'también'
    'después' = 'después'
    'según' = 'según'
    'válido' = 'válido'
    'específico' = 'específico'
    'básica' = 'básica'
    
    # Palabras con ñ
    'eño' = 'eño'
    'año' = 'año'
    'ño' = 'ño'
    
    # Signos y símbolos
    '??' = '??'
  '??' = '??'
    '??' = '??'
    '??' = '??'
    '??' = '??'
    '?' = '?'
    '?' = '?'
    
    # Casos específicos encontrados en los archivos
    'Gestión' = 'Gestión'
    'Promoción' = 'Promoción'
    'promoción' = 'promoción'
  'creación' = 'creación'
    'eliminación' = 'eliminación'
    'selección' = 'selección'
    'descripción' = 'descripción'
'Descripción' = 'Descripción'
    'Edición' = 'Edición'
    'edición' = 'edición'
    'Número' = 'Número'
    'número' = 'número'
    'único' = 'único'
    'Teléfono' = 'Teléfono'
    'teléfono' = 'teléfono'
    'Dirección' = 'Dirección'
 'dirección' = 'dirección'
  'están' = 'están'
    'más' = 'más'
    'válido' = 'válido'
    'básica' = 'básica'
    'específico' = 'específico'
}

foreach ($archivo in $paginasAdmin) {
    Write-Host "`nProcesando: $archivo" -ForegroundColor Yellow
    
    if (-not (Test-Path $archivo)) {
        Write-Host "  ? El archivo no existe: $archivo" -ForegroundColor Red
        continue
    }
    
    try {
        # Leer el contenido original con codificación UTF-8
        $contenido = Get-Content -Path $archivo -Raw -Encoding UTF8
        
        # Contador de reemplazos
        $totalReemplazos = 0
 
        # Aplicar cada reemplazo
      foreach ($clave in $reemplazos.Keys) {
            if ($contenido -match [regex]::Escape($clave)) {
     $contenido = $contenido -replace [regex]::Escape($clave), $reemplazos[$clave]
      $totalReemplazos++
            }
 }
        
        # Guardar el archivo con codificación UTF-8 SIN BOM
        $utf8NoBom = New-Object System.Text.UTF8Encoding $false
        [System.IO.File]::WriteAllText($archivo, $contenido, $utf8NoBom)
        
 Write-Host "  ? Archivo corregido. Reemplazos aplicados: $totalReemplazos" -ForegroundColor Green
    }
    catch {
        Write-Host "  ? Error procesando el archivo: $_" -ForegroundColor Red
    }
}

Write-Host "`n=== Corrección de codificación completada ===" -ForegroundColor Cyan
Write-Host "Por favor, verifica los archivos en tu editor y recarga la aplicación." -ForegroundColor White
