-- ============================================================
-- SCRIPT PARA CAMBIAR IVA AL 7% (SIN AGREGAR COLUMNA DESCUENTO)
-- ============================================================

USE [GestionReservas]
GO

PRINT '?? INICIANDO SCRIPT DE ACTUALIZACIÓN DE FACTURACIÓN...'
PRINT '?? Fecha de ejecución: ' + CONVERT(VARCHAR, GETDATE(), 103) + ' a las ' + CONVERT(VARCHAR, GETDATE(), 108)
PRINT ''
PRINT '?? NOTA: El descuento viene de la tabla Promocion, NO de una columna en Factura'
PRINT ''

-- Verificar estructura de tabla Factura
PRINT '?? Estructura actual de tabla Factura:'
SELECT 
    COLUMN_NAME as 'Columna',
    DATA_TYPE as 'Tipo',
    IS_NULLABLE as 'Permite NULL',
    ISNULL(COLUMN_DEFAULT, 'Sin default') as 'Valor por defecto'
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Factura'
ORDER BY ORDINAL_POSITION

PRINT ''

-- Verificar estructura de tabla Promocion
PRINT '?? Estructura actual de tabla Promocion:'
SELECT 
 COLUMN_NAME as 'Columna',
    DATA_TYPE as 'Tipo',
    IS_NULLABLE as 'Permite NULL',
    ISNULL(COLUMN_DEFAULT, 'Sin default') as 'Valor por defecto'
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Promocion'
ORDER BY ORDINAL_POSITION

PRINT ''

-- Verificar promociones activas
PRINT '?? PROMOCIONES ACTIVAS:'
SELECT 
    IdPromocion,
    Nombre,
    Descuento,
    Estado,
    FechaInicio,
    FechaFin
FROM Promocion 
WHERE Estado = 'ACTIVA'
    AND FechaInicio <= GETDATE() 
    AND FechaFin >= GETDATE()

PRINT ''

-- Mostrar estadísticas de facturas
PRINT '?? ESTADÍSTICAS DE FACTURAS:'
SELECT 
    COUNT(*) as 'Total_Facturas',
    COUNT(CASE WHEN Estado = 'Emitida' THEN 1 END) as 'Emitidas',
    COUNT(CASE WHEN Estado = 'Pagada' THEN 1 END) as 'Pagadas',
    COUNT(CASE WHEN Estado = 'Anulada' THEN 1 END) as 'Anuladas',
    AVG(Total) as 'Promedio_Total',
 SUM(Total) as 'Total_Facturado'
FROM Factura

PRINT ''
PRINT '? Verificación completada'
PRINT '?? CAMBIOS APLICADOS:'
PRINT ' • IVA cambiado al 7% en código C#'
PRINT '   • Descuento se obtiene de tabla Promocion'
PRINT '   • Tabla Factura mantiene estructura original'
PRINT '   • Sistema corregido para usar promociones'