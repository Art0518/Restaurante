# ?? GUÍA DE TESTING - SISTEMA DE PAGO BANCARIO

## CHECKLIST DE VALIDACIÓN

### 1. INTERFAZ DE USUARIO

- [ ] Botón cambiado a "Pagadas Ahora" 
- [ ] Icono de tarjeta de crédito visible
- [ ] Modal de pago abre correctamente
- [ ] Campo de cédula visible y funcional
- [ ] Monto total se calcula correctamente

### 2. VALIDACIONES

**Prueba 1: Sin reservas seleccionadas**
```
Paso 1: Ir a carrito
Paso 2: No seleccionar ninguna reserva
Paso 3: Clic en "Pagadas Ahora"
Esperado: Mensaje "Selecciona al menos una reserva para pagar"
```

**Prueba 2: Sin ingresar cédula**
```
Paso 1: Seleccionar una reserva
Paso 2: Clic en "Pagadas Ahora"
Paso 3: No ingresar cédula
Paso 4: Clic en "Procesar Pago"
Esperado: Mensaje "Por favor ingresa tu cédula"
```

### 3. CÁLCULO DE MONTO

**Prueba 3: Monto correcto**
```
Paso 1: Carrito con reserva de $100.00
Paso 2: Seleccionar reserva
Paso 3: Clic en "Pagadas Ahora"
Esperado: Modal muestre "$100.00" en "Total a pagar"
```

**Prueba 4: Múltiples reservas**
```
Paso 1: Carrito con 2 reservas: $50 + $75
Paso 2: Seleccionar ambas
Paso 3: Clic en "Pagadas Ahora"
Esperado: Modal muestre "$125.00"
```

### 4. TRANSACCIÓN BANCARIA

**Prueba 5: Pago exitoso**
```
Paso 1: Preparar datos de prueba
  - Cédula cliente: (cualquiera)
  - Monto: (cualquiera)
  
Paso 2: Seleccionar reserva con $100.00
Paso 3: Clic en "Pagadas Ahora"
Paso 4: Ingresar cédula "1234567890"
Paso 5: Clic en "Procesar Pago"

Esperado:
? Spinner carga visible
? Conecta a http://mibanca.runasp.net/api/Transacciones
? POST con datos correctos
? Respuesta exitosa recibida
? Modal se cierra automáticamente
? Mensaje de éxito mostrado
? Reserva desaparece del carrito
? Aparece en "Reservas Confirmadas"
```

**Prueba 6: Pago fallido**
```
Paso 1: Simular error en API (opcional con dev tools)
Paso 2: Seleccionar reserva
Paso 3: Clic en "Pagadas Ahora"
Paso 4: Ingresar cédula
Paso 5: Clic en "Procesar Pago"

Esperado:
? Mensaje de error mostrado
? Usuario puede reintentar
? Modal permanece abierto
? Reserva NO se confirma
```

### 5. DATOS ENVIADOS AL API

**Prueba 7: Verificar datos correctos (con Network en DevTools)**
```
URL: http://mibanca.runasp.net/api/Transacciones
Método: POST

Headers esperados:
{
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}

Body esperado:
{
  "cuenta_origen": "1750942508",
  "cuenta_destino": "1700000000",
  "monto": 100.00
}
```

### 6. BASE DE DATOS

**Prueba 8: Registro en BD después de pago**
```
Paso 1: Ejecutar pago exitoso
Paso 2: Consultar:
  SELECT * FROM Reservas 
  WHERE Estado = 'CONFIRMADA' AND IdUsuario = {usuario_id}
  
Esperado:
? Nueva reserva confirmada
? MetodoPago = 'Transferencia Bancaria'
? Fecha de confirmación = HOY
? Total = monto enviado al banco
```

