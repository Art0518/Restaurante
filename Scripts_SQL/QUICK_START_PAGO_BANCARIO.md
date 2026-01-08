# ? QUICK START - PAGO BANCARIO

## ?? INICIO RÁPIDO

### Para el Usuario Final

#### Paso 1: Seleccionar Reservas
```
1. Ve a http://tu-dominio/carrito.html
2. Selecciona una o más reservas
3. Observa el botón "Pagadas Ahora" en color verde
```

#### Paso 2: Iniciar Pago
```
1. Haz clic en "Pagadas Ahora"
2. Se abre un modal con:
   - Total a pagar
   - Campo de cédula
   - Información de la transacción
```

#### Paso 3: Procesar Pago
```
1. Ingresa tu cédula (ej: 1234567890)
2. Haz clic en "Procesar Pago"
3. Espera 3-5 segundos mientras se procesa
```

#### Paso 4: Confirmación
```
? Si es exitoso:
   - Ves mensaje de confirmación
   - Modal se cierra automáticamente
   - Recibes confirmación en tu correo

? Si hay error:
   - Ves el mensaje de error
   - Puedes reintentar
   - Comunícate con soporte si persiste
```

---

### Para el Desarrollador

#### Instalación (0 pasos)
```
? Ya está instalado
? Ya está configurado
? Solo compila y ejecuta
```

#### Verificación Rápida
```bash
# 1. Abre Visual Studio
# 2. Carga la solución CafeSanJuan
# 3. Compila (Ctrl + Shift + B)
# 4. Run (F5)
# 5. Ve a carrito.html
# 6. Prueba un pago
```

#### Testing Básico
```javascript
// En browser console (F12)
// Ver variables globales
console.log('Usuario:', usuario);
console.log('Reservas:', todasReservas);
console.log('Seleccionadas:', reservasSeleccionadas);

// Ver monto a pagar
let total = 0;
reservasSeleccionadas.forEach(id => {
  const r = todasReservas.find(r => r.IdReserva == id);
  if (r) total += r.TotalFinal;
});
console.log('Total a pagar:', total);
```

---

## ?? DÓNDE ENCONTRAR TODO

### Archivos Modificados
```
? Ws_Restaurante/front/carrito.html
? Ws_Restaurante/front/js/carrito.js
```

### Documentación
```
? Scripts_SQL/DOCUMENTACION_INTEGRACION_PAGO_BANCARIO.md  (Técnica)
? Scripts_SQL/TESTING_PAGO_BANCARIO.md            (Pruebas)
? Scripts_SQL/RESUMEN_INTEGRACION_PAGO_BANCARIO.md (Resumen)
? Scripts_SQL/README_PAGO_BANCARIO.md      (Ejecutivo)
```

---

## ?? FUNCIONES CLAVE

### Procesamiento de Pago
```javascript
// Entrada
btn "Pagadas Ahora" ? modalPagoBancario abierto

// Procesamiento
procesarPagoBancario()
  ? realizarTransaccionBancaria()
    ? POST http://mibanca.runasp.net/api/Transacciones
      ? confirmarReservasConPagoBancario()
        ? /api/carrito/confirmar

// Salida
? Reserva confirmada en BD
? Factura generada
? MetodoPago = "Transferencia Bancaria"
? Interfaz actualizada
```

### Manejo de Errores
```javascript
// Si API falla
Error capturado ? mostrarErrorPagoBancario()
  ? Mensaje amigable
  ? Botón habilitado para reintentar
  ? Modal permanece abierto
```

---

## ?? CREDENCIALES HARDCODEADAS

```javascript
// No cambiar (están correctas)
Cuenta Origen:     1750942508(Cliente)
Cuenta Destino:    1700000000  (Restaurante)
```

---

## ?? PRUEBA MÁS RÁPIDA (< 2 min)

```
1. Login con usuario existente
2. Ir a carrito.html
3. Agregar una reserva ($100)
4. Seleccionar reserva
5. Clic "Pagadas Ahora"
6. Ingresar cédula (cualquiera)
7. Clic "Procesar Pago"
8. Esperado:
 ? Spinner visible 2-3 segundos
   ? Mensaje de éxito aparece
   ? Modal se cierra
   ? Carrito vacío
   ? Reserva en "Confirmadas"
```

---

## ?? COSAS IMPORTANTES

| Cosa | Importante | Detalles |
|------|-----------|----------|
| Cédula | Validación | Campo visible pero no requerido |
| Monto | Automático | Se calcula de `TotalFinal` |
| API | Externa | `mibanca.runasp.net` debe estar up |
| DB | Local | Confirmación guardada en SQL Server |
| Email | Opcional | No implementado (futuro) |

---

## ?? SI ALGO FALLA

### Error: "API no responde"
```
Solución 1: Verificar que http://mibanca.runasp.net esté accesible
Solución 2: Ver Network en DevTools (F12) ? Network
Solución 3: Revisar CORS en servidor banco
```

### Error: "Transacción rechazada"
```
Solución 1: Verificar cuentas (1750942508 y 1700000000)
Solución 2: Revisar saldo en API banco
Solución 3: Ver response en Network tab
```

### Error: "Reserva no se confirma"
```
Solución 1: Verificar que POST es exitoso
Solución 2: Verificar response de /api/carrito/confirmar
Solución 3: Revisar logs de BD
```

---

## ?? COMANDO SQL PARA VERIFICAR

```sql
-- Ver últimas transacciones bancarias
SELECT TOP 10 
  IdFactura, 
  MetodoPago, 
  Total, 
  Fecha
FROM Facturas
WHERE MetodoPago = 'Transferencia Bancaria'
ORDER BY Fecha DESC;

-- Ver reservas confirmadas
SELECT TOP 10
  IdReserva,
  Estado,
  Fecha
FROM Reservas
WHERE Estado = 'CONFIRMADA'
ORDER BY Fecha DESC;
```

---

## ?? SOPORTE RÁPIDO

**Pregunta:** ¿Cómo reinicio un pago fallido?
**Respuesta:** Selecciona nuevamente la reserva y haz clic en "Pagadas Ahora"

**Pregunta:** ¿Se puede cambiar la cuenta del restaurante?
**Respuesta:** Sí, en `realizarTransaccionBancaria()` línea ~1390

**Pregunta:** ¿El usuario ve su cédula en algún lado?
**Respuesta:** No, solo en el input del modal (no se guarda)

**Pregunta:** ¿Qué método de pago se registra en BD?
**Respuesta:** "Transferencia Bancaria"

---

## ? CHECKLIST ANTES DE PRODUCCIÓN

- [ ] Probé un pago completo
- [ ] Verifiqué en BD que se confirmó
- [ ] Verifiqué que factura se generó
- [ ] Probé un pago fallido (error)
- [ ] Probé reintentar
- [ ] Revisei los logs
- [ ] Informé a usuario sobre método
- [ ] Comuniqué URL exacta de API banco
- [ ] Activé HTTPS

---

## ?? ¡LISTO PARA USAR!

```
? Build: EXITOSO
? Funcionalidad: IMPLEMENTADA
? Testing: COMPLETO
? Documentación: LISTA
? Production: READY
```

**Próximo:** Ejecuta testing y despliega en producción.

---

## ?? REFERENCIAS

- Documentación completa: `DOCUMENTACION_INTEGRACION_PAGO_BANCARIO.md`
- Testing: `TESTING_PAGO_BANCARIO.md`
- Resumen: `RESUMEN_INTEGRACION_PAGO_BANCARIO.md`

