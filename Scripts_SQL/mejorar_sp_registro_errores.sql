-- =====================================================
-- MEJORAR STORED PROCEDURE DE REGISTRO CON MANEJO DE ERRORES
-- =====================================================
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE dbo.sp_registrar_usuario
    @Nombre      VARCHAR(100),
    @Email       VARCHAR(100),
    @Contrasena  VARCHAR(100),
    @Telefono    VARCHAR(20),
 @Direccion   VARCHAR(150),
    @Cedula      VARCHAR(20),
    @Rol    VARCHAR(50) = 'CLIENTE',
    @Estado      VARCHAR(20) = 'ACTIVO'
AS
BEGIN
    SET NOCOUNT ON;

-- Verificar si el correo ya está registrado
IF EXISTS (SELECT 1 FROM dbo.Usuario WHERE Email = @Email)
    BEGIN
  -- ?? USAR RAISERROR PARA MEJOR MANEJO DE ERRORES
        RAISERROR('Ya existe un usuario con este correo electrónico.', 16, 1);
        RETURN;
END;

    -- Verificar si la cédula ya está registrada
    IF EXISTS (SELECT 1 FROM dbo.Usuario WHERE Cedula = @Cedula)
    BEGIN
  RAISERROR('Ya existe un usuario con esta cédula.', 16, 1);
        RETURN;
  END;

    -- Insertar nuevo usuario
    INSERT INTO dbo.Usuario (Nombre, Email, Contrasena, Telefono, Direccion, Cedula, Rol, Estado)
    VALUES (@Nombre, @Email, @Contrasena, @Telefono, @Direccion, @Cedula, @Rol, @Estado);

    -- Mensaje de éxito
    PRINT 'Usuario registrado correctamente.';
END;
GO

PRINT 'Stored procedure sp_registrar_usuario actualizado con mejor manejo de errores.';