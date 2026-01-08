# ?? ENDPOINTS DE MICROSERVICIOS - RAILWAY
# CafeSanJuan - Proyecto Restaurante
# Autor: Art0518
# Fecha: Diciembre 2024

## ?? ÍNDICE
1. SeguridadService - Usuarios y Autenticación
2. MenuService - Platos y Promociones
3. ReservasService - Reservas y Mesas
4. FacturacionService - Facturas y Carritos

---

# ?? 1. SEGURIDAD SERVICE
**Base URL:** https://seguridad-production-279b.up.railway.app

## ? Health Check
GET {{base_seguridad}}/api/usuarios

## ?? Registrar Usuario
POST {{base_seguridad}}/api/usuarios/registrar
Content-Type: application/json

{
  "nombre": "María",
  "apellido": "González",
  "email": "maria.gonzalez@example.com",
  "contrasena": "Password123!",
  "telefono": "7871234567",
  "cedula": "0987654321",
  "direccion": "Av. San Juan 456, San Juan, PR",
  "rol": "Usuario"
}

## ?? Login
POST {{base_seguridad}}/api/usuarios/login
Content-Type: application/json

{
  "email": "maria.gonzalez@example.com",
  "contrasena": "Password123!"
}

## ?? Obtener Usuario por ID
GET {{base_seguridad}}/api/usuarios/1

## ?? Listar Usuarios (con paginación)
GET {{base_seguridad}}/api/usuarios/listar?pagina=1&tamanoPagina=10

## ?? Filtrar Usuarios por Rol
GET {{base_seguridad}}/api/usuarios/listar?rol=Administrador

## ?? Filtrar Usuarios por Estado
GET {{base_seguridad}}/api/usuarios/listar?estado=ACTIVO

## ?? Actualizar Usuario
PUT {{base_seguridad}}/api/usuarios/1
Content-Type: application/json

{
  "nombre": "Juan Carlos Pérez",
  "email": "juancarlos@example.com",
  "telefono": "0987654321",
  "cedula": "1234567890",
  "direccion": "Nueva Dirección 456",
  "rol": "Administrador"
}

## ?? Cambiar Contraseña
POST {{base_seguridad}}/api/usuarios/1/cambiar-contrasena
Content-Type: application/json

{
  "contrasenaActual": "Password123!",
  "nuevaContrasena": "NuevoPassword456!"
}

## ?? Validar Token JWT
POST {{base_seguridad}}/api/usuarios/validar-token
Content-Type: application/json

{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}

## ??? Eliminar Usuario
DELETE {{base_seguridad}}/api/usuarios/1

---

# ??? 2. MENU SERVICE
**Base URL:** https://menu-production-279b.up.railway.app

## ? Listar Todos los Platos
GET {{base_menu}}/api/platos/listar

## ??? Obtener Plato por ID
GET {{base_menu}}/api/platos/1

## ? Crear Plato
POST {{base_menu}}/api/platos/crear
Content-Type: application/json

{
  "idRestaurante": 2,
  "nombre": "Mofongo Relleno de Camarones",
  "descripcion": "Delicioso mofongo tradicional puertorriqueño relleno de camarones al ajillo",
  "precio": 18.99,
  "categoria": "Platos Principales",
  "tipoComida": "Criolla",
  "imagenURL": "https://example.com/mofongo.jpg",
  "stock": 25,
  "estado": "ACTIVO"
}

## ?? Actualizar Plato
PUT {{base_menu}}/api/platos/1
Content-Type: application/json

{
  "nombre": "Mofongo Premium",
  "descripcion": "Mofongo mejorado",
  "precio": 22.99,
  "categoria": "Platos Principales",
  "tipoComida": "Criolla",
  "imagenURL": "https://example.com/mofongo-premium.jpg",
  "stock": 20,
  "estado": "ACTIVO"
}

## ??? Eliminar Plato
DELETE {{base_menu}}/api/platos/1

---

## ?? PROMOCIONES

## ? Listar Promociones Activas
GET {{base_menu}}/api/promociones/activas

## ?? Listar Todas las Promociones
GET {{base_menu}}/api/promociones/listar

## ?? Listar Promociones por Estado
GET {{base_menu}}/api/promociones/por-estado/Activa

## ??? Obtener Promoción por ID
GET {{base_menu}}/api/promociones/1

