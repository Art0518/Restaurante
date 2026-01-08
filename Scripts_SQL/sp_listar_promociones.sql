-- =============================================
-- Autor: Sistema
-- Fecha Creación: 2024
-- Descripción: SP para listar todas las promociones
-- =============================================

USE CafeSanJuan;
GO

-- Eliminar el SP si ya existe
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_listar_promociones]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_listar_promociones]
GO

CREATE PROCEDURE [dbo].[sp_listar_promociones]
AS
BEGIN
    SET NOCOUNT ON;
    
  BEGIN TRY
   SELECT 
IdPromocion,
        IdRestaurante,
       Nombre,
            Descuento,
            FechaInicio,
         FechaFin,
            Estado,
         -- Información adicional útil
     CASE 
WHEN FechaFin < CAST(GETDATE() AS DATE) THEN 'Expirada'
       WHEN FechaInicio > CAST(GETDATE() AS DATE) THEN 'Programada'
                WHEN CAST(GETDATE() AS DATE) BETWEEN FechaInicio AND FechaFin AND Estado = 'Activa' THEN 'Vigente'
         ELSE Estado
         END as EstadoCalculado,
        DATEDIFF(DAY, GETDATE(), FechaFin) as DiasRestantes
        FROM Promocion
        ORDER BY 
            CASE 
       WHEN Estado = 'Activa' AND CAST(GETDATE() AS DATE) BETWEEN FechaInicio AND FechaFin THEN 1
       WHEN Estado = 'Activa' THEN 2
     ELSE 3
            END,
      FechaInicio DESC;
 
END TRY
    BEGIN CATCH
        SELECT 
     'ERROR' as Estado,
            ERROR_MESSAGE() as Mensaje;
    END CATCH
END
GO

-- Dar permisos de ejecución
GRANT EXECUTE ON [dbo].[sp_listar_promociones] TO PUBLIC;
GO

PRINT 'Stored Procedure sp_listar_promociones creado exitosamente';