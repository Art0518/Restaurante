# Script para probar FacturacionService
# Ejecutar: .\test-facturacion.ps1

$baseUrl = "http://localhost:5004"

Write-Host "==============================================================================" -ForegroundColor Cyan
Write-Host "PROBANDO FACTURACION SERVICE - CafeSanJuan" -ForegroundColor Cyan
Write-Host "==============================================================================" -ForegroundColor Cyan
Write-Host ""

# Test 1: Verificar servicio
Write-Host "[TEST 1] Verificando servicio de Facturación..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/" -Method Get
    Write-Host "? Servicio respondió: $response" -ForegroundColor Green
} catch {
    Write-Host "? Error: $_" -ForegroundColor Red
}
Write-Host ""

# Test 2: Listar todas las facturas
Write-Host "[TEST 2] Listando todas las facturas..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/facturacion/listar" -Method Get
  Write-Host "? Facturas obtenidas: $($response.Count) registros" -ForegroundColor Green
    if ($response.Count -gt 0) {
        Write-Host "  Primera factura: IdFactura=$($response[0].IdFactura), Total=$($response[0].Total)" -ForegroundColor Gray
    }
} catch {
  Write-Host "? Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 3: Generar factura desde carrito
Write-Host "[TEST 3] Generando factura desde carrito..." -ForegroundColor Yellow
$body = @{
    idUsuario = 10028
    reservasIds = "10027,10028"
    promocionId = $null
metodoPago = ""
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/facturacion/generar-carrito" -Method Post -Body $body -ContentType "application/json"
    Write-Host "? Factura generada exitosamente" -ForegroundColor Green
    Write-Host "  IdFactura: $($response.data.IdFactura)" -ForegroundColor Gray
    Write-Host "  Subtotal: $($response.data.Subtotal)" -ForegroundColor Gray
    Write-Host "IVA: $($response.data.IVA)" -ForegroundColor Gray
    Write-Host "  Total: $($response.data.Total)" -ForegroundColor Gray
} catch {
 Write-Host "? Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "  Detalles: $($_.ErrorDetails.Message)" -ForegroundColor Red
    }
}
Write-Host ""

# Test 4: Obtener factura detallada
Write-Host "[TEST 4] Obteniendo factura detallada (ID=1)..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/facturacion/1/detallada" -Method Get
    Write-Host "? Factura detallada obtenida" -ForegroundColor Green
    if ($response.data.Factura) {
        Write-Host "  Estado: $($response.data.Factura.Rows[0].Estado)" -ForegroundColor Gray
   Write-Host "  Detalles: $($response.data.Detalles.Rows.Count) items" -ForegroundColor Gray
    }
} catch {
    Write-Host "? Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 5: Marcar factura como pagada
Write-Host "[TEST 5] Marcando factura como pagada (ID=1)..." -ForegroundColor Yellow
$body = @{
    idFactura = 1
metodoPago = "Tarjeta de Crédito"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/facturacion/marcar-pagada" -Method Post -Body $body -ContentType "application/json"
    Write-Host "? Factura marcada como pagada" -ForegroundColor Green
    Write-Host "  Mensaje: $($response.mensaje)" -ForegroundColor Gray
} catch {
    Write-Host "? Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 6: Listar facturas de usuario
Write-Host "[TEST 6] Listando facturas del usuario 10028..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/facturacion/usuario/10028" -Method Get
    Write-Host "? Facturas del usuario obtenidas: $($response.data.Rows.Count) facturas" -ForegroundColor Green
} catch {
Write-Host "? Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 7: Listar detalles de factura
Write-Host "[TEST 7] Listando detalles de factura (ID=1)..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/detallefactura/factura/1" -Method Get
    Write-Host "? Detalles obtenidos: $($response.data.Count) items" -ForegroundColor Green
} catch {
    Write-Host "? Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 8: Obtener estadísticas de factura
Write-Host "[TEST 8] Obteniendo estadísticas de factura (ID=1)..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/detallefactura/estadisticas/factura/1" -Method Get
    Write-Host "? Estadísticas obtenidas" -ForegroundColor Green
    if ($response.data.Estadisticas) {
        Write-Host "  Subtotal calculado: $($response.data.Estadisticas.SubtotalCalculado)" -ForegroundColor Gray
        Write-Host "  Cantidad de detalles: $($response.data.Estadisticas.CantidadDetalles)" -ForegroundColor Gray
    }
} catch {
    Write-Host "? Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

Write-Host "==============================================================================" -ForegroundColor Cyan
Write-Host "PRUEBAS COMPLETADAS" -ForegroundColor Cyan
Write-Host "==============================================================================" -ForegroundColor Cyan
