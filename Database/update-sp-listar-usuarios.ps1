# Script para actualizar el procedimiento almacenado sp_listar_usuarios
# Asegúrate de configurar las variables correctamente

# Configuración de conexión
$serverInstance = "localhost"  # o tu servidor SQL
$database = "nombre_de_tu_bd"  # REEMPLAZAR con tu nombre de BD
$sqlFile = "Database\update_sp_listar_usuarios.sql"

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Actualizando sp_listar_usuarios" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan

try {
  # Leer el contenido del archivo SQL
    $sqlContent = Get-Content -Path $sqlFile -Raw
    
    # Reemplazar el nombre de la base de datos
$sqlContent = $sqlContent -replace '\[nombre_de_tu_base_de_datos\]', "[$database]"
    
    # Ejecutar el script
  Write-Host "`nEjecutando script SQL..." -ForegroundColor Yellow
    
    Invoke-Sqlcmd -ServerInstance $serverInstance -Database $database -Query $sqlContent -Verbose
    
    Write-Host "`n? Procedimiento almacenado actualizado exitosamente!" -ForegroundColor Green
    
} catch {
    Write-Host "`n? Error al actualizar el procedimiento:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}

Write-Host "`nPresiona cualquier tecla para continuar..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
