-- ============================================================
-- ?? STORED PROCEDURE: sp_confirmar_reservas_selectivas
-- ?? Descripción: Confirma múltiples reservas del carrito aplicando descuento
-- ?? Autor: Art0518
-- ?? Fecha: Enero 2025
-- ============================================================

USE [db31553]
GO

-- Eliminar el SP si existe
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_confirmar_reservas_selectivas]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_confirmar_reservas_selectivas];
GO

CREATE PROCEDURE [dbo].[sp_confirmar_reservas_selectivas]
    @IdUsuario INT,
    @ReservasIds VARCHAR(MAX),
    @MetodoPago VARCHAR(50),
    @MontoDescuento DECIMAL(10,2) = 0,
    @Total DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validaciones básicas
        IF @IdUsuario <= 0
        BEGIN
            SELECT 'ERROR' AS Estado, 'ID de usuario no válido' AS Mensaje, 0 AS ReservasConfirmadas;
            ROLLBACK TRANSACTION;
          RETURN;
        END
        
     IF @ReservasIds IS NULL OR LTRIM(RTRIM(@ReservasIds)) = ''
        BEGIN
            SELECT 'ERROR' AS Estado, 'Debe especificar las reservas a confirmar' AS Mensaje, 0 AS ReservasConfirmadas;
        ROLLBACK TRANSACTION;
RETURN;
    END
   
        IF @MetodoPago IS NULL OR LTRIM(RTRIM(@MetodoPago)) = ''
        BEGIN
            SELECT 'ERROR' AS Estado, 'Método de pago es requerido' AS Mensaje, 0 AS ReservasConfirmadas;
            ROLLBACK TRANSACTION;
            RETURN;
  END
  
        IF @Total <= 0
   BEGIN
    SELECT 'ERROR' AS Estado, 'El total debe ser mayor a 0' AS Mensaje, 0 AS ReservasConfirmadas;
        ROLLBACK TRANSACTION;
            RETURN;
        END
        
        -- Crear tabla temporal para almacenar los IDs de reservas
        DECLARE @ReservasTemp TABLE (IdReserva INT);
        
        -- Insertar los IDs en la tabla temporal (dividir la cadena)
        INSERT INTO @ReservasTemp (IdReserva)
        SELECT CAST(value AS INT)
    FROM STRING_SPLIT(@ReservasIds, ',')
        WHERE LTRIM(RTRIM(value)) <> '';
        
    -- Verificar que las reservas existen y pertenecen al usuario
        DECLARE @ReservasExistentes INT;
      SELECT @ReservasExistentes = COUNT(*)
  FROM reservas.Reserva r
        INNER JOIN @ReservasTemp rt ON r.IdReserva = rt.IdReserva
        WHERE r.IdUsuario = @IdUsuario 
        AND r.Estado = 'HOLD';
      
        IF @ReservasExistentes = 0
        BEGIN
            ROLLBACK TRANSACTION;
      SELECT 'ERROR' AS Estado, 
         'No se encontraron reservas válidas para confirmar' AS Mensaje, 
      0 AS ReservasConfirmadas;
    RETURN;
        END
        
        -- Actualizar las reservas a estado CONFIRMADA
        UPDATE r
   SET 
            r.Estado = 'CONFIRMADA',
            r.MetodoPago = @MetodoPago,
            r.MontoDescuento = @MontoDescuento,
       r.Total = @Total / @ReservasExistentes  -- Distribuir el total entre las reservas
        FROM reservas.Reserva r
        INNER JOIN @ReservasTemp rt ON r.IdReserva = rt.IdReserva
    WHERE r.IdUsuario = @IdUsuario
        AND r.Estado = 'HOLD';
        
        DECLARE @FilasActualizadas INT = @@ROWCOUNT;
        
        IF @FilasActualizadas = 0
        BEGIN
  ROLLBACK TRANSACTION;
         SELECT 'ERROR' AS Estado, 
        'No se pudieron actualizar las reservas' AS Mensaje, 
    0 AS ReservasConfirmadas;
       RETURN;
  END
    
        COMMIT TRANSACTION;
  
        -- Retornar resultado exitoso
      SELECT 
  'SUCCESS' AS Estado, 
            'Se confirmaron ' + CAST(@FilasActualizadas AS VARCHAR(10)) + ' reserva(s) correctamente' AS Mensaje,
         @FilasActualizadas AS ReservasConfirmadas;
      
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        SELECT 
            'ERROR' AS Estado,
'Error al confirmar reservas: ' + ERROR_MESSAGE() AS Mensaje,
            0 AS ReservasConfirmadas;
    END CATCH
END
