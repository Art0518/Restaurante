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

    // Respuesta genérica para APIs REST
    public class ApiResponse<T>
  {
 public bool Success { get; set; }
 public string Mensaje { get; set; } = "";
     public T? Data { get; set; }
 }
}
