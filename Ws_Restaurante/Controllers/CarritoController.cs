using System;
using System.Data;
using System.Web.Http;
using Logica.Servicios;

namespace Ws_Restaurante.Controllers
{
    [RoutePrefix("api/carrito")]
    public class CarritoController : ApiController
    {
        private readonly ReservaLogica reservaLogica = new ReservaLogica();
        private readonly PromocionLogica promocionLogica = new PromocionLogica();

       // ============================================================
 // ?? LISTAR CARRITO DE RESERVAS DEL USUARIO CON PROMOCIONES
    // ============================================================
     [HttpGet]
 [Route("usuario/{idUsuario}")]
 public IHttpActionResult ListarCarritoUsuario(int idUsuario, int? promocionId = null)
        {
          try
    {
   if (idUsuario <= 0)
return BadRequest("ID de usuario no válido");

     DataSet resultado = reservaLogica.ListarCarritoReservas(idUsuario, promocionId);
  
  if (resultado == null || resultado.Tables.Count == 0)
  return BadRequest("No se pudo obtener el carrito");

       // Verificar si hay error en la respuesta del SP
      if (resultado.Tables[0].Columns.Contains("Estado") && 
 resultado.Tables[0].Rows.Count > 0 && 
  resultado.Tables[0].Rows[0]["Estado"].ToString() == "ERROR")
   {
        return BadRequest(resultado.Tables[0].Rows[0]["Mensaje"].ToString());
    }

    var response = new
       {
   success = true,
   reservas = resultado.Tables[0], // Lista de reservas en carrito
    resumen = resultado.Tables.Count > 1 ? resultado.Tables[1] : null // Resumen del carrito
   };

     return Ok(response);
       }
  catch (Exception ex)
            {
   return InternalServerError(new Exception($"Error al obtener carrito: {ex.Message}"));
      }
  }

        // ============================================================
 // ?? LISTAR PROMOCIONES ACTIVAS
    // ============================================================
  [HttpGet]
 [Route("promociones")]
  public IHttpActionResult ListarPromocionesActivas()
   {
try
     {
        DataTable promociones = promocionLogica.ListarPromocionesActivas();
       
   if (promociones == null)
      return BadRequest("No se pudieron obtener las promociones");

 // Verificar si hay error en la respuesta del SP
   if (promociones.Columns.Contains("Estado") && 
     promociones.Rows.Count > 0 && 
      promociones.Rows[0]["Estado"].ToString() == "ERROR")
         {
   return BadRequest(promociones.Rows[0]["Mensaje"].ToString());
      }

            return Ok(new { 
        success = true, 
      promociones = promociones
      });
        }
   catch (Exception ex)
       {
   return InternalServerError(new Exception($"Error al obtener promociones: {ex.Message}"));
        }
   }

