-- =====================================================
-- ACTUALIZAR STORED PROCEDURE PARA INCLUIR CÉDULA
-- =====================================================
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

-- Crear o actualizar SP de actualización de usuario
ALTER PROCEDURE sp_actualizar_usuario
    @IdUsuario   INT,
    @Nombre      VARCHAR(100) = NULL,
    @Email       VARCHAR(100) = NULL,
    @Telefono    VARCHAR(20) = NULL,
    @Direccion   VARCHAR(150) = NULL,
 @Cedula      VARCHAR(20) = NULL,    -- ?? NUEVO PARÁMETRO
    @Rol         VARCHAR(50) = NULL,
    @Estado      VARCHAR(20) = NULL,
    @Contrasena  VARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar si el usuario existe
  IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE IdUsuario = @IdUsuario)
    BEGIN
   RAISERROR('El usuario especificado no existe.', 16, 1);
        RETURN;
    END;

    -- Verificar si el nuevo email ya está en uso (por otro usuario)
    IF @Email IS NOT NULL AND EXISTS (
        SELECT 1 FROM dbo.Usuario 
 WHERE Email = @Email AND IdUsuario <> @IdUsuario
    )
    BEGIN
  RAISERROR('El correo electrónico ya está en uso por otro usuario.', 16, 1);
        RETURN;
    END;

    -- Verificar si la nueva cédula ya está en uso (por otro usuario)
    IF @Cedula IS NOT NULL AND EXISTS (
        SELECT 1 FROM dbo.Usuario 
   WHERE Cedula = @Cedula AND IdUsuario <> @IdUsuario
    )
    BEGIN
   RAISERROR('La cédula ya está en uso por otro usuario.', 16, 1);
        RETURN;
    END;

    -- Actualizar solo los campos que no son NULL
    UPDATE dbo.Usuario
  SET 
     Nombre = ISNULL(@Nombre, Nombre),
   Email = ISNULL(@Email, Email),
        Telefono = ISNULL(@Telefono, Telefono),
      Direccion = ISNULL(@Direccion, Direccion),
  Cedula = ISNULL(@Cedula, Cedula),      -- ?? NUEVA ACTUALIZACIÓN
   Rol = ISNULL(@Rol, Rol),
  Estado = ISNULL(@Estado, Estado),
        Contrasena = ISNULL(@Contrasena, Contrasena)
    WHERE IdUsuario = @IdUsuario;

    PRINT 'Usuario actualizado correctamente.';
END;
GO

-- Verificar que el stored procedure se creó correctamente
PRINT 'Stored procedure sp_actualizar_usuario actualizado con soporte para cédula.';