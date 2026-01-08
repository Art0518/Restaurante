-- ============================================================
-- SCRIPT PARA CORREGIR PRECIOS DE RESERVAS
-- ============================================================

USE [GestionReservas]
GO

PRINT '?? CORRIGIENDO PRECIOS DE RESERVAS...'
PRINT ''

BEGIN TRY
    BEGIN TRANSACTION

    -- Actualizar reservas que no tienen precio pero su mesa sí tiene precio
    UPDATE r 
    SET r.Total = m.PrecioReserva
    FROM Reserva r
    INNER JOIN Mesa m ON r.IdMesa = m.IdMesa
    WHERE (r.Total IS NULL OR r.Total = 0)
        AND m.PrecioReserva IS NOT NULL 
      AND m.PrecioReserva > 0
    AND r.Estado IN ('HOLD', 'CONFIRMADA')

    DECLARE @ReservasActualizadas INT = @@ROWCOUNT
    PRINT '? Reservas actualizadas: ' + CAST(@ReservasActualizadas AS VARCHAR)

    -- Asignar precio por defecto a mesas que no tienen precio
    UPDATE Mesa 
  SET PrecioReserva = 
        CASE 
            WHEN CapacidadPersonas <= 2 THEN 25.00
   WHEN CapacidadPersonas <= 4 THEN 35.00
         WHEN CapacidadPersonas <= 6 THEN 45.00
      ELSE 55.00
        END
    WHERE PrecioReserva IS NULL OR PrecioReserva = 0

    DECLARE @MesasActualizadas INT = @@ROWCOUNT
    PRINT '? Mesas con precio asignado: ' + CAST(@MesasActualizadas AS VARCHAR)

    -- Actualizar reservas que aún no tienen precio después del primer UPDATE
    UPDATE r 
    SET r.Total = m.PrecioReserva
    FROM Reserva r
    INNER JOIN Mesa m ON r.IdMesa = m.IdMesa
    WHERE (r.Total IS NULL OR r.Total = 0)
        AND m.PrecioReserva IS NOT NULL 
        AND m.PrecioReserva > 0
    AND r.Estado IN ('HOLD', 'CONFIRMADA')

    DECLARE @ReservasActualizadas2 INT = @@ROWCOUNT
    PRINT '? Reservas adicionales actualizadas: ' + CAST(@ReservasActualizadas2 AS VARCHAR)

    COMMIT TRANSACTION
    
    PRINT ''
    PRINT '? CORRECCIÓN COMPLETADA EXITOSAMENTE'
    
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION
    PRINT '? Error durante la corrección:'
    PRINT 'Error: ' + ERROR_MESSAGE()
    PRINT 'Línea: ' + CAST(ERROR_LINE() AS VARCHAR)
END CATCH

PRINT ''
PRINT '?? ESTADO FINAL:'

-- Mostrar reservas actualizadas
SELECT 
    r.IdReserva,
    m.NumeroMesa,
  r.Estado,
    r.Total,
    m.PrecioReserva,
    r.IdUsuario
FROM Reserva r
INNER JOIN Mesa m ON r.IdMesa = m.IdMesa
WHERE r.Estado IN ('HOLD', 'CONFIRMADA')
    AND r.Total > 0
ORDER BY r.IdReserva DESC