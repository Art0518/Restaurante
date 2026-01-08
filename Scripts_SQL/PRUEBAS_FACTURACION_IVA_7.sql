-- ============================================================
-- SCRIPT DE PRUEBAS - FACTURACIÓN CON IVA AL 7%
-- ============================================================

USE [GestionReservas]
GO

PRINT '?? INICIANDO PRUEBAS DE FACTURACIÓN CON IVA AL 7%'
PRINT '================================================'
PRINT ''

-- Simular cálculos de facturación
DECLARE @SubtotalOriginal DECIMAL(10,2) = 100.00
DECLARE @PorcentajeDescuento DECIMAL(5,2) = 15.00
DECLARE @PorcentajeIVA DECIMAL(5,4) = 0.0700

PRINT '?? EJEMPLO DE CÁLCULOS:'
PRINT 'Subtotal Original: $' + CAST(@SubtotalOriginal AS VARCHAR)
PRINT 'Descuento (' + CAST(@PorcentajeDescuento AS VARCHAR) + '%): $' + CAST(@SubtotalOriginal * (@PorcentajeDescuento/100) AS VARCHAR)
PRINT 'Subtotal con Descuento: $' + CAST(@SubtotalOriginal - (@SubtotalOriginal * (@PorcentajeDescuento/100)) AS VARCHAR)
PRINT 'IVA (7%): $' + CAST((@SubtotalOriginal - (@SubtotalOriginal * (@PorcentajeDescuento/100))) * @PorcentajeIVA AS VARCHAR)
PRINT 'Total Final: $' + CAST((@SubtotalOriginal - (@SubtotalOriginal * (@PorcentajeDescuento/100))) * (1 + @PorcentajeIVA) AS VARCHAR)
PRINT ''

-- Verificar facturas recientes con IVA correcto
PRINT '?? FACTURAS RECIENTES CON VERIFICACIÓN DE IVA:'
SELECT TOP 5
    f.IdFactura,
    f.FechaHora,
 f.Subtotal,
    ISNULL(f.Descuento, 0.00) as Descuento,
    f.IVA,
    f.Total,
    f.Estado,
    -- Calcular porcentaje de IVA real
    CASE 
        WHEN f.Subtotal > 0 THEN ROUND((f.IVA / f.Subtotal) * 100, 2)
 ELSE 0 
 END as 'IVA_Calculado_%',
  -- Verificar si el IVA es del 7%
    CASE 
        WHEN f.Subtotal > 0 AND ABS((f.IVA / f.Subtotal) - 0.07) < 0.001 THEN '? Correcto (7%)'
        WHEN f.Subtotal > 0 AND ABS((f.IVA / f.Subtotal) - 0.12) < 0.001 THEN '?? Anterior (12%)'
   ELSE '? Incorrecto'
    END as 'Verificacion_IVA'
FROM Factura f
WHERE f.IdFactura IS NOT NULL
ORDER BY f.FechaHora DESC

PRINT ''

-- Verificar estructura de DetalleFactura
PRINT '?? VERIFICACIÓN DE DETALLES DE FACTURA:'
SELECT TOP 5
    df.IdFactura,
    df.Descripcion,
    df.Cantidad,
    df.PrecioUnitario,
    df.Subtotal,
    -- Verificar si hay descuento aplicado
    CASE 
WHEN df.PrecioUnitario > df.Subtotal THEN '? Con descuento'
        WHEN df.PrecioUnitario = df.Subtotal THEN '? Sin descuento'
        ELSE '? Error en cálculo'
    END as 'Estado_Descuento',
    (df.PrecioUnitario - df.Subtotal) as 'Descuento_Aplicado'
FROM DetalleFactura df
ORDER BY df.IdFactura DESC

PRINT ''

-- Verificar consistencia de datos
PRINT '?? VERIFICACIÓN DE CONSISTENCIA:'

DECLARE @FacturasInconsistentes INT
SELECT @FacturasInconsistentes = COUNT(*)
FROM Factura f
WHERE f.Total != (f.Subtotal + f.IVA)

IF @FacturasInconsistentes = 0
    PRINT '? Todas las facturas tienen totales consistentes'
ELSE
    PRINT '? Se encontraron ' + CAST(@FacturasInconsistentes AS VARCHAR) + ' facturas con totales inconsistentes'

-- Verificar facturas con descuento
DECLARE @FacturasConDescuento INT
SELECT @FacturasConDescuento = COUNT(*)
FROM Factura f
WHERE f.Descuento > 0

PRINT '?? Facturas con descuento aplicado: ' + CAST(@FacturasConDescuento AS VARCHAR)

-- Verificar promedio de IVA
DECLARE @PromedioIVA DECIMAL(5,2)
SELECT @PromedioIVA = AVG(CASE WHEN f.Subtotal > 0 THEN (f.IVA / f.Subtotal) * 100 ELSE 0 END)
FROM Factura f
WHERE f.Subtotal > 0 AND f.IVA > 0

PRINT '?? Promedio de IVA aplicado: ' + CAST(@PromedioIVA AS VARCHAR) + '%'

PRINT ''
PRINT '? PRUEBAS COMPLETADAS'
PRINT ''
PRINT '?? RESULTADOS ESPERADOS:'
PRINT '   • IVA debe ser 7% en facturas nuevas'
PRINT '   • Precio unitario sin descuento'
PRINT '   • Subtotal con descuento aplicado'
PRINT '   • Totales consistentes (Subtotal + IVA = Total)'
PRINT '   • Columna Descuento disponible'