-- ============================================================
-- SCRIPT DE VERIFICACIÓN - FACTURACIÓN CON IVA AL 7%
-- ============================================================

USE [GestionReservas]
GO

PRINT '?? VERIFICANDO ESTRUCTURA DE TABLAS PARA FACTURACIÓN...'
PRINT ''

-- Verificar tabla Factura
PRINT '?? ESTRUCTURA TABLA FACTURA:'
SELECT 
    c.COLUMN_NAME as 'Columna',
    c.DATA_TYPE as 'Tipo',
    c.IS_NULLABLE as 'NULL?',
    c.COLUMN_DEFAULT as 'Default'
FROM INFORMATION_SCHEMA.COLUMNS c
WHERE c.TABLE_NAME = 'Factura'
ORDER BY c.ORDINAL_POSITION
PRINT ''

-- Verificar tabla DetalleFactura
PRINT '?? ESTRUCTURA TABLA DETALLEFACTURA:'
SELECT 
    c.COLUMN_NAME as 'Columna',
    c.DATA_TYPE as 'Tipo',
    c.IS_NULLABLE as 'NULL?',
    c.COLUMN_DEFAULT as 'Default'
FROM INFORMATION_SCHEMA.COLUMNS c
WHERE c.TABLE_NAME = 'DetalleFactura'
ORDER BY c.ORDINAL_POSITION
PRINT ''

-- Consulta de ejemplo con datos de facturación
PRINT '?? EJEMPLO DE FACTURAS RECIENTES CON IDRESERVA:'
SELECT TOP 5
    f.IdFactura,
 u.Nombre as Cliente,
  ISNULL(f.IdReserva, 0) as IdReservaReferencia, -- NUEVA COLUMNA DESTACADA
    f.FechaHora,
    f.Subtotal,
    f.IVA,
  f.Total,
f.Estado,
 -- Calcular porcentaje de IVA real
    CASE 
 WHEN f.Subtotal > 0 THEN ROUND((f.IVA / f.Subtotal) * 100, 2)
  ELSE 0 
    END as 'IVA_Porcentaje_%',
    -- Mostrar información de la reserva referenciada
  CASE 
 WHEN f.IdReserva IS NOT NULL THEN 
   (SELECT CONCAT('Mesa ', m.NumeroMesa, ' - ', CONVERT(VARCHAR, r.Fecha, 103)) 
     FROM Reserva r 
   INNER JOIN Mesa m ON r.IdMesa = m.IdMesa 
     WHERE r.IdReserva = f.IdReserva)
 ELSE 'Sin reserva asignada'
END as InfoReserva
FROM Factura f
INNER JOIN Usuario u ON f.IdUsuario = u.IdUsuario
WHERE f.IdFactura IS NOT NULL
ORDER BY f.FechaHora DESC
PRINT ''

-- Verificar detalles de facturas recientes
PRINT '?? DETALLES DE FACTURAS RECIENTES:'
SELECT TOP 10
    df.IdFactura,
    df.Descripcion,
    df.Cantidad,
    df.PrecioUnitario as 'Precio_Sin_Descuento',
    df.Subtotal as 'Subtotal_Con_Descuento',
    -- Calcular descuento aplicado por item
    (df.PrecioUnitario - df.Subtotal) as 'Descuento_Item'
FROM DetalleFactura df
ORDER BY df.IdFactura DESC, df.IdDetalle
PRINT ''

-- NUEVA SECCIÓN: Verificar relación Factura-Reserva
PRINT '?? VERIFICACIÓN RELACIÓN FACTURA-RESERVA:'
SELECT 
    f.IdFactura,
    f.IdReserva as ReservaReferencia,
    COUNT(df.IdReserva) as TotalReservasEnDetalle,
   CASE 
        WHEN f.IdReserva IS NULL THEN '? Sin IdReserva'
      WHEN f.IdReserva IS NOT NULL AND COUNT(df.IdReserva) > 0 THEN '? Correcta'
        ELSE '?? Inconsistente'
    END as EstadoRelacion
FROM Factura f
LEFT JOIN DetalleFactura df ON f.IdFactura = df.IdFactura
WHERE f.IdFactura IS NOT NULL
GROUP BY f.IdFactura, f.IdReserva
ORDER BY f.IdFactura DESC

PRINT ''

-- Estadísticas de IVA
PRINT '?? ESTADÍSTICAS DE IVA:'
SELECT 
    'Facturas con IVA al 7%' as Descripcion,
    COUNT(*) as Cantidad,
    AVG(CASE WHEN f.Subtotal > 0 THEN (f.IVA / f.Subtotal) * 100 ELSE 0 END) as 'Promedio_IVA_%'
FROM Factura f
WHERE f.Subtotal > 0 
    AND f.IVA > 0
    AND ABS((f.IVA / f.Subtotal) - 0.07) < 0.001 -- Tolerancia para decimales

UNION ALL

SELECT 
    'Facturas con IVA diferente' as Descripcion,
    COUNT(*) as Cantidad,
    AVG(CASE WHEN f.Subtotal > 0 THEN (f.IVA / f.Subtotal) * 100 ELSE 0 END) as 'Promedio_IVA_%'
FROM Factura f
WHERE f.Subtotal > 0 
    AND f.IVA > 0
    AND ABS((f.IVA / f.Subtotal) - 0.07) >= 0.001
PRINT ''

PRINT '? VERIFICACIÓN COMPLETADA'
PRINT '?? NOTA: El IVA debe ser 7% en todas las facturas nuevas'
PRINT '?? Las facturas anteriores pueden mantener el IVA del 12% que tenían'
PRINT '?? NUEVO: IdReserva debe estar presente en facturas nuevas'
PRINT '?? IdReserva permite prevenir facturas duplicadas'
PRINT '?? IdReserva mejora trazabilidad entre facturas y reservas'