#nullable enable

using Microsoft.AspNetCore.Mvc;
using ReservasService.Models;
using ReservasService.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace ReservasService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MesasController : ControllerBase
    {
        private readonly ILogger<MesasController> _logger;
        private readonly MesaDAO _mesaDAO;

 public MesasController(ILogger<MesasController> logger, IConfiguration configuration)
        {
    _logger = logger;
   var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
 _mesaDAO = new MesaDAO(connectionString);
  }

        // ============================================================
  // LISTAR TODAS LAS MESAS
        // ============================================================
  [HttpGet]
        [Route("")]
     public ActionResult<ApiResponse<List<object>>> Listar()
{
            try
    {
      _logger.LogInformation("REST: Listando todas las mesas");
        
          DataTable mesas = _mesaDAO.ListarMesas();

     // Convertir DataTable a lista de objetos
   var listaMesas = new List<object>();
   foreach (DataRow row in mesas.Rows)
         {
        listaMesas.Add(new
   {
               IdMesa = row["IdMesa"],
  IdRestaurante = row["IdRestaurante"],
         NumeroMesa = row["NumeroMesa"],
     TipoMesa = row["TipoMesa"],
          Capacidad = row["Capacidad"],
             Precio = row["Precio"] != DBNull.Value ? row["Precio"] : null,
          ImagenURL = row["ImagenURL"] != DBNull.Value ? row["ImagenURL"] : null,
        Estado = row["Estado"]
    });
           }

      return Ok(new ApiResponse<List<object>>
   {
              Success = true,
  Mensaje = "Mesas listadas correctamente",
        Data = listaMesas
       });
 }
      catch (Exception ex)
      {
         _logger.LogError($"Error al listar mesas: {ex.Message}");
      return StatusCode(500, new ApiResponse<List<object>> { Success = false, Mensaje = $"Error: {ex.Message}" });
   }
        }

     // ============================================================
        // OBTENER MESAS DISPONIBLES
     // ============================================================
  [HttpGet]
        [Route("disponibles")]
        public ActionResult<ApiResponse<List<object>>> ObtenerDisponibles([FromQuery] string zona, [FromQuery] int personas)
        {
         try
            {
     _logger.LogInformation($"REST: Obteniendo mesas disponibles - Zona: {zona}, Personas: {personas}");

         if (string.IsNullOrEmpty(zona))
       return BadRequest(new ApiResponse<List<object>> { Success = false, Mensaje = "Debe especificar una zona" });

 if (personas <= 0)
       return BadRequest(new ApiResponse<List<object>> { Success = false, Mensaje = "El número de personas debe ser mayor a 0" });

   DataTable mesas = _mesaDAO.MesasDisponibles(zona, personas);

          // Convertir DataTable a lista de objetos
      var listaMesas = new List<object>();
     foreach (DataRow row in mesas.Rows)
        {
 listaMesas.Add(new
         {
            IdMesa = row["IdMesa"],
    IdRestaurante = row["IdRestaurante"],
            NumeroMesa = row["NumeroMesa"],
         TipoMesa = row["TipoMesa"],
             Capacidad = row["Capacidad"],
   Precio = row["Precio"] != DBNull.Value ? row["Precio"] : null,
        ImagenURL = row["ImagenURL"] != DBNull.Value ? row["ImagenURL"] : null,
         Estado = row["Estado"]
      });
         }

                return Ok(new ApiResponse<List<object>>
  {
       Success = true,
             Mensaje = "Mesas disponibles obtenidas correctamente",
            Data = listaMesas
    });
            }
            catch (Exception ex)
            {
       _logger.LogError($"Error al obtener mesas disponibles: {ex.Message}");
     return StatusCode(500, new ApiResponse<List<object>> { Success = false, Mensaje = $"Error: {ex.Message}" });
            }
        }

     // ============================================================
        // OBTENER MESA POR ID
        // ============================================================
        [HttpGet]
[Route("{id}")]
        public ActionResult<ApiResponse<object>> ObtenerMesa(int id)
     {
   try
        {
   _logger.LogInformation($"REST: Obteniendo mesa {id}");

    if (id <= 0)
             return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "ID de mesa no válido" });

  DataTable todas = _mesaDAO.ListarMesas();
  DataRow[] filtro = todas.Select($"IdMesa = {id}");

        if (filtro.Length == 0)
        return NotFound(new ApiResponse<object> { Success = false, Mensaje = "Mesa no encontrada" });

          var mesa = filtro[0];

   var resultado = new
             {
   IdMesa = mesa["IdMesa"],
   NumeroMesa = mesa["NumeroMesa"],
     TipoMesa = mesa["TipoMesa"],
     Capacidad = mesa["Capacidad"],
       Precio = mesa["Precio"],
          ImagenURL = mesa["ImagenURL"],
   Estado = mesa["Estado"]
        };

     return Ok(new ApiResponse<object>
     {
     Success = true,
      Mensaje = "Mesa obtenida correctamente",
       Data = resultado
    });
            }
       catch (Exception ex)
      {
  _logger.LogError($"Error al obtener mesa: {ex.Message}");
    return StatusCode(500, new ApiResponse<object> { Success = false, Mensaje = $"Error: {ex.Message}" });
  }
        }

        // ============================================================
        // GESTIONAR MESA (CREAR O ACTUALIZAR)
        // ============================================================
 [HttpPost]
        [Route("gestionar")]
        public ActionResult<ApiResponse<string>> GestionarMesa([FromBody] GestionarMesaDto request)
     {
  try
      {
         _logger.LogInformation($"REST: Gestionando mesa - Operacion: {(request.IdMesa == null || request.IdMesa == 0 ? "INSERT" : "UPDATE")}");

     if (request.IdRestaurante <= 0)
            return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "Debe seleccionar un restaurante válido" });

   if (request.NumeroMesa <= 0)
          return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "Debe indicar el número de la mesa" });

          if (string.IsNullOrEmpty(request.TipoMesa))
