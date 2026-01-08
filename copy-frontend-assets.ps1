# ========================================
# SCRIPT PARA COPIAR ARCHIVOS JAVASCRIPT
# ========================================

Write-Host "?? Copiando archivos JavaScript del frontend HTML a Vue..." -ForegroundColor Cyan

# Crear directorios si no existen
$directories = @(
    "CafeSanJuanVue\public\js",
    "CafeSanJuanVue\public\img"
)

foreach ($dir in $directories) {
    if (-Not (Test-Path $dir)) {
  New-Item -ItemType Directory -Path $dir -Force
        Write-Host "? Creado directorio: $dir" -ForegroundColor Green
    }
}

# Copiar archivos JavaScript
$jsFiles = @(
    "auth.js",
"carrito.js",
    "global-init.js",
  "menu-clientes.js",
    "mis-reservas.js",
  "navbar.js",
    "notifications.js",
    "perfil.js",
    "reservas.js",
    "admin-clientes.js",
    "admin-gestion-reservas.js",
    "admin-menu.js",
"admin-mesas.js",
    "admin-promociones.js",
    "admin-reservas.js",
  "factura-download-mejorada.js"
)

Write-Host "`n?? Copiando archivos JavaScript..." -ForegroundColor Yellow

foreach ($file in $jsFiles) {
    $source = "Ws_Restaurante\front\js\$file"
    $dest = "CafeSanJuanVue\public\js\$file"
    
    if (Test-Path $source) {
        Copy-Item -Path $source -Destination $dest -Force
        Write-Host "  ? Copiado: $file" -ForegroundColor Green
    } else {
        Write-Host "  ??  No encontrado: $file" -ForegroundColor Yellow
    }
}

# Copiar imágenes
Write-Host "`n???  Copiando imágenes..." -ForegroundColor Yellow

$imgSource = "Ws_Restaurante\front\img"
$imgDest = "CafeSanJuanVue\public\img"

if (Test-Path $imgSource) {
    Copy-Item -Path "$imgSource\*" -Destination $imgDest -Recurse -Force
    Write-Host "  ? Imágenes copiadas" -ForegroundColor Green
} else {
    Write-Host "  ??  Carpeta de imágenes no encontrada" -ForegroundColor Yellow
}

Write-Host "`n? Proceso completado" -ForegroundColor Green
Write-Host "`n?? Siguiente paso: Ejecutar 'npm run dev' en CafeSanJuanVue" -ForegroundColor Cyan
