# Script completo para arreglar problemas de encoding UTF-8
Write-Host "?? Arreglando encoding UTF-8 - Proceso Completo" -ForegroundColor Cyan
Write-Host "=================================================" -ForegroundColor Cyan

# 1. Detener todos los procesos de Node
Write-Host "`n[1/6] Deteniendo procesos de Node..." -ForegroundColor Yellow
Stop-Process -Name "node" -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2
Write-Host "? Procesos detenidos" -ForegroundColor Green

# 2. Navegar al directorio Vue
Set-Location "CafeSanJuanVue"
Write-Host "`n[2/6] Limpiando caches y builds anteriores..." -ForegroundColor Yellow

# Eliminar directorios de cache
$foldersToDelete = @("dist", ".vite", "node_modules\.vite")
foreach ($folder in $foldersToDelete) {
    if (Test-Path $folder) {
        Remove-Item -Path $folder -Recurse -Force -ErrorAction SilentlyContinue
        Write-Host "  ? Eliminado: $folder" -ForegroundColor Gray
    }
}

# 3. Configurar encoding UTF-8
Write-Host "`n[3/6] Configurando encoding UTF-8 del sistema..." -ForegroundColor Yellow
$OutputEncoding = [System.Text.Encoding]::UTF8
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
[Console]::InputEncoding = [System.Text.Encoding]::UTF8
chcp 65001 > $null
Write-Host "? Encoding configurado" -ForegroundColor Green

# 4. Verificar archivos críticos
Write-Host "`n[4/6] Verificando archivos críticos..." -ForegroundColor Yellow
$filesToCheck = @(
    "index.html",
    "pages/+Layout.vue",
    "pages/components/Navbar.vue",
    "pages/index/+Page.vue"
)

foreach ($file in $filesToCheck) {
    if (Test-Path $file) {
      $content = Get-Content $file -Raw -Encoding UTF8
      if ($content -match "Rincón|Menú|caribeña") {
            Write-Host "  ? $file tiene UTF-8 correcto" -ForegroundColor Gray
        } else {
   Write-Host "  ? $file podría tener problemas" -ForegroundColor Yellow
        }
    }
}

# 5. Crear/Actualizar archivo de configuración Vite
Write-Host "`n[5/6] Verificando configuración de Vite..." -ForegroundColor Yellow

# Leer vite.config.js actual
$viteConfig = Get-Content "vite.config.js" -Raw -Encoding UTF8

# Verificar si ya tiene la configuración de charset
if ($viteConfig -notmatch "charset.*utf-8") {
    Write-Host "  ?? Agregando configuración de charset a Vite..." -ForegroundColor Cyan
    
    # Backup del archivo original
    Copy-Item "vite.config.js" "vite.config.js.backup" -Force
    
    # Modificar vite.config.js para forzar UTF-8
    $newConfig = $viteConfig -replace "(export default defineConfig\(\{)", 
@"
`$1
  server: {
    headers: {
    'Content-Type': 'text/html; charset=utf-8'
    }
  },
  build: {
    charset: 'utf8'
  },
"@
    
    Set-Content -Path "vite.config.js" -Value $newConfig -Encoding UTF8 -Force
    Write-Host "  ? Configuración de Vite actualizada" -ForegroundColor Gray
} else {
    Write-Host "  ? Configuración de charset ya existe" -ForegroundColor Gray
}

# 6. Iniciar servidor de desarrollo
Write-Host "`n[6/6] Iniciando servidor de desarrollo..." -ForegroundColor Yellow
Write-Host "=================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "?? El servidor se iniciará en modo UTF-8 estricto" -ForegroundColor Green
Write-Host "?? Abre tu navegador en: http://localhost:3000" -ForegroundColor Cyan
Write-Host "?? Presiona Ctrl+C para detener el servidor" -ForegroundColor Yellow
Write-Host ""

# Ejecutar con encoding UTF-8 forzado
$env:NODE_OPTIONS = "--max-old-space-size=4096"
$env:VITE_FORCE_UTF8 = "true"

npm run dev
