#nullable enable

using Grpc.Core;
using ReservasService.Models;
using ReservasService.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using ProtoModels = ReservasService.Protos;

namespace ReservasService.Services
{
    public class ReservasGrpcService : ProtoModels.ReservasGrpc.ReservasGrpcBase
    {
        private readonly ILogger<ReservasGrpcService> _logger;
    private readonly ReservaDAO _reservaDAO;
        private readonly MesaDAO _mesaDAO;
   private readonly RestauranteDAO _restauranteDAO;

        public ReservasGrpcService(ILogger<ReservasGrpcService> logger, string connectionString)
        {
     _logger = logger;
   _reservaDAO = new ReservaDAO(connectionString);
  _mesaDAO = new MesaDAO(connectionString);
            _restauranteDAO = new RestauranteDAO(connectionString);
        }

// RPC: Crear reserva
 public override Task<ProtoModels.CrearReservaResponse> CrearReserva(
            ProtoModels.CrearReservaRequest request,
       ServerCallContext context)
        {
       try
      {
            _logger.LogInformation($"Creando reserva para usuario {request.IdUsuario}");

       if (request.IdUsuario <= 0)
     return Task.FromResult(new ProtoModels.CrearReservaResponse
  {
          Success = false,
          Mensaje = "ID de usuario no válido"
});

        if (request.IdMesa <= 0)
      return Task.FromResult(new ProtoModels.CrearReservaResponse
        {
       Success = false,
        Mensaje = "ID de mesa no válido"
            });

     // Parsear fecha y hora
        DateTime fecha = DateTime.Parse(request.FechaReserva);
      DateTime fechaLocal = DateTime.SpecifyKind(fecha, DateTimeKind.Unspecified);
        
              DateTime horaBase = DateTime.Parse(request.HoraReserva);
       string hora = horaBase.ToString("HH:mm:ss");

              // Crear reserva usando el nuevo método del DAO
        DataTable resultado = _reservaDAO.CrearReserva(
        request.IdUsuario,
        request.IdMesa,
      fechaLocal,
  hora,
     request.NumeroPersonas,
  request.Notas ?? "",
     "HOLD", // Estado por defecto
         "" // Método de pago vacío para HOLD
      );

      int idReserva = 0;
                string mensaje = "Reserva creada correctamente";

      if (resultado.Rows.Count > 0)
  {
       if (resultado.Columns.Contains("IdReserva"))
          idReserva = Convert.ToInt32(resultado.Rows[0]["IdReserva"]);
      
     if (resultado.Columns.Contains("Resultado"))
   mensaje = resultado.Rows[0]["Resultado"]?.ToString() ?? mensaje;
     }

         // Crear objeto reserva para respuesta
  var reserva = new Reserva
    {
            IdReserva = idReserva,
 IdUsuario = request.IdUsuario,
          IdMesa = request.IdMesa,
    IdRestaurante = request.IdRestaurante,
         Fecha = fechaLocal,
       Hora = hora,
     NumeroPersonas = request.NumeroPersonas,
          Estado = "HOLD",
          Observaciones = request.Notas ?? "",
         FechaCreacion = DateTime.Now
       };

           return Task.FromResult(new ProtoModels.CrearReservaResponse
     {
          Success = true,
  Mensaje = mensaje,
   IdReserva = idReserva,
   Reserva = MapearReservaProto(reserva)
    });
            }
catch (Exception ex)
      {
     _logger.LogError($"Error al crear reserva: {ex.Message}");
        return Task.FromResult(new ProtoModels.CrearReservaResponse
     {
                Success = false,
   Mensaje = $"Error: {ex.Message}"
});
      }
    }

