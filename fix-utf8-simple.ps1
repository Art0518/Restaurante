# Script simple para convertir archivos Vue a UTF-8
Write-Host "Convirtiendo archivos Vue a UTF-8..." -ForegroundColor Cyan

$files = @(
    "CafeSanJuanVue\index.html",
    "CafeSanJuanVue\pages\+Layout.vue",
    "CafeSanJuanVue\pages\components\Navbar.vue",
    "CafeSanJuanVue\pages\components\AuthModal.vue",
    "CafeSanJuanVue\pages\index\+Page.vue",
    "CafeSanJuanVue\pages\menu\+Page.vue",
    "CafeSanJuanVue\pages\reservas\+Page.vue",
    "CafeSanJuanVue\pages\confirmacion\+Page.vue"
)

$utf8NoBom = New-Object System.Text.UTF8Encoding($false)

foreach ($file in $files) {
  if (Test-Path $file) {
        $content = [System.IO.File]::ReadAllText($file, [System.Text.Encoding]::UTF8)
        [System.IO.File]::WriteAllText($file, $content, $utf8NoBom)
 Write-Host "OK: $file" -ForegroundColor Green
    }
}

Write-Host "`nCompletado. Ahora reinicia el servidor Vue." -ForegroundColor Green
