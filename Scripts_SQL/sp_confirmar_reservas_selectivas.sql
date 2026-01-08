-- =============================================
-- Autor: Sistema
-- Fecha Creación: 2024
-- Descripción: SP para confirmar reservas selectivamente del carrito con promociones
-- =============================================

USE CafeSanJuan;
GO

-- Eliminar el SP si ya existe
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_confirmar_reservas_selectivas]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_confirmar_reservas_selectivas]
GO

CREATE PROCEDURE [dbo].[sp_confirmar_reservas_selectivas]
 @IdUsuario INT,
    @ReservasIds NVARCHAR(MAX), -- Lista de IDs separados por comas: "1,2,3"
    @MetodoPago VARCHAR(50),
    @PromocionId INT = NULL -- Promoción aplicada (opcional)
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

        -- Validar método de pago
   IF @MetodoPago IS NULL OR LTRIM(RTRIM(@MetodoPago)) = ''
      BEGIN
     SELECT 
        'ERROR' as Estado,
     'Método de pago es requerido' as Mensaje;
            ROLLBACK TRANSACTION;
      RETURN;
        END

     -- Variables para el loop y cálculos
        DECLARE @ReservaId INT;
        DECLARE @ReservasConfirmadas INT = 0;
    DECLARE @ReservasError INT = 0;
        DECLARE @ErrorMsg NVARCHAR(MAX) = '';
        DECLARE @DescuentoPromocion DECIMAL(5,2) = 0;
     DECLARE @PrecioOriginal DECIMAL(10,2);
        DECLARE @MontoDescuento DECIMAL(10,2);
  DECLARE @TotalFinal DECIMAL(10,2);

        -- Validar y obtener descuento de promoción
        IF @PromocionId IS NOT NULL
        BEGIN
            SELECT @DescuentoPromocion = Descuento
        FROM Promocion
      WHERE IdPromocion = @PromocionId
        AND Estado = 'Activa'
          AND CAST(GETDATE() AS DATE) >= CAST(FechaInicio AS DATE)
      AND CAST(GETDATE() AS DATE) <= CAST(FechaFin AS DATE);
            
IF @DescuentoPromocion IS NULL
    BEGIN
          SELECT 
        'ERROR' as Estado,
     'Promoción no válida o no activa' as Mensaje;
  ROLLBACK TRANSACTION;
     RETURN;
   END
        END

        -- Crear tabla temporal para los IDs
    CREATE TABLE #TempReservas (IdReserva INT);
        
    -- Parsear los IDs y insertarlos en la tabla temporal
        DECLARE @SQL NVARCHAR(MAX) = 'INSERT INTO #TempReservas (IdReserva) VALUES (' + REPLACE(@ReservasIds, ',', '),(') + ')';
        EXEC sp_executesql @SQL;
     
        -- Validar que todas las reservas pertenecen al usuario y están en estado HOLD
        IF EXISTS (
    SELECT 1 FROM #TempReservas t
            LEFT JOIN Reserva r ON t.IdReserva = r.IdReserva
     WHERE r.IdReserva IS NULL 
             OR r.IdUsuario != @IdUsuario 
           OR r.Estado != 'HOLD'
    )
        BEGIN
     SELECT 
                'ERROR' as Estado,
        'Una o más reservas no válidas (no existen, no pertenecen al usuario o no están en estado HOLD)' as Mensaje;
       ROLLBACK TRANSACTION;
            RETURN;
     END
        
-- Confirmar cada reserva CON CÁLCULOS DE PROMOCIÓN
        DECLARE reserva_cursor CURSOR FOR 
        SELECT IdReserva FROM #TempReservas;
    
     OPEN reserva_cursor;
     FETCH NEXT FROM reserva_cursor INTO @ReservaId;
   
        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- Obtener el precio de la mesa directamente
SELECT @PrecioOriginal = m.Precio
        FROM Reserva r
  INNER JOIN Mesa m ON r.IdMesa = m.IdMesa
            WHERE r.IdReserva = @ReservaId;
       
       -- Calcular descuento y total CON IVA 11.5%
            SET @MontoDescuento = ROUND(@PrecioOriginal * (@DescuentoPromocion / 100), 2);
  SET @TotalFinal = ROUND((@PrecioOriginal - @MontoDescuento) * 1.115, 2); -- IVA 11.5%
     
    -- Guardar el Total correctamente
     UPDATE Reserva 
            SET Estado = 'CONFIRMADA',
     MetodoPago = @MetodoPago,
                MontoDescuento = @MontoDescuento,
           Total = @TotalFinal
          WHERE IdReserva = @ReservaId;
     
            IF @@ROWCOUNT > 0
   BEGIN
           SET @ReservasConfirmadas = @ReservasConfirmadas + 1;
            END
  ELSE
            BEGIN
         SET @ReservasError = @ReservasError + 1;
        SET @ErrorMsg = @ErrorMsg + 'Error confirmando reserva ' + CAST(@ReservaId AS VARCHAR) + '; ';
  END      
FETCH NEXT FROM reserva_cursor INTO @ReservaId;
        END

        CLOSE reserva_cursor;
        DEALLOCATE reserva_cursor;

   DROP TABLE #TempReservas;
  
     -- Verificar si hubo errores
        IF @ReservasError > 0
        BEGIN
 SELECT 
 'ERROR' as Estado,
      'Se confirmaron ' + CAST(@ReservasConfirmadas AS VARCHAR) + ' reservas, pero hubo ' + CAST(@ReservasError AS VARCHAR) + ' errores: ' + @ErrorMsg as Mensaje;
 ROLLBACK TRANSACTION;
            RETURN;
        END  
        
        -- Todo exitoso
        COMMIT TRANSACTION;
 
  SELECT 
      'SUCCESS' as Estado,
            'Se confirmaron exitosamente ' + CAST(@ReservasConfirmadas AS VARCHAR) + ' reservas' + 
            CASE WHEN @PromocionId IS NOT NULL THEN ' con ' + CAST(@DescuentoPromocion AS VARCHAR) + '% de descuento' ELSE '' END as Mensaje,
    @ReservasConfirmadas as ReservasConfirmadas,
            @PromocionId as PromocionAplicada,
        @DescuentoPromocion as DescuentoAplicado;
   
    END TRY
    BEGIN CATCH
      IF @@TRANCOUNT > 0
    ROLLBACK TRANSACTION;
 
   SELECT 
            'ERROR' as Estado,
         'Error en el proceso: ' + ERROR_MESSAGE() as Mensaje;
    END CATCH
END
GO

-- Dar permisos de ejecución
GRANT EXECUTE ON [dbo].[sp_confirmar_reservas_selectivas] TO PUBLIC;
GO

PRINT 'Stored Procedure sp_confirmar_reservas_selectivas actualizado - IVA 11.5%';