        // RPC: Obtener reserva
        public override Task<ProtoModels.ObtenerReservaResponse> ObtenerReserva(
        ProtoModels.ObtenerReservaRequest request,
   ServerCallContext context)
        {
    try
      {
   _logger.LogInformation($"Obteniendo reserva {request.IdReserva}");

if (request.IdReserva <= 0)
              return Task.FromResult(new ProtoModels.ObtenerReservaResponse
        {
 Success = false,
  Mensaje = "ID de reserva no válido"
});

       DataTable dt = _reservaDAO.BuscarReservaPorId(request.IdReserva);

          if (dt.Rows.Count == 0)
  return Task.FromResult(new ProtoModels.ObtenerReservaResponse
   {
          Success = false,
     Mensaje = "Reserva no encontrada"
            });

     DataRow row = dt.Rows[0];
         
              var reserva = new Reserva
    {
          IdReserva = Convert.ToInt32(row["IdReserva"]),
     IdUsuario = Convert.ToInt32(row["IdUsuario"]),
       IdMesa = Convert.ToInt32(row["IdMesa"]),
        Fecha = Convert.ToDateTime(row["Fecha"]),
           Hora = row["Hora"]?.ToString() ?? "",
   NumeroPersonas = Convert.ToInt32(row["NumeroPersonas"]),
        Estado = row["Estado"]?.ToString() ?? "",
              Observaciones = row["Observaciones"]?.ToString() ?? ""
 };

    // Obtener mesa
      Models.Mesa? mesa = null;
      DataTable mesasTable = _mesaDAO.ListarMesas();
          DataRow[] mesaRows = mesasTable.Select($"IdMesa = {reserva.IdMesa}");
      if (mesaRows.Length > 0)
   {
        var mesaRow = mesaRows[0];
      mesa = new Models.Mesa
   {
IdMesa = Convert.ToInt32(mesaRow["IdMesa"]),
         IdRestaurante = Convert.ToInt32(mesaRow["IdRestaurante"]),
          NumeroMesa = Convert.ToInt32(mesaRow["NumeroMesa"]),
             Capacidad = Convert.ToInt32(mesaRow["Capacidad"]),
         Estado = mesaRow["Estado"]?.ToString() ?? "DISPONIBLE"
      };
         }

      // Obtener restaurante si existe IdRestaurante
                Models.Restaurante? restaurante = null;
       if (mesa is not null)
           {
var restaurantes = _restauranteDAO.ListarRestaurantesActivos();
     restaurante = restaurantes.FirstOrDefault(r => r.IdRestaurante == mesa.IdRestaurante);
             }

         return Task.FromResult(new ProtoModels.ObtenerReservaResponse
    {
      Success = true,
Mensaje = "Reserva obtenida correctamente",
    Reserva = MapearReservaProto(reserva),
         Mesa = mesa is not null ? MapearMesaProto(mesa) : null,
        Restaurante = restaurante is not null ? MapearRestauranteProto(restaurante) : null
        });
            }
 catch (Exception ex)
      {
       _logger.LogError($"Error al obtener reserva: {ex.Message}");
return Task.FromResult(new ProtoModels.ObtenerReservaResponse
      {
  Success = false,
 Mensaje = $"Error: {ex.Message}"
      });
         }
   }

        // RPC: Listar reservas del usuario
        public override Task<ProtoModels.ListarReservasUsuarioResponse> ListarReservasUsuario(
     ProtoModels.ListarReservasUsuarioRequest request,
            ServerCallContext context)
        {
          try
   {
                _logger.LogInformation($"Listando reservas del usuario {request.IdUsuario}");

if (request.IdUsuario <= 0)
        return Task.FromResult(new ProtoModels.ListarReservasUsuarioResponse
      {
     Success = false,
           Mensaje = "ID de usuario no válido"
      });

       // Usar filtro para obtener reservas del usuario
DataTable dt = _reservaDAO.ListarReservas();
                DataRow[] rows = dt.Select($"IdUsuario = {request.IdUsuario}");

                var reservas = new List<Reserva>();
            foreach (DataRow row in rows)
         {
        reservas.Add(new Reserva
            {
      IdReserva = Convert.ToInt32(row["IdReserva"]),
     IdUsuario = Convert.ToInt32(row["IdUsuario"]),
      IdMesa = Convert.ToInt32(row["IdMesa"]),
         Fecha = Convert.ToDateTime(row["Fecha"]),
              Hora = row["Hora"]?.ToString() ?? "",
            NumeroPersonas = Convert.ToInt32(row["NumeroPersonas"]),
       Estado = row["Estado"]?.ToString() ?? "",
       Observaciones = row["Observaciones"]?.ToString() ?? ""
                });
  }

       var reservasProto = reservas.Select(MapearReservaProto).ToList();

          return Task.FromResult(new ProtoModels.ListarReservasUsuarioResponse
        {
      Success = true,
             Mensaje = "Reservas listadas correctamente",
  Reservas = { reservasProto }
  });
            }
      catch (Exception ex)
            {
          _logger.LogError($"Error al listar reservas del usuario: {ex.Message}");
return Task.FromResult(new ProtoModels.ListarReservasUsuarioResponse
           {
           Success = false,
        Mensaje = $"Error: {ex.Message}"
 });
            }
        }

