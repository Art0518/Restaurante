-- ============================================================
-- STORED PROCEDURES ACTUALIZADOS - MESAS
-- Sistema de Reservas - CafeSanJuan
-- ============================================================

USE CafeSanJuan;
GO

-- ============================================================
-- 1. usp_Mesa_ListarTodas - Listar todas las mesas
-- ============================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Mesa_ListarTodas]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_Mesa_ListarTodas]
GO

CREATE PROCEDURE usp_Mesa_ListarTodas
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        IdMesa,
      IdRestaurante,
        NumeroMesa,
      TipoMesa,
        Capacidad,
    Precio,
      ImagenURL,
        Estado
    FROM reservas.Mesa
    ORDER BY IdRestaurante, NumeroMesa
END
GO

-- ============================================================
-- 2. usp_Mesa_ActualizarEstado - Actualizar estado de mesa
-- ============================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Mesa_ActualizarEstado]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_Mesa_ActualizarEstado]
GO

CREATE PROCEDURE usp_Mesa_ActualizarEstado
    @IdMesa INT,
    @NuevoEstado VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Validar que la mesa existe
    IF NOT EXISTS (SELECT 1 FROM reservas.Mesa WHERE IdMesa = @IdMesa)
    BEGIN
    SELECT 'Mesa no encontrada' AS Resultado
        RETURN
    END
    
    -- Validar estado válido
    IF @NuevoEstado NOT IN ('Disponible', 'Ocupada', 'Reservada', 'Inactiva')
    BEGIN
        SELECT 'Estado no válido. Use: Disponible, Ocupada, Reservada o Inactiva' AS Resultado
        RETURN
    END
    
    UPDATE reservas.Mesa
    SET Estado = @NuevoEstado
  WHERE IdMesa = @IdMesa
    
    SELECT 'Estado de mesa actualizado correctamente' AS Resultado
END
GO

-- ============================================================
-- 3. usp_Mesa_Gestionar - Crear o actualizar mesa
-- ============================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Mesa_Gestionar]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_Mesa_Gestionar]
GO

CREATE PROCEDURE usp_Mesa_Gestionar
    @Operacion VARCHAR(10),
    @IdMesa INT = NULL,
    @IdRestaurante INT,
@NumeroMesa INT,
    @TipoMesa VARCHAR(50),
    @Capacidad INT,
    @Precio DECIMAL(10,2) = NULL,
    @ImagenURL VARCHAR(500) = NULL,
    @Estado VARCHAR(50) = 'Disponible'
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Validaciones comunes
    IF @IdRestaurante <= 0
    BEGIN
  SELECT 'ID de restaurante no válido' AS Resultado
        RETURN
 END
  
    IF @NumeroMesa <= 0
    BEGIN
      SELECT 'Número de mesa debe ser mayor a 0' AS Resultado
        RETURN
    END
    
    IF @Capacidad <= 0
    BEGIN
SELECT 'Capacidad debe ser mayor a 0' AS Resultado
        RETURN
    END
    
    IF @Estado NOT IN ('Disponible', 'Ocupada', 'Reservada', 'Inactiva')
    BEGIN
      SELECT 'Estado no válido. Use: Disponible, Ocupada, Reservada o Inactiva' AS Resultado
        RETURN
    END
    
 -- CREAR NUEVA MESA
    IF @Operacion = 'INSERT'
    BEGIN
        -- Verificar que no exista una mesa con el mismo número en el restaurante
      IF EXISTS (
      SELECT 1 
FROM reservas.Mesa 
          WHERE IdRestaurante = @IdRestaurante 
      AND NumeroMesa = @NumeroMesa
        )
        BEGIN
            SELECT 'Ya existe una mesa con ese número en el restaurante' AS Resultado
            RETURN
        END
    
        -- Insertar nueva mesa (IdMesa es IDENTITY, se genera automáticamente)
        INSERT INTO reservas.Mesa (
      IdRestaurante, 
      NumeroMesa, 
     TipoMesa, 
  Capacidad, 
            Precio, 
            ImagenURL, 
          Estado
        )
  VALUES (
 @IdRestaurante, 
       @NumeroMesa, 
  @TipoMesa, 
    @Capacidad, 
     @Precio, 
            @ImagenURL, 
            @Estado
        )
  
        -- Retornar el ID de la mesa creada
    SELECT 
            SCOPE_IDENTITY() AS IdMesa,
   'Mesa creada correctamente' AS Resultado
    END
    
    -- ACTUALIZAR MESA EXISTENTE
    ELSE IF @Operacion = 'UPDATE'
    BEGIN
        -- Verificar que la mesa existe
     IF NOT EXISTS (SELECT 1 FROM reservas.Mesa WHERE IdMesa = @IdMesa)
        BEGIN
    SELECT 'Mesa no encontrada' AS Resultado
  RETURN
        END
        
        -- Verificar que no haya otra mesa con el mismo número en el restaurante
    IF EXISTS (
            SELECT 1 
         FROM reservas.Mesa 
    WHERE IdRestaurante = @IdRestaurante 
    AND NumeroMesa = @NumeroMesa
  AND IdMesa != @IdMesa
        )
      BEGIN
            SELECT 'Ya existe otra mesa con ese número en el restaurante' AS Resultado
       RETURN
        END
      
        -- Actualizar mesa existente
     UPDATE reservas.Mesa
  SET 
      IdRestaurante = @IdRestaurante,
            NumeroMesa = @NumeroMesa,
TipoMesa = @TipoMesa,
   Capacidad = @Capacidad,
            Precio = @Precio,
         ImagenURL = @ImagenURL,
            Estado = @Estado
        WHERE IdMesa = @IdMesa
     
        SELECT 'Mesa actualizada correctamente' AS Resultado
    END
    ELSE
    BEGIN
     SELECT 'Operación no válida. Use INSERT o UPDATE' AS Resultado
    END
END
GO

-- ============================================================
-- PRUEBAS DE LOS STORED PROCEDURES
-- ============================================================

PRINT '=========================================='
PRINT 'PRUEBAS DE STORED PROCEDURES - MESAS'
PRINT '=========================================='

-- Prueba 1: Listar todas las mesas
PRINT ''
PRINT '1. Listando todas las mesas...'
EXEC usp_Mesa_ListarTodas
GO

-- Prueba 2: Crear nueva mesa
PRINT ''
PRINT '2. Creando nueva mesa...'
EXEC usp_Mesa_Gestionar 
    @Operacion = 'INSERT',
    @IdMesa = NULL,
    @IdRestaurante = 2,
    @NumeroMesa = 100,
    @TipoMesa = 'Terraza Premium',
    @Capacidad = 6,
    @Precio = 55.00,
    @ImagenURL = 'https://example.com/mesa100.jpg',
  @Estado = 'Disponible'
GO

-- Prueba 3: Actualizar estado de mesa
PRINT ''
PRINT '3. Actualizando estado de mesa a Ocupada...'
EXEC usp_Mesa_ActualizarEstado 
    @IdMesa = 1,
    @NuevoEstado = 'Ocupada'
GO

-- Prueba 4: Volver a listar mesas para ver cambios
PRINT ''
PRINT '4. Listando mesas después de cambios...'
EXEC usp_Mesa_ListarTodas
GO

PRINT ''
PRINT '=========================================='
PRINT 'PRUEBAS COMPLETADAS'
PRINT '=========================================='
