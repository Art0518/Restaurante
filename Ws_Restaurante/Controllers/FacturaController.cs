using System;
using System.Web.Http;
using Logica.Servicios;
using GDatos.Entidades;
using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace Ws_Restaurante.Controllers
{
    // 🔹 Prefijo base de la API
    [RoutePrefix("api/facturas")]
    public class FacturaController : ApiController
    {
    private readonly FacturaLogica facturaLogica = new FacturaLogica();

        // ✅ GET /api/facturas
        // Devuelve todas las facturas registradas
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetFacturas()
        {
  try
 {
         DataTable dt = facturaLogica.ListarFacturas();
      var facturas = ConvertirDataTableALista(dt);
            
 return Ok(new
                {
        success = true,
            message = "Facturas obtenidas correctamente",
  data = facturas,
     count = facturas.Count
  });
      }
  catch (Exception ex)
    {
        return InternalServerError(new Exception("Error al listar facturas: " + ex.Message));
          }
        }

        // ✅ GET /api/facturas/{id}
// Devuelve el detalle de una factura específica
    [HttpGet]
    [Route("{id:int}")]
        public IHttpActionResult GetFactura(int id)
        {
       try
            {
         if (id <= 0)
        {
   return BadRequest("ID de factura no válido");
   }

      DataTable dt = facturaLogica.DetalleFactura(id);
    if (dt.Rows.Count == 0)
            return NotFound();

     var facturaDetalle = ConvertirDataTableALista(dt);
              return Ok(new
       {
          success = true,
      message = "Detalle de factura obtenido correctamente",
         data = facturaDetalle.FirstOrDefault()
    });
            }
       catch (Exception ex)
          {
        return InternalServerError(new Exception("Error al obtener la factura: " + ex.Message));
}
        }

  // ✅ POST /api/facturas
// Crea una nueva factura a partir de una reserva
        [HttpPost]
        [Route("")]
        public IHttpActionResult CrearFactura([FromBody] dynamic body)
        {
            try
   {
      if (body == null)
    {
           return BadRequest("Datos de factura requeridos");
          }

      int idUsuario = (int)body.idUsuario;
         int idReserva = (int)body.idReserva;

            if (idUsuario <= 0)
       {
     return BadRequest("ID de usuario no válido");
 }

        if (idReserva <= 0)
    {
        return BadRequest("ID de reserva no válido");
             }

        DataTable dt = facturaLogica.GenerarFactura(idUsuario, idReserva);
                var facturaGenerada = ConvertirDataTableALista(dt);

    return Ok(new
     {
   success = true,
   message = "Factura generada correctamente",
   data = facturaGenerada.FirstOrDefault()
           });
            }
   catch (Exception ex)
         {
         return BadRequest("Error al generar la factura: " + ex.Message);
     }
        }

        // ✅ PUT /api/facturas/{id}/anular
  // Anula una factura existente
        [HttpPut]
  [Route("{id:int}/anular")]
        public IHttpActionResult AnularFactura(int id)
        {
       try
     {
        if (id <= 0)
          {
 return BadRequest("ID de factura no válido");
  }

        facturaLogica.AnularFactura(id);
      return Ok(new 
          { 
       success = true,
        message = "Factura anulada correctamente",
              idFactura = id
     });
   }
            catch (Exception ex)
            {
      return BadRequest("Error al anular la factura: " + ex.Message);
         }
 }

        // ✅ POST /api/facturas/calcular
        // Calcula el subtotal, IVA y total sin guardar
        [HttpPost]
      [Route("calcular")]
        public IHttpActionResult CalcularTotales([FromBody] Factura factura)
        {
            try
   {
           if (factura == null)
       {
       return BadRequest("Datos de factura requeridos");
                }

       if (factura.Subtotal < 0)
   {
        return BadRequest("El subtotal no puede ser negativo");
          }

                Factura f = facturaLogica.CalcularTotales(factura);
    return Ok(new
      {
               success = true,
           message = "Totales calculados correctamente",
          data = new
  {
       subtotal = f.Subtotal,
      iva = f.IVA,
        total = f.Total
        }
            });
            }
         catch (Exception ex)
            {
      return BadRequest("Error al calcular totales: " + ex.Message);
  }
        }

     // ✅ NUEVO: POST /api/facturas/generar-carrito
        // Genera una factura desde las reservas del carrito
    [HttpPost]
        [Route("generar-carrito")]
        public IHttpActionResult GenerarFacturaCarrito([FromBody] dynamic body)
        {
        try
      {
                // Validaciones de entrada
    if (body == null)
      {
   return Content(System.Net.HttpStatusCode.BadRequest, new
       {
     success = false,
      Estado = "ERROR",
        Mensaje = "Datos requeridos para generar factura"
     });
         }

     // Extraer y validar parámetros
    if (body.IdUsuario == null)
 {
      return Content(System.Net.HttpStatusCode.BadRequest, new
       {
        success = false,
   Estado = "ERROR",
    Mensaje = "IdUsuario es requerido"
});
        }

 if (body.ReservasIds == null)
    {
     return Content(System.Net.HttpStatusCode.BadRequest, new
       {
success = false,
Estado = "ERROR",
     Mensaje = "ReservasIds es requerido"
         });
  }

    int idUsuario = Convert.ToInt32(body.IdUsuario);
       string reservasIds = body.ReservasIds.ToString();
      int? promocionId = body.PromocionId != null ? Convert.ToInt32(body.PromocionId) : (int?)null;
       string metodoPago = body.MetodoPago?.ToString();

       // Validaciones adicionales
       if (idUsuario <= 0)
    {
         return Content(System.Net.HttpStatusCode.BadRequest, new
        {
  success = false,
         Estado = "ERROR",
    Mensaje = "ID de usuario no válido"
         });
      }

        if (string.IsNullOrWhiteSpace(reservasIds))
  {
          return Content(System.Net.HttpStatusCode.BadRequest, new
     {
       success = false,
   Estado = "ERROR",
    Mensaje = "Debe seleccionar al menos una reserva"
               });
   }

      // Llamar a la lógica de negocio
     DataTable resultado = facturaLogica.GenerarFacturaCarrito(idUsuario, reservasIds, promocionId, metodoPago);

    if (resultado != null && resultado.Rows.Count > 0)
         {
     DataRow row = resultado.Rows[0];
   string estado = row["Estado"].ToString();

        if (estado == "SUCCESS")
            {
       return Ok(new
      {
    success = true,
  Estado = "SUCCESS",
            Mensaje = row["Mensaje"].ToString(),
            IdFactura = Convert.ToInt32(row["IdFactura"]),
     SubtotalBruto = Convert.ToDecimal(row["SubtotalBruto"]),
   Descuento = Convert.ToDecimal(row["Descuento"]),
     Subtotal = Convert.ToDecimal(row["Subtotal"]),
        IVA = Convert.ToDecimal(row["IVA"]),
        Total = Convert.ToDecimal(row["Total"]),
      PorcentajeDescuento = Convert.ToDecimal(row["PorcentajeDescuento"]),
CantidadReservas = Convert.ToInt32(row["CantidadReservas"]),
           MetodoPago = row["MetodoPago"].ToString()
      });
    }
      else
    {
       return Content(System.Net.HttpStatusCode.BadRequest, new
          {
       success = false,
     Estado = "ERROR",
  Mensaje = row["Mensaje"].ToString()
              });
 }
   }
        else
      {
  return Content(System.Net.HttpStatusCode.BadRequest, new
 {
     success = false,
            Estado = "ERROR",
     Mensaje = "No se recibió respuesta del servidor"
        });
      }
            }
  catch (FormatException ex)
            {
 return Content(System.Net.HttpStatusCode.BadRequest, new
   {
            success = false,
      Estado = "ERROR",
        Mensaje = "Formato de datos incorrecto: " + ex.Message
   });
    }
     catch (Exception ex)
            {
   return Content(System.Net.HttpStatusCode.BadRequest, new
          {
     success = false,
       Estado = "ERROR",
         Mensaje = "Error generando la factura: " + ex.Message
       });
     }
  }

     // ✅ NUEVO: POST /api/facturas/generar-confirmadas
        // Genera una factura específicamente para reservas confirmadas
        [HttpPost]
        [Route("generar-confirmadas")]
        public IHttpActionResult GenerarFacturaReservasConfirmadas([FromBody] dynamic body)
      {
            try
            {
   // Validaciones de entrada
          if (body == null)
         {
    return Content(System.Net.HttpStatusCode.BadRequest, new
         {
   success = false,
          Estado = "ERROR",
         Mensaje = "Datos requeridos para generar factura"
          });
  }

        // Extraer y validar parámetros
    if (body.IdUsuario == null)
         {
    return Content(System.Net.HttpStatusCode.BadRequest, new
              {
         success = false,
        Estado = "ERROR",
        Mensaje = "IdUsuario es requerido"
          });
     }

   if (body.ReservasIds == null)
 {
         return Content(System.Net.HttpStatusCode.BadRequest, new
           {
                  success = false,
         Estado = "ERROR",
     Mensaje = "ReservasIds es requerido"
     });
     }

          int idUsuario = Convert.ToInt32(body.IdUsuario);
            string reservasIds = body.ReservasIds.ToString();
  string tipoFactura = body.TipoFactura?.ToString() ?? "CONFIRMADA";

         // Validaciones adicionales
           if (idUsuario <= 0)
     {
       return Content(System.Net.HttpStatusCode.BadRequest, new
                  {
              success = false,
 Estado = "ERROR",
       Mensaje = "ID de usuario no válido"
       });
     }

      if (string.IsNullOrWhiteSpace(reservasIds))
       {
        return Content(System.Net.HttpStatusCode.BadRequest, new
            {
      success = false,
             Estado = "ERROR",
    Mensaje = "Debe seleccionar al menos una reserva confirmada"
           });
     }

           // ✅ Llamar a la nueva función para reservas confirmadas
         DataTable resultado = facturaLogica.GenerarFacturaReservasConfirmadas(idUsuario, reservasIds, tipoFactura);

       if (resultado != null && resultado.Rows.Count > 0)
          {
   DataRow row = resultado.Rows[0];
          string estado = row["Estado"].ToString();

             if (estado == "SUCCESS")
      {
       return Ok(new
     {
         success = true,
 Estado = "SUCCESS",
        Mensaje = row["Mensaje"].ToString(),
     IdFactura = Convert.ToInt32(row["IdFactura"]),
    SubtotalBruto = Convert.ToDecimal(row["SubtotalBruto"]),
   Subtotal = Convert.ToDecimal(row["Subtotal"]),
                IVA = Convert.ToDecimal(row["IVA"]),
        Total = Convert.ToDecimal(row["Total"]),
        CantidadReservas = Convert.ToInt32(row["CantidadReservas"]),
        MetodoPago = row["MetodoPago"].ToString(),
      TipoFactura = "CONFIRMADA",
        EstadoFactura = "Confirmada"
         });
       }
   else
    {
   return Content(System.Net.HttpStatusCode.BadRequest, new
          {
  success = false,
          Estado = "ERROR",
     Mensaje = row["Mensaje"].ToString()
        });
   }
        }
                else
           {
   return Content(System.Net.HttpStatusCode.BadRequest, new
           {
                success = false,
   Estado = "ERROR",
  Mensaje = "No se recibió respuesta del servidor"
         });
    }
            }
            catch (FormatException ex)
            {
        return Content(System.Net.HttpStatusCode.BadRequest, new
     {
        success = false,
        Estado = "ERROR",
        Mensaje = "Formato de datos incorrecto: " + ex.Message
     });
 }
       catch (Exception ex)
            {
  return Content(System.Net.HttpStatusCode.BadRequest, new
          {
      success = false,
        Estado = "ERROR",
        Mensaje = "Error generando la factura de confirmadas: " + ex.Message
     });
   }
        }

     // ✅ NUEVO: GET /api/facturas/detallada/{id}
        // Obtiene factura con todos sus detalles
     [HttpGet]
        [Route("detallada/{id:int}")]
        public IHttpActionResult GetFacturaDetallada(int id)
        {
            try
 {
      if (id <= 0)
     {
     return Content(System.Net.HttpStatusCode.BadRequest, new
        {
       success = false,
  message = "ID de factura no válido"
          });
  }

    DataSet ds = facturaLogica.ObtenerFacturaDetallada(id);

      if (ds != null && ds.Tables.Count >= 2)
 {
 DataTable facturaTable = ds.Tables[0];
DataTable detallesTable = ds.Tables[1];

    if (facturaTable.Rows.Count > 0)
   {
  return Ok(new
  {
     success = true,
   message = "Factura detallada obtenida correctamente",
factura = ConvertirDataTableALista(facturaTable),
      detalles = ConvertirDataTableALista(detallesTable)
    });
        }
         else
 {
 return NotFound();
             }
     }
      else
   {
        return Content(System.Net.HttpStatusCode.BadRequest, new
      {
  success = false,
message = "Error obteniendo los detalles de la factura"
        });
 }
  }
         catch (Exception ex)
 {
 return InternalServerError(new Exception("Error al obtener factura detallada: " + ex.Message));
   }
  }

        // ✅ NUEVO: POST /api/facturas/marcar-pagada
      // Marca una factura como pagada
        [HttpPost]
        [Route("marcar-pagada")]
    public IHttpActionResult MarcarFacturaPagada([FromBody] dynamic body)
  {
      try
       {
   // Validaciones de entrada
  if (body == null)
        {
   return Content(System.Net.HttpStatusCode.BadRequest, new
     {
   success = false,
   Estado = "ERROR",
Mensaje = "Datos requeridos para marcar factura como pagada"
   });
       }

 if (body.IdFactura == null)
   {
      return Content(System.Net.HttpStatusCode.BadRequest, new
      {
          success = false,
   Estado = "ERROR",
      Mensaje = "IdFactura es requerido"
   });
        }

       if (body.MetodoPago == null)
      {
 return Content(System.Net.HttpStatusCode.BadRequest, new
   {
 success = false,
        Estado = "ERROR",
 Mensaje = "MetodoPago es requerido"
  });
     }

   int idFactura = Convert.ToInt32(body.IdFactura);
 string metodoPago = body.MetodoPago.ToString();

    // Validaciones adicionales
   if (idFactura <= 0)
   {
   return Content(System.Net.HttpStatusCode.BadRequest, new
  {
       success = false,
    Estado = "ERROR",
Mensaje = "ID de factura no válido"
      });
   }

if (string.IsNullOrWhiteSpace(metodoPago))
    {
  return Content(System.Net.HttpStatusCode.BadRequest, new
     {
    success = false,
    Estado = "ERROR",
  Mensaje = "Método de pago no puede estar vacío"
      });
      }

    // Llamar a la lógica de negocio
     DataTable resultado = facturaLogica.MarcarFacturaPagada(idFactura, metodoPago);

 if (resultado != null && resultado.Rows.Count > 0)
      {
    DataRow row = resultado.Rows[0];
       string estado = row["Estado"].ToString();

         if (estado == "SUCCESS")
  {
           return Ok(new
    {
       success = true,
       Estado = "SUCCESS",
      Mensaje = row["Mensaje"].ToString(),
        IdFactura = Convert.ToInt32(row["IdFactura"])
  });
   }
          else
  {
     return Content(System.Net.HttpStatusCode.BadRequest, new
         {
  success = false,
     Estado = "ERROR",
     Mensaje = row["Mensaje"].ToString()
    });
 }
      }
        else
      {
  return Content(System.Net.HttpStatusCode.BadRequest, new
 {
     success = false,
            Estado = "ERROR",
     Mensaje = "No se recibió respuesta del servidor"
        });
      }
            }
         catch (Exception ex)
            {
   return Content(System.Net.HttpStatusCode.BadRequest, new
          {
     success = false,
       Estado = "ERROR",
         Mensaje = "Error marcando la factura como pagada: " + ex.Message
       });
     }
  }

     // ✅ NUEVO: GET /api/facturas/usuario/{id}
        [HttpGet]
      [Route("usuario/{id:int}")]
      public IHttpActionResult GetFacturasUsuario(int id)
    {
     try
   {
 if (id <= 0)
   {
          return Content(System.Net.HttpStatusCode.BadRequest, new
     {
     success = false,
      message = "ID de usuario no válido"
 });
   }

        DataTable dt = facturaLogica.ListarFacturasUsuario(id);
  var facturasUsuario = ConvertirDataTableALista(dt);

   return Ok(new
 {
      success = true,
 message = "Facturas del usuario obtenidas correctamente",
facturas = facturasUsuario,
 count = facturasUsuario.Count,
    idUsuario = id
       });
}
     catch (Exception ex)
     {
  return InternalServerError(new Exception("Error al listar facturas del usuario: " + ex.Message));
            }
     }

      // ✅ NUEVO: GET /api/facturas/estadisticas/{idUsuario}
        [HttpGet]
   [Route("estadisticas/{idUsuario:int}")]
    public IHttpActionResult GetEstadisticasFacturacionUsuario(int idUsuario)
     {
       try
  {
    if (idUsuario <= 0)
    {
         return Content(System.Net.HttpStatusCode.BadRequest, new
   {
         success = false,
      message = "ID de usuario no válido"
       });
  }

        DataTable dt = facturaLogica.ListarFacturasUsuario(idUsuario);
 var facturas = ConvertirDataTableALista(dt);

      var estadisticas = new
    {
       totalFacturas = facturas.Count,
     facturasPagadas = facturas.Count(f => f.ContainsKey("Estado") && f["Estado"].ToString() == "Pagada"),
        facturasEmitidas = facturas.Count(f => f.ContainsKey("Estado") && f["Estado"].ToString() == "Emitida"),
  facturasAnuladas = facturas.Count(f => f.ContainsKey("Estado") && f["Estado"].ToString() == "Anulada"),
 montoTotal = facturas.Where(f => f.ContainsKey("Total")).Sum(f => Convert.ToDecimal(f["Total"])),
  montoTotalPagado = facturas.Where(f => f.ContainsKey("Estado") && f["Estado"].ToString() == "Pagada" && f.ContainsKey("Total")).Sum(f => Convert.ToDecimal(f["Total"])),
   montoPendiente = facturas.Where(f => f.ContainsKey("Estado") && f["Estado"].ToString() == "Emitida" && f.ContainsKey("Total")).Sum(f => Convert.ToDecimal(f["Total"]))
   };

   return Ok(new
    {
   success = true,
message = "Estadísticas obtenidas correctamente",
          idUsuario = idUsuario,
estadisticas = estadisticas
 });
        }
       catch (Exception ex)
        {
     return InternalServerError(new Exception("Error al obtener estadísticas: " + ex.Message));
   }
        }

   // ✅ NUEVO: GET /api/facturas/validar-estado/{estado}
        [HttpGet]
  [Route("validar-estado/{estado}")]
     public IHttpActionResult ValidarEstadoFactura(string estado)
      {
     try
            {
    if (string.IsNullOrWhiteSpace(estado))
   {
     return Content(System.Net.HttpStatusCode.BadRequest, new
{
  success = false,
     message = "Estado no puede estar vacío"
   });
        }

      bool esValido = facturaLogica.ValidarEstadoFactura(estado);

   return Ok(new
    {
        success = true,
message = esValido ? "Estado válido" : "Estado no válido",
        estado = estado,
esValido = esValido,
   estadosValidos = new string[] { "Emitida", "Pagada", "Anulada" }
            });
            }
     catch (Exception ex)
    {
      return InternalServerError(new Exception("Error al validar estado: " + ex.Message));
    }
  }

        // ============================================================
        // 🛠️ MÉTODO AUXILIAR PARA CONVERTIR DATATABLE
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

        // ============================================================
        // 🛠️ MÉTODO AUXILIAR PARA MANEJAR ERRORES DE CONVERSIÓN
        // ============================================================
      private IHttpActionResult CrearRespuestaError(string mensaje, string estado = "ERROR")
     {
  return Content(System.Net.HttpStatusCode.BadRequest, new
     {
       success = false,
  Estado = estado,
  Mensaje = mensaje,
      timestamp = DateTime.Now
  });
        }

    // ============================================================
   // 🛠️ MÉTODO AUXILIAR PARA VALIDAR DATOS DE ENTRADA
        // ============================================================
        private string ValidarDatosEntrada(dynamic body, string[] camposRequeridos)
        {
      if (body == null)
  {
          return "Datos requeridos";
            }

    foreach (string campo in camposRequeridos)
        {
     try
         {
      var valor = body.GetType().GetProperty(campo)?.GetValue(body, null);
    if (valor == null)
  {
        return $"El campo {campo} es requerido";
   }
   }
            catch
     {
  return $"El campo {campo} es requerido";
   }
        }

        return null; // No hay errores
 }
    }
}