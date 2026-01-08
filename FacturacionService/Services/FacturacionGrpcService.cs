using Grpc.Core;
using FacturacionService.Models;
using FacturacionService.Data;
using FacturacionService.Protos;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace FacturacionService.Services
{
    public class FacturacionGrpcService : FacturacionGrpc.FacturacionGrpcBase
    {
        private readonly ILogger<FacturacionGrpcService> _logger;
        private readonly FacturaDAO _facturaDAO;
        private readonly DetalleFacturaDAO _detalleFacturaDAO;

     public FacturacionGrpcService(ILogger<FacturacionGrpcService> logger, string connectionString)
     {
         _logger = logger;
      _facturaDAO = new FacturaDAO(connectionString);
       _detalleFacturaDAO = new DetalleFacturaDAO(connectionString);
 }

        // RPC: Generar una nueva factura
        public override Task<GenerarFacturaResponse> GenerarFactura(
     GenerarFacturaRequest request,
ServerCallContext context)
 {
   try
     {
           _logger.LogInformation($"Generando factura para usuario {request.IdUsuario}, reserva {request.IdReserva}");

          // Validaciones
  if (request.IdUsuario <= 0)
       return Task.FromResult(new GenerarFacturaResponse
    {
       Success = false,
  Mensaje = "ID de usuario no válido"
        });

            if (request.IdReserva <= 0)
   return Task.FromResult(new GenerarFacturaResponse
      {
    Success = false,
  Mensaje = "ID de reserva no válido"
    });

      // Crear factura
   var factura = new Models.Factura
      {
 IdUsuario = request.IdUsuario,
    IdReserva = request.IdReserva,
   FechaEmision = DateTime.Now,
 Subtotal = (decimal)request.Subtotal,
  IVA = (decimal)request.Subtotal * 0.13m, // 13% IVA Costa Rica
         Total = (decimal)request.Subtotal * 1.13m,
    Estado = "Emitida",
  NumeroFactura = $"FAC-{DateTime.Now:yyyyMMddHHmmss}",
    MetodoPago = request.MetodoPago
    };

 factura = _facturaDAO.CrearFactura(factura);

             return Task.FromResult(new GenerarFacturaResponse
        {
          Success = true,
   Mensaje = "Factura generada correctamente",
        IdFactura = factura.IdFactura,
          Factura = MapearFacturaProto(factura)
             });
     }
            catch (Exception ex)
       {
    _logger.LogError($"Error al generar factura: {ex.Message}");
      return Task.FromResult(new GenerarFacturaResponse
   {
 Success = false,
     Mensaje = $"Error: {ex.Message}"
      });
            }
 }

        // RPC: Listar todas las facturas
        public override Task<ListarFacturasResponse> ListarFacturas(
       ListarFacturasRequest request,
      ServerCallContext context)
     {
         try
            {
   _logger.LogInformation("Listando todas las facturas");

                var facturas = _facturaDAO.ListarTodasFacturas();
      var facturasProto = facturas.Select(MapearFacturaProto).ToList();

      return Task.FromResult(new ListarFacturasResponse
     {
              Success = true,
               Mensaje = "Facturas listadas correctamente",
    Facturas = { facturasProto }
        });
      }
    catch (Exception ex)
            {
     _logger.LogError($"Error al listar facturas: {ex.Message}");
   return Task.FromResult(new ListarFacturasResponse
     {
 Success = false,
       Mensaje = $"Error: {ex.Message}"
  });
            }
     }

 // RPC: Obtener factura
        public override Task<ObtenerFacturaResponse> ObtenerFactura(
    ObtenerFacturaRequest request,
 ServerCallContext context)
        {
            try
 {
 _logger.LogInformation($"Obteniendo factura {request.IdFactura}");

       if (request.IdFactura <= 0)
      return Task.FromResult(new ObtenerFacturaResponse
       {
      Success = false,
      Mensaje = "ID de factura no válido"
        });

      var factura = _facturaDAO.ObtenerFacturaById(request.IdFactura);

  if (factura == null)
                return Task.FromResult(new ObtenerFacturaResponse
     {
    Success = false,
   Mensaje = "Factura no encontrada"
                 });

              var detalles = _detalleFacturaDAO.ObtenerDetallesFactura(request.IdFactura);
       var detallesProto = detalles.Select(MapearDetalleFacturaProto).ToList();

       return Task.FromResult(new ObtenerFacturaResponse
 {
         Success = true,
       Mensaje = "Factura obtenida correctamente",
                Factura = MapearFacturaProto(factura),
       Detalles = { detallesProto }
     });
            }
            catch (Exception ex)
      {
         _logger.LogError($"Error al obtener factura: {ex.Message}");
       return Task.FromResult(new ObtenerFacturaResponse
             {
        Success = false,
                    Mensaje = $"Error: {ex.Message}"
   });
            }
}

        // RPC: Marcar factura como pagada
        public override Task<MarcarFacturaPagadaResponse> MarcarFacturaPagada(
       MarcarFacturaPagadaRequest request,
            ServerCallContext context)
        {
      try
            {
 _logger.LogInformation($"Marcando factura {request.IdFactura} como pagada");

                if (request.IdFactura <= 0)
     return Task.FromResult(new MarcarFacturaPagadaResponse
   {
   Success = false,
  Mensaje = "ID de factura no válido"
       });

       var resultado = _facturaDAO.MarcarComoPageda(request.IdFactura, request.MetodoPago);

           if (!resultado)
          return Task.FromResult(new MarcarFacturaPagadaResponse
      {
              Success = false,
          Mensaje = "No se pudo marcar la factura como pagada"
        });

     return Task.FromResult(new MarcarFacturaPagadaResponse
    {
   Success = true,
 Mensaje = "Factura marcada como pagada",
      Estado = "Pagada"
              });
            }
            catch (Exception ex)
      {
_logger.LogError($"Error al marcar como pagada: {ex.Message}");
                return Task.FromResult(new MarcarFacturaPagadaResponse
            {
Success = false,
          Mensaje = $"Error: {ex.Message}"
                });
       }
      }

// RPC: Anular factura
        public override Task<AnularFacturaResponse> AnularFactura(
     AnularFacturaRequest request,
  ServerCallContext context)
     {
            try
     {
     _logger.LogInformation($"Anulando factura {request.IdFactura}");

                if (request.IdFactura <= 0)
      return Task.FromResult(new AnularFacturaResponse
  {
   Success = false,
      Mensaje = "ID de factura no válido"
   });

       _facturaDAO.AnularFactura(request.IdFactura);

       return Task.FromResult(new AnularFacturaResponse
           {
    Success = true,
          Mensaje = "Factura anulada correctamente"
         });
       }
          catch (Exception ex)
     {
          _logger.LogError($"Error al anular factura: {ex.Message}");
      return Task.FromResult(new AnularFacturaResponse
        {
      Success = false,
     Mensaje = $"Error: {ex.Message}"
     });
   }
   }

    // RPC: Listar facturas de un usuario
 public override Task<ListarFacturasUsuarioResponse> ListarFacturasUsuario(
     ListarFacturasUsuarioRequest request,
ServerCallContext context)
        {
    try
    {
       _logger.LogInformation($"Listando facturas del usuario {request.IdUsuario}");

       if (request.IdUsuario <= 0)
    return Task.FromResult(new ListarFacturasUsuarioResponse
  {
   Success = false,
   Mensaje = "ID de usuario no válido"
     });

  var facturas = _facturaDAO.ListarTodasFacturas().Where(f => f.IdUsuario == request.IdUsuario).ToList();
     var facturasProto = facturas.Select(f => MapearFacturaProto(f)).ToList();

      return Task.FromResult(new ListarFacturasUsuarioResponse
       {
Success = true,
         Mensaje = "Facturas del usuario listadas",
     Facturas = { facturasProto }
      });
       }
    catch (Exception ex)
   {
_logger.LogError($"Error al listar facturas del usuario: {ex.Message}");
        return Task.FromResult(new ListarFacturasUsuarioResponse
         {
    Success = false,
  Mensaje = $"Error: {ex.Message}"
 });
 }
        }

        // RPC: Obtener factura detallada
        public override Task<ObtenerFacturaDetalladaResponse> ObtenerFacturaDetallada(
          ObtenerFacturaDetalladaRequest request,
 ServerCallContext context)
   {
            try
     {
             _logger.LogInformation($"Obteniendo factura detallada {request.IdFactura}");

      if (request.IdFactura <= 0)
        return Task.FromResult(new ObtenerFacturaDetalladaResponse
         {
      Success = false,
        Mensaje = "ID de factura no válido"
              });

                var factura = _facturaDAO.ObtenerFacturaById(request.IdFactura);

       if (factura == null)
       return Task.FromResult(new ObtenerFacturaDetalladaResponse
   {
    Success = false,
       Mensaje = "Factura no encontrada"
           });

      var detalles = _detalleFacturaDAO.ObtenerDetallesFactura(request.IdFactura);
 var detallesProto = detalles.Select(MapearDetalleFacturaProto).ToList();

                return Task.FromResult(new ObtenerFacturaDetalladaResponse
     {
      Success = true,
 Mensaje = "Factura detallada obtenida",
   Factura = MapearFacturaProto(factura),
       Detalles = { detallesProto },
          Usuario = new Protos.Usuario { IdUsuario = factura.IdUsuario }
        });
  }
            catch (Exception ex)
    {
           _logger.LogError($"Error al obtener factura detallada: {ex.Message}");
     return Task.FromResult(new ObtenerFacturaDetalladaResponse
         {
           Success = false,
   Mensaje = $"Error: {ex.Message}"
  });
            }
 }

        // RPC: Calcular totales
  public override Task<CalcularTotalesResponse> CalcularTotales(
CalcularTotalesRequest request,
 ServerCallContext context)
        {
      try
      {
      _logger.LogInformation("Calculando totales de factura");

   if (request.Factura == null)
     return Task.FromResult(new CalcularTotalesResponse
          {
  Success = false
           });

  var factura = request.Factura;
   float porcentajeIVA = request.PorcentajeIva > 0 ? request.PorcentajeIva : 0.13f;

      factura.Iva = factura.Subtotal * porcentajeIVA;
      factura.Total = factura.Subtotal + factura.Iva;

    return Task.FromResult(new CalcularTotalesResponse
            {
   Success = true,
     Factura = factura
          });
    }
     catch (Exception ex)
     {
       _logger.LogError($"Error al calcular totales: {ex.Message}");
        return Task.FromResult(new CalcularTotalesResponse
        {
      Success = false
       });
     }
  }

        // Métodos auxiliares para mapeo
        private Protos.Factura MapearFacturaProto(Models.Factura factura)
        {
            return new Protos.Factura
        {
       IdFactura = factura.IdFactura,
    IdUsuario = factura.IdUsuario,
                IdReserva = factura.IdReserva ?? 0,
     FechaEmision = factura.FechaEmision.ToString("yyyy-MM-dd"),
     Subtotal = (float)factura.Subtotal,
   Iva = (float)factura.IVA,
        Total = (float)factura.Total,
                Estado = factura.Estado,
                NumeroFactura = factura.NumeroFactura,
       MetodoPago = factura.MetodoPago ?? string.Empty,
        FechaPago = factura.FechaPago?.ToString("yyyy-MM-dd") ?? string.Empty
    };
      }

    private Protos.DetalleFactura MapearDetalleFacturaProto(Models.DetalleFactura detalle)
        {
       return new Protos.DetalleFactura
            {
   IdDetalleFactura = detalle.IdDetalleFactura,
       IdFactura = detalle.IdFactura,
IdReserva = detalle.IdReserva ?? 0,
       Descripcion = detalle.Descripcion ?? string.Empty,
       Cantidad = detalle.Cantidad,
PrecioUnitario = (float)detalle.PrecioUnitario,
                Subtotal = (float)detalle.Subtotal
    };
        }
    }
}
