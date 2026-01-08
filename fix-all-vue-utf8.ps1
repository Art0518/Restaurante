# Script maestro para corregir UTF-8 en todos los archivos Vue
$ErrorActionPreference = "Stop"

Write-Host "?? CORRECCIÓN MASIVA UTF-8 - TODOS LOS ARCHIVOS VUE" -ForegroundColor Cyan
Write-Host "=================================================" -ForegroundColor Cyan
Write-Host ""

$archivos = @(
    "CafeSanJuanVue\pages\components\Footer.vue",
    "CafeSanJuanVue\pages\components\Navbar.vue",
 "CafeSanJuanVue\pages\components\AuthModal.vue",
    "CafeSanJuanVue\pages\index\+Page.vue",
    "CafeSanJuanVue\pages\menu\+Page.vue",
    "CafeSanJuanVue\pages\mesas\+Page.vue",
    "CafeSanJuanVue\pages\reservas\+Page.vue",
    "CafeSanJuanVue\pages\confirmacion\+Page.vue",
    "CafeSanJuanVue\pages\+Layout.vue"
)

$totalCorregidos = 0

foreach ($archivo in $archivos) {
    if (Test-Path $archivo) {
        Write-Host "?? Procesando: $archivo" -ForegroundColor Yellow
        
    try {
       # Leer el contenido
    $content = Get-Content $archivo -Raw -Encoding UTF8
   
         # Reemplazar caracteres problemáticos comunes
         $content = $content -replace 'Rinc?n', 'Rincón'
        $content = $content -replace 'tradici?n', 'tradición'
            $content = $content -replace 'puertorrique?a', 'puertorriqueña'
            $content = $content -replace 'caribe?a', 'caribeña'
     $content = $content -replace '?nica', 'única'
            $content = $content -replace 'n?mero', 'número'
         $content = $content -replace 'NunúmeroMesa', 'NumeroMesa'
 $content = $content -replace 'nunúmero', 'numero'
            $content = $content -replace 'f-nunúmero', 'f-numero'
            $content = $content -replace 'sesi?n', 'sesión'
      $content = $content -replace 'Autenticaci?n', 'Autenticación'
     $content = $content -replace 'est?', 'está'
            $content = $content -replace 'Men?', 'Menú'
        $content = $content -replace 'Gesti?n', 'Gestión'
        $content = $content -replace 'contrase?a', 'contraseña'
          $content = $content -replace 'c?dula', 'cédula'
     $content = $content -replace 'tel?fono', 'teléfono'
      $content = $content -replace 'direcci?n', 'dirección'
$content = $content -replace 'categor?a', 'categoría'
   $content = $content -replace 'descripci?n', 'descripción'
        
            # Guardar con UTF-8 sin BOM
         $Utf8NoBomEncoding = New-Object System.Text.UTF8Encoding $False
       [System.IO.File]::WriteAllText((Resolve-Path $archivo), $content, $Utf8NoBomEncoding)
        
            Write-Host "   ? Corregido" -ForegroundColor Green
            $totalCorregidos++
    }
        catch {
 Write-Host "   ? Error: $_" -ForegroundColor Red
 }
    }
    else {
        Write-Host "   ??  No encontrado" -ForegroundColor DarkYellow
    }
    Write-Host ""
}

Write-Host "=================================================" -ForegroundColor Cyan
Write-Host "? Total archivos corregidos: $totalCorregidos" -ForegroundColor Green
Write-Host ""
Write-Host "?? IMPORTANTE: Recarga el navegador con Ctrl+F5" -ForegroundColor Yellow
Write-Host "   Si el problema persiste, reinicia el servidor de desarrollo" -ForegroundColor Yellow
