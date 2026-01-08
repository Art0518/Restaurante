# Script para forzar UTF-8 en Footer.vue
$ErrorActionPreference = "Stop"

Write-Host "?? Corrigiendo Footer.vue con UTF-8..." -ForegroundColor Cyan

$filePath = "CafeSanJuanVue\pages\components\Footer.vue"

# Contenido correcto del Footer
$content = @"
<template>
  <footer>
    © 2025 Un Rincón en San Juan | Sabores del Caribe y tradición puertorriqueña
  </footer>
</template>

<script>
export default {
  name: 'FooterComponent'
}
</script>

<style scoped>
footer {
  background: linear-gradient(135deg, #3e2f21 0%, #6b8e23 100%);
  color: #f5d7a5;
  text-align: center;
  padding: 25px 20px;
  font-size: 0.95em;
font-weight: 500;
  border-top: 3px solid #8dbb35;
  box-shadow: 0 -4px 15px rgba(0, 0, 0, 0.1);
  margin-top: auto;
}

footer:hover {
  background: linear-gradient(135deg, #6b8e23 0%, #3e2f21 100%);
  transition: background 0.5s ease;
}
</style>
"@

# Guardar con UTF-8 sin BOM
$Utf8NoBomEncoding = New-Object System.Text.UTF8Encoding $False
[System.IO.File]::WriteAllText((Resolve-Path $filePath), $content, $Utf8NoBomEncoding)

Write-Host "? Footer.vue corregido con UTF-8 sin BOM" -ForegroundColor Green
Write-Host ""
Write-Host "Recarga el navegador con Ctrl+F5" -ForegroundColor Yellow
