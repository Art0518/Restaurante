# =============================================
# PRUEBA DE CADENAS DE CONEXIÓN PARA MONSTER
# =============================================

Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "  PROBANDO DIFERENTES CADENAS DE CONEXIÓN" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host ""

# Array de cadenas de conexión para probar
$connectionStrings = @(
    # Opción 1: TCP/IP con puerto
    @{
        Name = "TCP/IP con puerto 1433"
        ConnectionString = "Server=db31553.databaseasp.net,1433;Database=db31553;User Id=db31553;Password=0520ARTU;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=True;Connection Timeout=60;"
    },
  # Opción 2: Sin especificar puerto
  @{
        Name = "Sin puerto especificado"
        ConnectionString = "Server=db31553.databaseasp.net;Database=db31553;User Id=db31553;Password=0520ARTU;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=True;Connection Timeout=60;"
    },
    # Opción 3: Data Source en lugar de Server
    @{
        Name = "Data Source"
        ConnectionString = "Data Source=db31553.databaseasp.net;Initial Catalog=db31553;User ID=db31553;Password=0520ARTU;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=True;Connection Timeout=60;"
    },
    # Opción 4: Con instancia MSSQLSERVER
    @{
     Name = "Con instancia MSSQLSERVER"
        ConnectionString = "Server=db31553.databaseasp.net\\MSSQLSERVER;Database=db31553;User Id=db31553;Password=0520ARTU;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=True;Connection Timeout=60;"
    },
    # Opción 5: TCP explícito
    @{
        Name = "TCP explícito"
        ConnectionString = "Server=tcp:db31553.databaseasp.net,1433;Database=db31553;User Id=db31553;Password=0520ARTU;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=True;Connection Timeout=60;"
    },
  # Opción 6: Con Network Library
    @{
        Name = "Con Network Library DBMSSOCN"
  ConnectionString = "Server=db31553.databaseasp.net;Database=db31553;User Id=db31553;Password=0520ARTU;Network Library=DBMSSOCN;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=True;Connection Timeout=60;"
    },
    # Opción 7: Persist Security Info
    @{
        Name = "Con Persist Security Info"
        ConnectionString = "Server=db31553.databaseasp.net;Database=db31553;User Id=db31553;Password=0520ARTU;Persist Security Info=True;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=True;Connection Timeout=60;"
    }
)

$successfulConnection = $null

foreach ($connTest in $connectionStrings) {
  Write-Host "---------------------------------------------------" -ForegroundColor Yellow
 Write-Host "Probando: $($connTest.Name)" -ForegroundColor Yellow
    Write-Host "---------------------------------------------------" -ForegroundColor Yellow
    
    try {
        # Intentar con System.Data.SqlClient
        $connection = New-Object System.Data.SqlClient.SqlConnection($connTest.ConnectionString)
        $connection.Open()
        
  Write-Host "? CONEXIÓN EXITOSA!" -ForegroundColor Green
        Write-Host "  - Estado: $($connection.State)" -ForegroundColor White
        Write-Host "  - Versión del servidor: $($connection.ServerVersion)" -ForegroundColor White
 Write-Host "  - Base de datos: $($connection.Database)" -ForegroundColor White
        Write-Host ""
        
        # Ejecutar una consulta de prueba
        $command = $connection.CreateCommand()
        $command.CommandText = "SELECT @@VERSION AS Version, GETDATE() AS FechaHora"
        $reader = $command.ExecuteReader()
        
        if ($reader.Read()) {
  Write-Host "? CONSULTA EJECUTADA" -ForegroundColor Green
            Write-Host "  - Fecha/Hora servidor: $($reader['FechaHora'])" -ForegroundColor White
        }
        
        $reader.Close()
      $connection.Close()
        
        Write-Host ""
Write-Host "===============================================" -ForegroundColor Green
        Write-Host "  ¡CADENA DE CONEXIÓN FUNCIONAL ENCONTRADA!" -ForegroundColor Green
   Write-Host "===============================================" -ForegroundColor Green
  Write-Host ""
        Write-Host "Usa esta cadena de conexión:" -ForegroundColor Cyan
 Write-Host $connTest.ConnectionString -ForegroundColor White
Write-Host ""
     
        $successfulConnection = $connTest
      break
        
    } catch {
        Write-Host "? FALLÓ" -ForegroundColor Red
  Write-Host "  Error: $($_.Exception.Message.Split([Environment]::NewLine)[0])" -ForegroundColor DarkRed
    Write-Host ""
    }
}

if ($null -eq $successfulConnection) {
    Write-Host ""
    Write-Host "===============================================" -ForegroundColor Red
    Write-Host "  NINGUNA CADENA DE CONEXIÓN FUNCIONÓ" -ForegroundColor Red
    Write-Host "===============================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Esto significa que Monster está bloqueando TODAS las conexiones" -ForegroundColor Yellow
    Write-Host "desde aplicaciones .NET, incluso con diferentes configuraciones." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Opciones:" -ForegroundColor Cyan
    Write-Host "  1. Contactar al soporte de Monster para:" -ForegroundColor White
 Write-Host "     - Habilitar conexiones desde aplicaciones externas" -ForegroundColor White
    Write-Host "     - Agregar tu IP a la lista blanca" -ForegroundColor White
    Write-Host "     - Obtener la cadena de conexión correcta" -ForegroundColor White
    Write-Host ""
    Write-Host "  2. Verificar en SSMS:" -ForegroundColor White
    Write-Host "     - Clic derecho en la conexión > Propiedades" -ForegroundColor White
    Write-Host "     - Ver la cadena de conexión exacta que usa SSMS" -ForegroundColor White
    Write-Host ""
 Write-Host "  3. Preguntar a Monster si requieren:" -ForegroundColor White
    Write-Host "     - VPN para conectar desde aplicaciones" -ForegroundColor White
    Write-Host "     - Configuración especial de firewall" -ForegroundColor White
    Write-Host "     - Puerto diferente al 1433" -ForegroundColor White
    Write-Host ""
} else {
    Write-Host "Actualiza tus appsettings.json con esta cadena." -ForegroundColor Yellow
}

Write-Host ""
Read-Host "Presiona Enter para salir"
