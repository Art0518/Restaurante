# Script simple para forzar UTF-8 sin reemplazos
$ErrorActionPreference = "Stop"

Write-Host "?? Convirtiendo archivos Vue a UTF-8 sin BOM..." -ForegroundColor Cyan

$archivos = @(
    "CafeSanJuanVue\pages\components\Footer.vue",
    "CafeSanJuanVue\pages\components\Navbar.vue",
"CafeSanJuanVue\pages\components\AuthModal.vue",
    "CafeSanJuanVue\pages\index\+Page.vue",
    "CafeSanJuanVue\pages\menu\+Page.vue",
    "CafeSanJuanVue\pages\mesas\+Page.vue",
    "CafeSanJuanVue\pages\reservas\+Page.vue"
)

$Utf8NoBomEncoding = New-Object System.Text.UTF8Encoding $False

foreach ($archivo in $archivos) {
    if (Test-Path $archivo) {
        Write-Host "?? $archivo" -ForegroundColor Yellow
   $content = [System.IO.File]::ReadAllText((Resolve-Path $archivo))
        [System.IO.File]::WriteAllText((Resolve-Path $archivo), $content, $Utf8NoBomEncoding)
        Write-Host "   ? Convertido" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "? Conversión completada" -ForegroundColor Green
Write-Host "?? Recarga el navegador con Ctrl+Shift+R" -ForegroundColor Yellow
