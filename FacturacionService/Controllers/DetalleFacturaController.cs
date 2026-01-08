using Microsoft.AspNetCore.Mvc;
using FacturacionService.Data;
using FacturacionService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FacturacionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DetalleFacturaController : ControllerBase
    {
        private readonly ILogger<DetalleFacturaController> _logger;
      private readonly DetalleFacturaDAO _detalleDAO;

      public DetalleFacturaController(ILogger<DetalleFacturaController> logger, IConfiguration configuration)
        {
            _logger = logger;
            var connectionString = configuration.GetConnectionString("DefaultConnection");
    _detalleDAO = new DetalleFacturaDAO(connectionString);
        }

        // ============================================================
    // ? GET: Listar detalles por factura
        // ============================================================
    [HttpGet("factura/{idFactura:int}")]
  public ActionResult<ApiResponse<object>> ListarPorFactura(int idFactura)
        {
     try
   {
           if (idFactura <= 0)
      {
          return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "ID de factura no válido" });
}

    DataTable dt = _detalleDAO.ListarDetallesPorFactura(idFactura);
     var detalles = ConvertirDataTableALista(dt);

         return Ok(new ApiResponse<object>
         {
     Success = true,
 Mensaje = "Detalles obtenidos correctamente",
   Data = new
              {
 IdFactura = idFactura,
    Detalles = detalles,
                Count = detalles.Count
  }
        });
            }
         catch (Exception ex)
       {
       _logger.LogError($"Error al obtener detalles de factura: {ex.Message}");
           return StatusCode(500, new ApiResponse<object> { Success = false, Mensaje = $"Error: {ex.Message}" });
  }
  }

        // ============================================================
 // ? GET: Obtener detalle específico por ID
        // ============================================================
        [HttpGet("{id:int}")]
        public ActionResult<ApiResponse<object>> ObtenerDetalle(int id)
        {
     try
   {
    if (id <= 0)
              {
             return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "ID de detalle no válido" });
         }

           DataTable dt = _detalleDAO.ObtenerDetallePorId(id);
     var detalles = ConvertirDataTableALista(dt);

    if (detalles.Count == 0)
           {
    return NotFound(new ApiResponse<object> { Success = false, Mensaje = "Detalle no encontrado" });
 }

       return Ok(new ApiResponse<object>
    {
     Success = true,
               Mensaje = "Detalle obtenido correctamente",
           Data = detalles[0]
     });
    }
        catch (Exception ex)
     {
          _logger.LogError($"Error al obtener detalle: {ex.Message}");
       return StatusCode(500, new ApiResponse<object> { Success = false, Mensaje = $"Error: {ex.Message}" });
            }
    }

     // ============================================================
        // ? POST: Crear detalle de factura
        // ============================================================
    [HttpPost("")]
        public ActionResult<ApiResponse<object>> CrearDetalle([FromBody] CrearDetalleDto body)
     {
     try
      {
     if (body == null)
        {
   return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "Datos del detalle requeridos" });
          }

  // Validar campos requeridos
         if (body.IdFactura <= 0 || body.IdReserva <= 0)
     {
      return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "IDs de factura y reserva deben ser válidos" });
                }

         if (body.Cantidad <= 0)
           {
       return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "La cantidad debe ser mayor a cero" });
                }

        if (body.PrecioUnitario < 0)
      {
             return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "El precio unitario no puede ser negativo" });
   }

 _detalleDAO.InsertarDetalle(body.IdFactura, body.IdReserva, body.Descripcion, body.Cantidad, body.PrecioUnitario);

