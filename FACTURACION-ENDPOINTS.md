# ?? ENDPOINTS DE FACTURACIÓN - CAFESANJUAN

**Fecha:** Enero 2025  
**Autor:** Art0518  
**Servicio:** FacturacionService  

---

## ?? BASE URLs

| Ambiente | URL |
|----------|-----|
| **Desarrollo** | `http://localhost:5002` |
| **Producción (Railway)** | `https://factura-production-279b.up.railway.app` |

---

## ?? ÍNDICE DE ENDPOINTS

1. [Gestión de Facturas](#1-gestión-de-facturas)
2. [Carrito y Promociones](#2-carrito-y-promociones)
3. [Gestión de Pagos](#3-gestión-de-pagos)
4. [Consultas Detalladas](#4-consultas-detalladas)
5. [Utilitarios](#5-utilitarios)

---

## 1. GESTIÓN DE FACTURAS

### 1.1 Listar Todas las Facturas

**Endpoint:**
```
GET /api/facturas
```

**Descripción:** Obtiene todas las facturas registradas en el sistema.

**Response 200 OK:**
```json
{
  "success": true,
  "message": "Facturas obtenidas correctamente",
  "data": [
    {
      "IdFactura": 1,
      "IdUsuario": 10050,
      "FechaHora": "2025-01-20T10:30:00",
      "Subtotal": 100.00,
      "IVA": 11.50,
      "Total": 111.50,
      "Estado": "Pagada",
      "IdReserva": 133
    }
  ],
  "count": 1
}
```

**Ejemplo cURL:**
```bash
curl -X GET https://factura-production-279b.up.railway.app/api/facturas
```

---

### 1.2 Obtener Factura por ID

**Endpoint:**
```
GET /api/facturas/{id}
```

**Parámetros:**
- `id` (int, path, requerido): ID de la factura

**Response 200 OK:**
```json
{
  "success": true,
  "message": "Detalle de factura obtenido correctamente",
  "data": {
    "IdFactura": 1,
    "IdUsuario": 10050,
    "FechaHora": "2025-01-20T10:30:00",
    "Subtotal": 100.00,
    "IVA": 11.50,
    "Total": 111.50,
    "Estado": "Pagada"
  }
}
```

**Response 404 Not Found:**
```json
{
  "success": false,
  "message": "Factura no encontrada"
}
```

**Ejemplo cURL:**
```bash
curl -X GET https://factura-production-279b.up.railway.app/api/facturas/1
```

---

### 1.3 Crear Factura Simple

**Endpoint:**
```
POST /api/facturas
```

**Headers:**
```
Content-Type: application/json
```

**Body:**
```json
{
  "idUsuario": 10050,
  "idReserva": 133
}
```

**Response 200 OK:**
```json
{
  "success": true,
  "message": "Factura generada correctamente",
  "data": {
    "IdFactura": 45,
    "Total": 111.50,
    "Estado": "Emitida"
  }
}
```

**Ejemplo cURL:**
```bash
curl -X POST https://factura-production-279b.up.railway.app/api/facturas \
  -H "Content-Type: application/json" \
  -d '{
    "idUsuario": 10050,
    "idReserva": 133
  }'
```

---

### 1.4 Anular Factura

**Endpoint:**
```
PUT /api/facturas/{id}/anular
```

**Parámetros:**
- `id` (int, path, requerido): ID de la factura a anular

**Response 200 OK:**
```json
{
  "success": true,
  "message": "Factura anulada correctamente",
  "idFactura": 45
}
```

**Ejemplo cURL:**
```bash
curl -X PUT https://factura-production-279b.up.railway.app/api/facturas/45/anular
```

---

## 2. CARRITO Y PROMOCIONES

### 2.1 Generar Factura desde Carrito

**Endpoint:**
```
POST /api/facturas/generar-carrito
```

**Headers:**
```
Content-Type: application/json
```

**Body:**
```json
{
  "IdUsuario": 10050,
  "ReservasIds": "133,134,135",
  "PromocionId": 1,
  "MetodoPago": "TARJETA"
}
```

**Parámetros:**
- `IdUsuario` (int, requerido): ID del usuario
- `ReservasIds` (string, requerido): IDs de reservas separados por comas
- `PromocionId` (int, opcional): ID de la promoción a aplicar
- `MetodoPago` (string, opcional): Método de pago

**Response 200 OK:**
```json
{
  "success": true,
  "Estado": "SUCCESS",
  "Mensaje": "Factura generada correctamente",
  "IdFactura": 46,
  "SubtotalBruto": 150.00,
  "Descuento": 15.00,
  "Subtotal": 135.00,
  "IVA": 15.53,
  "Total": 150.53,
  "PorcentajeDescuento": 10.00,
  "CantidadReservas": 3,
  "MetodoPago": "TARJETA"
}
```

**Response 400 Bad Request:**
```json
{
  "success": false,
  "Estado": "ERROR",
  "Mensaje": "Debe seleccionar al menos una reserva"
}
```

**Ejemplo cURL:**
```bash
curl -X POST https://factura-production-279b.up.railway.app/api/facturas/generar-carrito \
  -H "Content-Type: application/json" \
  -d '{
    "IdUsuario": 10050,
    "ReservasIds": "133,134,135",
    "PromocionId": 1,
    "MetodoPago": "TARJETA"
  }'
```

---

### 2.2 Generar Factura de Reservas Confirmadas

**Endpoint:**
```
POST /api/facturas/generar-confirmadas
```

**Headers:**
```
Content-Type: application/json
```

**Body:**
```json
{
  "IdUsuario": 10050,
  "ReservasIds": "133,134",
  "TipoFactura": "CONFIRMADA"
}
```

**Parámetros:**
- `IdUsuario` (int, requerido): ID del usuario
- `ReservasIds` (string, requerido): IDs de reservas confirmadas separados por comas
- `TipoFactura` (string, opcional): Tipo de factura (default: "CONFIRMADA")

**Response 200 OK:**
```json
{
  "success": true,
  "Estado": "SUCCESS",
  "Mensaje": "Factura generada correctamente para reservas confirmadas",
  "IdFactura": 47,
  "SubtotalBruto": 100.00,
  "Subtotal": 100.00,
  "IVA": 11.50,
  "Total": 111.50,
  "CantidadReservas": 2,
  "MetodoPago": "EFECTIVO",
  "TipoFactura": "CONFIRMADA",
  "EstadoFactura": "Confirmada"
}
```

**Ejemplo cURL:**
```bash
curl -X POST https://factura-production-279b.up.railway.app/api/facturas/generar-confirmadas \
  -H "Content-Type: application/json" \
  -d '{
    "IdUsuario": 10050,
    "ReservasIds": "133,134",
    "TipoFactura": "CONFIRMADA"
  }'
```

---

## 3. GESTIÓN DE PAGOS

### 3.1 Marcar Factura como Pagada

**Endpoint:**
```
POST /api/facturas/marcar-pagada
```

**Headers:**
```
Content-Type: application/json
```

**Body:**
```json
{
  "IdFactura": 46,
  "MetodoPago": "TRANSFERENCIA"
}
```

**Parámetros:**
- `IdFactura` (int, requerido): ID de la factura
- `MetodoPago` (string, requerido): Método de pago (EFECTIVO, TARJETA, TRANSFERENCIA)

**Response 200 OK:**
```json
{
  "success": true,
  "Estado": "SUCCESS",
  "Mensaje": "Factura marcada como pagada correctamente",
  "IdFactura": 46
}
```

**Response 400 Bad Request:**
```json
{
  "success": false,
  "Estado": "ERROR",
  "Mensaje": "ID de factura no válido"
}
```

**Ejemplo cURL:**
```bash
curl -X POST https://factura-production-279b.up.railway.app/api/facturas/marcar-pagada \
  -H "Content-Type: application/json" \
  -d '{
  "IdFactura": 46,
    "MetodoPago": "TRANSFERENCIA"
  }'
```

---

## 4. CONSULTAS DETALLADAS

### 4.1 Obtener Factura Detallada

**Endpoint:**
```
GET /api/facturas/detallada/{id}
```

**Parámetros:**
- `id` (int, path, requerido): ID de la factura

**Descripción:** Obtiene una factura con todos sus detalles (items, reservas asociadas).

**Response 200 OK:**
```json
{
  "success": true,
  "message": "Factura detallada obtenida correctamente",
  "factura": [
    {
      "IdFactura": 46,
      "IdUsuario": 10050,
      "FechaHora": "2025-01-20T15:30:00",
      "Subtotal": 135.00,
    "IVA": 15.53,
      "Total": 150.53,
      "Estado": "Pagada"
    }
  ],
  "detalles": [
  {
      "IdDetalle": 1,
 "IdFactura": 46,
      "IdReserva": 133,
      "Descripcion": "Reserva Mesa 5 - 4 personas",
      "Cantidad": 1,
      "PrecioUnitario": 45.00,
      "Subtotal": 45.00
    },
    {
  "IdDetalle": 2,
      "IdFactura": 46,
   "IdReserva": 134,
      "Descripcion": "Reserva Mesa 3 - 2 personas",
      "Cantidad": 1,
   "PrecioUnitario": 45.00,
      "Subtotal": 45.00
    }
  ]
}
```

**Ejemplo cURL:**
```bash
curl -X GET https://factura-production-279b.up.railway.app/api/facturas/detallada/46
```

---

### 4.2 Listar Facturas de un Usuario

**Endpoint:**
```
GET /api/facturas/usuario/{id}
```

**Parámetros:**
- `id` (int, path, requerido): ID del usuario

**Response 200 OK:**
```json
{
  "success": true,
  "message": "Facturas del usuario obtenidas correctamente",
  "facturas": [
    {
 "IdFactura": 46,
    "FechaHora": "2025-01-20T15:30:00",
      "Total": 150.53,
      "Estado": "Pagada",
      "MetodoPago": "TARJETA"
    },
    {
      "IdFactura": 47,
      "FechaHora": "2025-01-21T12:00:00",
      "Total": 111.50,
      "Estado": "Emitida",
      "MetodoPago": null
    }
  ],
  "count": 2,
  "idUsuario": 10050
}
```

**Ejemplo cURL:**
```bash
curl -X GET https://factura-production-279b.up.railway.app/api/facturas/usuario/10050
```

---

### 4.3 Obtener Estadísticas de Facturación

**Endpoint:**
```
GET /api/facturas/estadisticas/{idUsuario}
```

**Parámetros:**
- `idUsuario` (int, path, requerido): ID del usuario

**Response 200 OK:**
```json
{
  "success": true,
  "message": "Estadísticas obtenidas correctamente",
  "idUsuario": 10050,
  "estadisticas": {
    "totalFacturas": 5,
    "facturasPagadas": 3,
  "facturasEmitidas": 1,
 "facturasAnuladas": 1,
    "montoTotal": 750.50,
    "montoTotalPagado": 450.30,
    "montoPendiente": 150.20
  }
}
```

**Ejemplo cURL:**
```bash
curl -X GET https://factura-production-279b.up.railway.app/api/facturas/estadisticas/10050
```

---

## 5. UTILITARIOS

### 5.1 Calcular Totales (Sin Guardar)

**Endpoint:**
```
POST /api/facturas/calcular
```

**Headers:**
```
Content-Type: application/json
```

**Body:**
```json
{
  "Subtotal": 100.00
}
```

**Descripción:** Calcula el IVA y total sin guardar en la base de datos.

**Response 200 OK:**
```json
{
  "success": true,
  "message": "Totales calculados correctamente",
  "data": {
    "subtotal": 100.00,
 "iva": 11.50,
    "total": 111.50
  }
}
```

**Ejemplo cURL:**
```bash
curl -X POST https://factura-production-279b.up.railway.app/api/facturas/calcular \
  -H "Content-Type: application/json" \
  -d '{
    "Subtotal": 100.00
  }'
```

---

### 5.2 Validar Estado de Factura

**Endpoint:**
```
GET /api/facturas/validar-estado/{estado}
```

**Parámetros:**
- `estado` (string, path, requerido): Estado a validar (Emitida, Pagada, Anulada)

**Response 200 OK:**
```json
{
  "success": true,
  "message": "Estado válido",
  "estado": "Pagada",
  "esValido": true,
  "estadosValidos": ["Emitida", "Pagada", "Anulada"]
}
```

**Ejemplo cURL:**
```bash
curl -X GET https://factura-production-279b.up.railway.app/api/facturas/validar-estado/Pagada
```

---

### 5.3 Información del Carrito

**Endpoint:**
```
GET /api/carrito/info
```

**Descripción:** Muestra información sobre dónde están los endpoints del carrito (delegados a ReservasService).

**Response 200 OK:**
```json
{
  "success": true,
  "mensaje": "Los endpoints de carrito están en ReservasService. Este servicio solo maneja facturación.",
  "endpoints": {
    "listarCarrito": "GET /api/carrito/usuario/{idUsuario} - En ReservasService",
    "generarFactura": "POST /api/facturas/generar-carrito - En FacturacionService",
  "marcarPagada": "POST /api/facturas/marcar-pagada - En FacturacionService"
  }
}
```

**Ejemplo cURL:**
```bash
curl -X GET https://factura-production-279b.up.railway.app/api/carrito/info
```

---

## ?? RESUMEN DE ENDPOINTS

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| **GET** | `/api/facturas` | Listar todas las facturas |
| **GET** | `/api/facturas/{id}` | Obtener factura por ID |
| **POST** | `/api/facturas` | Crear factura simple |
| **PUT** | `/api/facturas/{id}/anular` | Anular factura |
| **POST** | `/api/facturas/calcular` | Calcular totales sin guardar |
| **POST** | `/api/facturas/generar-carrito` | Generar factura desde carrito con promociones |
| **POST** | `/api/facturas/generar-confirmadas` | Generar factura de reservas confirmadas |
| **POST** | `/api/facturas/marcar-pagada` | Marcar factura como pagada |
| **GET** | `/api/facturas/detallada/{id}` | Obtener factura con detalles completos |
| **GET** | `/api/facturas/usuario/{id}` | Listar facturas de un usuario |
| **GET** | `/api/facturas/estadisticas/{idUsuario}` | Obtener estadísticas de facturación |
| **GET** | `/api/facturas/validar-estado/{estado}` | Validar estado de factura |
| **GET** | `/api/carrito/info` | Información sobre endpoints de carrito |

---

## ?? CONFIGURACIÓN

### IVA
- **Porcentaje:** 11.5% (0.115)
- **Cálculo:** `Total = Subtotal + (Subtotal * 0.115)`

### Estados de Factura
- **Emitida:** Factura creada pero no pagada
- **Pagada:** Factura confirmada y pagada
- **Anulada:** Factura cancelada

### Métodos de Pago Válidos
- `EFECTIVO`
- `TARJETA`
- `TRANSFERENCIA`

---

## ?? SERVICIOS RELACIONADOS

| Servicio | URL | Descripción |
|----------|-----|-------------|
| **ReservasService** | `https://reserva-production-279b.up.railway.app` | Gestión de carrito y reservas |
| **SeguridadService** | `https://seguridad-production-279b.up.railway.app` | Autenticación y usuarios |
| **MenuService** | `https://menu-production-279b.up.railway.app` | Platos y promociones |

---

## ?? NOTAS IMPORTANTES

1. **CORS:** Todos los endpoints tienen CORS habilitado con política `AllowAll`
2. **Formato de Fechas:** ISO 8601 (`yyyy-MM-ddTHH:mm:ss`)
3. **Formato Decimal:** 2 decimales (ej: `150.53`)
4. **Encoding:** UTF-8
5. **Timeout:** 60 segundos por defecto

---

## ?? COLECCIÓN POSTMAN

Para importar todos estos endpoints en Postman, usa el archivo:
```
CafeSanJuan-Railway.postman_collection.json
```

---

## ?? SOPORTE

**Autor:** Art0518  
**Repositorio:** https://github.com/Art0518/Restaurante  
**Documentación:** Ver `RAILWAY-API-ENDPOINTS.md` para endpoints completos de todos los servicios

---

**Última actualización:** Enero 2025
