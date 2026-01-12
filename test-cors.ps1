# Test CORS Configuration
# PowerShell version for Windows

Write-Host "?? Testing CORS Configuration on Microservices..." -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan

# URLs de los microservicios
$SeguridadUrl = "https://seguridad-production-279b.up.railway.app"
$MonsterUrl = "http://cafesanjuanr.runasp.net"
$LocalUrl = "http://localhost:8080"

function Test-Cors {
    param(
        [string]$ServiceName,
   [string]$BaseUrl
    )
    
    Write-Host ""
    Write-Host "?? Testing $ServiceName at $BaseUrl" -ForegroundColor Yellow
    Write-Host "--------------------" -ForegroundColor Yellow
    
    try {
   # Test OPTIONS preflight request
        Write-Host "?? Testing OPTIONS (preflight)..." -ForegroundColor White
        $response = Invoke-WebRequest -Uri "$BaseUrl/api/usuarios" -Method OPTIONS `
        -Headers @{
   "Origin" = "http://localhost:3000"
    "Access-Control-Request-Method" = "GET"
   "Access-Control-Request-Headers" = "Content-Type"
     } -UseBasicParsing -ErrorAction SilentlyContinue
        
        if ($response.Headers.ContainsKey("Access-Control-Allow-Origin")) {
            Write-Host "? CORS Headers Found:" -ForegroundColor Green
     Write-Host "   Access-Control-Allow-Origin: $($response.Headers['Access-Control-Allow-Origin'])" -ForegroundColor Green
        } else {
    Write-Host "? No CORS headers found" -ForegroundColor Red
 }
      
    } catch {
        Write-Host "? Error testing $ServiceName`: $($_.Exception.Message)" -ForegroundColor Red
 }
}

# Probar cada servicio
Test-Cors "SeguridadService (Railway)" $SeguridadUrl
Test-Cors "Monster Hosting" $MonsterUrl

Write-Host ""
Write-Host "? CORS Test Complete!" -ForegroundColor Green
Write-Host "?? Check the results above for CORS headers" -ForegroundColor Cyan

# Test desde JavaScript (simulación)
Write-Host ""
Write-Host "?? JavaScript Fetch Test Simulation:" -ForegroundColor Magenta
Write-Host "fetch('$SeguridadUrl/api/usuarios', {" -ForegroundColor White
Write-Host "  method: 'GET'," -ForegroundColor White
Write-Host "  headers: { 'Content-Type': 'application/json' }" -ForegroundColor White
Write-Host "})" -ForegroundColor White