      // RPC: Listar reservas por fecha
        public override Task<ProtoModels.ListarReservasFechaResponse> ListarReservasFecha(
        ProtoModels.ListarReservasFechaRequest request,
    ServerCallContext context)
    {
   try
   {
       _logger.LogInformation($"Listando reservas para {request.Fecha}");

  DateTime fecha = DateTime.Parse(request.Fecha);

                // Usar filtro para obtener reservas por fecha
                DataTable dt = _reservaDAO.ListarReservas();
           string filtro = $"Fecha = '{fecha:yyyy-MM-dd}'";
           
      if (request.IdRestaurante > 0)
  {
          // Necesitamos hacer un join con Mesa para filtrar por restaurante
                  DataTable mesas = _mesaDAO.ListarMesas();
               var mesasRestaurante = mesas.Select($"IdRestaurante = {request.IdRestaurante}")
                .Select(r => Convert.ToInt32(r["IdMesa"]))
       .ToList();
           
       if (mesasRestaurante.Any())
         {
   string mesasIds = string.Join(",", mesasRestaurante);
  filtro += $" AND IdMesa IN ({mesasIds})";
        }
             }

         DataRow[] rows = dt.Select(filtro);

    var reservas = new List<Reserva>();
       foreach (DataRow row in rows)
        {
         reservas.Add(new Reserva
                {
   IdReserva = Convert.ToInt32(row["IdReserva"]),
          IdUsuario = Convert.ToInt32(row["IdUsuario"]),
  IdMesa = Convert.ToInt32(row["IdMesa"]),
        Fecha = Convert.ToDateTime(row["Fecha"]),
      Hora = row["Hora"]?.ToString() ?? "",
 NumeroPersonas = Convert.ToInt32(row["NumeroPersonas"]),
             Estado = row["Estado"]?.ToString() ?? "",
       Observaciones = row["Observaciones"]?.ToString() ?? ""
   });
     }

              var reservasProto = reservas.Select(MapearReservaProto).ToList();

        return Task.FromResult(new ProtoModels.ListarReservasFechaResponse
                {
            Success = true,
Mensaje = "Reservas listadas correctamente",
         Reservas = { reservasProto }
        });
            }
     catch (Exception ex)
    {
          _logger.LogError($"Error al listar reservas por fecha: {ex.Message}");
                return Task.FromResult(new ProtoModels.ListarReservasFechaResponse
                {
        Success = false,
               Mensaje = $"Error: {ex.Message}"
    });
     }
    }

        // RPC: Actualizar reserva
  public override Task<ProtoModels.ActualizarReservaResponse> ActualizarReserva(
 ProtoModels.ActualizarReservaRequest request,
            ServerCallContext context)
        {
            try
            {
       _logger.LogInformation($"Actualizando reserva {request.IdReserva}");

 if (request.IdReserva <= 0)
        return Task.FromResult(new ProtoModels.ActualizarReservaResponse
         {
              Success = false,
        Mensaje = "ID de reserva no válido"
});

            DateTime fecha = DateTime.Parse(request.FechaReserva);
     DateTime fechaLocal = DateTime.SpecifyKind(fecha, DateTimeKind.Unspecified);
      
         DateTime horaBase = DateTime.Parse(request.HoraReserva);
          string hora = horaBase.ToString("HH:mm:ss");

     var reserva = new Reserva
        {
 IdReserva = request.IdReserva,
  Fecha = fechaLocal,
       Hora = hora,
               NumeroPersonas = request.NumeroPersonas,
         Observaciones = request.Notas ?? ""
      };

         string resultado = _reservaDAO.EditarReserva(reserva);

 return Task.FromResult(new ProtoModels.ActualizarReservaResponse
      {
 Success = true,
           Mensaje = "Reserva actualizada correctamente"
     });
            }
        catch (Exception ex)
            {
     _logger.LogError($"Error al actualizar reserva: {ex.Message}");
     return Task.FromResult(new ProtoModels.ActualizarReservaResponse
      {
          Success = false,
    Mensaje = $"Error: {ex.Message}"
       });
   }
        }

