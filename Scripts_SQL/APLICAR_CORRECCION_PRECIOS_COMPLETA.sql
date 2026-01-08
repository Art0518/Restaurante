-- =============================================
-- SCRIPT MAESTRO DE CORRECCIÓN DE PRECIOS
-- Aplica todas las correcciones necesarias para el sistema de carrito
-- =============================================

USE CafeSanJuan;
GO

PRINT '##############################################';
PRINT '# INICIO DE CORRECCIONES DE SISTEMA DE PRECIOS';
PRINT '##############################################';
PRINT '';

-- ==================================================
-- PASO 1: Actualizar el Stored Procedure
-- ==================================================
PRINT 'PASO 1: Actualizando Stored Procedure sp_confirmar_reservas_selectivas...';
PRINT '';

EXEC('
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[sp_confirmar_reservas_selectivas]'') AND type in (N''P'', N''PC''))
DROP PROCEDURE [dbo].[sp_confirmar_reservas_selectivas]
');
GO

CREATE PROCEDURE [dbo].[sp_confirmar_reservas_selectivas]
    @IdUsuario INT,
    @ReservasIds NVARCHAR(MAX),
  @MetodoPago VARCHAR(50),
    @PromocionId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
 
BEGIN TRY
  BEGIN TRANSACTION;
      
  IF NOT EXISTS (SELECT 1 FROM Usuario WHERE IdUsuario = @IdUsuario)
 BEGIN
   SELECT 'ERROR' as Estado, 'Usuario no encontrado' as Mensaje;
ROLLBACK TRANSACTION;
     RETURN;
  END
    
   IF @MetodoPago IS NULL OR LTRIM(RTRIM(@MetodoPago)) = ''
   BEGIN
   SELECT 'ERROR' as Estado, 'Método de pago es requerido' as Mensaje;
   ROLLBACK TRANSACTION;
     RETURN;
 END

        DECLARE @ReservaId INT;
     DECLARE @ReservasConfirmadas INT = 0;
  DECLARE @ReservasError INT = 0;
 DECLARE @ErrorMsg NVARCHAR(MAX) = '''';
 DECLARE @DescuentoPromocion DECIMAL(5,2) = 0;
 DECLARE @PrecioOriginal DECIMAL(10,2);
     DECLARE @MontoDescuento DECIMAL(10,2);
  DECLARE @TotalSinIVA DECIMAL(10,2);

   IF @PromocionId IS NOT NULL
      BEGIN
      SELECT @DescuentoPromocion = Descuento
  FROM Promocion
WHERE IdPromocion = @PromocionId
   AND Estado = ''Activa''
 AND CAST(GETDATE() AS DATE) >= CAST(FechaInicio AS DATE)
    AND CAST(GETDATE() AS DATE) <= CAST(FechaFin AS DATE);
    
     IF @DescuentoPromocion IS NULL
   BEGIN
    SELECT ''ERROR'' as Estado, ''Promoción no válida o no activa'' as Mensaje;
    ROLLBACK TRANSACTION;
 RETURN;
     END
   END

        CREATE TABLE #TempReservas (IdReserva INT);
    
DECLARE @SQL NVARCHAR(MAX) = ''INSERT INTO #TempReservas (IdReserva) VALUES ('' + REPLACE(@ReservasIds, '','', ''),('' ) + '')'';
 EXEC sp_executesql @SQL;
     
   IF EXISTS (
    SELECT 1 FROM #TempReservas t
     LEFT JOIN Reserva r ON t.IdReserva = r.IdReserva
      WHERE r.IdReserva IS NULL 
    OR r.IdUsuario != @IdUsuario 
     OR r.Estado != ''HOLD''
 )
   BEGIN
  SELECT ''ERROR'' as Estado, ''Una o más reservas no válidas'' as Mensaje;
     ROLLBACK TRANSACTION;
  RETURN;
  END
        
  DECLARE reserva_cursor CURSOR FOR 
  SELECT IdReserva FROM #TempReservas;
        
  OPEN reserva_cursor;
        FETCH NEXT FROM reserva_cursor INTO @ReservaId;
   
 WHILE @@FETCH_STATUS = 0
     BEGIN
  SELECT @PrecioOriginal = m.Precio
      FROM Reserva r
   INNER JOIN Mesa m ON r.IdMesa = m.IdMesa
    WHERE r.IdReserva = @ReservaId;
            
   SET @MontoDescuento = ROUND(@PrecioOriginal * (@DescuentoPromocion / 100), 2);
  SET @TotalSinIVA = @PrecioOriginal - @MontoDescuento;
     
 UPDATE Reserva 
   SET Estado = ''CONFIRMADA'',
     MetodoPago = @MetodoPago,
    MontoDescuento = @MontoDescuento,
     Total = @TotalSinIVA
       WHERE IdReserva = @ReservaId;
  
 IF @@ROWCOUNT > 0
    SET @ReservasConfirmadas = @ReservasConfirmadas + 1;
 ELSE
   BEGIN
     SET @ReservasError = @ReservasError + 1;
    SET @ErrorMsg = @ErrorMsg + ''Error confirmando reserva '' + CAST(@ReservaId AS VARCHAR) + ''; '';
END
      
 FETCH NEXT FROM reserva_cursor INTO @ReservaId;
    END

     CLOSE reserva_cursor;
     DEALLOCATE reserva_cursor;
        DROP TABLE #TempReservas;
    
   IF @ReservasError > 0
 BEGIN
SELECT ''ERROR'' as Estado, 
 ''Se confirmaron '' + CAST(@ReservasConfirmadas AS VARCHAR) + '' reservas, pero hubo '' + CAST(@ReservasError AS VARCHAR) + '' errores: '' + @ErrorMsg as Mensaje;
       ROLLBACK TRANSACTION;
      RETURN;
   END  
 
    COMMIT TRANSACTION;
 
 SELECT 
       ''SUCCESS'' as Estado,
     ''Se confirmaron exitosamente '' + CAST(@ReservasConfirmadas AS VARCHAR) + '' reservas'' + 
    CASE WHEN @PromocionId IS NOT NULL THEN '' con '' + CAST(@DescuentoPromocion AS VARCHAR) + ''% de descuento'' ELSE '''' END as Mensaje,
     @ReservasConfirmadas as ReservasConfirmadas,
    @PromocionId as PromocionAplicada,
    @DescuentoPromocion as DescuentoAplicado;
  
    END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
   ROLLBACK TRANSACTION;
 
    SELECT ''ERROR'' as Estado, ''Error en el proceso: '' + ERROR_MESSAGE() as Mensaje;
  END CATCH
