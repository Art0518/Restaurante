#nullable enable

using Microsoft.AspNetCore.Mvc;
using ReservasService.Models;
using ReservasService.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ReservasService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservasController : ControllerBase
    {
   private readonly ILogger<ReservasController> _logger;
      private readonly ReservaDAO _reservaDAO;
      private readonly RestauranteDAO _restauranteDAO;
        private readonly MesaDAO _mesaDAO;

        public ReservasController(ILogger<ReservasController> logger, IConfiguration configuration)
        {
            _logger = logger;
       var connectionString = configuration.GetConnectionString("DefaultConnection") 
      ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
      _reservaDAO = new ReservaDAO(connectionString);
     _restauranteDAO = new RestauranteDAO(connectionString);
            _mesaDAO = new MesaDAO(connectionString);
        }

        // ============================================================
        // LISTAR TODAS LAS RESERVAS
        // ============================================================
        [HttpGet]
        [Route("")]
        public ActionResult<ApiResponse<List<object>>> ListarReservas()
        {
            try
            {
                _logger.LogInformation("REST: Listando todas las reservas");
        
            DataTable reservas = _reservaDAO.ListarReservas();
         
    // Convertir DataTable a lista de objetos
           var listaReservas = new List<object>();
          foreach (DataRow row in reservas.Rows)
         {
    listaReservas.Add(new
        {
  IdReserva = row["IdReserva"],
      IdUsuario = row["IdUsuario"],
  IdMesa = row["IdMesa"],
     Fecha = row["Fecha"],
          Hora = row["Hora"],
             NumeroPersonas = row["NumeroPersonas"],
    Estado = row["Estado"],
   Observaciones = row["Observaciones"],
           MetodoPago = row["MetodoPago"],
           MontoDescuento = row["MontoDescuento"],
           Total = row["Total"]
 });
        }

         return Ok(new ApiResponse<List<object>>
    {
              Success = true,
      Mensaje = "Reservas listadas correctamente",
      Data = listaReservas
      });
      }
            catch (Exception ex)
            {
           _logger.LogError($"Error al listar reservas: {ex.Message}");
      return StatusCode(500, new ApiResponse<List<object>> { Success = false, Mensaje = $"Error: {ex.Message}" });
       }
        }

        // ============================================================
        // ?? CREAR RESERVA
      // ============================================================
        [HttpPost]
        [Route("crear")]
        public ActionResult<ApiResponse<object>> CrearReserva([FromBody] CrearReservaDto request)
        {
  try
       {
                _logger.LogInformation($"REST: Creando reserva para usuario {request.IdUsuario}");

                if (request.IdUsuario <= 0)
       return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "Debe indicar un usuario válido" });

              if (request.IdMesa <= 0)
             return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "Debe seleccionar una mesa válida" });

                if (string.IsNullOrEmpty(request.FechaReserva))
                return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "Debe especificar la fecha de reserva" });

        if (string.IsNullOrEmpty(request.HoraReserva))
        return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "Debe especificar la hora de reserva" });

  if (request.NumeroPersonas <= 0)
          return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "Debe indicar la cantidad de personas" });

             // ?? Parsear fecha y hora
     DateTime fecha = DateTime.Parse(request.FechaReserva);
      DateTime fechaLocal = DateTime.SpecifyKind(fecha, DateTimeKind.Unspecified);
  