 // ============================================================
        // ? ELIMINAR RESERVA DEL CARRITO
   // ============================================================
     [HttpDelete]
    [Route("eliminar/{idUsuario}/{idReserva}")]
        public IHttpActionResult EliminarReservaCarrito(int idUsuario, int idReserva)
        {
 try
  {
     if (idUsuario <= 0 || idReserva <= 0)
       return BadRequest("ID de usuario y reserva son requeridos");

     DataTable resultado = reservaLogica.EliminarReservaCarrito(idUsuario, idReserva);

  if (resultado == null || resultado.Rows.Count == 0)
  return BadRequest("No se pudo procesar la eliminación");

       // Verificar si hay error en la respuesta del SP
       if (resultado.Columns.Contains("Estado") && 
   resultado.Rows[0]["Estado"].ToString() == "ERROR")
        {
      return BadRequest(resultado.Rows[0]["Mensaje"].ToString());
}

       return Ok(new { 
   success = true, 
   message = resultado.Rows[0]["Mensaje"].ToString(),
     reservaEliminada = resultado.Rows[0]["ReservaEliminada"]
        });
     }
 catch (Exception ex)
   {
   return InternalServerError(new Exception($"Error eliminando reserva: {ex.Message}"));
         }
        }

// ============================================================
// ? CONFIRMAR RESERVAS SELECTIVAS CON PROMOCIONES
        // ============================================================
  [HttpPost]
      [Route("confirmar")]
   public IHttpActionResult ConfirmarReservasSelectivas([FromBody] dynamic data)
   {
  try
{
   // Validar que data no sea null
     if (data == null)
    return BadRequest("Datos requeridos");

   // Extraer y validar parámetros con manejo seguro de null
   int idUsuario = 0;
     string reservasIds = "";
     string metodoPago = "";
      int? promocionId = null;
      decimal montoTotal = 0;

     // IdUsuario
      if (data.IdUsuario != null)
      idUsuario = Convert.ToInt32(data.IdUsuario);
    else if (data.idUsuario != null)
        idUsuario = Convert.ToInt32(data.idUsuario);

          // ReservasIds
  if (data.ReservasIds != null)
      reservasIds = data.ReservasIds.ToString();
   else if (data.reservasIds != null)
  reservasIds = data.reservasIds.ToString();

    // MetodoPago
   if (data.MetodoPago != null)
     metodoPago = data.MetodoPago.ToString();
else if (data.metodoPago != null)
   metodoPago = data.metodoPago.ToString();

      // PromocionId (puede ser null)
    if (data.PromocionId != null && data.PromocionId.ToString() != "null" && data.PromocionId.ToString() != "")
      promocionId = Convert.ToInt32(data.PromocionId);
 else if (data.promocionId != null && data.promocionId.ToString() != "null" && data.promocionId.ToString() != "")
     promocionId = Convert.ToInt32(data.promocionId);

     // Monto Total (NUEVO: recibir del frontend)
      if (data.Monto != null)
     {
        if (decimal.TryParse(data.Monto.ToString(), out decimal monto))
         {
        montoTotal = monto;
      }
     }
  else if (data.monto != null)
     {
      if (decimal.TryParse(data.monto.ToString(), out decimal monto))
      {
        montoTotal = monto;
      }
   }

// Validaciones
     if (idUsuario <= 0)
        return BadRequest("ID de usuario no válido");

       if (string.IsNullOrEmpty(reservasIds))
        return BadRequest("Debe especificar las reservas a confirmar");

        if (string.IsNullOrEmpty(metodoPago))
   return BadRequest("Método de pago es requerido");

   DataTable resultado = reservaLogica.ConfirmarReservasSelectivas(idUsuario, reservasIds, metodoPago, promocionId);

      if (resultado == null || resultado.Rows.Count == 0)
  return BadRequest("No se pudo procesar la confirmación");

  // Verificar si hay error en la respuesta del SP
  if (resultado.Columns.Contains("Estado") && 
resultado.Rows[0]["Estado"].ToString() == "ERROR")
     {
  return BadRequest(resultado.Rows[0]["Mensaje"].ToString());
     }

   return Ok(new { 
    success = true, 
    message = resultado.Rows[0]["Mensaje"].ToString(),
   reservasConfirmadas = resultado.Rows[0]["ReservasConfirmadas"],
   idFacturaAfectada = resultado.Columns.Contains("IdFacturaAfectada") && resultado.Rows[0]["IdFacturaAfectada"] != DBNull.Value ? resultado.Rows[0]["IdFacturaAfectada"] : null,
   promocionAplicada = resultado.Columns.Contains("PromocionAplicada") ? resultado.Rows[0]["PromocionAplicada"] : null,
    descuentoAplicado = resultado.Columns.Contains("DescuentoAplicado") ? resultado.Rows[0]["DescuentoAplicado"] : null,
 monto = montoTotal // Retornar el monto que recibió del frontend
  });
      }
 catch (Exception ex)
 {
    return InternalServerError(new Exception($"Error confirmando reservas: {ex.Message}"));
 }
   }

        // ============================================================
        // ?? LISTAR PROMOCIONES VÁLIDAS PARA FECHAS DEL CARRITO DE UN USUARIO
        // ============================================================
        [HttpGet]
        [Route("promociones-validas/{idUsuario}")]
        public IHttpActionResult ListarPromocionesValidasParaCarrito(int idUsuario)
        {
 try
       {
      if (idUsuario <= 0)
          return BadRequest("ID de usuario no válido");

        DataTable promocionesValidas = promocionLogica.ListarPromocionesValidasParaCarrito(idUsuario);
     
        if (promocionesValidas == null)
        return BadRequest("No se pudieron obtener las promociones válidas");

       // Verificar si hay error en la respuesta del SP
   if (promocionesValidas.Columns.Contains("Estado") && 
       promocionesValidas.Rows.Count > 0 && 
   promocionesValidas.Rows[0]["Estado"].ToString() == "ERROR")
    {
     return BadRequest(promocionesValidas.Rows[0]["Mensaje"].ToString());
       }

    return Ok(new { 
   success = true, 
        promociones = promocionesValidas
      });
     }
      catch (Exception ex)
   {
        return InternalServerError(new Exception($"Error al obtener promociones válidas: {ex.Message}"));
      }
 }
    }
}