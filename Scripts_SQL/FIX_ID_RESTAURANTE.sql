-- =============================================
-- FIX RAPIDO - CORRECCION IdRestaurante = 2
-- Autor: Sistema
-- Fecha: 2024
-- Descripción: Corrección rápida para IdRestaurante = 2
-- =============================================

USE CafeSanJuan;
GO

PRINT '?? APLICANDO CORRECCION IdRestaurante = 2...';

-- Eliminar SP existente
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_gestionar_promocion]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_gestionar_promocion]
GO

-- Recrear SP con IdRestaurante = 2
CREATE PROCEDURE [dbo].[sp_gestionar_promocion]
    @IdPromocion INT,
    @Nombre VARCHAR(100),
    @Descuento DECIMAL(5,2),
    @FechaInicio DATE,
    @FechaFin DATE,
    @Estado VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validaciones
        IF @FechaFin <= @FechaInicio
        BEGIN
    RAISERROR('La fecha de fin debe ser posterior a la fecha de inicio', 16, 1);
    RETURN;
    END
        
        IF @Descuento <= 0 OR @Descuento > 100
        BEGIN
     RAISERROR('El descuento debe estar entre 0.01 y 100', 16, 1);
   RETURN;
        END
        
     -- Crear nueva promoción
        IF @IdPromocion = 0 OR @IdPromocion IS NULL
     BEGIN
            INSERT INTO Promocion (Nombre, Descuento, FechaInicio, FechaFin, Estado, IdRestaurante)
        VALUES (@Nombre, @Descuento, @FechaInicio, @FechaFin, @Estado, 2); -- ? IdRestaurante = 2
            
        SELECT SCOPE_IDENTITY() as IdPromocion, 'SUCCESS' as Estado, 'Promoción creada exitosamente' as Mensaje;
        END
  ELSE
        BEGIN
            -- Actualizar promoción
          UPDATE Promocion 
      SET Nombre = @Nombre, Descuento = @Descuento, FechaInicio = @FechaInicio, 
        FechaFin = @FechaFin, Estado = @Estado
            WHERE IdPromocion = @IdPromocion;
         
    IF @@ROWCOUNT = 0
            BEGIN
     RAISERROR('No se encontró la promoción a actualizar', 16, 1);
   RETURN;
            END
            
         SELECT @IdPromocion as IdPromocion, 'SUCCESS' as Estado, 'Promoción actualizada exitosamente' as Mensaje;
        END
        
    END TRY
    BEGIN CATCH
        SELECT 'ERROR' as Estado, ERROR_MESSAGE() as Mensaje;
    END CATCH
END
GO

GRANT EXECUTE ON [dbo].[sp_gestionar_promocion] TO PUBLIC;
GO

PRINT '? SP CORREGIDO - IdRestaurante = 2';
PRINT '?? LISTO PARA PROBAR NUEVAMENTE';