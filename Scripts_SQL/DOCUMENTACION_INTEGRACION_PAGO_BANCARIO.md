# DOCUMENTACIÓN - INTEGRACIÓN DE PAGO BANCARIO EN CARRITO

## ?? RESUMEN DE CAMBIOS

Se ha integrado exitosamente el sistema de pagos bancarios en el carrito de reservas. El botón "Confirmar Seleccionadas" ha sido reemplazado por "Pagadas Ahora" para iniciar una transacción bancaria.

---

## ?? CAMBIOS REALIZADOS

### 1. **MODIFICACIONES EN HTML** (`carrito.html`)

#### Cambio del Botón Principal
**Antes:**
```html
<button id="btn-confirmar-carrito" class="btn btn-success" disabled>
  <i class="bi bi-check-circle"></i> Confirmar Seleccionadas
</button>
```

**Después:**
```html
<button id="btn-confirmar-carrito" class="btn btn-success" disabled>
  <i class="bi bi-credit-card"></i> Pagadas Ahora
</button>
```

#### Nuevo Modal de Pago Bancario
Se añadió un nuevo modal `modalPagoBancario` con los siguientes campos:
- **Monto a pagar:** Se calcula automáticamente desde las reservas seleccionadas
- **Campo de cédula:** Se deja espacio para validar aunque se usan cédulas fijas
- **Información de transacción:** Aviso sobre la transferencia bancaria
- **Estado de procesamiento:** Spinner y mensajes de progreso

### 2. **MODIFICACIONES EN JAVASCRIPT** (`carrito.js`)

#### Event Listener Actualizado
El evento del botón `btn-confirmar-carrito` ahora:
1. Valida que haya reservas seleccionadas
2. Calcula el total a pagar
3. Abre el modal de pago bancario
4. Muestra el total en el modal

#### Nuevas Funciones Agregadas

##### `procesarPagoBancario()`
- Valida la cédula ingresada
- Calcula el monto total
- Muestra estado de procesamiento
- Llama a `realizarTransaccionBancaria()`

##### `realizarTransaccionBancaria(cedulaCliente, monto)`
- Realiza una petición POST al API bancario
- URL: `http://mibanca.runasp.net/api/Transacciones`
- **Cuentas utilizadas:**
  - Cuenta Cliente: `1750942508` (origen)
  - Cuenta Restaurante: `1700000000` (destino)
- Maneja respuestas JSON y texto
- Retorna objeto con estatus de transacción

##### `confirmarReservasConPagoBancario(resultadoPago)`
- Confirma las reservas después del pago exitoso
- Establece método de pago como "Transferencia Bancaria"
- Recarga el carrito y reservas confirmadas
- Muestra mensajes de éxito
- Cierra el modal automáticamente

##### `mostrarErrorPagoBancario(mensaje)`
- Muestra error en interfaz amigable
- Permite reintentar el pago
- Desactiva el spinner de carga

---

## ?? CONFIGURACIÓN DE CUENTAS

```javascript
CUENTA_CLIENTE = "1750942508"      // Cédula del cliente (origen)
CUENTA_RESTAURANTE = "1700000000"  // Cuenta del restaurante (destino)
```

Estas cuentas están hardcodeadas en las funciones de pago bancario y **no requieren entrada del usuario**.

---

## ?? FLUJO DE TRANSACCIÓN

```
1. Usuario selecciona reservas y hace clic en "Pagadas Ahora"
   ?
2. Modal de pago bancario se abre
   ?
3. Usuario ingresa cédula (validación)
   ?
4. Clic en "Procesar Pago"
   ?
5. Llamada a API bancario (POST /api/Transacciones)
   ?
6. Si éxito ? Confirmar reservas en BD
Si error ? Mostrar error y permitir reintentar
   ?
7. Actualizar interfaz y recargar carrito
```

---

## ?? ENDPOINT BANCARIO

**URL:** `http://mibanca.runasp.net/api/Transacciones`

**Método:** POST

**Headers:**
```javascript
{
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}
```

**Payload:**
```javascript
{
  "cuenta_origen": "1750942508",
  "cuenta_destino": "1700000000",
  "monto": 150.00
}
```

**Respuesta Éxito (200):**
```javascript
{
  "ok": true,
  "success": true,
  "mensaje": "Transacción realizada correctamente",
  "monto": 150.00
}
```

**Respuesta Error:**
```javascript
{
  "ok": false,
  "success": false,
  "mensaje": "Descripción del error"
}
```

---

## ? CARACTERÍSTICAS

? **Validación de cédula** - Campo disponible para validación futura  
? **Cálculo automático del monto** - Suma todos los totales finales  
? **Manejo de errores** - Respuestas en JSON y texto  
? **Mensajes amigables** - Feedback visual al usuario  
? **Spinner de carga** - Indica procesamiento en curso  
? **Confirmación automática** - Las reservas se confirman tras pago exitoso  
? **Método de pago registrado** - Se guarda como "Transferencia Bancaria"  

---

## ?? PRUEBAS RECOMENDADAS

1. **Transacción exitosa**
   - Seleccionar una reserva
   - Hacer clic en "Pagadas Ahora"
   - Ingresar cédula
   - Procesar pago

2. **Validación de cédula**
   - Intentar procesar sin ingresar cédula
   - Verificar mensaje de validación

3. **Error de transacción**
   - Simular error de API
   - Verificar que permite reintentar

4. **Actualización de interfaz**
   - Verificar que las reservas desaparecen del carrito
   - Verificar que aparecen en "Reservas Confirmadas"

---

## ?? ESTRUCTURA DE CÓDIGO

### Sección de Pago Bancario en `carrito.js`

```
procesarPagoBancario()
    ?
realizarTransaccionBancaria()
    ? (si éxito)
confirmarReservasConPagoBancario()
 ?
cargarCarritoUsuario()
cargarReservasConfirmadas()

    ? (si error)
mostrarErrorPagoBancario()
```

---

## ?? NOTAS IMPORTANTES

1. **Cuentas fijas:** Las cuentas del cliente y restaurante están hardcodeadas, no se solicitan al usuario.

2. **Integración con BD:** Después de confirmar el pago, se registra automáticamente con método "Transferencia Bancaria".

3. **IVA aplicado:** El monto enviado al banco incluye el IVA ya calculado (7%).

4. **Promociones:** Si hay promoción aplicada, el descuento ya está incluido en el total final.

5. **Manejo de sesión:** Requiere que el usuario esté autenticado (verificado al inicio de `carrito.js`).

---

## ?? PRÓXIMAS MEJORAS

- [ ] Historial de transacciones bancarias
- [ ] Reintentos automáticos en caso de error temporal
- [ ] Notificaciones por correo de confirmación
- [ ] Integración con comprobante de transferencia
- [ ] Validación de saldo disponible antes de procesar

