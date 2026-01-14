-- ============================================================
-- Script para corregir sp_listar_carrito_reservas
-- Actualiza las referencias de tablas para usar los esquemas correctos
-- ============================================================

USE [nombre_de_tu_base_de_datos]; -- REEMPLAZAR con el nombre de tu BD
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Eliminar el procedimiento si existe
IF OBJECT_ID('dbo.sp_listar_carrito_reservas', 'P') IS NOT NULL
 DROP PROCEDURE dbo.sp_listar_carrito_reservas;
GO

-- Crear el procedimiento corregido
CREATE PROCEDURE [dbo].[sp_listar_carrito_reservas]
    @IdUsuario INT,
    @PromocionId INT = NULL -- Promoción seleccionada (opcional)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validar que el usuario existe (Usuario está en schema seguridad)
        IF NOT EXISTS (SELECT 1 FROM seguridad.Usuario WHERE IdUsuario = @IdUsuario)
        BEGIN
SELECT 
          'ERROR' as Estado,
        'Usuario no encontrado' as Mensaje;
            RETURN;
        END

        -- Variables para cálculos
        DECLARE @DescuentoPromocion DECIMAL(5,2) = 0;
 
        -- Si se especifica una promoción, validar que esté activa (Promocion está en schema menu)
        IF @PromocionId IS NOT NULL
        BEGIN
            SELECT @DescuentoPromocion = Descuento
            FROM menu.Promocion
            WHERE IdPromocion = @PromocionId
      AND Estado = 'Activa'
AND CAST(GETDATE() AS DATE) >= CAST(FechaInicio AS DATE)
     AND CAST(GETDATE() AS DATE) <= CAST(FechaFin AS DATE);
      
      IF @DescuentoPromocion IS NULL
          SET @DescuentoPromocion = 0;
  END

  -- Traer las reservas en estado HOLD del usuario con cálculos de promoción
        -- (Reserva y Mesa están en schema reservas)
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

        FROM reservas.Reserva r
   INNER JOIN reservas.Mesa m ON r.IdMesa = m.IdMesa
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

        FROM reservas.Reserva r
   INNER JOIN reservas.Mesa m ON r.IdMesa = m.IdMesa
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

-- Verificar que el procedimiento se creó correctamente
PRINT 'Procedimiento dbo.sp_listar_carrito_reservas corregido exitosamente';
GO
