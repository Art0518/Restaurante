using GDatos.Entidades;
using Logica.Servicios;
using System;
using System.Data;
using System.Web.Http;
using System.Collections.Generic;

namespace Ws_Restaurante.Controllers
{
    [RoutePrefix("api/detallefactura")]
    public class DetalleFacturaController : ApiController
    {
     private readonly DetalleFacturaLogica detalleLogica = new DetalleFacturaLogica();

  // ✅ GET: /api/detallefactura/factura/{idFactura}
     [HttpGet]
[Route("factura/{idFactura:int}")]
     public IHttpActionResult ListarPorFactura(int idFactura)
 {
            try
            {
          if (idFactura <= 0)
   {
           return BadRequest("ID de factura no válido");
         }

      DataTable dt = detalleLogica.ListarDetallesPorFactura(idFactura);
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
      return InternalServerError(new Exception("Error al obtener detalles de factura: " + ex.Message));
}
    }

        // ✅ GET: /api/detallefactura/{id}
        [HttpGet]
   [Route("{id:int}")]
    public IHttpActionResult ObtenerDetalle(int id)
  {
        try
   {
       if (id <= 0)
          {
       return BadRequest("ID de detalle no válido");
       }

    DataTable dt = detalleLogica.ObtenerDetallePorId(id);
            var detalles = ConvertirDataTableALista(dt);

   if (detalles.Count == 0)
         {
    return NotFound();
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
  return InternalServerError(new Exception("Error al obtener detalle: " + ex.Message));
  }
     }

        // ✅ POST: /api/detallefactura
        [HttpPost]
 [Route("")]
public IHttpActionResult CrearDetalle([FromBody] dynamic body)
        {
   try
     {
    if (body == null)
       {
   return BadRequest("Datos del detalle requeridos");
  }

 // Validar y extraer datos
     if (body.IdFactura == null || body.IdReserva == null || 
    body.Descripcion == null || body.Cantidad == null || 
  body.PrecioUnitario == null)
 {
    return BadRequest("Faltan campos requeridos: IdFactura, IdReserva, Descripcion, Cantidad, PrecioUnitario");
     }

    int idFactura = Convert.ToInt32(body.IdFactura);
     int idReserva = Convert.ToInt32(body.IdReserva);
        string descripcion = body.Descripcion.ToString();
    int cantidad = Convert.ToInt32(body.Cantidad);
    decimal precioUnitario = Convert.ToDecimal(body.PrecioUnitario);

     // Validaciones
     if (idFactura <= 0 || idReserva <= 0)
    {
      return BadRequest("IDs de factura y reserva deben ser válidos");
       }

  if (cantidad <= 0)
         {
   return BadRequest("La cantidad debe ser mayor a cero");
   }

      if (precioUnitario < 0)
       {
  return BadRequest("El precio unitario no puede ser negativo");
  }

    detalleLogica.InsertarDetalle(idFactura, idReserva, descripcion, cantidad, precioUnitario);

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
      return BadRequest("Error al crear detalle de factura: " + ex.Message);
 }
}

        // ✅ PUT: /api/detallefactura/{id}
        [HttpPut]
 [Route("{id:int}")]
        public IHttpActionResult ActualizarDetalle(int id, [FromBody] dynamic body)
        {
try
   {
    if (id <= 0)
      {
         return BadRequest("ID de detalle no válido");
       }

      if (body == null)
    {
           return BadRequest("Datos para actualizar requeridos");
   }

     // Verificar que el detalle existe
    DataTable dtExiste = detalleLogica.ObtenerDetallePorId(id);
   if (dtExiste.Rows.Count == 0)
  {
     return NotFound();
 }

   // Extraer datos
     string descripcion = body.Descripcion?.ToString();
        int cantidad = body.Cantidad != null ? Convert.ToInt32(body.Cantidad) : 1;
      decimal precioUnitario = body.PrecioUnitario != null ? Convert.ToDecimal(body.PrecioUnitario) : 0;

   // Validaciones
 if (cantidad <= 0)
         {
         return BadRequest("La cantidad debe ser mayor a cero");
}

    if (precioUnitario < 0)
    {
     return BadRequest("El precio unitario no puede ser negativo");
   }

   detalleLogica.ActualizarDetalle(id, descripcion, cantidad, precioUnitario);

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
 return BadRequest("Error al actualizar detalle: " + ex.Message);
       }
        }

 // ✅ DELETE: /api/detallefactura/{id}
        [HttpDelete]
        [Route("{id:int}")]
    public IHttpActionResult EliminarDetalle(int id)
        {
  try
     {
  if (id <= 0)
          {
    return BadRequest("ID de detalle no válido");
   }

          // Verificar que el detalle existe
     DataTable dtExiste = detalleLogica.ObtenerDetallePorId(id);
if (dtExiste.Rows.Count == 0)
   {
         return NotFound();
  }

     detalleLogica.EliminarDetalle(id);

    return Ok(new
  {
      success = true,
     message = "Detalle eliminado correctamente",
  idDetalle = id
     });
     }
            catch (Exception ex)
   {
         return BadRequest("Error al eliminar detalle: " + ex.Message);
  }
        }

 // ✅ GET: /api/detallefactura/estadisticas/factura/{idFactura}
        [HttpGet]
[Route("estadisticas/factura/{idFactura:int}")]
   public IHttpActionResult ObtenerEstadisticasFactura(int idFactura)
      {
            try
            {
    if (idFactura <= 0)
   {
         return BadRequest("ID de factura no válido");
  }

      decimal subtotalCalculado = detalleLogica.CalcularSubtotalFactura(idFactura);
  int cantidadDetalles = detalleLogica.ContarDetallesFactura(idFactura);

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
  return InternalServerError(new Exception("Error al obtener estadísticas: " + ex.Message));
  }
        }

   // ============================================================
        // 🛠️ MÉTODOS AUXILIARES
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
