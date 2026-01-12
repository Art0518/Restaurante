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
    public class PlatosController : ControllerBase
    {
        private readonly ILogger<PlatosController> _logger;
        private readonly PlatoDAO _platoDAO;
        private readonly PromocionDAO _promocionDAO;

        public PlatosController(ILogger<PlatosController> logger, IConfiguration configuration)
 {
    _logger = logger;
         var connectionString = configuration.GetConnectionString("DefaultConnection");
     _platoDAO = new PlatoDAO(connectionString);
     _promocionDAO = new PromocionDAO(connectionString);
        }

        /// <summary>
        /// Obtener todos los platos
        /// </summary>
  [HttpGet("listar")]
   public ActionResult<ApiResponse<List<Plato>>> ObtenerPlatos()
      {
     try
            {
    _logger.LogInformation("REST: Listando todos los platos");

  var platos = _platoDAO.ListarTodosPlatos();

      return Ok(new ApiResponse<List<Plato>>
     {
         Success = true,
      Mensaje = "Platos listados correctamente",
                Data = platos
       });
      }
   catch (Exception ex)
         {
        _logger.LogError($"Error al listar platos: {ex.Message}");
           return StatusCode(500, new ApiResponse<List<Plato>> { Success = false, Mensaje = $"Error: {ex.Message}" });
         }
   }

 /// <summary>
        /// Obtener plato por ID
        /// </summary>
        [HttpGet("{idPlato}")]
        public ActionResult<ApiResponse<Plato>> ObtenerPlatoById(int idPlato)
        {
     try
            {
       _logger.LogInformation($"REST: Obteniendo plato {idPlato}");

       if (idPlato <= 0)
        return BadRequest(new ApiResponse<Plato> { Success = false, Mensaje = "ID de plato no válido" });

       var plato = _platoDAO.ObtenerPlatoById(idPlato);

             if (plato == null)
        return NotFound(new ApiResponse<Plato> { Success = false, Mensaje = "Plato no encontrado" });

                return Ok(new ApiResponse<Plato>
     {
         Success = true,
         Mensaje = "Plato obtenido correctamente",
         Data = plato
            });
         }
      catch (Exception ex)
   {
     _logger.LogError($"Error al obtener plato: {ex.Message}");
          return StatusCode(500, new ApiResponse<Plato> { Success = false, Mensaje = $"Error: {ex.Message}" });
    }
        }

        /// <summary>
        /// Crear un nuevo plato
        /// </summary>
        [HttpPost("crear")]
        public ActionResult<ApiResponse<Plato>> CrearPlato([FromBody] CrearPlatoDto request)
        {
            try
  {
            _logger.LogInformation($"REST: Creando plato: {request.Nombre}");

    if (string.IsNullOrWhiteSpace(request.Nombre))
       return BadRequest(new ApiResponse<Plato> { Success = false, Mensaje = "El nombre del plato es requerido" });

                var plato = new Plato
{
         IdRestaurante = request.IdRestaurante,
             Nombre = request.Nombre,
        Descripcion = request.Descripcion,
    Precio = request.Precio,
          Categoria = request.Categoria,
           TipoComida = request.TipoComida,
           ImagenURL = request.ImagenURL,
  Stock = request.Stock,
   Estado = request.Estado
   };

     plato = _platoDAO.CrearPlato(plato);

                return Ok(new ApiResponse<Plato>
           {
           Success = true,
    Mensaje = "Plato creado correctamente",
             Data = plato
           });
    }
 catch (Exception ex)
     {
          _logger.LogError($"Error al crear plato: {ex.Message}");
  return StatusCode(500, new ApiResponse<Plato> { Success = false, Mensaje = $"Error: {ex.Message}" });
   }
        }

   /// <summary>
     /// Actualizar plato
     /// </summary>
      [HttpPut("{idPlato}")]
        public ActionResult<ApiResponse<string>> ActualizarPlato(int idPlato, [FromBody] ActualizarPlatoDto request)
   {
try
      {
           _logger.LogInformation($"REST: Actualizando plato {idPlato}");

if (idPlato <= 0)
 return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "ID de plato no válido" });

    var plato = new Plato
           {
       IdPlato = idPlato,
         Nombre = request.Nombre,
       Descripcion = request.Descripcion,
          Precio = request.Precio,
             Categoria = request.Categoria,
     TipoComida = request.TipoComida,
         ImagenURL = request.ImagenURL,
   Stock = request.Stock,
                 Estado = request.Estado
          };

           var resultado = _platoDAO.ActualizarPlato(plato);

       if (!resultado)
        return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "No se pudo actualizar el plato" });

        return Ok(new ApiResponse<string>
   {
   Success = true,
   Mensaje = "Plato actualizado correctamente"
});
     }
            catch (Exception ex)
    {
       _logger.LogError($"Error al actualizar plato: {ex.Message}");
       return StatusCode(500, new ApiResponse<string> { Success = false, Mensaje = $"Error: {ex.Message}" });
    }
 }

        /// <summary>
  /// Eliminar plato
  /// </summary>
        [HttpDelete("{idPlato}")]
        public ActionResult<ApiResponse<string>> EliminarPlato(int idPlato)
  {
  try
     {
       _logger.LogInformation($"REST: Eliminando plato {idPlato}");

  if (idPlato <= 0)
        return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "ID de plato no válido" });

        var resultado = _platoDAO.EliminarPlato(idPlato);

       if (!resultado)
