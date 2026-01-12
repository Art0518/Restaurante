#!/bin/bash

echo "?? Testing CORS Configuration on Microservices..."
echo "=================================================="

# URLs de los microservicios
SEGURIDAD_URL="https://seguridad-production-279b.up.railway.app"
MONSTER_URL="http://cafesanjuanr.runasp.net"
LOCAL_URL="http://localhost:8080"

# Función para probar CORS
test_cors() {
    local service_name=$1
    local base_url=$2
    
    echo ""
    echo "?? Testing $service_name at $base_url"
    echo "--------------------"
    
    # Test OPTIONS preflight request
    echo "?? Testing OPTIONS (preflight)..."
 curl -s -I -X OPTIONS \
        -H "Origin: http://localhost:3000" \
   -H "Access-Control-Request-Method: GET" \
    -H "Access-Control-Request-Headers: Content-Type" \
        "$base_url/api/usuarios" | grep -i "access-control"
    
    echo ""
    echo "?? Testing GET request..."
    curl -s -I -X GET \
        -H "Origin: http://localhost:3000" \
   "$base_url/api/usuarios" | grep -i "access-control"
}

# Probar cada servicio
test_cors "SeguridadService (Railway)" $SEGURIDAD_URL
test_cors "Monster Hosting" $MONSTER_URL

echo ""
echo "? CORS Test Complete!"
echo "?? Look for 'access-control-allow-origin: *' in the responses above"