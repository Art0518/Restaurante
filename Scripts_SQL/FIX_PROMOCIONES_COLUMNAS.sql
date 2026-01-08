-- =============================================
-- CORRECCION RAPIDA - PROMOCIONES VALIDAS CARRITO
-- Autor: Sistema
-- Fecha: 2024
-- Descripción: Corrección rápida para columnas y estados correctos
-- =============================================

USE CafeSanJuan;
GO

PRINT '?? APLICANDO CORRECCION COLUMNAS Y ESTADOS...';

-- Eliminar SP existente con errores
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_listar_promociones_validas_carrito]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_listar_promociones_validas_carrito]
GO

-- Recrear SP con nombres correctos
CREATE PROCEDURE [dbo].[sp_listar_promociones_validas_carrito]
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;
  
    BEGIN TRY
-- Verificar que el usuario exista
        IF NOT EXISTS (SELECT 1 FROM Usuario WHERE IdUsuario = @IdUsuario)
        BEGIN
            SELECT 'ERROR' as Estado, 'Usuario no encontrado' as Mensaje;
         RETURN;
        END

        -- Obtener promociones que sean válidas para TODAS las fechas de reservas en carrito
        SELECT DISTINCT
   p.IdPromocion,
            p.Nombre,
    p.Descuento,
  p.FechaInicio,
            p.FechaFin,
 p.Estado,
     -- Info adicional
  DATEDIFF(DAY, GETDATE(), p.FechaFin) as DiasRestantes,
            'Válida para tus reservas' as Descripcion
 FROM Promocion p
        WHERE p.Estado = 'Activa'
          AND GETDATE() BETWEEN p.FechaInicio AND p.FechaFin  -- Promoción activa HOY
          AND p.IdPromocion IN (
            -- Solo promociones que cubran TODAS las fechas de reservas del carrito
   SELECT p2.IdPromocion
          FROM Promocion p2
      WHERE p2.Estado = 'Activa'
       AND NOT EXISTS (
-- No debe existir ninguna reserva en carrito cuya fecha esté fuera del rango de la promoción
        SELECT 1 
    FROM Reserva r
         WHERE r.IdUsuario = @IdUsuario 
        AND r.Estado = 'HOLD'  -- ? CORREGIDO: Estado correcto
         AND (r.Fecha < CAST(p2.FechaInicio AS DATE)  -- ? CORREGIDO: Columna correcta
  OR r.Fecha > CAST(p2.FechaFin AS DATE))  -- ? CORREGIDO: Columna correcta
    )
          )
   ORDER BY p.Descuento DESC; -- Mejores descuentos primero
      
    END TRY
    BEGIN CATCH
        SELECT 
        'ERROR' as Estado,
            ERROR_MESSAGE() as Mensaje;
    END CATCH
END
GO

GRANT EXECUTE ON [dbo].[sp_listar_promociones_validas_carrito] TO PUBLIC;
GO

PRINT '? SP CORREGIDO - Columnas y estados actualizados';
PRINT '';
PRINT '?? CORRECCIONES APLICADAS:';
PRINT '   ? FechaReserva ? Fecha (nombre correcto de columna)';
PRINT '   ? EN_CARRITO ? HOLD (estado correcto de reservas en carrito)';
PRINT '';
PRINT '?? PARA PROBAR:';
PRINT '   EXEC sp_listar_promociones_validas_carrito @IdUsuario = 1';
PRINT '';
PRINT '?? LISTO PARA USAR - Sin errores de columna';