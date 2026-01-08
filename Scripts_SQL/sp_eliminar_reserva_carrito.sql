-- =============================================
-- Autor: Sistema
-- Fecha Creación: 2024
-- Descripción: SP para eliminar definitivamente reservas en estado HOLD del carrito
-- =============================================

USE CafeSanJuan;
GO

-- Eliminar el SP si ya existe
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_eliminar_reserva_carrito]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_eliminar_reserva_carrito]
GO

CREATE PROCEDURE [dbo].[sp_eliminar_reserva_carrito]
  @IdUsuario INT,
    @IdReserva INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
  BEGIN TRANSACTION;
        
        -- Validar que el usuario existe
        IF NOT EXISTS (SELECT 1 FROM Usuario WHERE IdUsuario = @IdUsuario)
        BEGIN
            SELECT 
        'ERROR' as Estado,
           'Usuario no encontrado' as Mensaje;
   ROLLBACK TRANSACTION;
            RETURN;
      END
        
        -- Validar que la reserva existe, pertenece al usuario y está en estado HOLD
  IF NOT EXISTS (
  SELECT 1 FROM Reserva 
       WHERE IdReserva = @IdReserva 
         AND IdUsuario = @IdUsuario 
   AND Estado = 'HOLD'
        )
        BEGIN
     SELECT 
       'ERROR' as Estado,
                'Reserva no encontrada, no pertenece al usuario o no está en estado HOLD' as Mensaje;
            ROLLBACK TRANSACTION;
      RETURN;
        END
        
        -- Obtener información de la reserva antes de eliminarla (para el log)
        DECLARE @NumeroMesa INT, @Fecha DATE, @Hora TIME;
      
        SELECT @NumeroMesa = m.NumeroMesa, @Fecha = r.Fecha, @Hora = r.Hora
        FROM Reserva r
        INNER JOIN Mesa m ON r.IdMesa = m.IdMesa
  WHERE r.IdReserva = @IdReserva;

        -- Eliminar la reserva definitivamente
  DELETE FROM Reserva 
 WHERE IdReserva = @IdReserva 
      AND IdUsuario = @IdUsuario 
          AND Estado = 'HOLD';
        
        IF @@ROWCOUNT = 0
  BEGIN
            SELECT 
     'ERROR' as Estado,
           'No se pudo eliminar la reserva' as Mensaje;
  ROLLBACK TRANSACTION;
      RETURN;
   END
        
        -- Todo exitoso
        COMMIT TRANSACTION;
 
        SELECT 
  'SUCCESS' as Estado,
    'Reserva eliminada exitosamente del carrito' as Mensaje,
            @IdReserva as ReservaEliminada,
            @NumeroMesa as Mesa,
            @Fecha as Fecha,
         @Hora as Hora;
  
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
   
        SELECT 
    'ERROR' as Estado,
    'Error eliminando la reserva: ' + ERROR_MESSAGE() as Mensaje;
    END CATCH
END
GO

-- Dar permisos de ejecución
GRANT EXECUTE ON [dbo].[sp_eliminar_reserva_carrito] TO PUBLIC;
GO

PRINT 'Stored Procedure sp_eliminar_reserva_carrito creado exitosamente';