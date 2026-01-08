using Grpc.Net.Client;
using MenuService.Protos;
using System;
using System.Threading.Tasks;

namespace GrpcClients.Clients
{
    public class MenuGrpcClient
    {
 private readonly GrpcChannel _channel;
     private readonly MenuGrpc.MenuGrpcClient _client;

        public MenuGrpcClient(string serviceUrl)
        {
            // Permitir llamadas HTTP/2 sin encriptación (para desarrollo)
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
     
    _channel = GrpcChannel.ForAddress(serviceUrl);
   _client = new MenuGrpc.MenuGrpcClient(_channel);
        }

   // Obtener todos los platos
        public async Task<ObtenerPlatosResponse> ObtenerPlatosAsync()
      {
  try
  {
    var request = new ObtenerPlatosRequest();
     return await _client.ObtenerPlatosAsync(request);
      }
   catch (Exception ex)
   {
throw new Exception($"Error al obtener platos: {ex.Message}", ex);
          }
     }

  // Obtener plato por ID
  public async Task<ObtenerPlatoByIdResponse> ObtenerPlatoByIdAsync(int idPlato)
        {
try
{
   var request = new ObtenerPlatoByIdRequest { IdPlato = idPlato };
      return await _client.ObtenerPlatoByIdAsync(request);
   }
  catch (Exception ex)
      {
 throw new Exception($"Error al obtener plato: {ex.Message}", ex);
            }
  }

        // Crear plato
 public async Task<CrearPlatoResponse> CrearPlatoAsync(string nombre, string descripcion, float precio, string categoria, bool activo)
        {
 try
     {
     var request = new CrearPlatoRequest
   {
  Nombre = nombre,
    Descripcion = descripcion,
   Precio = precio,
              Categoria = categoria,
             Activo = activo
              };

return await _client.CrearPlatoAsync(request);
  }
 catch (Exception ex)
      {
    throw new Exception($"Error al crear plato: {ex.Message}", ex);
 }
        }

    // Actualizar plato
     public async Task<ActualizarPlatoResponse> ActualizarPlatoAsync(int idPlato, string nombre, string descripcion, float precio, string categoria, bool activo)
        {
  try
            {
  var request = new ActualizarPlatoRequest
  {
   IdPlato = idPlato,
       Nombre = nombre,
   Descripcion = descripcion,
   Precio = precio,
  Categoria = categoria,
      Activo = activo
   };

  return await _client.ActualizarPlatoAsync(request);
      }
       catch (Exception ex)
 {
       throw new Exception($"Error al actualizar plato: {ex.Message}", ex);
             }
        }

  // Eliminar plato
     public async Task<EliminarPlatoResponse> EliminarPlatoAsync(int idPlato)
   {
  try
    {
       var request = new EliminarPlatoRequest { IdPlato = idPlato };
        return await _client.EliminarPlatoAsync(request);
  }
            catch (Exception ex)
  {
   throw new Exception($"Error al eliminar plato: {ex.Message}", ex);
  }
        }

        // Obtener promociones
  public async Task<ObtenerPromocionesResponse> ObtenerPromocionesAsync()
        {
try
           {
   var request = new ObtenerPromocionesRequest();
       return await _client.ObtenerPromocionesAsync(request);
            }
 catch (Exception ex)
   {
       throw new Exception($"Error al obtener promociones: {ex.Message}", ex);
       }
  }

        // Obtener promoción por ID
       public async Task<ObtenerPromocionByIdResponse> ObtenerPromocionByIdAsync(int idPromocion)
        {
try
      {
var request = new ObtenerPromocionByIdRequest { IdPromocion = idPromocion };
         return await _client.ObtenerPromocionByIdAsync(request);
    }
  catch (Exception ex)
 {
        throw new Exception($"Error al obtener promoción: {ex.Message}", ex);
  }
        }

      // Crear promoción
 public async Task<CrearPromocionResponse> CrearPromocionAsync(string nombre, string descripcion, float porcentajeDescuento, string fechaInicio, string fechaFin, bool activo)
        {
          try
    {
   var request = new CrearPromocionRequest
  {
 Nombre = nombre,
       Descripcion = descripcion,
    PorcentajeDescuento = porcentajeDescuento,
   FechaInicio = fechaInicio,
        FechaFin = fechaFin,
  Activo = activo
  };

return await _client.CrearPromocionAsync(request);
     }
       catch (Exception ex)
   {
      throw new Exception($"Error al crear promoción: {ex.Message}", ex);
   }
        }

        public void Dispose()
  {
    _channel?.Dispose();
     }
    }
}
