-- ============================================================
-- SCRIPT PARA AGREGAR IDRESERVA A TABLA FACTURA
-- ============================================================

USE [GestionReservas]
GO

PRINT '?? ACTUALIZANDO ESTRUCTURA DE TABLA FACTURA...'
PRINT '?? Fecha de ejecución: ' + CONVERT(VARCHAR, GETDATE(), 103) + ' a las ' + CONVERT(VARCHAR, GETDATE(), 108)
PRINT ''

BEGIN TRY
    BEGIN TRANSACTION

    -- Agregar columna IdReserva si no existe
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Factura') AND name = 'IdReserva')
    BEGIN
        ALTER TABLE Factura ADD IdReserva INT NULL
    PRINT '? Columna IdReserva agregada a tabla Factura'
    END
 ELSE
    BEGIN
        PRINT '? Columna IdReserva ya existe en tabla Factura'
    END

    -- Agregar restricción de clave foránea si no existe
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Factura_Reserva')
    BEGIN
        ALTER TABLE Factura 
    ADD CONSTRAINT FK_Factura_Reserva 
        FOREIGN KEY (IdReserva) REFERENCES Reserva(IdReserva)
        PRINT '? Clave foránea FK_Factura_Reserva agregada'
    END
    ELSE
    BEGIN
        PRINT '? Clave foránea FK_Factura_Reserva ya existe'
    END

    COMMIT TRANSACTION
    PRINT '? Transacción completada exitosamente'

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION
  PRINT '? Error durante la ejecución:'
    PRINT 'Error: ' + ERROR_MESSAGE()
    PRINT 'Línea: ' + CAST(ERROR_LINE() AS VARCHAR)
    PRINT 'Número: ' + CAST(ERROR_NUMBER() AS VARCHAR)
END CATCH

PRINT ''
PRINT '?? VERIFICANDO ESTRUCTURA ACTUALIZADA...'

-- Verificar nueva estructura de tabla Factura
PRINT '?? Estructura actualizada de tabla Factura:'
SELECT 
    COLUMN_NAME as 'Columna',
    DATA_TYPE as 'Tipo',
    IS_NULLABLE as 'Permite NULL',
    ISNULL(COLUMN_DEFAULT, 'Sin default') as 'Valor por defecto'
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Factura'
ORDER BY ORDINAL_POSITION

PRINT ''
PRINT '?? Verificando claves foráneas:'
SELECT 
    fk.name as 'Nombre_FK',
    t1.name as 'Tabla_Origen',
    c1.name as 'Columna_Origen',
  t2.name as 'Tabla_Destino',
    c2.name as 'Columna_Destino'
FROM sys.foreign_keys fk
INNER JOIN sys.tables t1 ON fk.parent_object_id = t1.object_id
INNER JOIN sys.tables t2 ON fk.referenced_object_id = t2.object_id
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.columns c1 ON fkc.parent_column_id = c1.column_id AND fkc.parent_object_id = c1.object_id
INNER JOIN sys.columns c2 ON fkc.referenced_column_id = c2.column_id AND fkc.referenced_object_id = c2.object_id
WHERE t1.name = 'Factura'

PRINT ''
PRINT '? ACTUALIZACIÓN COMPLETADA'
PRINT ''
PRINT '?? CAMBIOS REALIZADOS:'
PRINT '   • Columna IdReserva agregada a tabla Factura'
PRINT '   • Clave foránea agregada hacia tabla Reserva'
PRINT '   • Soporte para prevenir facturas duplicadas'
PRINT '   • Mejora en trazabilidad de reservas facturadas'