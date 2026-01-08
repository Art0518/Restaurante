SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Eliminar el procedimiento si existe
IF OBJECT_ID('seguridad.sp_cambiar_estado_usuario', 'P') IS NOT NULL
  DROP PROCEDURE seguridad.sp_cambiar_estado_usuario;
GO

-- Crear el procedimiento almacenado para cambiar estado de usuario
CREATE PROCEDURE seguridad.sp_cambiar_estado_usuario
    @IdUsuario    INT,
    @NuevoEstado  VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

  -- Verificar que el usuario existe
    IF NOT EXISTS (SELECT 1 FROM seguridad.Usuario WHERE IdUsuario = @IdUsuario)
    BEGIN
      RAISERROR('El usuario no existe', 16, 1);
  RETURN;
    END

    -- Validar el estado
    IF @NuevoEstado NOT IN ('ACTIVO', 'INACTIVO', 'BLOQUEADO', 'SUSPENDIDO')
    BEGIN
        RAISERROR('Estado inválido. Debe ser: ACTIVO, INACTIVO, BLOQUEADO o SUSPENDIDO', 16, 1);
   RETURN;
    END

 -- Actualizar el estado
    UPDATE seguridad.Usuario
    SET 
        Estado = @NuevoEstado,
        FechaModificacion = GETDATE()
  WHERE IdUsuario = @IdUsuario;

    -- Mensaje de confirmación
    SELECT 
        IdUsuario,
        Nombre,
        Email,
    Estado,
        FechaModificacion
    FROM seguridad.Usuario
 WHERE IdUsuario = @IdUsuario;
END;
GO
