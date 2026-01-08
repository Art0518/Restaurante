-- =============================================
-- Autor: Sistema
-- Fecha Creación: 2024
-- Descripción: SP para gestionar promociones (crear/actualizar)
-- =============================================

USE CafeSanJuan;
GO

-- Eliminar el SP si ya existe
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_gestionar_promocion]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_gestionar_promocion]
GO

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
    -- Validar que la fecha de fin sea mayor a la de inicio
   IF @FechaFin <= @FechaInicio
      BEGIN
  RAISERROR('La fecha de fin debe ser posterior a la fecha de inicio', 16, 1);
RETURN;
        END
     
        -- Validar descuento
        IF @Descuento <= 0 OR @Descuento > 100
 BEGIN
       RAISERROR('El descuento debe estar entre 0.01 y 100', 16, 1);
            RETURN;
 END
        
        -- Si IdPromocion = 0, crear nueva promoción
IF @IdPromocion = 0 OR @IdPromocion IS NULL
        BEGIN
       INSERT INTO Promocion (Nombre, Descuento, FechaInicio, FechaFin, Estado, IdRestaurante)
     VALUES (@Nombre, @Descuento, @FechaInicio, @FechaFin, @Estado, 2); -- ? IdRestaurante = 2 (CORREGIDO)
      
  SELECT SCOPE_IDENTITY() as IdPromocion, 'SUCCESS' as Estado, 'Promoción creada exitosamente' as Mensaje;
        END
        ELSE
        BEGIN
    -- Actualizar promoción existente
      UPDATE Promocion 
 SET 
         Nombre = @Nombre,
   Descuento = @Descuento,
    FechaInicio = @FechaInicio,
     FechaFin = @FechaFin,
   Estado = @Estado
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
    SELECT 
        'ERROR' as Estado,
            ERROR_MESSAGE() as Mensaje;
 END CATCH
END
GO

-- Dar permisos de ejecución
GRANT EXECUTE ON [dbo].[sp_gestionar_promocion] TO PUBLIC;
GO

PRINT 'Stored Procedure sp_gestionar_promocion creado exitosamente';