## ? Crear Promoción
POST {{base_menu}}/api/promociones/crear
Content-Type: application/json

{
  "idRestaurante": 2,
  "nombre": "Descuento Black Friday",
  "descripcion": "20% de descuento en todos los platos",
  "porcentajeDescuento": 20,
  "fechaInicio": "2024-11-25",
  "fechaFin": "2024-11-30",
  "activo": true
}

---

# ?? 3. RESERVAS SERVICE
**Base URL:** https://reserva-production-279b.up.railway.app

## ? Listar Todas las Reservas
GET {{base_reservas}}/api/reservas

## ? Crear Reserva
POST {{base_reservas}}/api/reservas/crear
Content-Type: application/json

{
  "idUsuario": 1,
  "idMesa": 5,
  "idRestaurante": 2,
  "fechaReserva": "2024-12-25",
  "horaReserva": "19:00",
  "numeroPersonas": 4,
  "notas": "Mesa cerca de la ventana por favor",
  "estado": "HOLD",
  "metodoPago": "TARJETA"
}

## ??? Obtener Reserva por ID
GET {{base_reservas}}/api/reservas/1

## ?? Filtrar Reservas por Usuario
GET {{base_reservas}}/api/reservas/filtrar?idUsuario=1

## ?? Filtrar Reservas por Estado
GET {{base_reservas}}/api/reservas/filtrar?estado=CONFIRMADA

## ?? Filtrar Reservas por Fecha
GET {{base_reservas}}/api/reservas/filtrar?fecha=2024-12-25

## ?? Listar Reservas Confirmadas de Usuario
GET {{base_reservas}}/api/reservas/confirmadas/1

## ?? Listar Todas las Reservas (Admin)
GET {{base_reservas}}/api/reservas/admin/todas

## ?? Actualizar Estado de Reserva
PUT {{base_reservas}}/api/reservas/estado
Content-Type: application/json

{
  "idReserva": 1,
  "estado": "CONFIRMADA",
  "metodoPago": "TARJETA"
}

## ?? Editar Reserva
PUT {{base_reservas}}/api/reservas/editar
Content-Type: application/json

{
  "idReserva": 1,
  "fechaReserva": "2024-12-26",
  "horaReserva": "20:00",
  "numeroPersonas": 6,
  "notas": "Celebración de cumpleaños"
}

## ?? Generar Factura desde Admin
POST {{base_reservas}}/api/reservas/admin/generar-factura
Content-Type: application/json

{
  "idReserva": 1,
  "metodoPago": "EFECTIVO",
  "tipoFactura": "ADMIN"
}

---

## ?? RESTAURANTES Y MESAS

## ?? Listar Restaurantes
GET {{base_reservas}}/api/reservas/restaurantes

## ?? Listar Mesas por Restaurante
GET {{base_reservas}}/api/reservas/mesas/2

---

# ?? 4. FACTURACION SERVICE
**Base URL:** https://factura-production-279b.up.railway.app

## ? Listar Todas las Facturas
GET {{base_facturacion}}/api/facturacion/listar

## ??? Obtener Factura por ID
GET {{base_facturacion}}/api/facturacion/1

## ??? Obtener Factura Detallada
GET {{base_facturacion}}/api/facturacion/1/detallada

## ?? Listar Facturas de Usuario
GET {{base_facturacion}}/api/facturacion/usuario/1

## ? Generar Factura Básica
POST {{base_facturacion}}/api/facturacion/generar
Content-Type: application/json

{
  "idUsuario": 1,
  "idReserva": 1,
  "subtotal": 100.00,
  "metodoPago": "TARJETA"
}

## ?? Generar Factura desde Carrito
POST {{base_facturacion}}/api/facturacion/generar-carrito
Content-Type: application/json

{
  "idUsuario": 1,
  "reservasIds": "1,2,3",
  "promocionId": 1,
  "metodoPago": "TARJETA"
}

## ? Generar Factura de Reservas Confirmadas
POST {{base_facturacion}}/api/facturacion/generar-confirmadas
Content-Type: application/json

{
  "idUsuario": 1,
  "reservasIds": "1,2",
  "tipoFactura": "CONFIRMADA"
}

## ?? Marcar Factura como Pagada
POST {{base_facturacion}}/api/facturacion/marcar-pagada
Content-Type: application/json

