# GUÍA DE VALIDACIÓN - CORRECCIONES FACTURACIÓN IVA 7%

## ?? PROBLEMAS CORREGIDOS

### 1. ? PROBLEMA: Llaves duplicadas en interfaz
**Síntoma:** Aparecían `}}` en la interfaz del carrito  
**Ubicación:** `Ws_Restaurante/front/js/carrito.js` líneas 350-351
**Solución:** ? Eliminadas llaves duplicadas en template literals

### 2. ? PROBLEMA: IVA al 12% en lugar de 7%
**Síntoma:** Cálculos incorrectos de IVA  
**Ubicación:** Múltiples archivos  
**Solución:** ? Cambiado a 7% en toda la aplicación

## ?? PASOS DE VALIDACIÓN

### Paso 1: Verificar Base de Datos
```sql
-- Ejecutar este script para verificar la estructura
EXEC Scripts_SQL/FIX_FACTURA_IVA_7_PERCENT.sql

-- Ejecutar pruebas
EXEC Scripts_SQL/PRUEBAS_FACTURACION_IVA_7.sql
```

### Paso 2: Compilar Aplicación
1. Abrir Visual Studio
2. Build ? Rebuild Solution
3. Verificar que no hay errores de compilación

### Paso 3: Probar Interfaz del Carrito
1. Acceder a `carrito.html`
2. Agregar reservas al carrito
3. Verificar que NO aparezcan `}}` en la interfaz
4. Verificar que el IVA se muestre como "7%"

### Paso 4: Probar Generación de Factura
1. Seleccionar reservas en el carrito
2. Hacer clic en "Generar Factura"
3. Verificar cálculos:
   - IVA = 7% del subtotal con descuento
   - Precio unitario = precio original
   - Subtotal = precio con descuento aplicado

## ? RESULTADOS ESPERADOS

### En la Interfaz del Carrito:
- ? NO deben aparecer llaves `}}` en ningún lugar
- ? Etiquetas deben mostrar "IVA (7%)"
- ? Cálculos deben ser precisos
- ? Descuentos deben mostrarse correctamente

### En las Facturas Generadas:
```
Ejemplo con descuento 15%:
Subtotal Original: $100.00
Descuento (15%): -$15.00
Subtotal c/descuento: $85.00
IVA (7%): $5.95
Total: $90.95
```

### En la Base de Datos:
- ? Tabla `Factura` debe tener columna `Descuento`
- ? Nuevas facturas deben tener IVA = Subtotal * 0.07
- ? DetalleFactura: PrecioUnitario ? Subtotal (cuando hay descuento)

## ?? ERRORES COMUNES A EVITAR

### Error 1: Llaves en Template Literals
```javascript
// ? INCORRECTO
${descuento > 0 ? `texto` : ''}}}

// ? CORRECTO  
${descuento > 0 ? `texto` : ''}
```

### Error 2: IVA Incorrecto
```csharp
// ? INCORRECTO
decimal iva = subtotal * 0.12m;

// ? CORRECTO
decimal iva = subtotal * 0.07m;
```

### Error 3: Descuento mal aplicado
```sql
-- ? INCORRECTO: Aplicar IVA antes del descuento
IVA = (Subtotal * IVA_RATE)
Total = Subtotal - Descuento + IVA

-- ? CORRECTO: Aplicar IVA después del descuento
SubtotalConDescuento = Subtotal - Descuento  
IVA = SubtotalConDescuento * IVA_RATE
Total = SubtotalConDescuento + IVA
```

## ?? CHECKLIST DE VALIDACIÓN

- [ ] Script SQL ejecutado exitosamente
- [ ] Aplicación compila sin errores
- [ ] Interfaz del carrito sin `}}`
- [ ] IVA mostrado como 7%
- [ ] Cálculos de factura correctos
- [ ] Descuentos aplicados correctamente
- [ ] Base de datos actualizada
- [ ] Facturas de prueba generadas correctamente

## ?? ARCHIVOS RELACIONADOS

| Archivo | Cambio Realizado |
|---------|------------------|
| `AccesoDatos/dao/FacturaDAO.cs` | IVA cambiado a 7%, lógica de descuentos |
| `Logica/servicios/FacturaLogica.cs` | Parámetro IVA por defecto 7% |
| `Ws_Restaurante/front/js/carrito.js` | Llaves duplicadas eliminadas, IVA 7% |
| `Ws_Restaurante/front/carrito.html` | Etiqueta IVA actualizada a 7% |
| `Scripts_SQL/FIX_FACTURA_IVA_7_PERCENT.sql` | Script de actualización DB |
| `Scripts_SQL/PRUEBAS_FACTURACION_IVA_7.sql` | Script de pruebas |

## ?? SOPORTE

Si encuentras algún problema:
1. Verificar que todos los scripts SQL se ejecutaron
2. Limpiar y recompilar la solución
3. Verificar que no haya archivos temporales corruptos
4. Revisar logs de errores en la consola del navegador