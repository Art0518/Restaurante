SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Eliminar el procedimiento si existe
IF OBJECT_ID('seguridad.sp_listar_usuarios', 'P') IS NOT NULL
  DROP PROCEDURE seguridad.sp_listar_usuarios;
GO

-- Crear el procedimiento en el schema correcto con paginación
CREATE PROCEDURE seguridad.sp_listar_usuarios
    @Rol        VARCHAR(50) = NULL,
    @Estado     VARCHAR(20) = NULL,
    @Pagina      INT = 1,    -- Página actual (por defecto 1)
    @TamanoPagina INT = 50       -- Registros por página (por defecto 50)
AS
BEGIN
    SET NOCOUNT ON;

    -- Calcular el offset
    DECLARE @Offset INT = (@Pagina - 1) * @TamanoPagina;

    -- Retornar el total de registros (para saber cuántas páginas hay)
    SELECT COUNT(*) AS TotalRegistros
    FROM seguridad.Usuario
    WHERE 
        (@Rol IS NULL OR Rol = @Rol)
        AND (@Estado IS NULL OR Estado = @Estado);

    -- Retornar los registros paginados
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
    WHERE 
        (@Rol IS NULL OR Rol = @Rol)
        AND (@Estado IS NULL OR Estado = @Estado)
    ORDER BY IdUsuario DESC
    OFFSET @Offset ROWS
    FETCH NEXT @TamanoPagina ROWS ONLY;
END;
GO