  // RPC: Cancelar reserva
        public override Task<ProtoModels.CancelarReservaResponse> CancelarReserva(
            ProtoModels.CancelarReservaRequest request,
     ServerCallContext context)
    {
 try
            {
                _logger.LogInformation($"Cancelando reserva {request.IdReserva}");

      if (request.IdReserva <= 0)
      return Task.FromResult(new ProtoModels.CancelarReservaResponse
             {
 Success = false,
        Mensaje = "ID de reserva no válido"
                 });

        // Cambiar estado a CANCELADA
        _reservaDAO.ActualizarEstado(request.IdReserva, "CANCELADA", "");

        return Task.FromResult(new ProtoModels.CancelarReservaResponse
                {
          Success = true,
  Mensaje = "Reserva cancelada correctamente"
   });
  }
            catch (Exception ex)
            {
     _logger.LogError($"Error al cancelar reserva: {ex.Message}");
                return Task.FromResult(new ProtoModels.CancelarReservaResponse
                {
        Success = false,
        Mensaje = $"Error: {ex.Message}"
                });
            }
        }

        // RPC: Obtener mesas
        public override Task<ProtoModels.ObtenerMesasResponse> ObtenerMesas(
            ProtoModels.ObtenerMesasRequest request,
         ServerCallContext context)
        {
      try
         {
       _logger.LogInformation($"Obteniendo mesas del restaurante {request.IdRestaurante}");

   var mesas = _mesaDAO.ListarMesasPorRestaurante(request.IdRestaurante);
                var mesasProto = mesas.Select(MapearMesaProto).ToList();

                return Task.FromResult(new ProtoModels.ObtenerMesasResponse
       {
      Success = true,
 Mensaje = "Mesas listadas correctamente",
         Mesas = { mesasProto }
         });
     }
catch (Exception ex)
            {
   _logger.LogError($"Error al obtener mesas: {ex.Message}");
    return Task.FromResult(new ProtoModels.ObtenerMesasResponse
      {
    Success = false,
       Mensaje = $"Error: {ex.Message}"
                });
            }
        }

  // RPC: Obtener restaurantes
public override Task<ProtoModels.ObtenerRestaurantesResponse> ObtenerRestaurantes(
         ProtoModels.ObtenerRestaurantesRequest request,
            ServerCallContext context)
        {
            try
            {
           _logger.LogInformation("Obteniendo restaurantes activos");

            var restaurantes = _restauranteDAO.ListarRestaurantesActivos();
    var restaurantesProto = restaurantes.Select(MapearRestauranteProto).ToList();

         return Task.FromResult(new ProtoModels.ObtenerRestaurantesResponse
   {
        Success = true,
         Mensaje = "Restaurantes listados correctamente",
             Restaurantes = { restaurantesProto }
  });
          }
         catch (Exception ex)
            {
      _logger.LogError($"Error al obtener restaurantes: {ex.Message}");
                return Task.FromResult(new ProtoModels.ObtenerRestaurantesResponse
 {
            Success = false,
           Mensaje = $"Error: {ex.Message}"
     });
            }
}

        // Métodos auxiliares para mapeo
        private ProtoModels.Reserva MapearReservaProto(Reserva reserva)
      {
     return new ProtoModels.Reserva
      {
 IdReserva = reserva.IdReserva,
        IdUsuario = reserva.IdUsuario,
       IdMesa = reserva.IdMesa,
 IdRestaurante = reserva.IdRestaurante,
         FechaReserva = reserva.Fecha.ToString("yyyy-MM-dd"),
  HoraReserva = reserva.Hora,
            NumeroPersonas = reserva.NumeroPersonas,
  Estado = reserva.Estado,
                Notas = reserva.Observaciones ?? string.Empty,
        FechaCreacion = reserva.FechaCreacion.ToString("yyyy-MM-dd")
            };
        }

      private ProtoModels.Mesa MapearMesaProto(Models.Mesa mesa)
   {
            return new ProtoModels.Mesa
      {
      IdMesa = mesa.IdMesa,
          IdRestaurante = mesa.IdRestaurante,
    NumeroMesa = mesa.NumeroMesa,
   Capacidad = mesa.Capacidad,
     Ubicacion = mesa.TipoMesa ?? string.Empty,
       Disponible = mesa.Estado == "DISPONIBLE"
        };
    }

        private ProtoModels.Restaurante MapearRestauranteProto(Models.Restaurante restaurante)
      {
            return new ProtoModels.Restaurante
      {
IdRestaurante = restaurante.IdRestaurante,
                Nombre = restaurante.Nombre,
           Direccion = restaurante.Direccion ?? string.Empty,
          Telefono = restaurante.Telefono ?? string.Empty,
 HorarioApertura = restaurante.HorarioApertura ?? string.Empty,
            HorarioCierre = restaurante.HorarioCierre ?? string.Empty,
     Activo = restaurante.Activo
   };
        }
    }
}
