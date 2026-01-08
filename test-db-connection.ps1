# Script para probar la conexión a la base de datos Monster
# Este script usa .NET para verificar si la aplicación puede conectarse

$connectionString = "Server=db31553.databaseasp.net,1433;Database=db31553;User Id=db31553;Password=0520ARTU;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=True;Connection Timeout=30"

Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "  TEST DE CONEXIÓN A BASE DE DATOS" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host ""

try {
    # Cargar la DLL de System.Data.SqlClient
    Add-Type -AssemblyName "System.Data.SqlClient"
    
    Write-Host "Intentando conectar a: db31553.databaseasp.net" -ForegroundColor Yellow
    Write-Host "Base de datos: db31553" -ForegroundColor Yellow
    Write-Host ""
    
    # Crear conexión
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    
    # Intentar abrir la conexión
    $connection.Open()
    
    Write-Host "? CONEXIÓN EXITOSA!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Información del servidor:" -ForegroundColor Cyan
    Write-Host "  - Version: $($connection.ServerVersion)" -ForegroundColor White
    Write-Host "  - Estado: $($connection.State)" -ForegroundColor White
    Write-Host "  - Base de datos: $($connection.Database)" -ForegroundColor White
    Write-Host ""
    
    # Probar una consulta simple
    $command = $connection.CreateCommand()
    $command.CommandText = "SELECT 1 AS Test, GETDATE() AS FechaHora"
    
    $reader = $command.ExecuteReader()
    
    if ($reader.Read()) {
        Write-Host "? CONSULTA EJECUTADA CORRECTAMENTE" -ForegroundColor Green
    Write-Host "  - Test: $($reader['Test'])" -ForegroundColor White
        Write-Host "  - Fecha/Hora del servidor: $($reader['FechaHora'])" -ForegroundColor White
    }
    
    $reader.Close()
    $connection.Close()
    
    Write-Host ""
    Write-Host "===============================================" -ForegroundColor Green
    Write-Host "  LA CONEXIÓN FUNCIONA CORRECTAMENTE" -ForegroundColor Green
    Write-Host "===============================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Ahora intenta ejecutar el servicio nuevamente." -ForegroundColor Yellow
    
} catch {
    Write-Host ""
    Write-Host "? ERROR DE CONEXIÓN" -ForegroundColor Red
    Write-Host ""
    Write-Host "Mensaje de error:" -ForegroundColor Yellow
    Write-Host "  $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Posibles causas:" -ForegroundColor Yellow
    Write-Host "  1. El servidor Monster está bloqueando tu IP" -ForegroundColor White
    Write-Host "  2. Las credenciales han cambiado" -ForegroundColor White
    Write-Host "  3. El firewall de Windows está bloqueando la conexión" -ForegroundColor White
    Write-Host "  4. Monster requiere conexión desde una IP específica" -ForegroundColor White
    Write-Host ""
    Write-Host "Soluciones:" -ForegroundColor Cyan
    Write-Host "  1. Verifica que puedes conectarte desde SQL Server Management Studio" -ForegroundColor White
    Write-Host "  2. Contacta al soporte de Monster para verificar restricciones de IP" -ForegroundColor White
    Write-Host "  3. Verifica que no haya un firewall bloqueando el puerto 1433" -ForegroundColor White
}

Write-Host ""
Read-Host "Presiona Enter para salir"
