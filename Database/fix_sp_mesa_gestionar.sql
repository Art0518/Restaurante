-- ============================================================
-- CORREGIR STORED PROCEDURE usp_Mesa_Gestionar
-- ============================================================
USE CafeSanJuan;
GO

-- Eliminar el procedimiento existente
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
          SELECT 'Ya existe una mesa con ese número en el restaurante' AS Resultado, 0 AS IdMesa
  RETURN
 END
        
        -- Insertar nueva mesa
        DECLARE @NuevoIdMesa INT;
    
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
   );
        
        SET @NuevoIdMesa = SCOPE_IDENTITY();
        
        -- Retornar el ID de la mesa creada
        SELECT 
            'Mesa creada correctamente' AS Resultado,
      @NuevoIdMesa AS IdMesa;
    END
    
    -- ACTUALIZAR MESA EXISTENTE
  ELSE IF @Operacion = 'UPDATE'
    BEGIN
    -- Verificar que la mesa existe
        IF NOT EXISTS (SELECT 1 FROM reservas.Mesa WHERE IdMesa = @IdMesa)
BEGIN
       SELECT 'Mesa no encontrada' AS Resultado, 0 AS IdMesa
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
  SELECT 'Ya existe otra mesa con ese número en el restaurante' AS Resultado, 0 AS IdMesa
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
        WHERE IdMesa = @IdMesa;
        
        SELECT 'Mesa actualizada correctamente' AS Resultado, @IdMesa AS IdMesa;
    END
    ELSE
    BEGIN
   SELECT 'Operación no válida. Use INSERT o UPDATE' AS Resultado, 0 AS IdMesa;
    END
END
GO

PRINT '=========================================='
PRINT 'STORED PROCEDURE CREADO CORRECTAMENTE'
PRINT '=========================================='
GO

-- ============================================================
-- PRUEBA 1: Verificar la estructura de la tabla
-- ============================================================
PRINT ''
PRINT '1. Verificando estructura de la tabla reservas.Mesa...'
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'reservas' 
  AND TABLE_NAME = 'Mesa'
ORDER BY ORDINAL_POSITION;
GO

-- ============================================================
-- PRUEBA 2: Ver mesas actuales
-- ============================================================
PRINT ''
PRINT '2. Mesas actuales en la base de datos:'
SELECT * FROM reservas.Mesa ORDER BY IdMesa;
GO

-- ============================================================
-- PRUEBA 3: Intentar crear nueva mesa con número único
-- ============================================================
PRINT ''
PRINT '3. Creando nueva mesa de prueba (número 999)...'

-- Primero eliminar si existe
DELETE FROM reservas.Mesa WHERE NumeroMesa = 999;

-- Ahora crear
EXEC usp_Mesa_Gestionar 
    @Operacion = 'INSERT',
    @IdMesa = NULL,
    @IdRestaurante = 2,
    @NumeroMesa = 999,
  @TipoMesa = 'Interior TEST',
    @Capacidad = 4,
    @Precio = 35.00,
    @ImagenURL = 'https://example.com/test.jpg',
    @Estado = 'Disponible';
GO

-- ============================================================
-- PRUEBA 4: Verificar que se creó la mesa
-- ============================================================
PRINT ''
PRINT '4. Verificando que se creó la mesa de prueba:'
SELECT * FROM reservas.Mesa WHERE NumeroMesa = 999;
GO

-- ============================================================
-- PRUEBA 5: Intentar crear mesa duplicada (debe fallar)
-- ============================================================
PRINT ''
PRINT '5. Intentando crear mesa duplicada (debe fallar)...'
EXEC usp_Mesa_Gestionar 
    @Operacion = 'INSERT',
    @IdMesa = NULL,
    @IdRestaurante = 2,
    @NumeroMesa = 999,
 @TipoMesa = 'Interior DUPLICADO',
    @Capacidad = 6,
    @Precio = 45.00,
    @ImagenURL = 'https://example.com/duplicado.jpg',
    @Estado = 'Disponible';
GO

-- ============================================================
-- PRUEBA 6: Actualizar la mesa creada
-- ============================================================
PRINT ''
PRINT '6. Actualizando la mesa de prueba...'

DECLARE @IdMesaTest INT;
SELECT @IdMesaTest = IdMesa FROM reservas.Mesa WHERE NumeroMesa = 999;

IF @IdMesaTest IS NOT NULL
BEGIN
    EXEC usp_Mesa_Gestionar 
        @Operacion = 'UPDATE',
        @IdMesa = @IdMesaTest,
        @IdRestaurante = 2,
        @NumeroMesa = 999,
        @TipoMesa = 'Interior ACTUALIZADO',
        @Capacidad = 8,
        @Precio = 55.00,
        @ImagenURL = 'https://example.com/actualizado.jpg',
        @Estado = 'Disponible';
        
    PRINT ''
    PRINT 'Verificando la mesa actualizada:'
    SELECT * FROM reservas.Mesa WHERE IdMesa = @IdMesaTest;
END
ELSE
BEGIN
    PRINT 'ERROR: No se encontró la mesa de prueba para actualizar'
END
GO

-- ============================================================
-- LIMPIEZA: Eliminar mesa de prueba
-- ============================================================
PRINT ''
PRINT '7. Limpiando mesa de prueba...'
DELETE FROM reservas.Mesa WHERE NumeroMesa = 999;
PRINT 'Mesa de prueba eliminada'
GO

PRINT ''
PRINT '=========================================='
PRINT 'TODAS LAS PRUEBAS COMPLETADAS'
PRINT '=========================================='
PRINT ''
PRINT 'NOTA: Si todas las pruebas funcionaron correctamente,'
PRINT 'el stored procedure está listo para usarse desde la API.'
PRINT ''
