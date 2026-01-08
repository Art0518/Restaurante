using Grpc.Net.Client;
using FacturacionService.Protos;
using System;
using System.Threading.Tasks;

namespace GrpcClients.Clients
{
    public class FacturacionGrpcClient
    {
 private readonly GrpcChannel _channel;
        private readonly FacturacionGrpc.FacturacionGrpcClient _client;

      public FacturacionGrpcClient(string serviceUrl)
        {
            // Permitir llamadas HTTP/2 sin encriptación (para desarrollo)
  AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            
     _channel = GrpcChannel.ForAddress(serviceUrl);
    _client = new FacturacionGrpc.FacturacionGrpcClient(_channel);
        }

        // Generar factura
        public async Task<GenerarFacturaResponse> GenerarFacturaAsync(int idUsuario, int idReserva, float subtotal, string metodoPago)
    {
  try
     {
          var request = new GenerarFacturaRequest
           {
           IdUsuario = idUsuario,
            IdReserva = idReserva,
 Subtotal = subtotal,
      MetodoPago = metodoPago
       };

    return await _client.GenerarFacturaAsync(request);
            }
 catch (Exception ex)
            {
      throw new Exception($"Error al generar factura: {ex.Message}", ex);
            }
      }

        // Listar todas las facturas
   public async Task<ListarFacturasResponse> ListarFacturasAsync()
        {
            try
   {
 var request = new ListarFacturasRequest();
     return await _client.ListarFacturasAsync(request);
            }
   catch (Exception ex)
  {
throw new Exception($"Error al listar facturas: {ex.Message}", ex);
    }
        }

 // Obtener factura
   public async Task<ObtenerFacturaResponse> ObtenerFacturaAsync(int idFactura)
        {
  try
      {
       var request = new ObtenerFacturaRequest { IdFactura = idFactura };
                return await _client.ObtenerFacturaAsync(request);
            }
            catch (Exception ex)
  {
             throw new Exception($"Error al obtener factura: {ex.Message}", ex);
            }
        }

   // Marcar como pagada
        public async Task<MarcarFacturaPagadaResponse> MarcarFacturaPagadaAsync(int idFactura, string metodoPago, string fechaPago)
    {
     try
      {
    var request = new MarcarFacturaPagadaRequest
 {
 IdFactura = idFactura,
  MetodoPago = metodoPago,
        FechaPago = fechaPago
         };

         return await _client.MarcarFacturaPagadaAsync(request);
            }
   catch (Exception ex)
     {
        throw new Exception($"Error al marcar como pagada: {ex.Message}", ex);
  }
        }

        // Anular factura
        public async Task<AnularFacturaResponse> AnularFacturaAsync(int idFactura, string motivo = "")
        {
            try
            {
        var request = new AnularFacturaRequest
     {
    IdFactura = idFactura,
  Motivo = motivo
     };

 return await _client.AnularFacturaAsync(request);
      }
            catch (Exception ex)
   {
    throw new Exception($"Error al anular factura: {ex.Message}", ex);
        }
        }

        // Listar facturas del usuario
  public async Task<ListarFacturasUsuarioResponse> ListarFacturasUsuarioAsync(int idUsuario)
        {
   try
            {
                var request = new ListarFacturasUsuarioRequest { IdUsuario = idUsuario };
return await _client.ListarFacturasUsuarioAsync(request);
}
            catch (Exception ex)
   {
 throw new Exception($"Error al listar facturas del usuario: {ex.Message}", ex);
         }
     }

        // Obtener factura detallada
      public async Task<ObtenerFacturaDetalladaResponse> ObtenerFacturaDetalladaAsync(int idFactura)
        {
         try
     {
      var request = new ObtenerFacturaDetalladaRequest { IdFactura = idFactura };
    return await _client.ObtenerFacturaDetalladaAsync(request);
     }
  catch (Exception ex)
            {
     throw new Exception($"Error al obtener factura detallada: {ex.Message}", ex);
            }
        }

      // Calcular totales
        public async Task<CalcularTotalesResponse> CalcularTotalesAsync(Factura factura, float porcentajeIva = 0.13f)
        {
 try
            {
       var request = new CalcularTotalesRequest
      {
      Factura = factura,
         PorcentajeIva = porcentajeIva
                };

     return await _client.CalcularTotalesAsync(request);
     }
   catch (Exception ex)
    {
   throw new Exception($"Error al calcular totales: {ex.Message}", ex);
  }
        }

        public void Dispose()
        {
  _channel?.Dispose();
        }
    }
}