// Convertir hora
  DateTime horaBase = DateTime.Parse(request.HoraReserva);
       string hora = horaBase.ToString("HH:mm:ss");

         // ? Estado por defecto: HOLD si no viene especificado
                string estado = string.IsNullOrEmpty(request.Estado) ? "HOLD" : request.Estado;
   string metodoPago = string.IsNullOrEmpty(request.MetodoPago) ? "" : request.MetodoPago;

        DataTable resultado = _reservaDAO.CrearReserva(
          request.IdUsuario,
     request.IdMesa,
      fechaLocal,
        hora,
         request.NumeroPersonas,
        request.Notas ?? "",
        estado,
     metodoPago
     );

     if (resultado.Rows.Count > 0)
            {
     var row = resultado.Rows[0];
   
                    if (resultado.Columns.Contains("Resultado"))
     {
        string mensaje = row["Resultado"]?.ToString() ?? "Resultado desconocido";

                if (mensaje.Contains("ya está reservada"))
                return Conflict(new ApiResponse<object> { Success = false, Mensaje = mensaje });
            }

       // Convertir resultado a objeto
        var reservaCreada = new
       {
              IdReserva = resultado.Columns.Contains("IdReserva") ? Convert.ToInt32(row["IdReserva"]) : 0,
        Mensaje = resultado.Columns.Contains("Resultado") ? row["Resultado"]?.ToString() : "Reserva creada correctamente",
   Estado = estado,
   FechaReserva = fechaLocal.ToString("yyyy-MM-dd"),
            HoraReserva = hora,
 NumeroPersonas = request.NumeroPersonas
     };

    return Ok(new ApiResponse<object>
        {
         Success = true,
Mensaje = "Reserva creada correctamente",
        Data = reservaCreada
   });
       }

   return Ok(new ApiResponse<object>
   {
               Success = true,
              Mensaje = "Reserva creada correctamente",
            Data = new { Estado = estado }
      });
            }
    catch (Exception ex)
      {
     _logger.LogError($"Error al crear reserva: {ex.Message}");
     return StatusCode(500, new ApiResponse<object> { Success = false, Mensaje = $"Error: {ex.Message}" });
          }
      }

        // ============================================================
      // CAMBIAR ESTADO DE RESERVA + MÉTODO DE PAGO
        // ============================================================
      [HttpPut]
 [Route("estado")]
        public ActionResult<ApiResponse<string>> ActualizarEstado([FromBody] ActualizarEstadoDto request)
        {
   try
          {
                _logger.LogInformation($"REST: Actualizando estado de reserva {request.IdReserva}");

     if (request.IdReserva <= 0)
    return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "ID de reserva no válido" });

      if (string.IsNullOrEmpty(request.Estado))
                    return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "Debe especificar un estado" });

   if (string.IsNullOrEmpty(request.MetodoPago))
       return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "Debe enviar un método de pago: EFECTIVO, TRANSFERENCIA o TARJETA" });

  _reservaDAO.ActualizarEstado(request.IdReserva, request.Estado, request.MetodoPago);

                return Ok(new ApiResponse<string>
           {
     Success = true,
         Mensaje = "Estado actualizado correctamente"
          });
            }
        catch (Exception ex)
            {
            _logger.LogError($"Error al actualizar estado: {ex.Message}");
      return StatusCode(500, new ApiResponse<string> { Success = false, Mensaje = $"Error: {ex.Message}" });
         }
      }

        // ============================================================
        // ?? EDITAR RESERVA COMPLETA
        // ============================================================
   [HttpPut]
      [Route("editar")]
      public ActionResult<ApiResponse<string>> EditarReserva([FromBody] EditarReservaDto request)
{
            try
        {
          _logger.LogInformation($"REST: Editando reserva {request.IdReserva}");

    if (request.IdReserva <= 0)
     return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "ID de reserva no válido" });

      // Parsear fecha y hora
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

   return Ok(new ApiResponse<string>
    {
         Success = true,
              Mensaje = "Reserva actualizada correctamente"
  });
     }
            catch (Exception ex)
        {
        _logger.LogError($"Error al editar reserva: {ex.Message}");
         return StatusCode(500, new ApiResponse<string> { Success = false, Mensaje = $"Error: {ex.Message}" });
            }
        }

        // ============================================================
  // OBTENER RESERVA POR ID
        // ============================================================
        [HttpGet]
   [Route("{id}")]
        public ActionResult<ApiResponse<object>> ObtenerReservaPorId(int id)
        {
   try
  {
  _logger.LogInformation($"REST: Obteniendo reserva {id}");

           if (id <= 0)
     return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "ID de reserva no válido" });

  DataTable reservas = _reservaDAO.ListarReservas();
        DataRow[] fila = reservas.Select($"IdReserva = {id}");

     if (fila.Length == 0)
        return NotFound(new ApiResponse<object> { Success = false, Mensaje = "Reserva no encontrada" });

    var r = fila[0];

                var reserva = new
        {
    IdReserva = r["IdReserva"],
     IdUsuario = r["IdUsuario"],
      IdMesa = r["IdMesa"],
   Fecha = r["Fecha"],
          Hora = r["Hora"],
          NumeroPersonas = r["NumeroPersonas"],
       Estado = r["Estado"],
        Observaciones = r["Observaciones"]
  };

   return Ok(new ApiResponse<object>
     {
     Success = true,
         Mensaje = "Reserva obtenida correctamente",
     Data = reserva
     });
       }
 catch (Exception ex)
            {
              _logger.LogError($"Error al obtener reserva: {ex.Message}");
   return StatusCode(500, new ApiResponse<object> { Success = false, Mensaje = $"Error: {ex.Message}" });
            }
        }

    // ============================================================
        // FILTRAR RESERVAS
        // ============================================================
  [HttpGet]
        [Route("filtrar")]
        public ActionResult<ApiResponse<List<object>>> FiltrarReservas([FromQuery] int? idUsuario = null, [FromQuery] string? estado = null, [FromQuery] DateTime? fecha = null)
  {
 try
  {
          _logger.LogInformation("REST: Filtrando reservas");

  DataTable reservas = _reservaDAO.ListarReservas();
                string filtro = "";

   if (idUsuario.HasValue)
       filtro += $"IdUsuario = {idUsuario.Value}";

                if (!string.IsNullOrEmpty(estado))
       filtro += (filtro != "" ? " AND " : "") + $"Estado = '{estado}'";

                if (fecha.HasValue)
      filtro += (filtro != "" ? " AND " : "") + $"Fecha = '{fecha.Value:yyyy-MM-dd}'";

       DataRow[] filasFiltradas = string.IsNullOrEmpty(filtro)
   ? reservas.Select()
         : reservas.Select(filtro);

         // Convertir a lista de objetos
                var listaReservas = new List<object>();
  foreach (DataRow row in filasFiltradas)
    {
       listaReservas.Add(new
                    {
    IdReserva = row["IdReserva"],
            IdUsuario = row["IdUsuario"],
             IdMesa = row["IdMesa"],
  Fecha = row["Fecha"],
      Hora = row["Hora"],
                   NumeroPersonas = row["NumeroPersonas"],
      Estado = row["Estado"],
     Observaciones = row["Observaciones"],
            MetodoPago = row["MetodoPago"],
           MontoDescuento = row["MontoDescuento"],
   Total = row["Total"]
   });
 }

           return Ok(new ApiResponse<List<object>>
      {
   Success = true,
       Mensaje = $"Reservas filtradas: {listaReservas.Count}",
 Data = listaReservas
            });
     }
            catch (Exception ex)
      {
     _logger.LogError($"Error al filtrar reservas: {ex.Message}");
     return StatusCode(500, new ApiResponse<List<object>> { Success = false, Mensaje = $"Error: {ex.Message}" });
            }
   }

        // ============================================================
        // LISTAR RESERVAS CONFIRMADAS DE UN USUARIO
        // ============================================================
 [HttpGet]
        [Route("confirmadas/{idUsuario}")]
    public ActionResult<ApiResponse<List<object>>> ListarReservasConfirmadas(int idUsuario)
        {
      try
      {
      _logger.LogInformation($"REST: Listando reservas confirmadas del usuario {idUsuario}");

           if (idUsuario <= 0)
          return BadRequest(new ApiResponse<List<object>> { Success = false, Mensaje = "ID de usuario no válido" });

              DataTable reservasConfirmadas = _reservaDAO.ListarReservasConfirmadas(idUsuario);

    // Convertir a lista de objetos
              var listaReservas = new List<object>();
           foreach (DataRow row in reservasConfirmadas.Rows)
     {
          listaReservas.Add(new
           {
         IdReserva = row["IdReserva"],
      IdUsuario = row["IdUsuario"],
  IdMesa = row["IdMesa"],
     NumeroMesa = row["NumeroMesa"],
       Fecha = row["Fecha"],
  Hora = row["Hora"],
         NumeroPersonas = row["NumeroPersonas"],
 Total = row["Total"],
       MetodoPago = row["MetodoPago"],
    Estado = row["Estado"],
            Observaciones = row["Observaciones"],
        TipoMesa = row["TipoMesa"]
           });
  }

           return Ok(new ApiResponse<List<object>>
           {
     Success = true,
                    Mensaje = "Reservas confirmadas obtenidas exitosamente",
  Data = listaReservas
     });
     }
            catch (Exception ex)
            {
      _logger.LogError($"Error al obtener reservas confirmadas: {ex.Message}");
       return StatusCode(500, new ApiResponse<List<object>> { Success = false, Mensaje = $"Error: {ex.Message}" });
      }
 }

        // ============================================================
        // LISTAR TODAS LAS RESERVAS PARA ADMINISTRADOR
        // ============================================================
   [HttpGet]
    [Route("admin/todas")]
        public ActionResult<ApiResponse<List<object>>> ListarTodasReservasAdmin()
{
            try
  {
                _logger.LogInformation("REST: Listando todas las reservas (Admin)");

      DataTable todasReservas = _reservaDAO.ListarTodasReservasAdmin();

   // Convertir a lista de objetos
         var listaReservas = new List<object>();
   foreach (DataRow row in todasReservas.Rows)
                {
 listaReservas.Add(new
           {
   IdReserva = row["IdReserva"],
      IdUsuario = row["IdUsuario"],
       NombreUsuario = row["NombreUsuario"],
      EmailUsuario = row["EmailUsuario"],
       IdMesa = row["IdMesa"],
             NumeroMesa = row["NumeroMesa"],
    Fecha = row["Fecha"],
               Hora = row["Hora"],
  NumeroPersonas = row["NumeroPersonas"],
         Total = row["Total"],
     MetodoPago = row["MetodoPago"],
 Estado = row["Estado"],
    Observaciones = row["Observaciones"],
          TipoMesa = row["TipoMesa"]
  });
      }

              return Ok(new ApiResponse<List<object>>
      {
           Success = true,
      Mensaje = "Todas las reservas obtenidas exitosamente",
 Data = listaReservas
      });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener todas las reservas: {ex.Message}");
             return StatusCode(500, new ApiResponse<List<object>> { Success = false, Mensaje = $"Error: {ex.Message}" });
            }
        }

      // ============================================================
      // GENERAR FACTURA DESDE ADMIN PARA CUALQUIER RESERVA
        // ============================================================
        [HttpPost]
        [Route("admin/generar-factura")]
        public ActionResult<ApiResponse<object>> GenerarFacturaDesdeAdmin([FromBody] GenerarFacturaAdminDto request)
        {
  try
  {
     _logger.LogInformation($"REST: Generando factura desde admin para reserva {request.IdReserva}");

              if (request.IdReserva <= 0)
     return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "ID de reserva no válido" });

           if (string.IsNullOrWhiteSpace(request.MetodoPago))
          return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "Método de pago es requerido" });

    string tipoFactura = string.IsNullOrEmpty(request.TipoFactura) ? "ADMIN" : request.TipoFactura;

             DataTable resultado = _reservaDAO.GenerarFacturaDesdeAdmin(request.IdReserva, request.MetodoPago, tipoFactura);

       if (resultado != null && resultado.Rows.Count > 0)
          {
     DataRow row = resultado.Rows[0];
       string estado = row["Estado"]?.ToString() ?? "ERROR";

  if (estado == "SUCCESS")
         {
    return Ok(new ApiResponse<object>
            {
         Success = true,
    Mensaje = row["Mensaje"]?.ToString() ?? "Factura generada",
      Data = new
                {
          Estado = "SUCCESS",
         Mensaje = row["Mensaje"]?.ToString() ?? "",
       IdFactura = Convert.ToInt32(row["IdFactura"]),
            Total = Convert.ToDecimal(row["Total"])
        }
     });
       }
          else
    {
             return BadRequest(new ApiResponse<object>
{
          Success = false,
  Mensaje = row["Mensaje"]?.ToString() ?? "Error desconocido"
});
      }
       }
        else
     {
      return BadRequest(new ApiResponse<object>
  {
      Success = false,
         Mensaje = "No se recibió respuesta del servidor"
      });
     }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generando factura desde admin: {ex.Message}");
    return StatusCode(500, new ApiResponse<object> { Success = false, Mensaje = $"Error: {ex.Message}" });
  }
        }

     // ============================================================
        // ENDPOINTS AUXILIARES - RESTAURANTES Y MESAS
        // ============================================================
        
      [HttpGet]
        [Route("restaurantes")]
        public ActionResult<ApiResponse<List<Restaurante>>> ObtenerRestaurantes()
        {
 try
   {
                _logger.LogInformation("REST: Obteniendo restaurantes activos");
       
      var restaurantes = _restauranteDAO.ListarRestaurantesActivos();

     return Ok(new ApiResponse<List<Restaurante>>
      {
       Success = true,
         Mensaje = "Restaurantes obtenidos correctamente",
         Data = restaurantes
     });
     }
    catch (Exception ex)
  {
_logger.LogError($"Error al obtener restaurantes: {ex.Message}");
             return StatusCode(500, new ApiResponse<List<Restaurante>> { Success = false, Mensaje = $"Error: {ex.Message}" });
   }
        }

        [HttpGet]
  [Route("mesas/{idRestaurante}")]
        public ActionResult<ApiResponse<List<Mesa>>> ObtenerMesasPorRestaurante(int idRestaurante)
        {
 try
    {
        _logger.LogInformation($"REST: Obteniendo mesas del restaurante {idRestaurante}");

                if (idRestaurante <= 0)
      return BadRequest(new ApiResponse<List<Mesa>> { Success = false, Mensaje = "ID de restaurante no válido" });

     var mesas = _mesaDAO.ListarMesasPorRestaurante(idRestaurante);

       return Ok(new ApiResponse<List<Mesa>>
   {
 Success = true,
     Mensaje = "Mesas obtenidas correctamente",
  Data = mesas
     });
            }
catch (Exception ex)
    {
      _logger.LogError($"Error al obtener mesas: {ex.Message}");
     return StatusCode(500, new ApiResponse<List<Mesa>> { Success = false, Mensaje = $"Error: {ex.Message}" });
   }
     }
    }

    // ============================================================
    // DTOs para las solicitudes
    // ============================================================
    
    public class CrearReservaDto
    {
        public int IdUsuario { get; set; }
        public int IdMesa { get; set; }
    public int IdRestaurante { get; set; }
        public string FechaReserva { get; set; } = "";
        public string HoraReserva { get; set; } = "";
        public int NumeroPersonas { get; set; }
        public string? Notas { get; set; }
   public string? Estado { get; set; }
        public string? MetodoPago { get; set; }
    }

    public class ActualizarEstadoDto
    {
        public int IdReserva { get; set; }
   public string Estado { get; set; } = "";
        public string MetodoPago { get; set; } = "";
    }

    public class EditarReservaDto
    {
public int IdReserva { get; set; }
        public string FechaReserva { get; set; } = "";
        public string HoraReserva { get; set; } = "";
   public int NumeroPersonas { get; set; }
        public string? Notas { get; set; }
    }

    public class GenerarFacturaAdminDto
    {
    public int IdReserva { get; set; }
        public string MetodoPago { get; set; } = "";
        public string? TipoFactura { get; set; }
    }

  // Respuesta genérica para APIs REST
    public class ApiResponse<T>
    {
    public bool Success { get; set; }
        public string Mensaje { get; set; } = "";
   public T? Data { get; set; }
  }
}
