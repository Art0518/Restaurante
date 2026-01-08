-- =============================================
-- Script para corregir precios de reservas confirmadas existentes
-- que tienen Total = 0 o NULL
-- =============================================

USE CafeSanJuan;
GO

PRINT '==========================================';
PRINT 'INICIANDO CORRECCIÓN DE PRECIOS DE RESERVAS CONFIRMADAS';
PRINT '==========================================';
PRINT '';

-- Verificar cuántas reservas tienen problema
DECLARE @ReservasConProblema INT;

SELECT @ReservasConProblema = COUNT(*)
FROM Reserva r
WHERE r.Estado = 'CONFIRMADA'
  AND (r.Total IS NULL OR r.Total = 0);

PRINT 'Reservas confirmadas con Total = 0 o NULL: ' + CAST(@ReservasConProblema AS VARCHAR);
PRINT '';

IF @ReservasConProblema > 0
BEGIN
    PRINT 'Corrigiendo precios...';
    PRINT '';
    
    -- Actualizar reservas confirmadas sin precio
    UPDATE r
    SET r.Total = CASE 
        -- Si tiene descuento aplicado, restar el descuento
     WHEN r.MontoDescuento IS NOT NULL AND r.MontoDescuento > 0 
        THEN m.Precio - r.MontoDescuento
   -- Si no tiene descuento, usar precio original
        ELSE m.Precio
    END
    FROM Reserva r
    INNER JOIN Mesa m ON r.IdMesa = m.IdMesa
    WHERE r.Estado = 'CONFIRMADA'
      AND (r.Total IS NULL OR r.Total = 0);

    PRINT 'Reservas corregidas: ' + CAST(@@ROWCOUNT AS VARCHAR);
    PRINT '';
    
  -- Mostrar las reservas corregidas
    SELECT 
        r.IdReserva,
        r.IdUsuario,
        m.NumeroMesa,
        r.Fecha,
 r.Hora,
        m.Precio AS PrecioMesa,
  r.MontoDescuento,
        r.Total AS TotalCorregido,
        r.MetodoPago,
    r.Estado
    FROM Reserva r
    INNER JOIN Mesa m ON r.IdMesa = m.IdMesa
    WHERE r.Estado = 'CONFIRMADA'
    ORDER BY r.Fecha DESC, r.Hora DESC;
    
    PRINT '';
    PRINT 'Verificación de totales con IVA (7%):';
    PRINT '';
    
    -- Mostrar cómo se vería con IVA
    SELECT 
        r.IdReserva,
     m.NumeroMesa,
      r.Fecha,
        r.Total AS SubtotalSinIVA,
        ROUND(r.Total * 0.07, 2) AS IVA,
      ROUND(r.Total * 1.07, 2) AS TotalConIVA,
    r.MetodoPago
    FROM Reserva r
    INNER JOIN Mesa m ON r.IdMesa = m.IdMesa
    WHERE r.Estado = 'CONFIRMADA'
    ORDER BY r.Fecha DESC;
    
END
ELSE
BEGIN
  PRINT 'No hay reservas confirmadas con problemas de precio.';
    PRINT 'Mostrando estado actual de todas las reservas confirmadas:';
    PRINT '';
    
  SELECT 
r.IdReserva,
        r.IdUsuario,
        m.NumeroMesa,
        r.Fecha,
   r.Hora,
        m.Precio AS PrecioMesa,
        r.MontoDescuento,
    r.Total AS SubtotalSinIVA,
 ROUND(r.Total * 0.07, 2) AS IVA,
        ROUND(r.Total * 1.07, 2) AS TotalConIVA,
        r.MetodoPago,
        r.Estado
    FROM Reserva r
    INNER JOIN Mesa m ON r.IdMesa = m.IdMesa
    WHERE r.Estado = 'CONFIRMADA'
    ORDER BY r.Fecha DESC, r.Hora DESC;
END

PRINT '';
PRINT '==========================================';
PRINT 'CORRECCIÓN COMPLETADA';
PRINT '==========================================';
GO
