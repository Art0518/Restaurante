-- =============================================
-- INSTALADOR COMPLETO - SISTEMA PROMOCIONES
-- Autor: Sistema
-- Fecha Creación: 2024
-- Descripción: Script para instalar/actualizar todos los SP de promociones
-- =============================================

USE CafeSanJuan;
GO

PRINT '=== INICIANDO INSTALACION SISTEMA PROMOCIONES ===';
PRINT '';

-- ============================================================
-- 1. SP LISTAR PROMOCIONES
-- ============================================================
PRINT '1. Creando SP para listar promociones...';

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
GRANT EXECUTE ON [dbo].[sp_listar_promociones] TO PUBLIC;
GO
PRINT '? SP sp_listar_promociones creado exitosamente';

-- ============================================================
-- 2. SP GESTIONAR PROMOCION
-- ============================================================
PRINT '2. Creando SP para gestionar promociones...';

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
GRANT EXECUTE ON [dbo].[sp_gestionar_promocion] TO PUBLIC;
GO
PRINT '? SP sp_gestionar_promocion creado exitosamente';

-- ============================================================
-- 3. SP ELIMINAR PROMOCION
-- ============================================================
PRINT '3. Creando SP para eliminar promociones...';

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
GRANT EXECUTE ON [dbo].[sp_eliminar_promocion] TO PUBLIC;
GO
PRINT '? SP sp_eliminar_promocion creado exitosamente';

PRINT '';
PRINT '=== INSTALACION COMPLETADA EXITOSAMENTE ===';
PRINT '';
PRINT '?? STORED PROCEDURES INSTALADOS:';
PRINT '   ? sp_listar_promociones - Listar todas las promociones';
PRINT '   ? sp_gestionar_promocion - Crear/actualizar promociones';
PRINT '   ? sp_eliminar_promocion - Eliminar promociones';
PRINT '   ? sp_listar_promociones_activas - Listar promociones activas (ya existía)';
PRINT '   ? sp_listar_promociones_validas_carrito - Promociones válidas para carrito';
PRINT '';
PRINT '?? PARA PROBAR:';
PRINT '   EXEC sp_listar_promociones';
PRINT '   EXEC sp_listar_promociones_activas';
PRINT '   EXEC sp_listar_promociones_validas_carrito @IdUsuario = 1';
PRINT '';
PRINT '? SISTEMA PROMOCIONES LISTO PARA USAR';

-- ============================================================
-- 4. SP PROMOCIONES VÁLIDAS PARA CARRITO
-- ============================================================
PRINT '4. Creando SP para promociones válidas en carrito...';

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_listar_promociones_validas_carrito]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_listar_promociones_validas_carrito]
GO

CREATE PROCEDURE [dbo].[sp_listar_promociones_validas_carrito]
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;
  
    BEGIN TRY
     -- Verificar que el usuario exista
      IF NOT EXISTS (SELECT 1 FROM Usuario WHERE IdUsuario = @IdUsuario)
        BEGIN
            SELECT 'ERROR' as Estado, 'Usuario no encontrado' as Mensaje;
          RETURN;
        END

        -- Obtener promociones que sean válidas para TODAS las fechas de reservas en carrito
SELECT DISTINCT
            p.IdPromocion,
            p.Nombre,
 p.Descuento,
    p.FechaInicio,
            p.FechaFin,
   p.Estado,
    -- Info adicional
   DATEDIFF(DAY, GETDATE(), p.FechaFin) as DiasRestantes,
            'Válida para tus reservas' as Descripcion
        FROM Promocion p
        WHERE p.Estado = 'Activa'
  AND GETDATE() BETWEEN p.FechaInicio AND p.FechaFin  -- Promoción activa HOY
          AND p.IdPromocion IN (
     -- Solo promociones que cubran TODAS las fechas de reservas del carrito
   SELECT p2.IdPromocion
  FROM Promocion p2
         WHERE p2.Estado = 'Activa'
            AND NOT EXISTS (
                -- No debe existir ninguna reserva en carrito cuya fecha esté fuera del rango de la promoción
    SELECT 1 
    FROM Reserva r
    WHERE r.IdUsuario = @IdUsuario 
    AND r.Estado = 'HOLD'  -- Estado correcto para reservas en carrito
       AND (r.Fecha < CAST(p2.FechaInicio AS DATE) 
       OR r.Fecha > CAST(p2.FechaFin AS DATE))
   )
        )
        ORDER BY p.Descuento DESC; -- Mejores descuentos primero
        
    END TRY
    BEGIN CATCH
     SELECT 
 'ERROR' as Estado,
 ERROR_MESSAGE() as Mensaje;
    END CATCH
END
GO
GRANT EXECUTE ON [dbo].[sp_listar_promociones_validas_carrito] TO PUBLIC;
GO
PRINT '? SP sp_listar_promociones_validas_carrito creado exitosamente';