return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "Debe especificar el tipo de mesa" });

    if (request.Capacidad <= 0)
         return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "La capacidad debe ser mayor a 0" });

     if (string.IsNullOrEmpty(request.Estado))
     return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "Debe especificar el estado de la mesa" });

        // Determinar operación: si IdMesa es null o 0, es INSERT
string operacion = (request.IdMesa == null || request.IdMesa == 0) ? "INSERT" : "UPDATE";

        _mesaDAO.GestionarMesa(
            operacion,
            request.IdMesa,  // Ahora puede ser null
  request.IdRestaurante,
       request.NumeroMesa,
            request.TipoMesa,
            request.Capacidad,
       request.Precio,
     request.ImagenURL ?? "",
            request.Estado
        );

        string mensaje = (request.IdMesa != null && request.IdMesa > 0) ? "Mesa actualizada correctamente" : "Mesa registrada correctamente";

        return Ok(new ApiResponse<string>
   {
      Success = true,
            Mensaje = mensaje
        });
    }
    catch (Exception ex)
    {
    _logger.LogError($"Error al gestionar mesa: {ex.Message}");
        return StatusCode(500, new ApiResponse<string> { Success = false, Mensaje = $"Error: {ex.Message}" });
    }
}

        // ============================================================
        // ACTUALIZAR ESTADO DE MESA
        // ============================================================
 [HttpPut]
      [Route("{id}/estado")]
        public ActionResult<ApiResponse<string>> ActualizarEstado(int id, [FromBody] ActualizarEstadoMesaDto request)
   {
     try
        {
      _logger.LogInformation($"REST: Actualizando estado de mesa {id}");

   if (id <= 0)
   return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "ID de mesa no válido" });

      if (string.IsNullOrEmpty(request.Estado))
    return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "Debe especificar el estado" });

         _mesaDAO.ActualizarEstado(id, request.Estado);

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
   // ELIMINAR (INACTIVAR) MESA
   // ============================================================
        [HttpDelete]
