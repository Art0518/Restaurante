-- ============================================================
-- 📝 STORED PROCEDURE: sp_eliminar_reserva_carrito
-- 📌 Descripción: Elimina una reserva del carrito (estado HOLD)
-- 🔧 Autor: Art0518
-- 📅 Fecha: Enero 2025
-- ============================================================


-- Crear el nuevo SP
CREATE PROCEDURE [dbo].[sp_eliminar_reserva_carrito]
    @IdUsuario INT,
    @IdReserva INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
  
        -- Validaciones básicas
        IF @IdUsuario <= 0
   BEGIN
            SELECT 'ERROR' AS Estado, 
         'ID de usuario no válido' AS Mensaje, 
          0 AS ReservaEliminada;
ROLLBACK TRANSACTION;
     RETURN;
        END
  
        IF @IdReserva <= 0
        BEGIN
            SELECT 'ERROR' AS Estado, 
         'ID de reserva no válido' AS Mensaje, 
           0 AS ReservaEliminada;
   ROLLBACK TRANSACTION;
  RETURN;
        END
    
 -- Verificar que la reserva existe, pertenece al usuario y está en HOLD
        DECLARE @ReservaExiste INT;
        DECLARE @EstadoReserva VARCHAR(50);
        
  SELECT @ReservaExiste = 1,
        @EstadoReserva = Estado
    FROM reservas.Reserva
        WHERE IdReserva = @IdReserva
   AND IdUsuario = @IdUsuario;
        
        -- Si no existe la reserva
  IF @ReservaExiste IS NULL
    BEGIN
          ROLLBACK TRANSACTION;
         SELECT 'ERROR' AS Estado, 
  'Reserva no encontrada o no pertenece al usuario' AS Mensaje, 
  0 AS ReservaEliminada;
          RETURN;
        END
        
        -- Verificar que la reserva esté en estado HOLD
        IF @EstadoReserva != 'HOLD'
        BEGIN
   ROLLBACK TRANSACTION;
  SELECT 'ERROR' AS Estado, 
 'Solo se pueden eliminar reservas en estado HOLD' AS Mensaje, 
    0 AS ReservaEliminada;
      RETURN;
     END
        
        -- Eliminar la reserva
      DELETE FROM reservas.Reserva
        WHERE IdReserva = @IdReserva
    AND IdUsuario = @IdUsuario
          AND Estado = 'HOLD';
  
        DECLARE @FilasEliminadas INT = @@ROWCOUNT;
        
        IF @FilasEliminadas = 0
        BEGIN
            ROLLBACK TRANSACTION;
        SELECT 'ERROR' AS Estado, 
    'No se pudo eliminar la reserva' AS Mensaje, 
       0 AS ReservaEliminada;
       RETURN;
      END
   
        COMMIT TRANSACTION;
        
      -- Retornar resultado exitoso
SELECT 
 'SUCCESS' AS Estado, 
            'Reserva eliminada correctamente' AS Mensaje,
  @IdReserva AS ReservaEliminada;
      
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
   ROLLBACK TRANSACTION;
        
     SELECT 
            'ERROR' AS Estado,
     'Error al eliminar reserva: ' + ERROR_MESSAGE() AS Mensaje,
     0 AS ReservaEliminada;
    END CATCH
END
