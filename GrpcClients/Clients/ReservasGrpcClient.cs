using Grpc.Net.Client;
using ReservasService.Protos;
using System;
using System.Threading.Tasks;

namespace GrpcClients.Clients
{
    public class ReservasGrpcClient
    {
 private readonly GrpcChannel _channel;
        private readonly ReservasGrpc.ReservasGrpcClient _client;

      public ReservasGrpcClient(string serviceUrl)
        {
            // Permitir llamadas HTTP/2 sin encriptación (para desarrollo)
     AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
      
     _channel = GrpcChannel.ForAddress(serviceUrl);
   _client = new ReservasGrpc.ReservasGrpcClient(_channel);
        }

        // Crear reserva
        public async Task<CrearReservaResponse> CrearReservaAsync(int idUsuario, int idMesa, int idRestaurante, string fechaReserva, string horaReserva, int numeroPersonas, string notas = "")
        {
 try
            {
  var request = new CrearReservaRequest
           {
IdUsuario = idUsuario,
IdMesa = idMesa,
IdRestaurante = idRestaurante,
  FechaReserva = fechaReserva,
      HoraReserva = horaReserva,
     NumeroPersonas = numeroPersonas,
  Notas = notas
   };

   return await _client.CrearReservaAsync(request);
      }
  catch (Exception ex)
     {
      throw new Exception($"Error al crear reserva: {ex.Message}", ex);
          }
        }

  // Obtener reserva
      public async Task<ObtenerReservaResponse> ObtenerReservaAsync(int idReserva)
        {
try
    {
   var request = new ObtenerReservaRequest { IdReserva = idReserva };
  return await _client.ObtenerReservaAsync(request);
         }
 catch (Exception ex)
          {
throw new Exception($"Error al obtener reserva: {ex.Message}", ex);
  }
}

     // Listar reservas del usuario
      public async Task<ListarReservasUsuarioResponse> ListarReservasUsuarioAsync(int idUsuario)
   {
try
       {
 var request = new ListarReservasUsuarioRequest { IdUsuario = idUsuario };
        return await _client.ListarReservasUsuarioAsync(request);
        }
 catch (Exception ex)
 {
             throw new Exception($"Error al listar reservas del usuario: {ex.Message}", ex);
           }
 }

        // Listar reservas por fecha
   public async Task<ListarReservasFechaResponse> ListarReservasFechaAsync(string fecha, int idRestaurante)
 {
try
 {
           var request = new ListarReservasFechaRequest
     {
         Fecha = fecha,
IdRestaurante = idRestaurante
          };

 return await _client.ListarReservasFechaAsync(request);
          }
 catch (Exception ex)
    {
  throw new Exception($"Error al listar reservas por fecha: {ex.Message}", ex);
    }
}

  // Actualizar reserva
 public async Task<ActualizarReservaResponse> ActualizarReservaAsync(int idReserva, string fechaReserva, string horaReserva, int numeroPersonas, string notas = "")
        {
try
     {
 var request = new ActualizarReservaRequest
       {
     IdReserva = idReserva,
FechaReserva = fechaReserva,
HoraReserva = horaReserva,
   NumeroPersonas = numeroPersonas,
  Notas = notas
  };

return await _client.ActualizarReservaAsync(request);
  }
 catch (Exception ex)
 {
throw new Exception($"Error al actualizar reserva: {ex.Message}", ex);
    }
      }

        // Cancelar reserva
   public async Task<CancelarReservaResponse> CancelarReservaAsync(int idReserva, string motivo = "")
  {
try
     {
  var request = new CancelarReservaRequest
    {
         IdReserva = idReserva,
Motivo = motivo
 };

 return await _client.CancelarReservaAsync(request);
     }
 catch (Exception ex)
   {
        throw new Exception($"Error al cancelar reserva: {ex.Message}", ex);
      }
   }

 // Obtener mesas
      public async Task<ObtenerMesasResponse> ObtenerMesasAsync(int idRestaurante)
        {
try
  {
     var request = new ObtenerMesasRequest { IdRestaurante = idRestaurante };
 return await _client.ObtenerMesasAsync(request);
       }
 catch (Exception ex)
 {
 throw new Exception($"Error al obtener mesas: {ex.Message}", ex);
   }
        }

        // Obtener restaurantes
    public async Task<ObtenerRestaurantesResponse> ObtenerRestaurantesAsync()
        {
try
 {
    var request = new ObtenerRestaurantesRequest();
         return await _client.ObtenerRestaurantesAsync(request);
  }
    catch (Exception ex)
            {
  throw new Exception($"Error al obtener restaurantes: {ex.Message}", ex);
     }
  }

        public void Dispose()
  {
 _channel?.Dispose();
      }
    }
}