END
GO

GRANT EXECUTE ON [dbo].[sp_confirmar_reservas_selectivas] TO PUBLIC;
GO

PRINT 'Stored Procedure actualizado correctamente.';
PRINT '';

-- ==================================================
-- PASO 2: Corregir reservas existentes
-- ==================================================
PRINT 'PASO 2: Corrigiendo reservas confirmadas existentes...';
PRINT '';

DECLARE @ReservasCorregidas INT;

UPDATE r
SET r.Total = CASE 
    WHEN r.MontoDescuento IS NOT NULL AND r.MontoDescuento > 0 
    THEN m.Precio - r.MontoDescuento
    ELSE m.Precio
END
FROM Reserva r
INNER JOIN Mesa m ON r.IdMesa = m.IdMesa
WHERE r.Estado = 'CONFIRMADA'
  AND (r.Total IS NULL OR r.Total = 0);

SET @ReservasCorregidas = @@ROWCOUNT;

PRINT 'Reservas corregidas: ' + CAST(@ReservasCorregidas AS VARCHAR);
PRINT '';

-- ==================================================
-- PASO 3: Verificación final
-- ==================================================
PRINT 'PASO 3: Verificación final de datos...';
PRINT '';

SELECT 
    COUNT(*) AS TotalReservasConfirmadas,
    COUNT(CASE WHEN Total IS NULL OR Total = 0 THEN 1 END) AS ReservasConProblema,
    COUNT(CASE WHEN Total > 0 THEN 1 END) AS ReservasCorrectas
FROM Reserva
WHERE Estado = 'CONFIRMADA';

PRINT '';
PRINT '##############################################';
PRINT '# CORRECCIONES COMPLETADAS EXITOSAMENTE';
PRINT '##############################################';
PRINT '';
PRINT 'IMPORTANTE: El campo Total ahora guarda el precio SIN IVA';
PRINT 'El IVA (7%) se calcula en el frontend al mostrar las reservas';
PRINT '';
GO
