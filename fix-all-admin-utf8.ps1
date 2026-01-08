# Script para corregir TODOS los caracteres UTF-8 mal codificados en páginas admin
Write-Host "=== Corrección masiva UTF-8 en páginas admin ===" -ForegroundColor Cyan

$archivos = @(
    "CafeSanJuanVue\pages\admin-promociones\+Page.vue",
    "CafeSanJuanVue\pages\admin-mesas\+Page.vue",
  "CafeSanJuanVue\pages\admin-menu\+Page.vue",
    "CafeSanJuanVue\pages\admin-clientes\+Page.vue",
    "CafeSanJuanVue\pages\admin-facturacion\+Page.vue",
    "CafeSanJuanVue\pages\components\Navbar.vue"
)

foreach ($archivo in $archivos) {
    if (-not (Test-Path $archivo)) {
        Write-Host "??  Archivo no encontrado: $archivo" -ForegroundColor Yellow
        continue
    }
    
    Write-Host "`n?? Procesando: $archivo" -ForegroundColor Cyan
    
    # Leer archivo como bytes
    $bytes = [System.IO.File]::ReadAllBytes($archivo)
    
    # Convertir a string con codificación correcta
    $encoding = [System.Text.Encoding]::UTF8
    $contenido = $encoding.GetString($bytes)
    
    # Reemplazar todos los caracteres problemáticos
    $contenido = $contenido -replace 'Gestión', 'Gestión'
    $contenido = $contenido -replace 'Promoción', 'Promoción'
    $contenido = $contenido -replace 'promoción', 'promoción'
    $contenido = $contenido -replace 'Administración', 'Administración'
    $contenido = $contenido -replace 'administración', 'administración'
    $contenido = $contenido -replace 'Edición', 'Edición'
    $contenido = $contenido -replace 'edición', 'edición'
  $contenido = $contenido -replace 'Número', 'Número'
    $contenido = $contenido -replace 'número', 'número'
$contenido = $contenido -replace 'único', 'único'
    $contenido = $contenido -replace 'Descripción', 'Descripción'
    $contenido = $contenido -replace 'descripción', 'descripción'
    $contenido = $contenido -replace 'Teléfono', 'Teléfono'
 $contenido = $contenido -replace 'teléfono', 'teléfono'
    $contenido = $contenido -replace 'Dirección', 'Dirección'
    $contenido = $contenido -replace 'dirección', 'dirección'
    $contenido = $contenido -replace 'categoría', 'categoría'
    $contenido = $contenido -replace 'información', 'información'
    $contenido = $contenido -replace 'página', 'página'
    $contenido = $contenido -replace 'búsqueda', 'búsqueda'
    $contenido = $contenido -replace 'están', 'están'
    $contenido = $contenido -replace 'más', 'más'
    $contenido = $contenido -replace 'será', 'será'
    $contenido = $contenido -replace 'está', 'está'
    $contenido = $contenido -replace 'después', 'después'
    $contenido = $contenido -replace 'según', 'según'
    $contenido = $contenido -replace 'válido', 'válido'
    $contenido = $contenido -replace 'específico', 'específico'
    $contenido = $contenido -replace 'básica', 'básica'
    $contenido = $contenido -replace 'método', 'método'
    $contenido = $contenido -replace 'también', 'también'
    $contenido = $contenido -replace 'código', 'código'
    $contenido = $contenido -replace 'creación', 'creación'
    $contenido = $contenido -replace 'eliminación', 'eliminación'
    $contenido = $contenido -replace 'selección', 'selección'
    $contenido = $contenido -replace 'facturación', 'facturación'
    $contenido = $contenido -replace 'Facturación', 'Facturación'
    $contenido = $contenido -replace 'Generación', 'Generación'
    $contenido = $contenido -replace 'generación', 'generación'
    $contenido = $contenido -replace 'Inactivación', 'Inactivación'
  $contenido = $contenido -replace 'inactivación', 'inactivación'
    
    # Emojis y símbolos
    $contenido = $contenido -replace '??', '??'
    $contenido = $contenido -replace '??', '??'
    $contenido = $contenido -replace '??', '??'
    $contenido = $contenido -replace '??', '??'
    $contenido = $contenido -replace '??', '??'
    $contenido = $contenido -replace '?', '?'
    $contenido = $contenido -replace '?', '?'
    $contenido = $contenido -replace '??', '??'
    $contenido = $contenido -replace '??', '??'
    $contenido = $contenido -replace '??', '??'
    
    # Signos de interrogación y exclamación
    $contenido = $contenido -replace '¿Está', '¿Está'
    $contenido = $contenido -replace '¿Estás', '¿Estás'
    $contenido = $contenido -replace '¿Generar', '¿Generar'
    
    # Guardar con UTF-8 SIN BOM
  $utf8NoBom = New-Object System.Text.UTF8Encoding $false
    [System.IO.File]::WriteAllText($archivo, $contenido, $utf8NoBom)
    
    Write-Host "? Archivo corregido exitosamente" -ForegroundColor Green
}

Write-Host "`n=== Corrección completada ===" -ForegroundColor Green
Write-Host "Verifica los cambios y recarga tu aplicación Vue." -ForegroundColor White
