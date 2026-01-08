-- ============================================================
-- SCRIPT DE DEPURACIÓN - VERIFICAR RESERVAS Y PRECIOS
-- ============================================================

USE [GestionReservas]
GO

PRINT '?? VERIFICANDO RESERVAS EN CARRITO...'
PRINT ''

-- Verificar todas las reservas del usuario
DECLARE @IdUsuario INT = 2 -- Cambiar por el ID del usuario que está probando

PRINT '?? RESERVAS DEL USUARIO ' + CAST(@IdUsuario AS VARCHAR) + ':'
SELECT 
    r.IdReserva,
    r.IdMesa,
    m.NumeroMesa,
    r.Fecha,
    r.Hora,
    r.NumeroPersonas,
    r.Estado,
 r.Total,
    r.MetodoPago,
    r.FechaCreacion
FROM Reserva r
INNER JOIN Mesa m ON r.IdMesa = m.IdMesa
WHERE r.IdUsuario = @IdUsuario
ORDER BY r.FechaCreacion DESC

PRINT ''

-- Verificar reservas específicamente en estado HOLD o CONFIRMADA
PRINT '?? RESERVAS EN CARRITO (HOLD o CONFIRMADA):'
SELECT 
    r.IdReserva,
  m.NumeroMesa,
    r.Fecha,
    r.Hora,
    r.NumeroPersonas,
    r.Estado,
    ISNULL(r.Total, 0) as Total,
    CASE 
  WHEN r.Total IS NULL THEN '? SIN PRECIO'
        WHEN r.Total = 0 THEN '?? PRECIO CERO'
        WHEN r.Total > 0 THEN '? PRECIO VÁLIDO'
  ELSE '? DESCONOCIDO'
    END as EstadoPrecio
FROM Reserva r
INNER JOIN Mesa m ON r.IdMesa = m.IdMesa
WHERE r.IdUsuario = @IdUsuario 
    AND r.Estado IN ('HOLD', 'CONFIRMADA')
ORDER BY r.FechaCreacion DESC

PRINT ''

-- Verificar precios de mesas (usando columnas reales)
PRINT '?? ESTRUCTURA DE MESAS:'
SELECT 
    m.IdMesa,
    m.NumeroMesa,
    m.Estado,
    'Usar NumeroPersonas de Reserva para calcular precio' as NotaPrecio
FROM Mesa m
ORDER BY m.NumeroMesa

PRINT ''

-- Verificar lógica de precios basada en número de personas
PRINT '?? LÓGICA DE PRECIOS POR PERSONAS:'
SELECT 
    r.IdReserva,
    m.NumeroMesa,
  r.NumeroPersonas,
  r.Estado,
  ISNULL(r.Total, 0) as TotalActual,
    CASE 
        WHEN r.NumeroPersonas <= 2 THEN 25.00
        WHEN r.NumeroPersonas <= 4 THEN 35.00
        WHEN r.NumeroPersonas <= 6 THEN 45.00
        ELSE 55.00
    END as PrecioSugerido,
    CASE 
  WHEN r.Total IS NULL THEN '? SIN PRECIO'
        WHEN r.Total = 0 THEN '?? PRECIO CERO'
        WHEN r.Total > 0 THEN '? PRECIO VÁLIDO'
  ELSE '? DESCONOCIDO'
    END as EstadoPrecio
FROM Reserva r
INNER JOIN Mesa m ON r.IdMesa = m.IdMesa
WHERE r.IdUsuario = @IdUsuario 
    AND r.Estado IN ('HOLD', 'CONFIRMADA')
ORDER BY r.FechaCreacion DESC

PRINT ''

-- Verificar si hay una reserva específica problemática
PRINT '?? DIAGNÓSTICO ESPECÍFICO:'

-- Buscar la última reserva del usuario
DECLARE @UltimaReserva INT
SELECT TOP 1 @UltimaReserva = IdReserva 
FROM Reserva 
WHERE IdUsuario = @IdUsuario 
ORDER BY FechaCreacion DESC

IF @UltimaReserva IS NOT NULL
BEGIN
    PRINT 'Analizando reserva ID: ' + CAST(@UltimaReserva AS VARCHAR)
    
    SELECT 
        r.IdReserva,
        'Mesa: ' + CAST(m.NumeroMesa AS VARCHAR) as Mesa,
'Fecha: ' + CONVERT(VARCHAR, r.Fecha, 103) as Fecha,
        'Hora: ' + r.Hora as Hora,
        'Personas: ' + CAST(r.NumeroPersonas AS VARCHAR) as Personas,
      'Estado: ' + r.Estado as Estado,
        'Total Reserva: $' + CAST(ISNULL(r.Total, 0) AS VARCHAR) as TotalReserva,
        'Precio Mesa: $' + CAST(ISNULL(m.PrecioReserva, 0) AS VARCHAR) as PrecioMesa,
  'Usuario: ' + CAST(r.IdUsuario AS VARCHAR) as Usuario
    FROM Reserva r
    INNER JOIN Mesa m ON r.IdMesa = m.IdMesa
    WHERE r.IdReserva = @UltimaReserva
END
ELSE
BEGIN
    PRINT '? No se encontraron reservas para el usuario'
END

PRINT ''
PRINT '?? POSIBLES PROBLEMAS:'
PRINT '   • Reservas sin precio (Total = NULL o 0)'
PRINT '   • Mesas sin precio configurado'
PRINT '   • Reservas en estado incorrecto'
PRINT '   • Usuario incorrecto'