using Grpc.Core;
using MenuService.Models;
using MenuService.Data;
using MenuService.Protos;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace MenuService.Services
{
    public class MenuGrpcService : MenuGrpc.MenuGrpcBase
    {
        private readonly ILogger<MenuGrpcService> _logger;
      private readonly PlatoDAO _platoDAO;
        private readonly PromocionDAO _promocionDAO;

      public MenuGrpcService(ILogger<MenuGrpcService> logger, string connectionString)
        {
        _logger = logger;
            _platoDAO = new PlatoDAO(connectionString);
        _promocionDAO = new PromocionDAO(connectionString);
        }

        // RPC: Obtener platos
 public override Task<ObtenerPlatosResponse> ObtenerPlatos(
   ObtenerPlatosRequest request,
       ServerCallContext context)
        {
            try
            {
       _logger.LogInformation("Listando todos los platos");

            var platos = _platoDAO.ListarTodosPlatos();
             var platosProto = platos.Select(MapearPlatoProto).ToList();

  return Task.FromResult(new ObtenerPlatosResponse
        {
       Success = true,
    Mensaje = "Platos listados correctamente",
            Platos = { platosProto }
     });
            }
  catch (Exception ex)
            {
      _logger.LogError($"Error al listar platos: {ex.Message}");
      return Task.FromResult(new ObtenerPlatosResponse
     {
         Success = false,
          Mensaje = $"Error: {ex.Message}"
  });
     }
  }

     // RPC: Obtener plato por ID
        public override Task<ObtenerPlatoByIdResponse> ObtenerPlatoById(
       ObtenerPlatoByIdRequest request,
            ServerCallContext context)
        {
 try
            {
  _logger.LogInformation($"Obteniendo plato {request.IdPlato}");

       if (request.IdPlato <= 0)
 return Task.FromResult(new ObtenerPlatoByIdResponse
     {
       Success = false,
    Mensaje = "ID de plato no válido"
             });

         var plato = _platoDAO.ObtenerPlatoById(request.IdPlato);

     if (plato == null)
       return Task.FromResult(new ObtenerPlatoByIdResponse
     {
 Success = false,
        Mensaje = "Plato no encontrado"
 });

            return Task.FromResult(new ObtenerPlatoByIdResponse
{
         Success = true,
     Mensaje = "Plato obtenido correctamente",
      Plato = MapearPlatoProto(plato)
      });
            }
 catch (Exception ex)
       {
      _logger.LogError($"Error al obtener plato: {ex.Message}");
     return Task.FromResult(new ObtenerPlatoByIdResponse
             {
      Success = false,
           Mensaje = $"Error: {ex.Message}"
        });
  }
        }

        // RPC: Crear plato
        public override Task<CrearPlatoResponse> CrearPlato(
            CrearPlatoRequest request,
            ServerCallContext context)
        {
try
        {
      _logger.LogInformation($"Creando plato: {request.Nombre}");

    if (string.IsNullOrWhiteSpace(request.Nombre))
        return Task.FromResult(new CrearPlatoResponse
     {
      Success = false,
      Mensaje = "El nombre del plato es requerido"
      });

   var plato = new Models.Plato
      {
   IdRestaurante = 2, // Por defecto restaurante 2
       Nombre = request.Nombre,
    Descripcion = request.Descripcion,
          Precio = (decimal)request.Precio,
    Categoria = request.Categoria,
        TipoComida = request.Categoria, // Usar categoría por defecto
  ImagenURL = "",
          Stock = 20,
        Estado = "ACTIVO"
     };

           plato = _platoDAO.CrearPlato(plato);

     return Task.FromResult(new CrearPlatoResponse
    {
      Success = true,
               Mensaje = "Plato creado correctamente",
        IdPlato = plato.IdPlato,
Plato = MapearPlatoProto(plato)
       });
          }
  catch (Exception ex)
            {
        _logger.LogError($"Error al crear plato: {ex.Message}");
    return Task.FromResult(new CrearPlatoResponse
     {
             Success = false,
       Mensaje = $"Error: {ex.Message}"
   });
   }
        }

    // RPC: Actualizar plato
        public override Task<ActualizarPlatoResponse> ActualizarPlato(
       ActualizarPlatoRequest request,
        ServerCallContext context)
        {
          try
         {
    _logger.LogInformation($"Actualizando plato {request.IdPlato}");

 if (request.IdPlato <= 0)
        return Task.FromResult(new ActualizarPlatoResponse
        {
       Success = false,
           Mensaje = "ID de plato no válido"
 });

          var plato = new Models.Plato
 {
          IdPlato = request.IdPlato,
 Nombre = request.Nombre,
     Descripcion = request.Descripcion,
   Precio = (decimal)request.Precio,
   Categoria = request.Categoria,
  TipoComida = request.Categoria,
           ImagenURL = "",
           Stock = 20,
         Estado = "ACTIVO"
   };

        var resultado = _platoDAO.ActualizarPlato(plato);

      if (!resultado)
        return Task.FromResult(new ActualizarPlatoResponse
   {
     Success = false,
        Mensaje = "No se pudo actualizar el plato"
    });

    return Task.FromResult(new ActualizarPlatoResponse
  {
      Success = true,
          Mensaje = "Plato actualizado correctamente"
    });
            }
  catch (Exception ex)
  {
    _logger.LogError($"Error al actualizar plato: {ex.Message}");
    return Task.FromResult(new ActualizarPlatoResponse
    {
     Success = false,
        Mensaje = $"Error: {ex.Message}"
    });
   }
  }

        // RPC: Eliminar plato
        public override Task<EliminarPlatoResponse> EliminarPlato(
      EliminarPlatoRequest request,
     ServerCallContext context)
        {
            try
     {
      _logger.LogInformation($"Eliminando plato {request.IdPlato}");

   if (request.IdPlato <= 0)
     return Task.FromResult(new EliminarPlatoResponse
         {
      Success = false,
   Mensaje = "ID de plato no válido"
       });

            var resultado = _platoDAO.EliminarPlato(request.IdPlato);

           if (!resultado)
            return Task.FromResult(new EliminarPlatoResponse
     {
   Success = false,
            Mensaje = "No se pudo eliminar el plato"
              });

        return Task.FromResult(new EliminarPlatoResponse
   {
           Success = true,
            Mensaje = "Plato eliminado correctamente"
       });
   }
     catch (Exception ex)
            {
     _logger.LogError($"Error al eliminar plato: {ex.Message}");
 return Task.FromResult(new EliminarPlatoResponse
      {
        Success = false,
         Mensaje = $"Error: {ex.Message}"
    });
            }
        }

        // RPC: Obtener promociones
      public override Task<ObtenerPromocionesResponse> ObtenerPromociones(
  ObtenerPromocionesRequest request,
   ServerCallContext context)
        {
            try
 {
                _logger.LogInformation("Listando promociones activas");

     var promociones = _promocionDAO.ListarPromocionesActivas();
     var promocionesProto = promociones.Select(MapearPromocionProto).ToList();

       return Task.FromResult(new ObtenerPromocionesResponse
            {
              Success = true,
        Mensaje = "Promociones listadas correctamente",
      Promociones = { promocionesProto }
       });
            }
    catch (Exception ex)
      {
                _logger.LogError($"Error al listar promociones: {ex.Message}");
           return Task.FromResult(new ObtenerPromocionesResponse
      {
         Success = false,
           Mensaje = $"Error: {ex.Message}"
                });
   }
        }

    // RPC: Obtener promoción por ID
        public override Task<ObtenerPromocionByIdResponse> ObtenerPromocionById(
       ObtenerPromocionByIdRequest request,
            ServerCallContext context)
        {
            try
     {
         _logger.LogInformation($"Obteniendo promoción {request.IdPromocion}");

        if (request.IdPromocion <= 0)
        return Task.FromResult(new ObtenerPromocionByIdResponse
        {
     Success = false,
            Mensaje = "ID de promoción no válido"
         });

        var promocion = _promocionDAO.ObtenerPromocionById(request.IdPromocion);

         if (promocion == null)
         return Task.FromResult(new ObtenerPromocionByIdResponse
{
         Success = false,
      Mensaje = "Promoción no encontrada"
           });

      return Task.FromResult(new ObtenerPromocionByIdResponse
                {
 Success = true,
     Mensaje = "Promoción obtenida correctamente",
           Promocion = MapearPromocionProto(promocion)
});
     }
            catch (Exception ex)
        {
  _logger.LogError($"Error al obtener promoción: {ex.Message}");
     return Task.FromResult(new ObtenerPromocionByIdResponse
       {
     Success = false,
         Mensaje = $"Error: {ex.Message}"
          });
    }
    }

        // RPC: Crear promoción
 public override Task<CrearPromocionResponse> CrearPromocion(
   CrearPromocionRequest request,
     ServerCallContext context)
  {
      try
          {
        _logger.LogInformation($"Creando promoción: {request.Nombre}");

  if (string.IsNullOrWhiteSpace(request.Nombre))
 return Task.FromResult(new CrearPromocionResponse
       {
     Success = false,
           Mensaje = "El nombre de la promoción es requerido"
      });

        var promocion = new Models.Promocion
     {
 IdRestaurante = 2, // Por defecto restaurante 2
   Nombre = request.Nombre,
  Descuento = (decimal)request.PorcentajeDescuento,
      FechaInicio = DateTime.Parse(request.FechaInicio),
              FechaFin = DateTime.Parse(request.FechaFin),
         Estado = request.Activo ? "Activa" : "Inactiva"
     };

        promocion = _promocionDAO.CrearPromocion(promocion);

      return Task.FromResult(new CrearPromocionResponse
 {
Success = true,
     Mensaje = "Promoción creada correctamente",
  IdPromocion = promocion.IdPromocion,
       Promocion = MapearPromocionProto(promocion)
     });
         }
catch (Exception ex)
  {
     _logger.LogError($"Error al crear promoción: {ex.Message}");
  return Task.FromResult(new CrearPromocionResponse
      {
    Success = false,
          Mensaje = $"Error: {ex.Message}"
     });
}
 }

        // Métodos auxiliares para mapeo
        private Protos.Plato MapearPlatoProto(Models.Plato plato)
        {
     return new Protos.Plato
        {
       IdPlato = plato.IdPlato,
    Nombre = plato.Nombre,
  Descripcion = plato.Descripcion ?? string.Empty,
 Precio = (float)plato.Precio,
       Categoria = plato.Categoria ?? string.Empty,
          Activo = plato.Estado == "ACTIVO",
  FechaCreacion = DateTime.Now.ToString("yyyy-MM-dd")
            };
  }

     private Protos.Promocion MapearPromocionProto(Models.Promocion promocion)
        {
return new Protos.Promocion
   {
   IdPromocion = promocion.IdPromocion,
            Nombre = promocion.Nombre,
 Descripcion = "", // No hay descripción en la tabla
 PorcentajeDescuento = (float)promocion.Descuento,
      FechaInicio = promocion.FechaInicio.ToString("yyyy-MM-dd"),
       FechaFin = promocion.FechaFin.ToString("yyyy-MM-dd"),
   Activo = promocion.Estado == "Activa",
 FechaCreacion = DateTime.Now.ToString("yyyy-MM-dd")
            };
        }
    }
}
