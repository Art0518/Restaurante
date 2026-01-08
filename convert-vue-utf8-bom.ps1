# Script para convertir TODOS los archivos .vue a UTF-8 con BOM
Write-Host "?? Convirtiendo archivos Vue a UTF-8 con BOM" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

$totalFixed = 0
$encoding = New-Object System.Text.UTF8Encoding($true) # UTF-8 con BOM

# Archivos a convertir
$files = @(
    "CafeSanJuanVue\index.html",
    "CafeSanJuanVue\pages\+Layout.vue",
  "CafeSanJuanVue\pages\components\Navbar.vue",
    "CafeSanJuanVue\pages\components\AuthModal.vue",
    "CafeSanJuanVue\pages\components\Footer.vue",
    "CafeSanJuanVue\pages\components\Loading.vue",
    "CafeSanJuanVue\pages\components\NotificationModal.vue",
    "CafeSanJuanVue\pages\components\ConfirmationModal.vue",
  "CafeSanJuanVue\pages\index\+Page.vue",
    "CafeSanJuanVue\pages\menu\+Page.vue",
    "CafeSanJuanVue\pages\reservas\+Page.vue",
    "CafeSanJuanVue\pages\confirmacion\+Page.vue",
    "CafeSanJuanVue\pages\mi-perfil\+Page.vue",
    "CafeSanJuanVue\pages\mis-reservas\+Page.vue",
    "CafeSanJuanVue\pages\carrito\+Page.vue",
    "CafeSanJuanVue\pages\mesas\+Page.vue"
)

foreach ($file in $files) {
    if (Test-Path $file) {
        try {
            # Leer contenido actual
      $content = Get-Content $file -Raw -Encoding UTF8
          
      # Verificar si tiene caracteres problemáticos
            if ($content -match "?|Ã³|Ã­|Ã±|Ã¡|Ã©|Ãº") {
        Write-Host "??  $file tiene caracteres corruptos - Intentando arreglar..." -ForegroundColor Yellow
   
         # Mapeo de caracteres corruptos comunes
         $replacements = @{
         "Ã³" = "ó"
                  "Ã­" = "í"
    "Ã±" = "ñ"
        "Ã¡" = "á"
        "Ã©" = "é"
               "Ãº" = "ú"
           "Ã" = "Ñ"
    "Ã³n" = "ón"
   "Ã­a" = "ía"
          "Ã±o" = "ño"
     "Ã¡s" = "ás"
        "Ã©s" = "és"
    "Ãº" = "ú"
           "â€œ" = '"'
       "â€" = '"'
        "â€™" = "'"
  "â€"" = "—"
        "â€"" = "–"
        "Â¿" = "¿"
  "Â¡" = "¡"
             "â—†" = "?"
        "â€?" = "×"
     }
        
       foreach ($key in $replacements.Keys) {
         $content = $content -replace [regex]::Escape($key), $replacements[$key]
        }
            }
  
            # Guardar con UTF-8 BOM
         [System.IO.File]::WriteAllText($file, $content, $encoding)
            Write-Host "? $file convertido" -ForegroundColor Green
  $totalFixed++
        }
        catch {
 Write-Host "? Error en $file : $_" -ForegroundColor Red
     }
    }
  else {
        Write-Host "??  No encontrado: $file" -ForegroundColor Yellow
}
}

Write-Host "`n=============================================" -ForegroundColor Cyan
Write-Host "? Proceso completado: $totalFixed archivos convertidos" -ForegroundColor Green
Write-Host ""
Write-Host "?? SIGUIENTE PASO:" -ForegroundColor Yellow
Write-Host "   1. Detener el servidor Vue (Ctrl+C)" -ForegroundColor White
Write-Host "   2. Ejecutar: cd CafeSanJuanVue" -ForegroundColor White
Write-Host "   3. Ejecutar: npm run dev" -ForegroundColor White
Write-Host "   4. Abrir: http://localhost:3000" -ForegroundColor White
Write-Host "   5. Hacer HARD REFRESH: Ctrl+Shift+R o Ctrl+F5" -ForegroundColor Cyan
Write-Host ""
