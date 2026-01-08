# Script para convertir todos los archivos Vue a UTF-8
Write-Host "?? Convirtiendo archivos Vue a UTF-8..." -ForegroundColor Cyan

$vueFiles = Get-ChildItem -Path "CafeSanJuanVue\pages" -Filter "*.vue" -Recurse

foreach ($file in $vueFiles) {
    try {
  Write-Host "Procesando: $($file.FullName)" -ForegroundColor Yellow
   
      # Leer contenido
      $content = Get-Content -Path $file.FullName -Raw -Encoding UTF8
        
  # Guardar en UTF-8 sin BOM
        $utf8NoBom = New-Object System.Text.UTF8Encoding $false
        [System.IO.File]::WriteAllText($file.FullName, $content, $utf8NoBom)
 
        Write-Host "  ? Convertido" -ForegroundColor Green
    }
    catch {
        Write-Host "  ? Error: $_" -ForegroundColor Red
    }
}

Write-Host "`n? Conversión completada!" -ForegroundColor Green
