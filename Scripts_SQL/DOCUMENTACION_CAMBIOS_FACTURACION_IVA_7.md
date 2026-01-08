# DOCUMENTACIÓN - CAMBIOS EN FACTURACIÓN: IVA AL 7% Y MANEJO DE DESCUENTOS

## ?? CAMBIOS REALIZADOS

### 1. IVA CAMBIADO DEL 12% AL 7%

**Archivos modificados:**
- `AccesoDatos/dao/FacturaDAO.cs` - Línea del cálculo: `decimal iva = subtotal * 0.07m;`
- `Logica/servicios/FacturaLogica.cs` - Parámetro por defecto: `decimal porcentajeIVA = 0.07m`
- `Ws_Restaurante/front/js/carrito.js` - Etiqueta: `IVA (7%)`
- `Ws_Restaurante/front/carrito.html` - Etiqueta: `IVA (7%)`

### 2. MANEJO CORRECTO DE PRECIOS EN DETALLE DE FACTURA

**Lógica implementada:**
- **Precio Unitario**: Sin descuento (precio original de la reserva)
- **Subtotal**: Con descuento aplicado (precio original - descuento)
- **IVA**: Calculado sobre el subtotal con descuento al 7%
- **Total**: Subtotal con descuento + IVA

### 3. COLUMNA DESCUENTO EN TABLA FACTURA

Se agregó la columna `Descuento` para almacenar el monto total de descuento aplicado en la factura.

**Script creado:**
- `Scripts_SQL/FIX_FACTURA_IVA_7_PERCENT.sql`

## ?? FLUJO DE FACTURACIÓN ACTUALIZADO

### Cuando se genera una factura:

1. **Estado inicial**: "Emitida"
2. **Cálculos**:
   ```
   Subtotal Bruto = Suma de precios originales
   Descuento = Subtotal Bruto * (% promoción / 100)
   Subtotal = Subtotal Bruto - Descuento
   IVA = Subtotal * 0.07 (7%)
   Total = Subtotal + IVA
   ```

3. **Datos guardados**:
   - **Factura**: Subtotal, IVA, Total, Descuento, Estado="Emitida"
- **DetalleFactura**: PrecioUnitario (sin descuento), Subtotal (con descuento)

### Cuando se marca como pagada:

1. **Estado cambia a**: "Pagada"
2. **Se actualiza**: Método de pago en reservas
3. **Reservas mantienen estado**: "CONFIRMADA"

## ?? EJEMPLOS DE CÁLCULO

### Ejemplo con descuento:
```
Reserva original: $100.00
Promoción: 15% de descuento

Precio Unitario (sin descuento): $100.00
Descuento: $100.00 * 15% = $15.00
Subtotal (con descuento): $100.00 - $15.00 = $85.00
IVA (7%): $85.00 * 7% = $5.95
Total: $85.00 + $5.95 = $90.95
```

### Ejemplo sin descuento:
```
Reserva original: $100.00
Sin promoción

Precio Unitario: $100.00
Descuento: $0.00
Subtotal: $100.00
IVA (7%): $100.00 * 7% = $7.00
Total: $100.00 + $7.00 = $107.00
```

## ?? ESTADOS DE FACTURA

- **Emitida**: Factura generada pero no pagada
- **Pagada**: Factura pagada y confirmada
- **Anulada**: Factura cancelada

## ? FUNCIONALIDADES VERIFICADAS

- ? Generación de factura con IVA al 7%
- ? Aplicación correcta de descuentos
- ? Manejo de precios unitarios sin descuento
- ? Subtotales con descuento aplicado
- ? Interfaz actualizada con etiquetas de IVA al 7%
- ? Descarga de factura con información correcta

## ?? ARCHIVOS PRINCIPALES AFECTADOS

1. `AccesoDatos/dao/FacturaDAO.cs`
2. `Logica/servicios/FacturaLogica.cs`
3. `Ws_Restaurante/Controllers/FacturaController.cs`
4. `Ws_Restaurante/front/js/carrito.js`
5. `Ws_Restaurante/front/carrito.html`

## ?? NOTAS IMPORTANTES

- El IVA se calcula sobre el subtotal YA con descuento aplicado
- Los precios unitarios en el detalle muestran el precio original
- Los subtotales en el detalle muestran el precio con descuento
- La factura almacena el monto total de descuento aplicado
- El estado "Emitida" indica que aún no se ha realizado el pago