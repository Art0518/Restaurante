-- =============================================
-- Autor: Sistema
-- Fecha Creación: 2024
-- Descripción: SP para eliminar promoción
-- =============================================

USE CafeSanJuan;
GO

-- Eliminar el SP si ya existe
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_eliminar_promocion]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_eliminar_promocion]
GO

CREATE PROCEDURE [dbo].[sp_eliminar_promocion]
    @IdPromocion INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Verificar que la promoción exista
        IF NOT EXISTS (SELECT 1 FROM Promocion WHERE IdPromocion = @IdPromocion)
        BEGIN
     RAISERROR('No se encontró la promoción a eliminar', 16, 1);
     RETURN;
    END
        
        -- Eliminar la promoción
    DELETE FROM Promocion 
     WHERE IdPromocion = @IdPromocion;
        
        SELECT 
            'SUCCESS' as Estado,
    'Promoción eliminada exitosamente' as Mensaje;
    
    END TRY
    BEGIN CATCH
      SELECT 
    'ERROR' as Estado,
       ERROR_MESSAGE() as Mensaje;
    END CATCH
END
GO

-- Dar permisos de ejecución
GRANT EXECUTE ON [dbo].[sp_eliminar_promocion] TO PUBLIC;
GO

PRINT 'Stored Procedure sp_eliminar_promocion creado exitosamente';