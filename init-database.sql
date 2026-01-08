-- =============================================
-- Script de Inicialización Base de Datos
-- CafeSanJuan - Sistema de Gestión de Restaurantes
-- =============================================

USE db31553;
GO

-- =============================================
-- RESTAURANTES Y MESAS
-- =============================================

-- Insertar restaurantes de ejemplo
IF NOT EXISTS (SELECT 1 FROM reservas.Restaurante WHERE IdRestaurante = 1)
BEGIN
    SET IDENTITY_INSERT reservas.Restaurante ON;
    
    INSERT INTO reservas.Restaurante (IdRestaurante, Nombre, Direccion, Telefono, HorarioApertura, HorarioCierre, Activo, FechaCreacion)
    VALUES 
    (1, 'Café San Juan - Zona Colonial', 'Calle El Conde #123, Zona Colonial, Santo Domingo', '809-555-1234', '07:00', '22:00', 1, GETDATE()),
    (2, 'Café San Juan - Piantini', 'Av. Abraham Lincoln #456, Piantini, Santo Domingo', '809-555-5678', '08:00', '23:00', 1, GETDATE());
 
    SET IDENTITY_INSERT reservas.Restaurante OFF;
END
GO

-- Insertar mesas para Restaurante 1 (Zona Colonial)
IF NOT EXISTS (SELECT 1 FROM reservas.Mesa WHERE IdRestaurante = 1)
BEGIN
    INSERT INTO reservas.Mesa (IdRestaurante, NumeroMesa, Capacidad, Ubicacion, Disponible, FechaCreacion)
    VALUES 
    (1, 1, 2, 'Terraza', 1, GETDATE()),
    (1, 2, 4, 'Terraza', 1, GETDATE()),
    (1, 3, 4, 'Interior', 1, GETDATE()),
    (1, 4, 6, 'Interior', 1, GETDATE()),
    (1, 5, 2, 'Ventana', 1, GETDATE()),
    (1, 6, 4, 'Ventana', 1, GETDATE()),
    (1, 7, 8, 'Salón VIP', 1, GETDATE()),
    (1, 8, 2, 'Barra', 1, GETDATE());
END
GO

-- Insertar mesas para Restaurante 2 (Piantini)
IF NOT EXISTS (SELECT 1 FROM reservas.Mesa WHERE IdRestaurante = 2)
BEGIN
    INSERT INTO reservas.Mesa (IdRestaurante, NumeroMesa, Capacidad, Ubicacion, Disponible, FechaCreacion)
    VALUES 
    (2, 1, 4, 'Terraza', 1, GETDATE()),
    (2, 2, 4, 'Terraza', 1, GETDATE()),
    (2, 3, 6, 'Interior', 1, GETDATE()),
    (2, 4, 2, 'Interior', 1, GETDATE()),
    (2, 5, 4, 'Ventana', 1, GETDATE()),
    (2, 6, 6, 'Ventana', 1, GETDATE()),
    (2, 7, 10, 'Salón VIP', 1, GETDATE()),
    (2, 8, 2, 'Barra', 1, GETDATE());
END
GO

PRINT 'Base de datos inicializada correctamente';
PRINT 'Restaurantes creados: 2';
PRINT 'Mesas creadas por restaurante: 8';
PRINT '';
PRINT 'Ahora puedes probar las APIs usando el archivo test-apis.http';
GO
