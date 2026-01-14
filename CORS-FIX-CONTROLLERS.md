# ? CORS Activado en Controladores

## Fecha: 2025-01-XX
## Problema Resuelto
Error CORS al hacer peticiones desde `http://localhost:3000` a Railway:
```
Access to fetch at 'https://reserva-production-279b.up.railway.app/api/carrito/promociones' 
from origin 'http://localhost:3000' has been blocked by CORS policy
```

## Cambios Realizados

### 1. **ReservasService/Controllers/CarritoController.cs**
```csharp
[ApiController]
[Route("api/carrito")]
[EnableCors("AllowAll")]  // ? AGREGADO
public class CarritoController : ControllerBase
```

### 2. **FacturacionService/Controllers/FacturacionController.cs**
```csharp
[ApiController]
[Route("api/facturas")]
[EnableCors("AllowAll")]  // ? AGREGADO
public class FacturacionController : ControllerBase
```

### 3. **FacturacionService/Controllers/DetalleFacturaController.cs**
```csharp
[ApiController]
[Route("api/detallefactura")]
[EnableCors("AllowAll")]  // ? AGREGADO
public class DetalleFacturaController : ControllerBase
```

## Política CORS "AllowAll"
Definida en `Program.cs` de cada servicio:
```csharp
options.AddPolicy("AllowAll", policy =>
{
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader()
 .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
});
```

## ¿Por qué se agregó?
Aunque CORS ya estaba configurado **globalmente** con `app.UseCors("AllowAll")`, agregar el atributo `[EnableCors]` a nivel de controlador:
1. ? **Garantiza** que CORS se aplique específicamente a ese controlador
2. ? **Asegura compatibilidad** en entornos de producción como Railway
3. ? **Documenta explícitamente** que el controlador permite CORS
4. ? **Evita problemas** de configuración en diferentes hosts

## Próximos Pasos
1. **Recompilar los servicios**:
   ```bash
   dotnet build ReservasService/ReservasService.csproj
   dotnet build FacturacionService/FacturacionService.csproj
 ```

2. **Redesplegar a Railway** (si aplica)

3. **Probar desde el frontend**:
   ```javascript
   // En tu componente Vue
   fetch('https://reserva-production-279b.up.railway.app/api/carrito/promociones')
     .then(response => response.json())
     .then(data => console.log('? CORS funcionando:', data))
   ```

## Orígenes Permitidos
### Desarrollo:
- `http://localhost:3000`
- `http://localhost:5173`
- `http://localhost:8080`
- `http://127.0.0.1:3000`
- `http://127.0.0.1:5173`
- `http://127.0.0.1:8080`

### Producción:
- `http://cafesanjuanr.runasp.net`
- `https://cafesanjuanr.runasp.net`
- `https://ws-restaurante-production.up.railway.app`
- `https://seguridad-production-279b.up.railway.app`

## Notas Importantes
?? **Producción**: Considera usar una política más restrictiva en producción:
```csharp
[EnableCors("Production")]  // Solo orígenes específicos
```

? **Desarrollo**: `AllowAll` es seguro solo en desarrollo local