{
  "idFactura": 1,
  "metodoPago": "EFECTIVO",
  "fechaPago": "2024-12-20"
}

## ?? Marcar Factura como Pagada (Legacy)
PUT {{base_facturacion}}/api/facturacion/1/marcar-pagada
Content-Type: application/json

{
  "idFactura": 1,
  "metodoPago": "TRANSFERENCIA"
}

## ? Anular Factura
PUT {{base_facturacion}}/api/facturacion/1/anular
Content-Type: application/json

{
  "motivo": "Cliente solicitó cancelación"
}

## ?? Calcular Totales
POST {{base_facturacion}}/api/facturacion/calcular-totales
Content-Type: application/json

{
  "factura": {
    "subtotal": 150.00
  },
  "porcentajeIva": 0.115
}

---

# ?? GRAPHQL ENDPOINTS

## SeguridadService GraphQL
POST https://seguridad-production-279b.up.railway.app/graphql

## MenuService GraphQL
POST https://menu-production-279b.up.railway.app/graphql

## ReservasService GraphQL
POST https://reserva-production-279b.up.railway.app/graphql

## FacturacionService GraphQL
POST https://factura-production-279b.up.railway.app/graphql

---

# ?? VARIABLES DE ENTORNO PARA POSTMAN

```json
{
  "base_seguridad": "https://seguridad-production-279b.up.railway.app",
  "base_menu": "https://menu-production-279b.up.railway.app",
  "base_reservas": "https://reserva-production-279b.up.railway.app",
  "base_facturacion": "https://factura-production-279b.up.railway.app"
}
```

---

# ?? NOTAS IMPORTANTES

## Códigos de Estado HTTP
- **200 OK**: Solicitud exitosa
- **201 Created**: Recurso creado exitosamente
- **400 Bad Request**: Error en la solicitud (validación)
- **401 Unauthorized**: No autenticado o token inválido
- **404 Not Found**: Recurso no encontrado
- **500 Internal Server Error**: Error del servidor

## Headers Comunes
```
Content-Type: application/json
Authorization: Bearer {token}  // Para endpoints protegidos
```

## Formato de Fechas
- **Fecha**: `yyyy-MM-dd` (Ejemplo: `2024-12-25`)
- **Hora**: `HH:mm` (Ejemplo: `19:00`)
- **DateTime**: `yyyy-MM-ddTHH:mm:ss` (Ejemplo: `2024-12-25T19:00:00`)

## Estados de Reserva
- `HOLD`: Reserva temporal (en carrito)
- `CONFIRMADA`: Reserva confirmada por el usuario
- `PAGADA`: Reserva pagada
- `COMPLETADA`: Reserva utilizada
- `CANCELADA`: Reserva cancelada

## Métodos de Pago
- `EFECTIVO`
- `TARJETA`
- `TRANSFERENCIA`

## Roles de Usuario
- `Usuario`: Cliente normal
- `Administrador`: Acceso completo
- `Empleado`: Acceso limitado

---

# ?? PRUEBAS RÁPIDAS

## 1. Health Check de Todos los Servicios
```bash
curl https://seguridad-production-279b.up.railway.app/api/usuarios
curl https://menu-production-279b.up.railway.app/api/platos/listar
curl https://reserva-production-279b.up.railway.app/api/reservas
curl https://factura-production-279b.up.railway.app/api/facturacion/listar
```

## 2. Flujo Completo de Reserva
1. **Registrar usuario** ? `POST /api/usuarios/registrar`
2. **Login** ? `POST /api/usuarios/login` (obtener token)
3. **Ver platos** ? `GET /api/platos/listar`
4. **Crear reserva** ? `POST /api/reservas/crear`
5. **Generar factura** ? `POST /api/facturacion/generar-carrito`

---

# ?? ENDPOINTS MÁS USADOS

1. **Login**: `POST /api/usuarios/login`
2. **Listar Platos**: `GET /api/platos/listar`
3. **Crear Reserva**: `POST /api/reservas/crear`
4. **Listar Mis Reservas**: `GET /api/reservas/confirmadas/{idUsuario}`
5. **Generar Factura**: `POST /api/facturacion/generar-carrito`

---

**Última actualización**: Diciembre 2024
**Versión**: 1.0
**Mantenedor**: Art0518
**Repositorio**: https://github.com/Art0518/Restaurante
