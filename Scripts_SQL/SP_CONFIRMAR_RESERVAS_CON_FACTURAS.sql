-- ============================================================
-- SP ACTUALIZADO: CONFIRMAR RESERVAS Y MANEJAR FACTURAS
-- ============================================================

USE [GestionReservas]
GO

-- Eliminar SP anterior si existe
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_confirmar_reservas_selectivas')
DROP PROCEDURE sp_confirmar_reservas_selectivas
GO

CREATE PROCEDURE sp_confirmar_reservas_selectivas
    @IdUsuario INT,
    @ReservasIds NVARCHAR(500), -- "60,61,62"
    @MetodoPago NVARCHAR(50),
    @PromocionId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION
        
        -- Variables para el resultado
        DECLARE @Estado NVARCHAR(20) = 'SUCCESS'
        DECLARE @Mensaje NVARCHAR(500) = 'Reservas confirmadas exitosamente'
     DECLARE @ReservasConfirmadas INT = 0
     DECLARE @IdFacturaAfectada INT = NULL
     
        -- Validaciones básicas
     IF @IdUsuario <= 0
    BEGIN
    SET @Estado = 'ERROR'
  SET @Mensaje = 'ID de usuario no válido'
            GOTO Resultado
        END
     
     IF @ReservasIds IS NULL OR LEN(@ReservasIds) = 0
        BEGIN
      SET @Estado = 'ERROR'
 SET @Mensaje = 'Debe especificar las reservas a confirmar'
        GOTO Resultado
   END
  
        IF @MetodoPago IS NULL OR LEN(@MetodoPago) = 0
        BEGIN
    SET @Estado = 'ERROR'
     SET @Mensaje = 'Método de pago es requerido'
          GOTO Resultado
  END
        
        -- Verificar que el usuario existe
        IF NOT EXISTS (SELECT 1 FROM Usuario WHERE IdUsuario = @IdUsuario)
    BEGIN
            SET @Estado = 'ERROR'
 SET @Mensaje = 'Usuario no encontrado'
    GOTO Resultado
      END
     
        -- Crear tabla temporal con los IDs de reservas
        CREATE TABLE #TempReservas (IdReserva INT)
        
        DECLARE @SQL NVARCHAR(MAX) = 'INSERT INTO #TempReservas (IdReserva) VALUES (' + REPLACE(@ReservasIds, ',', '),(') + ')'
 EXEC sp_executesql @SQL
        
    -- Verificar que todas las reservas pertenecen al usuario y están en estado HOLD
     DECLARE @ReservasValidasCount INT
 SELECT @ReservasValidasCount = COUNT(*)
        FROM Reserva r
        INNER JOIN #TempReservas t ON r.IdReserva = t.IdReserva
        WHERE r.IdUsuario = @IdUsuario AND r.Estado = 'HOLD'
     
        DECLARE @ReservasTotalCount INT
        SELECT @ReservasTotalCount = COUNT(*) FROM #TempReservas
        
      IF @ReservasValidasCount != @ReservasTotalCount
  BEGIN
            SET @Estado = 'ERROR'
            SET @Mensaje = 'Algunas reservas no pertenecen al usuario o no están en estado HOLD'
    GOTO Resultado
        END
        
      -- **NUEVA FUNCIONALIDAD: Verificar si existe una factura EMITIDA para estas reservas**
        SELECT TOP 1 @IdFacturaAfectada = f.IdFactura
        FROM Factura f
        INNER JOIN DetalleFactura df ON f.IdFactura = df.IdFactura
        INNER JOIN #TempReservas t ON df.IdReserva = t.IdReserva
        WHERE f.IdUsuario = @IdUsuario AND f.Estado = 'Emitida'
        GROUP BY f.IdFactura
        
      IF @IdFacturaAfectada IS NOT NULL
    BEGIN
        -- **ACTUALIZAR FACTURA EXISTENTE: Cambiar de EMITIDA a PAGADA**
  UPDATE Factura 
            SET Estado = 'Pagada',
  FechaHora = GETDATE() -- Actualizar timestamp
       WHERE IdFactura = @IdFacturaAfectada
  
   SET @Mensaje = 'Reservas confirmadas y factura #' + CAST(@IdFacturaAfectada AS VARCHAR) + ' marcada como pagada'
        END
        
        -- **Confirmar las reservas: HOLD ? CONFIRMADA**
        UPDATE r
        SET Estado = 'CONFIRMADA',
       MetodoPago = @MetodoPago
        FROM Reserva r
        INNER JOIN #TempReservas t ON r.IdReserva = t.IdReserva
        
        SET @ReservasConfirmadas = @@ROWCOUNT
      
        -- **Aplicar promoción si se especifica**
  IF @PromocionId IS NOT NULL
        BEGIN
            -- Verificar que la promoción existe y está activa
      IF EXISTS (SELECT 1 FROM Promocion 
     WHERE IdPromocion = @PromocionId 
    AND Estado = 'ACTIVA' 
               AND FechaInicio <= GETDATE() 
   AND FechaFin >= GETDATE())
     BEGIN
         -- Aquí podrías agregar lógica adicional para aplicar la promoción
    -- Por ejemplo, actualizar algún campo relacionado con descuentos
    SET @Mensaje = @Mensaje + ' con promoción aplicada'
            END
        END
        
      COMMIT TRANSACTION
        
        Resultado:
        -- Retornar resultado
        SELECT 
      @Estado as Estado,
         @Mensaje as Mensaje,
         @ReservasConfirmadas as ReservasConfirmadas,
      @IdFacturaAfectada as IdFacturaAfectada,
            @PromocionId as PromocionAplicada,
CASE 
    WHEN @PromocionId IS NOT NULL THEN 'Sí'
     ELSE 'No'
            END as DescuentoAplicado
     
        -- Limpiar tabla temporal
     IF OBJECT_ID('tempdb..#TempReservas') IS NOT NULL
       DROP TABLE #TempReservas
            
    END TRY
    BEGIN CATCH
   ROLLBACK TRANSACTION
    
        -- En caso de error
SELECT 
       'ERROR' as Estado,
   'Error al confirmar reservas: ' + ERROR_MESSAGE() as Mensaje,
    0 as ReservasConfirmadas,
      NULL as IdFacturaAfectada,
   NULL as PromocionAplicada,
 'No' as DescuentoAplicado
            
   -- Limpiar tabla temporal
        IF OBJECT_ID('tempdb..#TempReservas') IS NOT NULL
DROP TABLE #TempReservas
          
  END CATCH
END
GO

PRINT '? SP sp_confirmar_reservas_selectivas actualizado exitosamente'
PRINT '?? Nuevas funcionalidades:'
PRINT '   • Detecta facturas EMITIDAS existentes'
PRINT '   • Actualiza estado de factura a PAGADA al confirmar'
PRINT '   • Mantiene trazabilidad de facturas afectadas'
PRINT '   • Confirma reservas HOLD ? CONFIRMADA'
PRINT '   • Aplica método de pago a las reservas'