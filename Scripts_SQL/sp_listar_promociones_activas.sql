-- =============================================
-- Autor: Sistema
-- Fecha Creación: 2024
-- Descripción: SP para listar promociones activas SOLO en fechas exactas (no antes ni después)
-- =============================================

USE CafeSanJuan;
GO

-- Eliminar el SP si ya existe
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_listar_promociones_activas]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_listar_promociones_activas]
GO

CREATE PROCEDURE [dbo].[sp_listar_promociones_activas]
    @FechaConsulta DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
  
    BEGIN TRY
        -- Si no se proporciona fecha, usar la fecha actual
        IF @FechaConsulta IS NULL
         SET @FechaConsulta = CAST(GETDATE() AS DATE);
        
        -- Listar promociones EXACTAMENTE activas para la fecha (no antes ni después)
        SELECT 
 IdPromocion,
  IdRestaurante,
        Nombre,
        Descuento,
            FechaInicio,
    FechaFin,
         Estado,
            -- Información adicional
     DATEDIFF(DAY, @FechaConsulta, FechaFin) as DiasRestantes,
      CASE 
                WHEN DATEDIFF(DAY, @FechaConsulta, FechaFin) <= 3 THEN 'Próximo a vencer'
  ELSE 'Vigente'
        END as EstadoVigencia
        FROM Promocion
        WHERE Estado = 'Activa'
          AND @FechaConsulta >= CAST(FechaInicio AS DATE)   -- Fecha consulta >= inicio
          AND @FechaConsulta <= CAST(FechaFin AS DATE)      -- Fecha consulta <= fin
  -- ? SOLO promociones que estén EXACTAMENTE en el rango, no antes ni después
        ORDER BY Descuento DESC; -- Ordenar por mayor descuento primero
        
    END TRY
    BEGIN CATCH
  SELECT 
            'ERROR' as Estado,
       ERROR_MESSAGE() as Mensaje;
    END CATCH
END
GO

-- Dar permisos de ejecución
GRANT EXECUTE ON [dbo].[sp_listar_promociones_activas] TO PUBLIC;
GO

PRINT 'Stored Procedure sp_listar_promociones_activas actualizado - Solo fechas exactas';