return Ok(new ApiResponse<object>
                {
         Success = true,
   Mensaje = "Detalle de factura creado correctamente",
        Data = new
    {
      IdFactura = body.IdFactura,
   IdReserva = body.IdReserva,
Subtotal = body.Cantidad * body.PrecioUnitario
    }
        });
 }
          catch (Exception ex)
            {
         _logger.LogError($"Error al crear detalle de factura: {ex.Message}");
        return StatusCode(500, new ApiResponse<object> { Success = false, Mensaje = $"Error: {ex.Message}" });
    }
        }

        // ============================================================
        // ? PUT: Actualizar detalle de factura
   // ============================================================
    [HttpPut("{id:int}")]
        public ActionResult<ApiResponse<object>> ActualizarDetalle(int id, [FromBody] ActualizarDetalleDto body)
        {
         try
      {
         if (id <= 0)
        {
     return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "ID de detalle no válido" });
      }

    if (body == null)
     {
               return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "Datos para actualizar requeridos" });
}

     // Verificar que el detalle existe
      DataTable dtExiste = _detalleDAO.ObtenerDetallePorId(id);
     if (dtExiste.Rows.Count == 0)
            {
return NotFound(new ApiResponse<object> { Success = false, Mensaje = "Detalle no encontrado" });
          }

   // Validaciones
 if (body.Cantidad <= 0)
       {
         return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "La cantidad debe ser mayor a cero" });
       }

     if (body.PrecioUnitario < 0)
        {
        return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "El precio unitario no puede ser negativo" });
        }

     _detalleDAO.ActualizarDetalle(id, body.Descripcion, body.Cantidad, body.PrecioUnitario);

   return Ok(new ApiResponse<object>
    {
           Success = true,
     Mensaje = "Detalle actualizado correctamente",
           Data = new
            {
            IdDetalle = id,
         NuevoSubtotal = body.Cantidad * body.PrecioUnitario
       }
    });
            }
 catch (Exception ex)
            {
           _logger.LogError($"Error al actualizar detalle: {ex.Message}");
      return StatusCode(500, new ApiResponse<object> { Success = false, Mensaje = $"Error: {ex.Message}" });
            }
        }

      // ============================================================
     // ? DELETE: Eliminar detalle de factura
        // ============================================================
        [HttpDelete("{id:int}")]
        public ActionResult<ApiResponse<object>> EliminarDetalle(int id)
   {
            try
          {
         if (id <= 0)
      {
             return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "ID de detalle no válido" });
        }

                // Verificar que el detalle existe
       DataTable dtExiste = _detalleDAO.ObtenerDetallePorId(id);
     if (dtExiste.Rows.Count == 0)
         {
              return NotFound(new ApiResponse<object> { Success = false, Mensaje = "Detalle no encontrado" });
     }

   _detalleDAO.EliminarDetalle(id);

     return Ok(new ApiResponse<object>
           {
          Success = true,
        Mensaje = "Detalle eliminado correctamente",
      Data = new { IdDetalle = id }
              });
}
  catch (Exception ex)
            {
       _logger.LogError($"Error al eliminar detalle: {ex.Message}");
 return StatusCode(500, new ApiResponse<object> { Success = false, Mensaje = $"Error: {ex.Message}" });
      }
  }

        // ============================================================
// ? GET: Estadísticas de factura
      // ============================================================
 [HttpGet("estadisticas/factura/{idFactura:int}")]
        public ActionResult<ApiResponse<object>> ObtenerEstadisticasFactura(int idFactura)
   {
     try
            {
        if (idFactura <= 0)
                {
return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "ID de factura no válido" });
         }

          decimal subtotalCalculado = _detalleDAO.CalcularSubtotalFactura(idFactura);
 int cantidadDetalles = _detalleDAO.ContarDetallesFactura(idFactura);

         return Ok(new ApiResponse<object>
         {
             Success = true,
          Mensaje = "Estadísticas obtenidas correctamente",
    Data = new
          {
       IdFactura = idFactura,
    Estadisticas = new
      {
                 SubtotalCalculado = subtotalCalculado,
   CantidadDetalles = cantidadDetalles,
      PromedioDetalle = cantidadDetalles > 0 ? subtotalCalculado / cantidadDetalles : 0
     }
   }
         });
    }
      catch (Exception ex)
  {
 _logger.LogError($"Error al obtener estadísticas: {ex.Message}");
    return StatusCode(500, new ApiResponse<object> { Success = false, Mensaje = $"Error: {ex.Message}" });
          }
        }

    // ============================================================
        // ??? MÉTODOS AUXILIARES
        // ============================================================
        private List<Dictionary<string, object>> ConvertirDataTableALista(DataTable table)
        {
          var lista = new List<Dictionary<string, object>>();

            if (table == null || table.Rows.Count == 0)
          {
       return lista;
          }

        foreach (DataRow row in table.Rows)
        {
      var diccionario = new Dictionary<string, object>();
     foreach (DataColumn column in table.Columns)
   {
         diccionario[column.ColumnName] = row[column] == DBNull.Value ? null : row[column];
                }
        lista.Add(diccionario);
     }

            return lista;
        }
    }

    // ============================================================
  // DTOs
    // ============================================================
    public class CrearDetalleDto
    {
        public int IdFactura { get; set; }
        public int IdReserva { get; set; }
  public string Descripcion { get; set; } = "";
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
  }

    public class ActualizarDetalleDto
 {
        public string Descripcion { get; set; } = "";
  public int Cantidad { get; set; }
 public decimal PrecioUnitario { get; set; }
  }
}
