-- =============================================
-- Autor: Sistema
-- Fecha Creación: 2024
-- Descripción: Stored Procedure para listar reservas en estado HOLD del usuario (carrito)
--        Incluye cálculos de Subtotal, Descuentos, IVA (11.5% Puerto Rico) y Total
-- =============================================

USE CafeSanJuan;
GO

-- Eliminar el SP si ya existe
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_listar_carrito_reservas]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_listar_carrito_reservas]
GO

CREATE PROCEDURE [dbo].[sp_listar_carrito_reservas]
    @IdUsuario INT,
    @PromocionId INT = NULL -- Promoción seleccionada (opcional)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
  -- Validar que el usuario existe
IF NOT EXISTS (SELECT 1 FROM Usuario WHERE IdUsuario = @IdUsuario)
        BEGIN
     SELECT 
       'ERROR' as Estado,
    'Usuario no encontrado' as Mensaje;
RETURN;
      END

   -- Variables para cálculos
        DECLARE @DescuentoPromocion DECIMAL(5,2) = 0;
 
        -- Si se especifica una promoción, validar que esté activa
        IF @PromocionId IS NOT NULL
 BEGIN
   SELECT @DescuentoPromocion = Descuento
    FROM Promocion
            WHERE IdPromocion = @PromocionId
   AND Estado = 'Activa'
      AND CAST(GETDATE() AS DATE) >= CAST(FechaInicio AS DATE)
      AND CAST(GETDATE() AS DATE) <= CAST(FechaFin AS DATE);
            
        IF @DescuentoPromocion IS NULL
       SET @DescuentoPromocion = 0;
        END

        -- Traer las reservas en estado HOLD del usuario con cálculos de promoción
        SELECT 
 r.IdReserva,
        r.IdUsuario,
         r.IdMesa,
  m.NumeroMesa,
            m.Capacidad as CapacidadMesa,
         m.Precio as PrecioMesa,
    r.Fecha,
r.Hora,
      r.NumeroPersonas,
          r.Estado,
r.Observaciones,
            r.MetodoPago,
            r.MontoDescuento,
         r.Total,
            
   -- Cálculos con promoción
    CASE 
                WHEN r.Total IS NOT NULL AND r.Total > 0 THEN r.Total
       ELSE m.Precio 
       END as Subtotal,

        -- Descuento aplicado
       @PromocionId as PromocionSeleccionada,
        @DescuentoPromocion as PorcentajeDescuento,
    ROUND((CASE 
         WHEN r.Total IS NOT NULL AND r.Total > 0 THEN r.Total
     ELSE m.Precio 
       END) * (@DescuentoPromocion / 100), 2) as MontoDescuentoCalculado,
   
       -- Subtotal después del descuento
    ROUND((CASE 
     WHEN r.Total IS NOT NULL AND r.Total > 0 THEN r.Total
   ELSE m.Precio 
        END) * (1 - @DescuentoPromocion / 100), 2) as SubtotalConDescuento,

            -- IVA sobre el subtotal con descuento
   ROUND((CASE 
     WHEN r.Total IS NOT NULL AND r.Total > 0 THEN r.Total
     ELSE m.Precio 
   END) * (1 - @DescuentoPromocion / 100) * 0.115, 2) as IVA,
    
     -- Total final (Subtotal con descuento + IVA)
   ROUND((CASE 
      WHEN r.Total IS NOT NULL AND r.Total > 0 THEN r.Total
         ELSE m.Precio 
   END) * (1 - @DescuentoPromocion / 100) * 1.115, 2) as TotalFinal

        FROM Reserva r
        INNER JOIN Mesa m ON r.IdMesa = m.IdMesa
    WHERE r.IdUsuario = @IdUsuario 
      AND r.Estado = 'HOLD'
      ORDER BY r.Fecha DESC, r.Hora DESC;

      -- Retornar resumen del carrito con descuentos
        SELECT 
            COUNT(*) as TotalReservas,
    
            -- Subtotal (sin descuento ni IVA)
      ISNULL(SUM(CASE 
    WHEN r.Total IS NOT NULL AND r.Total > 0 THEN r.Total
     ELSE m.Precio 
            END), 0) as Subtotal,
     
      -- Descuento total
      @PromocionId as PromocionAplicada,
  @DescuentoPromocion as PorcentajeDescuento,
         ROUND(ISNULL(SUM(CASE 
            WHEN r.Total IS NOT NULL AND r.Total > 0 THEN r.Total
      ELSE m.Precio 
    END), 0) * (@DescuentoPromocion / 100), 2) as MontoDescuentoTotal,
    
     -- Subtotal con descuento
     ROUND(ISNULL(SUM(CASE 
    WHEN r.Total IS NOT NULL AND r.Total > 0 THEN r.Total
         ELSE m.Precio 
            END), 0) * (1 - @DescuentoPromocion / 100), 2) as SubtotalConDescuento,
         
            -- IVA (11.5% sobre subtotal con descuento)
      ROUND(ISNULL(SUM(CASE 
    WHEN r.Total IS NOT NULL AND r.Total > 0 THEN r.Total
        ELSE m.Precio 
 END), 0) * (1 - @DescuentoPromocion / 100) * 0.115, 2) as IVA,
          
 -- Total final con descuento e IVA
      ROUND(ISNULL(SUM(CASE 
      WHEN r.Total IS NOT NULL AND r.Total > 0 THEN r.Total
      ELSE m.Precio 
    END), 0) * (1 - @DescuentoPromocion / 100) * 1.115, 2) as TotalCarrito

        FROM Reserva r
   INNER JOIN Mesa m ON r.IdMesa = m.IdMesa
        WHERE r.IdUsuario = @IdUsuario 
          AND r.Estado = 'HOLD';

    END TRY
    BEGIN CATCH
SELECT 
    'ERROR' as Estado,
            ERROR_MESSAGE() as Mensaje;
    END CATCH
END
GO

-- Dar permisos de ejecución
GRANT EXECUTE ON [dbo].[sp_listar_carrito_reservas] TO PUBLIC;
GO

PRINT 'Stored Procedure sp_listar_carrito_reservas actualizado con sistema de promociones';