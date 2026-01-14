# ReservasService

Servicio de gestión de reservas para el Restaurante Café San Juan.

## Última actualización
- 2024-01-15: Corrección de esquemas en sp_listar_carrito_reservas

## Características
- Gestión de reservas
- Carrito de reservas
- Promociones aplicables
- Integración con facturación

## Endpoints principales
- GET /api/carrito/usuario/{id}
- POST /api/carrito/confirmar
- DELETE /api/carrito/eliminar/{idUsuario}/{idReserva}
