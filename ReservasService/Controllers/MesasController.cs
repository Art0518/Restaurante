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
        // GESTIONAR MESA (CREAR O ACTUALIZAR) - VERSIÓN MEJORADA
        // ============================================================
        [HttpPost]
        [Route("admin/gestionar")]
        public ActionResult<ApiResponse<string>> GestionarMesaAdmin([FromBody] GestionarMesaAdminDto request)
        {
            try
            {
                _logger.LogInformation($"REST: Gestionando mesa Admin - Operacion: {(request.IdMesa == null || request.IdMesa == 0 ? "CREAR" : "ACTUALIZAR")}");

                // === VALIDACIONES ESTRICTAS ===

                // Validar IdRestaurante (obligatorio)
                if (request.IdRestaurante <= 0)
                    return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "Debe seleccionar un restaurante válido" });

                // Validar TipoMesa (solo Interior o Exterior)
                if (string.IsNullOrWhiteSpace(request.TipoMesa) ||
        (!request.TipoMesa.Equals("Interior", StringComparison.OrdinalIgnoreCase) &&
      !request.TipoMesa.Equals("Exterior", StringComparison.OrdinalIgnoreCase)))
                {
                    return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "El tipo de mesa debe ser 'Interior' o 'Exterior'" });
                }

                // Validar Capacidad (entre 2 y 6 personas)
                if (request.Capacidad < 2 || request.Capacidad > 6)
                    return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "La capacidad debe estar entre 2 y 6 personas" });

                // Validar Precio (opcional, pero si se envía debe ser positivo)
                if (request.Precio.HasValue && request.Precio.Value <= 0)
                    return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "El precio debe ser mayor a 0" });

                bool esCreacion = (request.IdMesa == null || request.IdMesa == 0);

                if (esCreacion)
                {
                    // === CREAR MESA ===

                    // Validar NumeroMesa (obligatorio para crear)
                    if (request.NumeroMesa <= 0)
                        return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "Debe indicar el número de la mesa" });

                    // Verificar que el número de mesa no se repita según la zona
                    if (VerificarNumeroMesaExiste(request.IdRestaurante, request.NumeroMesa, request.TipoMesa))
                    {
                        return BadRequest(new ApiResponse<string> {
                            Success = false,
                            Mensaje = $"Ya existe una mesa número {request.NumeroMesa} en la zona {request.TipoMesa}"
                        });
                    }

                    // Para creación, siempre ACTIVA
                    string estado = "DISPONIBLE";

                    _mesaDAO.GestionarMesa(
                        "INSERT",
                        null, // IdMesa es null para INSERT
                        request.IdRestaurante,
                        request.NumeroMesa,
                        request.TipoMesa,
                        request.Capacidad,
                        request.Precio ?? 0,
                        request.ImagenURL ?? "",
                        estado
                    );

                    return Ok(new ApiResponse<string>
                    {
                        Success = true,
                        Mensaje = $"Mesa {request.NumeroMesa} creada exitosamente en zona {request.TipoMesa}"
                    });
                }
                else
                {
                    // === ACTUALIZAR MESA ===

                    // Verificar que la mesa existe
                    if (!VerificarMesaExiste(request.IdMesa.Value))
                    {
                        return NotFound(new ApiResponse<string> { Success = false, Mensaje = "Mesa no encontrada" });
                    }

                    // Si se cambia el número de mesa, verificar que no existe en esa zona
                    if (request.NumeroMesa > 0)
                    {
                        if (VerificarNumeroMesaExiste(request.IdRestaurante, request.NumeroMesa, request.TipoMesa, request.IdMesa.Value))
                        {
                            return BadRequest(new ApiResponse<string> {
                                Success = false,
                                Mensaje = $"Ya existe otra mesa número {request.NumeroMesa} en la zona {request.TipoMesa}"
                            });
                        }
                    }

                    // Validar Estado (solo al actualizar)
                    var estadosPermitidos = new[] { "DISPONIBLE", "OCUPADA", "INACTIVA" };
                    if (!string.IsNullOrEmpty(request.Estado) && !estadosPermitidos.Contains(request.Estado.ToUpper()))
                    {
                        return BadRequest(new ApiResponse<string> {
                            Success = false,
                            Mensaje = "El estado debe ser 'DISPONIBLE', 'OCUPADA' o 'INACTIVA'"
                        });
                    }

                    // Obtener datos actuales de la mesa
                    var mesaActual = ObtenerMesaActual(request.IdMesa.Value);
                    if (mesaActual == null)
                    {
                        return NotFound(new ApiResponse<string> { Success = false, Mensaje = "Mesa no encontrada" });
                    }

                    _mesaDAO.GestionarMesa(
                       "UPDATE",
                        request.IdMesa.Value,
                       request.IdRestaurante,
                     request.NumeroMesa > 0 ? request.NumeroMesa : mesaActual.NumeroMesa,
                         request.TipoMesa,
                  request.Capacidad,
        request.Precio ?? mesaActual.Precio,
      string.IsNullOrWhiteSpace(request.ImagenURL) ? mesaActual.ImagenURL : request.ImagenURL,
         string.IsNullOrWhiteSpace(request.Estado) ? mesaActual.Estado : request.Estado.ToUpper()
             );

           return Ok(new ApiResponse<string>
      {
   Success = true,
        Mensaje = $"Mesa {request.IdMesa} actualizada exitosamente"
 });
                }
            }
 catch (Exception ex)
            {
    _logger.LogError($"Error al gestionar mesa: {ex.Message}");
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

                if (idMesa <= 0)
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
        // ============================================================
        // VERIFICAR SI MESA ESTÁ OCUPADA EN FECHA Y HORA ESPECÍFICA
        // ============================================================
        [HttpGet]
        [Route("{idMesa}/ocupada")]
        public ActionResult<ApiResponse<object>> VerificarMesaOcupada(int idMesa, [FromQuery] DateTime fecha, [FromQuery] string hora)
        {
            try
            {
                _logger.LogInformation($"REST: Verificando si mesa {idMesa} está ocupada para {fecha:yyyy-MM-dd} hora: {hora}");

                if (idMesa <= 0)
                    return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "ID de mesa no válido" });

                if (string.IsNullOrWhiteSpace(hora))
                    return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "Debe especificar la hora" });

                // Intentar parsear la hora
                if (!TimeSpan.TryParse(hora, out TimeSpan ts))
                {
                    return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "Formato de hora inválido. Use HH:mm o HH:mm:ss" });
                }

                bool disponible = _mesaDAO.EstaDisponibleEnHora(idMesa, fecha.Date, ts);
                bool ocupada = !disponible; // Invertir la lógica

                string estado = ocupada ? "OCUPADA" : "DISPONIBLE";
                string mensaje = ocupada
                    ? $"La mesa {idMesa} está OCUPADA el {fecha:yyyy-MM-dd} a las {hora}"
                    : $"La mesa {idMesa} está DISPONIBLE el {fecha:yyyy-MM-dd} a las {hora}";

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Mensaje = mensaje,
                    Data = new {
                        IdMesa = idMesa,
                        Fecha = fecha.ToString("yyyy-MM-dd"),
                        Hora = hora,
                        Estado = estado,
                        Ocupada = ocupada,
                        Disponible = disponible
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al verificar si mesa está ocupada: {ex.Message}");
                return StatusCode(500, new ApiResponse<object> { Success = false, Mensaje = $"Error: {ex.Message}" });
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

        // ============================================================
        // DTOs PARA ADMINISTRACIÓN DE MESAS
        // ============================================================
        
        public class GestionarMesaAdminDto
        {
     public int? IdMesa { get; set; } // null para crear, valor para actualizar
            public int IdRestaurante { get; set; }
 public int NumeroMesa { get; set; }
            public string TipoMesa { get; set; } = ""; // Solo "Interior" o "Exterior"
      public int Capacidad { get; set; } // Entre 2 y 6
        public decimal? Precio { get; set; }
   public string? ImagenURL { get; set; }
      public string? Estado { get; set; } // Solo al actualizar: DISPONIBLE, OCUPADA, INACTIVA
 }

        public class ActualizarImagenMesaDto
        {
         public string? ImagenURL { get; set; } // null o vacío para eliminar imagen
  }

        // Respuesta genérica para APIs REST
        public class ApiResponse<T>
        {
            public bool Success { get; set; }
            public string Mensaje { get; set; } = "";
            public T? Data { get; set; }
        }

        // ============================================================
        // ELIMINAR/CAMBIAR FOTO DE MESA
        // ============================================================
        [HttpPut]
     [Route("{id}/imagen")]
        public ActionResult<ApiResponse<string>> ActualizarImagenMesa(int id, [FromBody] ActualizarImagenMesaDto request)
   {
  try
  {
_logger.LogInformation($"REST: Actualizando imagen de mesa {id}");

   if (id <= 0)
   return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "ID de mesa no válido" });

           // Verificar que la mesa existe
      if (!VerificarMesaExiste(id))
   {
      return NotFound(new ApiResponse<string> { Success = false, Mensaje = "Mesa no encontrada" });
                }

      // Obtener datos actuales
        var mesaActual = ObtenerMesaActual(id);
       if (mesaActual == null)
        {
         return NotFound(new ApiResponse<string> { Success = false, Mensaje = "Mesa no encontrada" });
          }

         // Actualizar solo la imagen
              _mesaDAO.GestionarMesa(
       "UPDATE",
          id,
 mesaActual.IdRestaurante,
  mesaActual.NumeroMesa,
          mesaActual.TipoMesa,
         mesaActual.Capacidad,
              mesaActual.Precio,
   request.ImagenURL ?? "", // Si es null o vacío, se elimina la imagen
              mesaActual.Estado
         );

     string mensaje = string.IsNullOrWhiteSpace(request.ImagenURL) 
        ? "Imagen eliminada correctamente" 
        : "Imagen actualizada correctamente";

         return Ok(new ApiResponse<string>
     {
        Success = true,
      Mensaje = mensaje
     });
       }
   catch (Exception ex)
     {
    _logger.LogError($"Error al actualizar imagen: {ex.Message}");
   return StatusCode(500, new ApiResponse<string> { Success = false, Mensaje = $"Error: {ex.Message}" });
  }
        }

        // ============================================================
     // MÉTODOS AUXILIARES PRIVADOS
        // ============================================================
        private bool VerificarNumeroMesaExiste(int idRestaurante, int numeroMesa, string tipoMesa, int? excluyeIdMesa = null)
        {
         try
 {
              DataTable mesas = _mesaDAO.ListarMesas();
  
     foreach (DataRow row in mesas.Rows)
          {
              int idMesa = Convert.ToInt32(row["IdMesa"]);
int idRest = Convert.ToInt32(row["IdRestaurante"]);
               int numMesa = Convert.ToInt32(row["NumeroMesa"]);
     string tipo = row["TipoMesa"]?.ToString()?.Trim() ?? "";

    // Excluir la mesa actual si se está actualizando
        if (excluyeIdMesa.HasValue && idMesa == excluyeIdMesa.Value)
       continue;

           if (idRest == idRestaurante && 
              numMesa == numeroMesa && 
 tipo.Equals(tipoMesa, StringComparison.OrdinalIgnoreCase))
             {
 return true; // Ya existe
        }
    }

        return false; // No existe
      }
            catch (Exception)
  {
   return true; // En caso de error, asumir que existe para evitar duplicados
  }
        }

        private bool VerificarMesaExiste(int idMesa)
        {
            try
  {
     DataTable mesas = _mesaDAO.ListarMesas();
         DataRow[] filtro = mesas.Select($"IdMesa = {idMesa}");
        return filtro.Length > 0;
       }
    catch (Exception)
            {
                return false;
       }
     }

  private Mesa? ObtenerMesaActual(int idMesa)
        {
            try
{
                DataTable mesas = _mesaDAO.ListarMesas();
      DataRow[] filtro = mesas.Select($"IdMesa = {idMesa}");
        
          if (filtro.Length == 0) return null;

      var row = filtro[0];
             return new Mesa
       {
         IdMesa = Convert.ToInt32(row["IdMesa"]),
        IdRestaurante = Convert.ToInt32(row["IdRestaurante"]),
   NumeroMesa = Convert.ToInt32(row["NumeroMesa"]),
        TipoMesa = row["TipoMesa"]?.ToString() ?? "",
    Capacidad = Convert.ToInt32(row["Capacidad"]),
            Precio = row["Precio"] != DBNull.Value ? Convert.ToDecimal(row["Precio"]) : 0m,
         ImagenURL = row["ImagenURL"]?.ToString() ?? "",
          Estado = row["Estado"]?.ToString() ?? ""
          };
       }
            catch (Exception)
            {
                return null;
   }
        }

        // ============================================================
     // LISTAR MESAS PARA ADMINISTRACIÓN (CON FILTROS)
        // ============================================================
       [HttpGet]
      [Route("admin/listar")]
 public ActionResult<ApiResponse<List<object>>> ListarMesasAdmin(
         [FromQuery] int? idRestaurante = null,
     [FromQuery] string? tipoMesa = null,
    [FromQuery] string? estado = null,
    [FromQuery] int? capacidad = null)
  {
        try
          {
    _logger.LogInformation("REST: Listando mesas para administración con filtros");
        
     DataTable todasMesas = _mesaDAO.ListarMesas();
  string filtro = "";

       // Construir filtros
      if (idRestaurante.HasValue)
  filtro += $"IdRestaurante = {idRestaurante.Value}";

    if (!string.IsNullOrEmpty(tipoMesa))
          {
      filtro += (filtro != "" ? " AND " : "") + $"TipoMesa = '{tipoMesa}'";
  }

       if (!string.IsNullOrEmpty(estado))
    {
      filtro += (filtro != "" ? " AND " : "") + $"Estado = '{estado}'";
   }

       if (capacidad.HasValue)
        {
   filtro += (filtro != "" ? " AND " : "") + $"Capacidad = {capacidad.Value}";
         }

     DataRow[] mesasFiltradas = string.IsNullOrEmpty(filtro)
       ? todasMesas.Select()
         : todasMesas.Select(filtro);

    // Convertir a lista de objetos con información adicional
   var listaMesas = new List<object>();
            foreach (DataRow row in mesasFiltradas)
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
    Estado = row["Estado"],
        // Información adicional
           TieneImagen = row["ImagenURL"] != DBNull.Value && !string.IsNullOrWhiteSpace(row["ImagenURL"].ToString()),
          PrecioFormateado = row["Precio"] != DBNull.Value ? $"${row["Precio"]:F2}" : "Sin precio"
   });
      }

      // Ordenar por TipoMesa y luego por NumeroMesa
        var mesasOrdenadas = listaMesas.OrderBy(m => ((dynamic)m).TipoMesa).ThenBy(m => ((dynamic)m).NumeroMesa);

        return Ok(new ApiResponse<List<object>>
       {
         Success = true,
      Mensaje = $"Se encontraron {listaMesas.Count} mesas",
       Data = mesasOrdenadas.ToList()
  });
   }
   catch (Exception ex)
   {
    _logger.LogError($"Error al listar mesas admin: {ex.Message}");
     return StatusCode(500, new ApiResponse<List<object>> { Success = false, Mensaje = $"Error: {ex.Message}" });
  }
        }
    }
}