return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "No se pudo eliminar el plato" });

      return Ok(new ApiResponse<string>
{
    Success = true,
Mensaje = "Plato eliminado correctamente"
         });
            }
     catch (Exception ex)
 {
    _logger.LogError($"Error al eliminar plato: {ex.Message}");
       return StatusCode(500, new ApiResponse<string> { Success = false, Mensaje = $"Error: {ex.Message}" });
 }
        }

        // ============================================================
   // ENDPOINTS PARA ADMINISTRACIÓN DE PLATOS
 // ============================================================

        /// <summary>
        /// Crear plato para administración (con valores automáticos)
        /// </summary>
        [HttpPost("admin/crear")]
        public ActionResult<ApiResponse<Plato>> CrearPlatoAdmin([FromBody] CrearPlatoAdminDto request)
        {
         try
       {
      _logger.LogInformation($"REST: Creando plato admin: {request.Nombre}");

         // === VALIDACIONES ===
  if (string.IsNullOrWhiteSpace(request.Nombre))
  return BadRequest(new ApiResponse<Plato> { Success = false, Mensaje = "El nombre del plato es obligatorio" });

    if (string.IsNullOrWhiteSpace(request.Categoria))
              return BadRequest(new ApiResponse<Plato> { Success = false, Mensaje = "La categoría es obligatoria" });

   if (string.IsNullOrWhiteSpace(request.TipoComida))
   return BadRequest(new ApiResponse<Plato> { Success = false, Mensaje = "El tipo de comida es obligatorio" });

  if (request.Precio <= 0)
  return BadRequest(new ApiResponse<Plato> { Success = false, Mensaje = "El precio debe ser mayor a 0" });

       // Verificar si ya existe un plato con ese nombre
      var platosExistentes = _platoDAO.ListarTodosPlatos();
     if (platosExistentes.Any(p => p.Nombre.Equals(request.Nombre, StringComparison.OrdinalIgnoreCase) && p.IdRestaurante == 2))
            {
              return BadRequest(new ApiResponse<Plato> { Success = false, Mensaje = "Ya existe un plato con ese nombre" });
      }

     // Crear plato con valores automáticos
        var plato = new Plato
          {
    IdRestaurante = 2, // Automático
        Nombre = request.Nombre,
      Categoria = request.Categoria,
    TipoComida = request.TipoComida,
           Descripcion = request.Descripcion ?? "",
       Precio = request.Precio,
      ImagenURL = request.ImagenURL ?? "",
          Stock = 20, // Automático
        Estado = "ACTIVO" // Automático
                };

           plato = _platoDAO.CrearPlato(plato);

         return Ok(new ApiResponse<Plato>
      {
     Success = true,
          Mensaje = $"Plato '{request.Nombre}' creado exitosamente",
        Data = plato
            });
     }
      catch (Exception ex)
         {
     _logger.LogError($"Error al crear plato admin: {ex.Message}");
    return StatusCode(500, new ApiResponse<Plato> { Success = false, Mensaje = $"Error: {ex.Message}" });
            }
    }

        /// <summary>
        /// Actualizar plato para administración
     /// </summary>
        [HttpPut("admin/{idPlato}")]
        public ActionResult<ApiResponse<string>> ActualizarPlatoAdmin(int idPlato, [FromBody] ActualizarPlatoAdminDto request)
        {
            try
         {
      _logger.LogInformation($"REST: Actualizando plato admin {idPlato}");

        if (idPlato <= 0)
           return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "ID de plato no válido" });

      // Verificar que el plato existe
    var platoExistente = _platoDAO.ObtenerPlatoById(idPlato);
     if (platoExistente == null)
  {
        return NotFound(new ApiResponse<string> { Success = false, Mensaje = "Plato no encontrado" });
     }

    // === VALIDACIONES ===
             if (!string.IsNullOrWhiteSpace(request.Nombre))
             {
  // Verificar si ya existe otro plato con ese nombre
  var platosExistentes = _platoDAO.ListarTodosPlatos();
              if (platosExistentes.Any(p => p.Nombre.Equals(request.Nombre, StringComparison.OrdinalIgnoreCase)
           && p.IdRestaurante == 2 && p.IdPlato != idPlato))
          {
              return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "Ya existe otro plato con ese nombre" });
               }
       }

             if (request.Precio.HasValue && request.Precio.Value <= 0)
          return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "El precio debe ser mayor a 0" });

           if (request.Stock.HasValue && request.Stock.Value < 0)
         return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "El stock no puede ser negativo" });

      // Validar estado
      if (!string.IsNullOrEmpty(request.Estado))
  {
      var estadosPermitidos = new[] { "ACTIVO", "INACTIVO", "AGOTADO" };
           if (!estadosPermitidos.Contains(request.Estado.ToUpper()))
        {
              return BadRequest(new ApiResponse<string>
        {
       Success = false,
  Mensaje = "El estado debe ser 'ACTIVO', 'INACTIVO' o 'AGOTADO'"
           });
  }
  }

          // Actualizar solo los campos proporcionados
       var plato = new Plato
  {
     IdPlato = idPlato,
             IdRestaurante = platoExistente.IdRestaurante, // Mantener el existente
        Nombre = !string.IsNullOrWhiteSpace(request.Nombre) ? request.Nombre : platoExistente.Nombre,
         Categoria = !string.IsNullOrWhiteSpace(request.Categoria) ? request.Categoria : platoExistente.Categoria,
           TipoComida = !string.IsNullOrWhiteSpace(request.TipoComida) ? request.TipoComida : platoExistente.TipoComida,
     Descripcion = request.Descripcion ?? platoExistente.Descripcion,
     Precio = request.Precio ?? platoExistente.Precio,
    ImagenURL = request.ImagenURL ?? platoExistente.ImagenURL,
             Stock = request.Stock ?? platoExistente.Stock,
        Estado = !string.IsNullOrEmpty(request.Estado) ? request.Estado.ToUpper() : platoExistente.Estado
   };

          var resultado = _platoDAO.ActualizarPlato(plato);

    if (!resultado)
   return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "No se pudo actualizar el plato" });

     return Ok(new ApiResponse<string>
     {
     Success = true,
     Mensaje = $"Plato '{plato.Nombre}' actualizado correctamente"
       });
     }
            catch (Exception ex)
 {
      _logger.LogError($"Error al actualizar plato admin: {ex.Message}");
    return StatusCode(500, new ApiResponse<string> { Success = false, Mensaje = $"Error: {ex.Message}" });
     }
}

    /// <summary>
 /// Eliminar plato (cambiar estado a INACTIVO)
        /// </summary>
        [HttpDelete("admin/{idPlato}")]
     public ActionResult<ApiResponse<string>> EliminarPlatoAdmin(int idPlato)
        {
       try
            {
        _logger.LogInformation($"REST: Eliminando plato admin {idPlato}");

        if (idPlato <= 0)
    return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "ID de plato no válido" });

         // Verificar que el plato existe
       var platoExistente = _platoDAO.ObtenerPlatoById(idPlato);
  if (platoExistente == null)
     {
      return NotFound(new ApiResponse<string> { Success = false, Mensaje = "Plato no encontrado" });
     }

             // En lugar de eliminar, cambiar estado a INACTIVO
              var plato = new Plato
           {
     IdPlato = idPlato,
               IdRestaurante = platoExistente.IdRestaurante,
      Nombre = platoExistente.Nombre,
     Categoria = platoExistente.Categoria,
           TipoComida = platoExistente.TipoComida,
   Descripcion = platoExistente.Descripcion,
       Precio = platoExistente.Precio,
  ImagenURL = platoExistente.ImagenURL,
     Stock = platoExistente.Stock,
                    Estado = "INACTIVO" // Cambiar a INACTIVO en lugar de eliminar
          };

          var resultado = _platoDAO.ActualizarPlato(plato);

          if (!resultado)
    return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "No se pudo eliminar el plato" });

  return Ok(new ApiResponse<string>
        {
  Success = true,
          Mensaje = $"Plato '{platoExistente.Nombre}' eliminado correctamente"
       });
         }
     catch (Exception ex)
     {
_logger.LogError($"Error al eliminar plato admin: {ex.Message}");
     return StatusCode(500, new ApiResponse<string> { Success = false, Mensaje = $"Error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Listar platos para administración con filtros
        /// </summary>
        [HttpGet("admin/listar")]
        public ActionResult<ApiResponse<List<object>>> ListarPlatosAdmin(
            [FromQuery] string? categoria = null,
  [FromQuery] string? tipoComida = null,
 [FromQuery] string? estado = null)
        {
            try
          {
  _logger.LogInformation("REST: Listando platos para administración con filtros");

              var todosPlatos = _platoDAO.ListarTodosPlatos()
               .Where(p => p.IdRestaurante == 2) // Solo del restaurante 2
         .ToList();

      // Aplicar filtros
        var platosFiltrados = todosPlatos.AsEnumerable();

      if (!string.IsNullOrEmpty(categoria))
{
             platosFiltrados = platosFiltrados.Where(p =>
         p.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase));
    }

         if (!string.IsNullOrEmpty(tipoComida))
                {
        platosFiltrados = platosFiltrados.Where(p =>
     p.TipoComida.Equals(tipoComida, StringComparison.OrdinalIgnoreCase));
          }

         if (!string.IsNullOrEmpty(estado))
         {
             platosFiltrados = platosFiltrados.Where(p =>
       p.Estado.Equals(estado, StringComparison.OrdinalIgnoreCase));
     }

          // Convertir a formato de respuesta con información adicional
                var platosRespuesta = platosFiltrados.Select(p => new
 {
           p.IdPlato,
       p.IdRestaurante,
     p.Nombre,
      p.Categoria,
     p.TipoComida,
          p.Descripcion,
      p.Precio,
         p.ImagenURL,
             p.Stock,
       p.Estado,
        // Información adicional
         PrecioFormateado = $"${p.Precio:F2}",
          TieneImagen = !string.IsNullOrWhiteSpace(p.ImagenURL),
         StockStatus = p.Stock <= 0 ? "Agotado" : p.Stock <= 5 ? "Bajo" : "Normal",
  EstadoColor = p.Estado.ToUpper() switch
          {
   "ACTIVO" => "green",
        "INACTIVO" => "red",
      "AGOTADO" => "orange",
               _ => "gray"
   }
      }).OrderBy(p => p.Categoria).ThenBy(p => p.Nombre).Cast<object>().ToList();

      return Ok(new ApiResponse<List<object>>
                {
        Success = true,
         Mensaje = $"Se encontraron {platosRespuesta.Count} platos",
    Data = platosRespuesta
       });
            }
         catch (Exception ex)
            {
        _logger.LogError($"Error al listar platos admin: {ex.Message}");
     return StatusCode(500, new ApiResponse<List<object>> { Success = false, Mensaje = $"Error: {ex.Message}" });
        }
        }

        /// <summary>
  /// Actualizar solo la imagen de un plato
        /// </summary>
        [HttpPut("admin/{idPlato}/imagen")]
        public ActionResult<ApiResponse<string>> ActualizarImagenPlato(int idPlato, [FromBody] ActualizarImagenPlatoDto request)
        {
     try
            {
      _logger.LogInformation($"REST: Actualizando imagen de plato {idPlato}");

  if (idPlato <= 0)
  return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "ID de plato no válido" });

     // Verificar que el plato existe
          var platoExistente = _platoDAO.ObtenerPlatoById(idPlato);
                if (platoExistente == null)
         {
             return NotFound(new ApiResponse<string> { Success = false, Mensaje = "Plato no encontrado" });
     }

           // Actualizar solo la imagen
                var plato = new Plato
        {
           IdPlato = idPlato,
            IdRestaurante = platoExistente.IdRestaurante,
            Nombre = platoExistente.Nombre,
    Categoria = platoExistente.Categoria,
             TipoComida = platoExistente.TipoComida,
        Descripcion = platoExistente.Descripcion,
      Precio = platoExistente.Precio,
 ImagenURL = request.ImagenURL ?? "", // Si es null o vacío, se elimina la imagen
      Stock = platoExistente.Stock,
        Estado = platoExistente.Estado
    };

           var resultado = _platoDAO.ActualizarPlato(plato);

            if (!resultado)
          return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "No se pudo actualizar la imagen" });

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

        // DTOs para las solicitudes
        public class CrearPlatoDto
        {
    public int IdRestaurante { get; set; } = 2; // Por defecto restaurante 2
            public string Nombre { get; set; } = "";
       public string Descripcion { get; set; } = "";
     public decimal Precio { get; set; }
         public string Categoria { get; set; } = "";
            public string TipoComida { get; set; } = "";
   public string ImagenURL { get; set; } = "";
          public int Stock { get; set; } = 20;
 public string Estado { get; set; } = "ACTIVO";
        }

        public class ActualizarPlatoDto
 {
            public string Nombre { get; set; } = "";
     public string Descripcion { get; set; } = "";
        public decimal Precio { get; set; }
public string Categoria { get; set; } = "";
    public string TipoComida { get; set; } = "";
            public string ImagenURL { get; set; } = "";
            public int Stock { get; set; }
    public string Estado { get; set; } = "ACTIVO";
  }

   // ============================================================
        // DTOs PARA ADMINISTRACIÓN DE PLATOS
        // ============================================================

        public class CrearPlatoAdminDto
        {
            public string Nombre { get; set; } = ""; // Obligatorio
   public string Categoria { get; set; } = ""; // Obligatorio  
  public string TipoComida { get; set; } = ""; // Obligatorio
        public string? Descripcion { get; set; } // Opcional
            public decimal Precio { get; set; } // Obligatorio, > 0
    public string? ImagenURL { get; set; } // Opcional
        }

        public class ActualizarPlatoAdminDto
     {
         public string? Nombre { get; set; } // Opcional
            public string? Categoria { get; set; } // Opcional
public string? TipoComida { get; set; } // Opcional
     public string? Descripcion { get; set; } // Opcional
    public decimal? Precio { get; set; } // Opcional, pero si se envía debe ser > 0
            public string? ImagenURL { get; set; } // Opcional
    public int? Stock { get; set; } // Opcional, pero debe ser >= 0
            public string? Estado { get; set; } // Opcional: ACTIVO, INACTIVO, AGOTADO
        }

        public class ActualizarImagenPlatoDto
        {
        public string? ImagenURL { get; set; } // null o vacío para eliminar imagen
        }
    }

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

    // ============================================================
    // DTOs PARA ADMINISTRACIÓN DE PROMOCIONES
 // ============================================================

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

    // Respuesta genérica para APIs REST
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Mensaje { get; set; } = "";
     public T? Data { get; set; }
    }
}