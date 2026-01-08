SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Eliminar el procedimiento si existe
IF OBJECT_ID('seguridad.sp_actualizar_usuario', 'P') IS NOT NULL
  DROP PROCEDURE seguridad.sp_actualizar_usuario;
GO

-- Crear el procedimiento almacenado para actualizar usuarios
CREATE PROCEDURE seguridad.sp_actualizar_usuario
    @IdUsuario   INT,
    @Nombre      VARCHAR(100) = NULL,
    @Email       VARCHAR(100) = NULL,
    @Telefono    VARCHAR(20) = NULL,
    @Direccion   VARCHAR(150) = NULL,
    @Cedula      VARCHAR(20) = NULL,
    @Rol   VARCHAR(50) = NULL,
    @Estado      VARCHAR(20) = NULL,
    @Contrasena  VARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar que el usuario existe
    IF NOT EXISTS (SELECT 1 FROM seguridad.Usuario WHERE IdUsuario = @IdUsuario)
    BEGIN
        RAISERROR('El usuario no existe', 16, 1);
        RETURN;
    END

    -- Obtener datos actuales del usuario
    DECLARE @EmailActual VARCHAR(100);
    DECLARE @CedulaActual VARCHAR(20);
    
    SELECT 
 @EmailActual = Email,
        @CedulaActual = Cedula
    FROM seguridad.Usuario 
    WHERE IdUsuario = @IdUsuario;

    -- Verificar email duplicado SOLO si está cambiando el email
 IF @Email IS NOT NULL AND @Email <> @EmailActual
    BEGIN
    IF EXISTS (SELECT 1 FROM seguridad.Usuario WHERE Email = @Email AND IdUsuario <> @IdUsuario)
  BEGIN
          RAISERROR('El correo electrónico ya está en uso por otro usuario', 16, 1);
            RETURN;
        END
    END

    -- Verificar cédula duplicada SOLO si está cambiando la cédula
    IF @Cedula IS NOT NULL AND @Cedula <> '' AND @Cedula <> @CedulaActual
    BEGIN
   IF EXISTS (SELECT 1 FROM seguridad.Usuario WHERE Cedula = @Cedula AND IdUsuario <> @IdUsuario)
        BEGIN
            RAISERROR('La cédula ya está en uso por otro usuario', 16, 1);
 RETURN;
  END
    END

    -- Actualizar solo los campos que no son NULL
 UPDATE seguridad.Usuario
    SET 
      Nombre = ISNULL(@Nombre, Nombre),
        Email = ISNULL(@Email, Email),
        Telefono = ISNULL(@Telefono, Telefono),
        Direccion = ISNULL(@Direccion, Direccion),
 Cedula = ISNULL(@Cedula, Cedula),
      Rol = ISNULL(@Rol, Rol),
        Estado = ISNULL(@Estado, Estado),
        Contrasena = ISNULL(@Contrasena, Contrasena)
    WHERE IdUsuario = @IdUsuario;

    -- Retornar el usuario actualizado
    SELECT 
        IdUsuario,
        Nombre,
   Email,
      Cedula,
        Rol,
        Estado,
      Telefono,
     Direccion,
        TipoIdentificacion
    FROM seguridad.Usuario
    WHERE IdUsuario = @IdUsuario;
END;
GO
