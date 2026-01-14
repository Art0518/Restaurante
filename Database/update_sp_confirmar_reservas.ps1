# ============================================================
# ?? Script PowerShell para actualizar SP en la base de datos
# ?? Descripción: Ejecuta el SP sp_confirmar_reservas_selectivas
# ?? Autor: Art0518
# ?? Fecha: Enero 2025
# ============================================================

Write-Host "?? Actualizando Stored Procedure: sp_confirmar_reservas_selectivas" -ForegroundColor Cyan
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host ""

# Parámetros de conexión
$Server = "db31553.public.databaseasp.net"
$Database = "db31553"
$Username = "db31553"
$Password = "0520ARTU"
$SqlFile = "Database/sp_confirmar_reservas_selectivas.sql"

# Verificar que el archivo SQL existe
if (-Not (Test-Path $SqlFile)) {
    Write-Host "? Error: No se encontró el archivo $SqlFile" -ForegroundColor Red
    exit 1
}

Write-Host "?? Archivo SQL encontrado: $SqlFile" -ForegroundColor Green
Write-Host "?? Conectando a: $Server\$Database" -ForegroundColor Yellow
Write-Host ""

try {
    # Leer el contenido del archivo SQL
    $SqlContent = Get-Content -Path $SqlFile -Raw -Encoding UTF8
    
    # Crear la cadena de conexión
    $ConnectionString = "Server=$Server;Database=$Database;User Id=$Username;Password=$Password;Encrypt=True;TrustServerCertificate=True;Connection Timeout=60;"
    
    # Crear la conexión
    $Connection = New-Object System.Data.SqlClient.SqlConnection
    $Connection.ConnectionString = $ConnectionString
  $Connection.Open()
    
    Write-Host "? Conexión establecida correctamente" -ForegroundColor Green
    Write-Host "??  Ejecutando script SQL..." -ForegroundColor Yellow
    Write-Host ""
    
    # Crear el comando SQL
    $Command = New-Object System.Data.SqlClient.SqlCommand
    $Command.Connection = $Connection
    $Command.CommandText = $SqlContent
    $Command.CommandTimeout = 120
    
    # Ejecutar el comando
    $Result = $Command.ExecuteNonQuery()
    
    Write-Host "? Stored Procedure actualizado exitosamente" -ForegroundColor Green
    Write-Host "?? Filas afectadas: $Result" -ForegroundColor Cyan
    Write-Host ""
    
    # Cerrar la conexión
    $Connection.Close()
    
    Write-Host "================================================================" -ForegroundColor Cyan
    Write-Host "? PROCESO COMPLETADO EXITOSAMENTE" -ForegroundColor Green
    Write-Host "================================================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "?? Parámetros del SP:" -ForegroundColor Yellow
    Write-Host " - @IdUsuario (INT)" -ForegroundColor White
    Write-Host "   - @ReservasIds (VARCHAR(MAX))" -ForegroundColor White
    Write-Host "   - @MetodoPago (VARCHAR(50))" -ForegroundColor White
    Write-Host "   - @MontoDescuento (DECIMAL(10,2)) [Default: 0]" -ForegroundColor White
    Write-Host "   - @Total (DECIMAL(10,2))" -ForegroundColor White
    Write-Host ""
    Write-Host "?? Ejemplo de uso:" -ForegroundColor Yellow
    Write-Host "   EXEC sp_confirmar_reservas_selectivas" -ForegroundColor White
    Write-Host "       @IdUsuario = 10050," -ForegroundColor White
    Write-Host "       @ReservasIds = '133'," -ForegroundColor White
  Write-Host "       @MetodoPago = 'TARJETA'," -ForegroundColor White
    Write-Host "       @MontoDescuento = 0," -ForegroundColor White
    Write-Host "       @Total = 22.3;" -ForegroundColor White
    Write-Host ""
    
    exit 0
}
catch {
    Write-Host "" -ForegroundColor Red
    Write-Host "? ERROR AL EJECUTAR EL SCRIPT" -ForegroundColor Red
    Write-Host "================================================================" -ForegroundColor Red
    Write-Host "Mensaje: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "================================================================" -ForegroundColor Red
    
    if ($Connection.State -eq 'Open') {
        $Connection.Close()
    }
    
    exit 1
}
