# ?? CORS Configuration for Microservices

## ? **CONFIGURACIÓN COMPLETADA**

Se ha actualizado la configuración de CORS en todos los microservices para permitir peticiones desde localhost y Monster hosting.

## ?? **Microservices Actualizados**

### 1. SeguridadService ?
- **Railway URL**: `https://seguridad-production-279b.up.railway.app`
- **Endpoints**: `/api/usuarios/*`
- **Status**: CORS Habilitado

### 2. ReservasService ?  
- **Railway URL**: TBD
- **Endpoints**: `/api/reservas/*`, `/api/mesas/*`
- **Status**: CORS Habilitado

### 3. MenuService ?
- **Railway URL**: TBD  
- **Endpoints**: `/api/platos/*`, `/api/promociones/*`
- **Status**: CORS Habilitado

### 4. FacturacionService ?
- **Railway URL**: TBD
- **Endpoints**: `/api/facturas/*`, `/api/carrito/*`
- **Status**: CORS Habilitado

## ?? **Políticas CORS Implementadas**

### **AllowAll Policy** (Activa)
```csharp
policy.AllowAnyOrigin()
  .AllowAnyMethod()
      .AllowAnyHeader()
   .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
```

### **Development Policy**
- ? `http://localhost:3000` (React/Next.js)
- ? `http://localhost:5173` (Vite/Vue)
- ? `http://localhost:8080` (General)
- ? `http://127.0.0.1:*` (IP local)

### **Production Policy** 
- ? `http://cafesanjuanr.runasp.net` (Monster hosting)
- ? `https://cafesanjuanr.runasp.net` (Monster hosting SSL)
- ? Railway URLs cuando estén disponibles

## ?? **Testing CORS**

### **1. Ejecutar Test Script**
```bash
# En PowerShell
./test-cors.ps1

# En Bash/Linux
./test-cors.sh
```

### **2. Manual Browser Test**
Abre el developer console en tu navegador y ejecuta:

```javascript
// Test SeguridadService
fetch('https://seguridad-production-279b.up.railway.app/api/usuarios', {
  method: 'GET',
  headers: { 'Content-Type': 'application/json' }
})
.then(response => response.json())
.then(data => console.log('? CORS working:', data))
.catch(error => console.error('? CORS error:', error));

// Test Monster hosting
fetch('http://cafesanjuanr.runasp.net/api/usuarios', {
  method: 'GET', 
  headers: { 'Content-Type': 'application/json' }
})
.then(response => response.json())
.then(data => console.log('? Monster CORS working:', data))
.catch(error => console.error('? Monster CORS error:', error));
```

### **3. Vue App Testing**
Tu configuración en `api.config.js` ahora tiene:
```javascript
useMockData: false // ? Mock deshabilitado para probar APIs reales
```

## ?? **Deployment Instructions**

### **Monster Hosting**
1. Asegúrate de que el web.config incluye:
```xml
<system.webServer>
  <httpProtocol>
    <customHeaders>
      <add name="Access-Control-Allow-Origin" value="*" />
      <add name="Access-Control-Allow-Methods" value="GET,POST,PUT,DELETE,OPTIONS" />
      <add name="Access-Control-Allow-Headers" value="Content-Type,Authorization" />
    </customHeaders>
</httpProtocol>
</system.webServer>
```

### **Railway Deployment**
Los microservices ya están configurados correctamente para Railway. Solo ejecuta:
```bash
railway deploy
```

## ?? **Troubleshooting**

### **Error: "CORS policy blocked"**
1. Verifica que el microservice esté ejecutándose
2. Confirma que la URL es correcta
3. Revisa que no hay errores en el endpoint

### **Error: "Failed to fetch"**
1. Verifica conectividad de red
2. Asegúrate de que el servicio esté disponible
3. Revisa si hay errores de certificado SSL

### **Error 404 en endpoints**
1. Verifica que el controller esté registrado
2. Confirma que las rutas son correctas
3. Revisa los logs del microservice

## ?? **Verificar Status**

### **Health Check URLs**
- SeguridadService: `https://seguridad-production-279b.up.railway.app/`
- Monster: `http://cafesanjuanr.runasp.net/`
- Local: `http://localhost:8080/`

### **Expected Response**
```
Servicio de [Nombre] - CafeSanJuan (gRPC + REST + GraphQL) - CORS ENABLED
```

## ?? **Notas Importantes**

1. **AllowAll Policy**: Muy permisiva, ideal para desarrollo y testing
2. **Producción**: Considera usar políticas más restrictivas en producción
3. **HTTPS**: Monster hosting soporta HTTPS, úsalo cuando sea posible
4. **GraphQL**: Todos los services también soportan GraphQL en `/graphql`

## ? **Status Actual**
- ? CORS configurado en todos los microservices
- ? Políticas para desarrollo y producción
- ? Mock data deshabilitado en Vue app  
- ? Scripts de testing creados
- ? Build successful
- ?? **Ready for testing!**