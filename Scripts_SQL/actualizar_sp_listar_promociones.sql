-- ============================================
-- Script para actualizar sp_listar_promociones
-- Agrega la columna Nombre al resultado
-- ============================================

USE [CafeSanJuan] -- Cambia por el nombre de tu base de datos
GO

-- Eliminar el procedimiento existente si existe
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_listar_promociones')
    DROP PROCEDURE sp_listar_promociones
GO

-- Crear el procedimiento actualizado
CREATE PROCEDURE sp_listar_promociones
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        IdPromocion,
        IdRestaurante,
        Nombre,          -- ? COLUMNA AGREGADA
        Descuento,
        FechaInicio,
 FechaFin,
        Estado
  FROM Promocion
    ORDER BY IdPromocion DESC;
END
GO

PRINT 'Stored procedure sp_listar_promociones actualizado correctamente ?'
GO

-- ============================================
-- Script de VERIFICACIÓN
-- ============================================
-- Ejecuta este SELECT para verificar que la columna existe
SELECT TOP 5 
    IdPromocion, 
    Nombre,      -- ? Debe mostrar los nombres
    Descuento, 
    FechaInicio,
    FechaFin,
    Estado
FROM Promocion
ORDER BY IdPromocion DESC;
GO