**Prueba 9: Factura generada**
```
Paso 1: Después del pago, ejecutar:
  SELECT * FROM Facturas 
  WHERE IdUsuario = {usuario_id}
  ORDER BY Fecha DESC
  
Esperado:
? Nueva factura creada
? Estado = 'Pagada'
? MetodoPago = 'Transferencia Bancaria'
? Detalles incluyen la(s) reserva(s)
```

### 7. FLUJO COMPLETO

**Prueba 10: Flujo end-to-end**
```
1. Login como usuario
2. Agregar reserva al carrito
3. Aplicar promoción (si aplica)
4. Seleccionar reservas
5. Clic "Pagadas Ahora"
6. Ingresar cédula
7. Procesar pago
8. Verificar:
   - Modal se cierra
   - Mensaje de éxito
   - Carrito vacío o actualizado
   - Reserva en "Confirmadas"
   - Factura generada en BD
   - MetodoPago registrado correctamente
```

---

## ?? HERRAMIENTAS DE DEBUGGING

### Activar Console Logging
Agregar en el navegador (F12 ? Console):
```javascript
// Ver todas las llamadas bancarias
console.log('Transacción:', {
  origen: "1750942508",
  destino: "1700000000",
  monto: totalAPagar
});
```

### Network Tab (DevTools)
```
1. Abre DevTools (F12)
2. Ve a Tab "Network"
3. Filtra por "Transacciones"
4. Procesa el pago
5. Verifica Request y Response
```

### Errores Comunes

| Error | Causa | Solución |
|-------|-------|----------|
| CORS | API no acepta origen | Verificar CORS en API banco |
| 404 | URL incorrecta | Verificar endpoint exacto |
| 400 | Datos inválidos | Verificar formato JSON |
| No cierra modal | JS error | Ver console para excepciones |
| No confirma reserva | Error en API carrito | Verificar respuesta de `/api/carrito/confirmar` |

---

## ?? SCRIPT SQL PARA VERIFICACIÓN

```sql
-- Reservas confirmadas con método bancario
SELECT 
  R.IdReserva,
  R.IdUsuario,
  R.Fecha,
  R.Estado,
  F.MetodoPago,
  F.Total,
  F.Fecha as FechaFactura
FROM Reservas R
LEFT JOIN Facturas F ON R.IdReserva = F.IdReserva
WHERE F.MetodoPago = 'Transferencia Bancaria'
ORDER BY F.Fecha DESC;

-- Últimas transacciones
SELECT TOP 10
  IdFactura,
  IdUsuario,
  Estado,
  MetodoPago,
  Total,
  Fecha
FROM Facturas
ORDER BY Fecha DESC;
```

---

## ?? CASOS EDGE

**Caso 1: Conexión perdida**
```
Acción: Desconectar internet durante transacción
Esperado: Error capturado, usuario informado
```

**Caso 2: Saldo insuficiente en API banco**
```
Acción: API responde con error de saldo
Esperado: Mensaje amigable, permitir reintentar
```

**Caso 3: Reserva cancelada mientras se procesa pago**
```
Acción: Otro usuario cancela reserva durante POST
Esperado: Manejo gracioso, no confirmar si no existe
```

**Caso 4: Datos duplicados**
```
Acción: Procesar pago 2 veces rápidamente
Esperado: Segunda transacción rechazada o procesada diferente
```

---

## ?? CHECKLIST FINAL

- [ ] Código compila sin errores
- [ ] Modal abre y cierra correctamente
- [ ] Validaciones funcionan
- [ ] Monto se calcula correctamente
- [ ] API recibe datos correctos
- [ ] Respuesta se maneja (éxito y error)
- [ ] Reservas se confirman en BD
- [ ] Factura se genera
- [ ] Método de pago se registra
- [ ] Interfaz se actualiza
- [ ] Mensajes son claros
- [ ] No hay console errors

---

## ? APROBACIÓN

Una vez pasen todas las pruebas, marcar como completado:

```
Fecha de Testing: _______________
Tester: _______________
Resultado: ? APROBADO / ? RECHAZADO

Firma: _______________
```