[Route("{id}")]
   public ActionResult<ApiResponse<string>> EliminarMesa(int id)
        {
     try
   {
    _logger.LogInformation($"REST: Inactivando mesa {id}");

   if (id <= 0)
     return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "ID de mesa no válido" });

   _mesaDAO.ActualizarEstado(id, "INACTIVA");

        return Ok(new ApiResponse<string>
     {
    Success = true,
    Mensaje = "Mesa inactivada correctamente"
   });
       }
          catch (Exception ex)
        {
_logger.LogError($"Error al eliminar mesa: {ex.Message}");
       return StatusCode(500, new ApiResponse<string> { Success = false, Mensaje = $"Error: {ex.Message}" });
   }
     }

        // ============================================================
     // OBTENER DISPONIBILIDAD DE MESA POR FECHA
    // ============================================================
 [HttpGet]
  [Route("{idMesa}/disponibilidad")]
     public ActionResult<ApiResponse<object>> ObtenerDisponibilidadMesa(int idMesa, [FromQuery] DateTime fecha, [FromQuery] string? hora = null)
 {
 try
 {
 _logger.LogInformation($"REST: Obteniendo disponibilidad de mesa {idMesa} para {fecha:yyyy-MM-dd} hora: {hora}");

 if (idMesa <=0)
 return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "ID de mesa no válido" });

 // Si se solicita una hora específica, devolver disponibilidad para esa fecha+hora
 if (!string.IsNullOrWhiteSpace(hora))
 {
 // Intentar parsear la hora (acepta "HH:mm" o "HH:mm:ss")
 if (!TimeSpan.TryParse(hora, out TimeSpan ts))
 {
 return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "Formato de hora inválido. Use HH:mm o HH:mm:ss" });
 }

 bool disponible = _mesaDAO.EstaDisponibleEnHora(idMesa, fecha.Date, ts);

 return Ok(new ApiResponse<object>
 {
 Success = true,
 Mensaje = "Disponibilidad verificada correctamente",
 Data = new { Disponible = disponible }
 });
 }

 // Comportamiento anterior: devolver lista de horas ya reservadas en la fecha
 DataTable dt = _mesaDAO.ObtenerDisponibilidad(idMesa, fecha);

 var listaHoras = new List<object>();

 foreach (DataRow row in dt.Rows)
 {
 string valor = row["Hora"]?.ToString()?.Trim() ?? "";
 string horaSolo = valor;

 // Si por alguna razón el SP devuelve DATETIME, se corrige aquí
 if (valor.Contains(" "))
 {
 horaSolo = valor.Split(' ')[1]; // extrae solo HH:mm:ss
 }

 listaHoras.Add(new { Hora = horaSolo });
 }

 return Ok(new ApiResponse<object>
 {
 Success = true,
 Mensaje = "Disponibilidad obtenida correctamente",
 Data = listaHoras
 });
 }
 catch (Exception ex)
 {
 _logger.LogError($"Error al obtener disponibilidad: {ex.Message}");
 return StatusCode(500, new ApiResponse<object> { Success = false, Mensaje = $"Error: {ex.Message}" });
 }
 }
 }

 // ============================================================
 // DTOs para las solicitudes
 // ============================================================

 public class GestionarMesaDto
 {
 public int? IdMesa { get; set; } // Cambiado a nullable
 public int IdRestaurante { get; set; }
 public int NumeroMesa { get; set; }
 public string TipoMesa { get; set; } = "";
 public int Capacidad { get; set; }
 public decimal? Precio { get; set; }
 public string? ImagenURL { get; set; }
 public string Estado { get; set; } = "";
 }

 public class ActualizarEstadoMesaDto
 {
 public string Estado { get; set; } = "";
 }
}
