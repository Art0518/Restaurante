#nullable enable

using Microsoft.AspNetCore.Mvc;
using MenuService.Models;
using MenuService.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MenuService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PromocionesController : ControllerBase
    {
     private readonly ILogger<PromocionesController> _logger;
        private readonly PromocionDAO _promocionDAO;

public PromocionesController(ILogger<PromocionesController> logger, IConfiguration configuration)
        {
            _logger = logger;
            var connectionString = configuration.GetConnectionString("DefaultConnection");
    _promocionDAO = new PromocionDAO(connectionString);
        }

        /// <summary>
    /// Obtener promociones activas (solo las que están en el rango de fechas actual)
        /// </summary>
        [HttpGet("activas")]
     public ActionResult<ApiResponse<List<Promocion>>> ObtenerPromociones()
        {
     try
  {
    _logger.LogInformation("REST: Listando promociones activas");

       var promociones = _promocionDAO.ListarPromocionesActivas();

   return Ok(new ApiResponse<List<Promocion>>
       {
            Success = true,
       Mensaje = "Promociones listadas correctamente",
              Data = promociones
          });
            }
       catch (Exception ex)
     {
           _logger.LogError($"Error al listar promociones: {ex.Message}");
           return StatusCode(500, new ApiResponse<List<Promocion>> { Success = false, Mensaje = $"Error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Listar todas las promociones (sin filtro de fechas) - Para debugging
        /// </summary>
        [HttpGet("listar")]
        public ActionResult<ApiResponse<List<Promocion>>> ListarTodasPromociones()
        {
      try
     {
    _logger.LogInformation("REST: Listando todas las promociones");

     var promociones = _promocionDAO.ListarTodasPromociones();

 return Ok(new ApiResponse<List<Promocion>>
                {
      Success = true,
        Mensaje = $"Total de promociones: {promociones.Count}",
        Data = promociones
   });
            }
            catch (Exception ex)
   {
       _logger.LogError($"Error al listar todas las promociones: {ex.Message}");
 return StatusCode(500, new ApiResponse<List<Promocion>> { Success = false, Mensaje = $"Error: {ex.Message}" });
            }
        }

    /// <summary>
        /// Listar promociones por estado (Activa o Inactiva)
        /// </summary>
 [HttpGet("por-estado/{estado}")]
   public ActionResult<ApiResponse<List<Promocion>>> ListarPromocionesPorEstado(string estado)
   {
     try
          {
      _logger.LogInformation($"REST: Listando promociones con estado: {estado}");

    var promociones = _promocionDAO.ListarPromocionesPorEstado(estado);

      return Ok(new ApiResponse<List<Promocion>>
                {
       Success = true,
            Mensaje = $"Promociones con estado '{estado}': {promociones.Count}",
              Data = promociones
         });
     }
       catch (Exception ex)
        {
       _logger.LogError($"Error al listar promociones por estado: {ex.Message}");
    return StatusCode(500, new ApiResponse<List<Promocion>> { Success = false, Mensaje = $"Error: {ex.Message}" });
    }
      }

        /// <summary>
        /// Obtener promoción por ID
      /// </summary>
        [HttpGet("{idPromocion}")]
   public ActionResult<ApiResponse<Promocion>> ObtenerPromocionById(int idPromocion)
        {
  try
    {
         _logger.LogInformation($"REST: Obteniendo promoción {idPromocion}");

         if (idPromocion <= 0)
           return BadRequest(new ApiResponse<Promocion> { Success = false, Mensaje = "ID de promoción no válido" });

      var promocion = _promocionDAO.ObtenerPromocionById(idPromocion);

       if (promocion == null)
            return NotFound(new ApiResponse<Promocion> { Success = false, Mensaje = "Promoción no encontrada" });

        return Ok(new ApiResponse<Promocion>
    {
          Success = true,
             Mensaje = "Promoción obtenida correctamente",
              Data = promocion
     });
  }
        catch (Exception ex)
            {
                _logger.LogError($"Error al obtener promoción: {ex.Message}");
      return StatusCode(500, new ApiResponse<Promocion> { Success = false, Mensaje = $"Error: {ex.Message}" });
         }
      }

        /// <summary>
        /// Crear una nueva promoción
 /// </summary>
        [HttpPost("crear")]
        public ActionResult<ApiResponse<Promocion>> CrearPromocion([FromBody] CrearPromocionDto request)
     {
        try
         {
         _logger.LogInformation($"REST: Creando promoción: {request.Nombre}");

       if (string.IsNullOrWhiteSpace(request.Nombre))
           return BadRequest(new ApiResponse<Promocion> { Success = false, Mensaje = "El nombre de la promoción es requerido" });

          var promocion = new Promocion
     {
    IdRestaurante = request.IdRestaurante,
               Nombre = request.Nombre,
          Descuento = request.PorcentajeDescuento,
       FechaInicio = DateTime.Parse(request.FechaInicio),
         FechaFin = DateTime.Parse(request.FechaFin),
      Estado = request.Activo ? "Activa" : "Inactiva"
};

      promocion = _promocionDAO.CrearPromocion(promocion);

      return Ok(new ApiResponse<Promocion>
             {
           Success = true,
    Mensaje = "Promoción creada correctamente",
          Data = promocion
                });
    }
 catch (Exception ex)
   {
          _logger.LogError($"Error al crear promoción: {ex.Message}");
              return StatusCode(500, new ApiResponse<Promocion> { Success = false, Mensaje = $"Error: {ex.Message}" });
            }
        }

        // ============================================================
     // ENDPOINTS PARA ADMINISTRACIÓN DE PROMOCIONES
        // ============================================================

        /// <summary>
  /// Crear promoción para administración (con valores automáticos)
     /// </summary>
  [HttpPost("admin/crear")]
        public ActionResult<ApiResponse<Promocion>> CrearPromocionAdmin([FromBody] CrearPromocionAdminDto request)
        {
            try
  {
          _logger.LogInformation($"REST: Creando promoción admin: {request.Nombre}");

      // === VALIDACIONES ===
                if (string.IsNullOrWhiteSpace(request.Nombre))
    return BadRequest(new ApiResponse<Promocion> { Success = false, Mensaje = "El nombre de la promoción es obligatorio" });

         if (request.PorcentajeDescuento <= 0 || request.PorcentajeDescuento > 100)
          return BadRequest(new ApiResponse<Promocion> { Success = false, Mensaje = "El porcentaje de descuento debe estar entre 1 y 100" });

         if (string.IsNullOrWhiteSpace(request.FechaInicio) || string.IsNullOrWhiteSpace(request.FechaFin))
      return BadRequest(new ApiResponse<Promocion> { Success = false, Mensaje = "Las fechas de inicio y fin son obligatorias" });

   // Validar fechas
   if (!DateTime.TryParse(request.FechaInicio, out DateTime fechaInicio))
         return BadRequest(new ApiResponse<Promocion> { Success = false, Mensaje = "Fecha de inicio inválida" });

 if (!DateTime.TryParse(request.FechaFin, out DateTime fechaFin))
     return BadRequest(new ApiResponse<Promocion> { Success = false, Mensaje = "Fecha de fin inválida" });

    if (fechaFin <= fechaInicio)
          return BadRequest(new ApiResponse<Promocion> { Success = false, Mensaje = "La fecha de fin debe ser posterior a la fecha de inicio" });

      // Verificar si ya existe una promoción con ese nombre
   var promocionesExistentes = _promocionDAO.ListarTodasPromociones();
                if (promocionesExistentes.Any(p => p.Nombre.Equals(request.Nombre, StringComparison.OrdinalIgnoreCase)))
     {
   return BadRequest(new ApiResponse<Promocion> { Success = false, Mensaje = "Ya existe una promoción con ese nombre" });
                }

            // Crear promoción con valores automáticos
 var promocion = new Promocion
            {
       IdRestaurante = 2, // Automático
Nombre = request.Nombre,
          Descuento = request.PorcentajeDescuento,
        FechaInicio = fechaInicio,
  FechaFin = fechaFin,
      Estado = "Activa" // Automático
         };

      promocion = _promocionDAO.CrearPromocion(promocion);

     return Ok(new ApiResponse<Promocion>
       {
        Success = true,
  Mensaje = $"Promoción '{request.Nombre}' creada exitosamente",
         Data = promocion
      });
    }
       catch (Exception ex)
  {
    _logger.LogError($"Error al crear promoción admin: {ex.Message}");
  return StatusCode(500, new ApiResponse<Promocion> { Success = false, Mensaje = $"Error: {ex.Message}" });
   }
        }

        /// <summary>
    /// Actualizar promoción para administración
        /// </summary>
        [HttpPut("admin/{idPromocion}")]
        public ActionResult<ApiResponse<string>> ActualizarPromocionAdmin(int idPromocion, [FromBody] ActualizarPromocionAdminDto request)
        {
       try
          {
                _logger.LogInformation($"REST: Actualizando promoción admin {idPromocion}");

       if (idPromocion <= 0)
     return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "ID de promoción no válido" });

    // Verificar que la promoción existe
var promocionExistente = _promocionDAO.ObtenerPromocionById(idPromocion);
         if (promocionExistente == null)
   {
      return NotFound(new ApiResponse<string> { Success = false, Mensaje = "Promoción no encontrada" });
                }

     // === VALIDACIONES ===
        if (!string.IsNullOrWhiteSpace(request.Nombre))
          {
           // Verificar si ya existe otra promoción con ese nombre
   var promocionesExistentes = _promocionDAO.ListarTodasPromociones();
     if (promocionesExistentes.Any(p => p.Nombre.Equals(request.Nombre, StringComparison.OrdinalIgnoreCase)
 && p.IdPromocion != idPromocion))
        {
     return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "Ya existe otra promoción con ese nombre" });
             }
           }

       if (request.PorcentajeDescuento.HasValue && (request.PorcentajeDescuento.Value <= 0 || request.PorcentajeDescuento.Value > 100))
         return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "El porcentaje de descuento debe estar entre 1 y 100" });

     // Validar fechas si se proporcionan
         DateTime? fechaInicio = null;
        DateTime? fechaFin = null;

  if (!string.IsNullOrWhiteSpace(request.FechaInicio))
           {
  if (!DateTime.TryParse(request.FechaInicio, out DateTime tempInicio))
            return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "Fecha de inicio inválida" });
         fechaInicio = tempInicio;
           }

                if (!string.IsNullOrWhiteSpace(request.FechaFin))
     {
 if (!DateTime.TryParse(request.FechaFin, out DateTime tempFin))
              return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "Fecha de fin inválida" });
        fechaFin = tempFin;
           }

      // Validar que fechaFin > fechaInicio si ambas se proporcionan
 DateTime fechaInicioFinal = fechaInicio ?? promocionExistente.FechaInicio;
                DateTime fechaFinFinal = fechaFin ?? promocionExistente.FechaFin;

       if (fechaFinFinal <= fechaInicioFinal)
        return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "La fecha de fin debe ser posterior a la fecha de inicio" });

        // Validar estado
                if (!string.IsNullOrEmpty(request.Estado))
      {
            var estadosPermitidos = new[] { "ACTIVA", "INACTIVA" };
       if (!estadosPermitidos.Contains(request.Estado.ToUpper()))
        {
      return BadRequest(new ApiResponse<string>
          {
            Success = false,
             Mensaje = "El estado debe ser 'ACTIVA' o 'INACTIVA'"
  });
     }
    }

    // Crear promoción actualizada
  var promocionActualizada = new Promocion
      {
 IdPromocion = idPromocion,
             IdRestaurante = promocionExistente.IdRestaurante,
          Nombre = !string.IsNullOrWhiteSpace(request.Nombre) ? request.Nombre : promocionExistente.Nombre,
 Descuento = request.PorcentajeDescuento ?? promocionExistente.Descuento,
         FechaInicio = fechaInicioFinal,
      FechaFin = fechaFinFinal,
         Estado = !string.IsNullOrEmpty(request.Estado) ? request.Estado : promocionExistente.Estado
        };

       // Actualizar la promoción
           bool resultado = _promocionDAO.ActualizarPromocion(promocionActualizada);

   if (!resultado)
 return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "No se pudo actualizar la promoción" });

      return Ok(new ApiResponse<string>
 {
         Success = true,
            Mensaje = $"Promoción actualizada correctamente"
                });
          }
         catch (Exception ex)
         {
         _logger.LogError($"Error al actualizar promoción admin: {ex.Message}");
         return StatusCode(500, new ApiResponse<string> { Success = false, Mensaje = $"Error: {ex.Message}" });
            }
 }

        /// <summary>
        /// Inactivar promoción (cambiar estado a INACTIVA)
        /// </summary>
        [HttpDelete("admin/{idPromocion}")]
        public ActionResult<ApiResponse<string>> InactivarPromocionAdmin(int idPromocion)
        {
         try
            {
                _logger.LogInformation($"REST: Inactivando promoción admin {idPromocion}");

       if (idPromocion <= 0)
 return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "ID de promoción no válido" });

  // Verificar que la promoción existe
      var promocionExistente = _promocionDAO.ObtenerPromocionById(idPromocion);
     if (promocionExistente == null)
      {
    return NotFound(new ApiResponse<string> { Success = false, Mensaje = "Promoción no encontrada" });
      }

         // Cambiar estado a INACTIVA
    var promocionInactiva = new Promocion
      {
          IdPromocion = idPromocion,
             IdRestaurante = promocionExistente.IdRestaurante,
       Nombre = promocionExistente.Nombre,
        Descuento = promocionExistente.Descuento,
FechaInicio = promocionExistente.FechaInicio,
         FechaFin = promocionExistente.FechaFin,
Estado = "Inactiva"
         };

            bool resultado = _promocionDAO.ActualizarPromocion(promocionInactiva);

       if (!resultado)
     return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "No se pudo inactivar la promoción" });

      return Ok(new ApiResponse<string>
 {
  Success = true,
        Mensaje = $"Promoción '{promocionExistente.Nombre}' inactivada correctamente"
      });
            }
          catch (Exception ex)
  {
            _logger.LogError($"Error al inactivar promoción admin: {ex.Message}");
    return StatusCode(500, new ApiResponse<string> { Success = false, Mensaje = $"Error: {ex.Message}" });
            }
}

        /// <summary>
        /// Listar promociones para administración con filtros
    /// </summary>
        [HttpGet("admin/listar")]
        public ActionResult<ApiResponse<List<object>>> ListarPromocionesAdmin(
    [FromQuery] string? estado = null,
    [FromQuery] bool? vigentes = null)
        {
    try
            {
 _logger.LogInformation("REST: Listando promociones para administración con filtros");

         var todasPromociones = _promocionDAO.ListarTodasPromociones();

           // Aplicar filtros
 var promocionesFiltradas = todasPromociones.AsEnumerable();

     if (!string.IsNullOrEmpty(estado))
     {
    promocionesFiltradas = promocionesFiltradas.Where(p =>
      p.Estado.Equals(estado, StringComparison.OrdinalIgnoreCase));
        }

  if (vigentes.HasValue)
 {
           DateTime ahora = DateTime.Now;
  if (vigentes.Value)
        {
      // Solo promociones vigentes (activas y en rango de fechas)
         promocionesFiltradas = promocionesFiltradas.Where(p =>
  p.Estado.Equals("Activa", StringComparison.OrdinalIgnoreCase) &&
      p.FechaInicio <= ahora && p.FechaFin >= ahora);
         }
else
    {
        // Solo promociones no vigentes
                 promocionesFiltradas = promocionesFiltradas.Where(p =>
      !p.Estado.Equals("Activa", StringComparison.OrdinalIgnoreCase) ||
         p.FechaInicio > ahora || p.FechaFin < ahora);
  }
          }

     // Convertir a formato de respuesta con información adicional
       var promocionesRespuesta = promocionesFiltradas.Select(p => new
       {
    p.IdPromocion,
   p.IdRestaurante,
        p.Nombre,
                 p.Descuento,
  p.FechaInicio,
  p.FechaFin,
       p.Estado,
         // Información adicional
        DescuentoFormateado = $"{p.Descuento:F1}%",
           FechaInicioFormateada = p.FechaInicio.ToString("dd/MM/yyyy"),
      FechaFinFormateada = p.FechaFin.ToString("dd/MM/yyyy"),
    DuracionDias = (p.FechaFin - p.FechaInicio).Days + 1,
  EsVigente = p.Estado.Equals("Activa", StringComparison.OrdinalIgnoreCase) &&
    p.FechaInicio <= DateTime.Now && p.FechaFin >= DateTime.Now,
          EstadoColor = p.Estado.ToUpper() switch
      {
            "ACTIVA" => DateTime.Now >= p.FechaInicio && DateTime.Now <= p.FechaFin ? "green" : "orange",
            "INACTIVA" => "red",
    _ => "gray"
   },
      DiasRestantes = p.Estado.Equals("Activa", StringComparison.OrdinalIgnoreCase) && DateTime.Now <= p.FechaFin
     ? Math.Max(0, (p.FechaFin - DateTime.Now).Days)
  : 0
    }).OrderByDescending(p => p.FechaInicio).ToList();

        return Ok(new ApiResponse<List<object>>
      {
      Success = true,
        Mensaje = $"Se encontraron {promocionesRespuesta.Count} promociones",
     Data = promocionesRespuesta.Cast<object>().ToList()
           });
         }
      catch (Exception ex)
            {
     _logger.LogError($"Error al listar promociones admin: {ex.Message}");
      return StatusCode(500, new ApiResponse<List<object>> { Success = false, Mensaje = $"Error: {ex.Message}" });
            }
        }
    }

    // ============================================================
  // DTOs PARA PROMOCIONES
    // ============================================================

    public class CrearPromocionDto
{
        public int IdRestaurante { get; set; } = 2; // Por defecto restaurante 2
        public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = ""; // Se puede usar en Nombre si se necesita
        public decimal PorcentajeDescuento { get; set; }
        public string FechaInicio { get; set; } = "";
      public string FechaFin { get; set; } = "";
        public bool Activo { get; set; } = true;
    }

    public class CrearPromocionAdminDto
    {
        public string Nombre { get; set; } = ""; // Obligatorio
        public decimal PorcentajeDescuento { get; set; } // Obligatorio, entre 1 y 100
        public string FechaInicio { get; set; } = ""; // Obligatorio, formato: "YYYY-MM-DD"
        public string FechaFin { get; set; } = ""; // Obligatorio, formato: "YYYY-MM-DD"
    }

    public class ActualizarPromocionAdminDto
    {
        public string? Nombre { get; set; } // Opcional
        public decimal? PorcentajeDescuento { get; set; } // Opcional, pero si se envía debe estar entre 1 y 100
        public string? FechaInicio { get; set; } // Opcional, formato: "YYYY-MM-DD"
        public string? FechaFin { get; set; } // Opcional, formato: "YYYY-MM-DD"
        public string? Estado { get; set; } // Opcional: ACTIVA, INACTIVA
    }
}
