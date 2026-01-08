# Script para FORZAR el reemplazo de caracteres corruptos en archivos Vue
Write-Host "Arreglando caracteres corruptos en archivos Vue..." -ForegroundColor Cyan

# Mapeo de reemplazos
$replacements = @{
    # Vocales con tilde
 "ó" = "ó"
    "í" = "í"
    "ñ" = "ñ"
    "á" = "á"
    "é" = "é"
    "ú" = "ú"
    "Ó" = "Ó"
    "Í" = "Í"
    "Ñ" = "Ñ"
    "Á" = "Á"
    "É" = "É"
    "Ú" = "Ú"
    # Signos de interrogacion y exclamacion
    "¿" = "¿"
    "¡" = "¡"
    # Simbolos especiales
    "×" = "×"
    "?" = "?"
    "?" = "?"
    "•" = "•"
    # Comillas especiales
    """ = '"'
 """ = '"'
    "'" = "'"
}

$files = Get-ChildItem -Path "CafeSanJuanVue\pages" -Filter "*.vue" -Recurse

foreach ($file in $files) {
  try {
        $content = Get-Content $file.FullName -Raw -Encoding UTF8
      $modified = $false
        
        foreach ($key in $replacements.Keys) {
 if ($content -match [regex]::Escape($key)) {
             $content = $content -replace [regex]::Escape($key), $replacements[$key]
        $modified = $true
    }
        }
        
   if ($modified) {
      $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
      [System.IO.File]::WriteAllText($file.FullName, $content, $utf8NoBom)
            Write-Host "ARREGLADO: $($file.Name)" -ForegroundColor Green
        } else {
   Write-Host "OK: $($file.Name)" -ForegroundColor Gray
        }
    }
    catch {
   Write-Host "ERROR: $($file.Name) - $_" -ForegroundColor Red
  }
}

# Tambien el index.html
$indexFile = "CafeSanJuanVue\index.html"
if (Test-Path $indexFile) {
    $content = Get-Content $indexFile -Raw -Encoding UTF8
    $modified = $false
    
    foreach ($key in $replacements.Keys) {
        if ($content -match [regex]::Escape($key)) {
   $content = $content -replace [regex]::Escape($key), $replacements[$key]
    $modified = $true
        }
    }
    
    if ($modified) {
        $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
        [System.IO.File]::WriteAllText($indexFile, $content, $utf8NoBom)
     Write-Host "ARREGLADO: index.html" -ForegroundColor Green
    }
}

Write-Host "`nCompletado! Reinicia el servidor Vue con Ctrl+C y luego npm run dev" -ForegroundColor Yellow
