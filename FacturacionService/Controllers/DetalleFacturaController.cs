using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using FacturacionService.Models;
using FacturacionService.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace FacturacionService.Controllers
{
    [ApiController]
    [Route("api/detallefactura")]
    [EnableCors("AllowAll")]
    public class DetalleFacturaController : ControllerBase
    {
    private readonly ILogger<DetalleFacturaController> _logger;
        private readonly DetalleFacturaDAO _detalleDAO;
        private readonly string _connectionString;

    public DetalleFacturaController(ILogger<DetalleFacturaController> logger, IConfiguration configuration)
        {
            _logger = logger;
  _connectionString = configuration.GetConnectionString("DefaultConnection") 
         ?? throw new InvalidOperationException("Connection string not found");
      _detalleDAO = new DetalleFacturaDAO(_connectionString);
        }

   // ? GET: /api/detallefactura/factura/{idFactura}
        [HttpGet("factura/{idFactura:int}")]
        public IActionResult ListarPorFactura(int idFactura)
        {
       try
    {
        if (idFactura <= 0)
       {
         return BadRequest(new { success = false, message = "ID de factura no válido" });
          }

           DataTable dt = _detalleDAO.ListarDetallesPorFactura(idFactura);
       var detalles = ConvertirDataTableALista(dt);

          return Ok(new
    {
          success = true,
  message = "Detalles obtenidos correctamente",
        idFactura = idFactura,
           detalles = detalles,
           count = detalles.Count
     });
         }
      catch (Exception ex)
        {
    _logger.LogError($"Error al obtener detalles de factura: {ex.Message}");
        return StatusCode(500, new { success = false, message = "Error al obtener detalles de factura: " + ex.Message });
            }
        }

        // ? GET: /api/detallefactura/{id}
        [HttpGet("{id:int}")]
 public IActionResult ObtenerDetalle(int id)
        {
      try
   {
      if (id <= 0)
{
       return BadRequest(new { success = false, message = "ID de detalle no válido" });
  }

             DataTable dt = _detalleDAO.ObtenerDetallePorId(id);
        var detalles = ConvertirDataTableALista(dt);

            if (detalles.Count == 0)
      {
        return NotFound(new { success = false, message = "Detalle no encontrado" });
           }

      return Ok(new
       {
    success = true,
      message = "Detalle obtenido correctamente",
        detalle = detalles[0]
    });
         }
     catch (Exception ex)
   {
           _logger.LogError($"Error al obtener detalle: {ex.Message}");
          return StatusCode(500, new { success = false, message = "Error al obtener detalle: " + ex.Message });
            }
   }

     // ? POST: /api/detallefactura
  [HttpPost("")]
        public IActionResult CrearDetalle([FromBody] dynamic body)
     {
            try
         {
     if (body == null)
    {
       return BadRequest(new { success = false, message = "Datos del detalle requeridos" });
            }

                // Validar y extraer datos
      if (body.IdFactura == null || body.IdReserva == null ||
     body.Descripcion == null || body.Cantidad == null ||
   body.PrecioUnitario == null)
       {
return BadRequest(new { success = false, message = "Faltan campos requeridos: IdFactura, IdReserva, Descripcion, Cantidad, PrecioUnitario" });
           }

      int idFactura = Convert.ToInt32(body.IdFactura);
                int idReserva = Convert.ToInt32(body.IdReserva);
     string descripcion = body.Descripcion.ToString();
      int cantidad = Convert.ToInt32(body.Cantidad);
   decimal precioUnitario = Convert.ToDecimal(body.PrecioUnitario);

// Validaciones
        if (idFactura <= 0 || idReserva <= 0)
           {
           return BadRequest(new { success = false, message = "IDs de factura y reserva deben ser válidos" });
      }

    if (cantidad <= 0)
                {
  return BadRequest(new { success = false, message = "La cantidad debe ser mayor a cero" });
       }

if (precioUnitario < 0)
    {
        return BadRequest(new { success = false, message = "El precio unitario no puede ser negativo" });
     }

         _detalleDAO.InsertarDetalle(idFactura, idReserva, descripcion, cantidad, precioUnitario);

                return Ok(new
                {
    success = true,
      message = "Detalle de factura creado correctamente",
       idFactura = idFactura,
    idReserva = idReserva,
          subtotal = cantidad * precioUnitario
           });
     }
       catch (Exception ex)
            {
              _logger.LogError($"Error al crear detalle de factura: {ex.Message}");
        return BadRequest(new { success = false, message = "Error al crear detalle de factura: " + ex.Message });
         }
  }

  // ? PUT: /api/detallefactura/{id}
        [HttpPut("{id:int}")]
        public IActionResult ActualizarDetalle(int id, [FromBody] dynamic body)
   {
          try
   {
      if (id <= 0)
            {
 return BadRequest(new { success = false, message = "ID de detalle no válido" });
                }

   if (body == null)
        {
     return BadRequest(new { success = false, message = "Datos para actualizar requeridos" });
             }

 // Verificar que el detalle existe
     DataTable dtExiste = _detalleDAO.ObtenerDetallePorId(id);
     if (dtExiste.Rows.Count == 0)
        {
        return NotFound(new { success = false, message = "Detalle no encontrado" });
        }

  // Extraer datos
       string descripcion = body.Descripcion?.ToString();
  int cantidad = body.Cantidad != null ? Convert.ToInt32(body.Cantidad) : 1;
        decimal precioUnitario = body.PrecioUnitario != null ? Convert.ToDecimal(body.PrecioUnitario) : 0;

  // Validaciones
    if (cantidad <= 0)
           {
      return BadRequest(new { success = false, message = "La cantidad debe ser mayor a cero" });
                }

      if (precioUnitario < 0)
         {
         return BadRequest(new { success = false, message = "El precio unitario no puede ser negativo" });
   }

   _detalleDAO.ActualizarDetalle(id, descripcion, cantidad, precioUnitario);

 return Ok(new
       {
     success = true,
        message = "Detalle actualizado correctamente",
         idDetalle = id,
 nuevoSubtotal = cantidad * precioUnitario
             });
          }
    catch (Exception ex)
            {
      _logger.LogError($"Error al actualizar detalle: {ex.Message}");
                return BadRequest(new { success = false, message = "Error al actualizar detalle: " + ex.Message });
            }
        }

        // ? DELETE: /api/detallefactura/{id}
   [HttpDelete("{id:int}")]
        public IActionResult EliminarDetalle(int id)
        {
            try
       {
            if (id <= 0)
           {
 return BadRequest(new { success = false, message = "ID de detalle no válido" });
      }

      // Verificar que el detalle existe
          DataTable dtExiste = _detalleDAO.ObtenerDetallePorId(id);
      if (dtExiste.Rows.Count == 0)
                {
  return NotFound(new { success = false, message = "Detalle no encontrado" });
      }

       _detalleDAO.EliminarDetalle(id);

           return Ok(new
            {
       success = true,
       message = "Detalle eliminado correctamente",
      idDetalle = id
       });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar detalle: {ex.Message}");
    return BadRequest(new { success = false, message = "Error al eliminar detalle: " + ex.Message });
        }
     }

  // ? GET: /api/detallefactura/estadisticas/factura/{idFactura}
    [HttpGet("estadisticas/factura/{idFactura:int}")]
        public IActionResult ObtenerEstadisticasFactura(int idFactura)
        {
  try
            {
              if (idFactura <= 0)
 {
        return BadRequest(new { success = false, message = "ID de factura no válido" });
       }

             decimal subtotalCalculado = _detalleDAO.CalcularSubtotalFactura(idFactura);
 int cantidadDetalles = _detalleDAO.ContarDetallesFactura(idFactura);

    return Ok(new
      {
      success = true,
     message = "Estadísticas obtenidas correctamente",
    idFactura = idFactura,
       estadisticas = new
        {
       subtotalCalculado = subtotalCalculado,
                cantidadDetalles = cantidadDetalles,
      promedioDetalle = cantidadDetalles > 0 ? subtotalCalculado / cantidadDetalles : 0
              }
            });
    }
   catch (Exception ex)
            {
      _logger.LogError($"Error al obtener estadísticas: {ex.Message}");
   return StatusCode(500, new { success = false, message = "Error al obtener estadísticas: " + ex.Message });
        }
        }

        // ============================================================
      // ??? MÉTODOS AUXILIARES
    // ============================================================

        // Convertir DataTable a Lista de diccionarios
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
}
