SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Eliminar el procedimiento si existe
IF OBJECT_ID('seguridad.sp_login_usuario', 'P') IS NOT NULL
    DROP PROCEDURE seguridad.sp_login_usuario;
GO

-- Crear el procedimiento en el schema correcto
CREATE PROCEDURE seguridad.sp_login_usuario
    @Email       VARCHAR(100),
    @Contrasena  VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        IdUsuario,
        Nombre,
        Email,
        Cedula,
        Rol,
        Estado,
        Telefono,
        Direccion
    FROM seguridad.Usuario
    WHERE Email = @Email
      AND Contrasena = @Contrasena;
END;
GO
