using Microsoft.AspNetCore.Mvc;
using ReservasService.Data;
using System;
using System.Data;

namespace ReservasService.Controllers
{
    [ApiController]
  [Route("api/carrito")]
    public class CarritoController : ControllerBase
    {
   private readonly ILogger<CarritoController> _logger;
        private readonly ReservaDAO _reservaDAO;
        private readonly PromocionDAO _promocionDAO;
      private readonly string _connectionString;

        public CarritoController(ILogger<CarritoController> logger, IConfiguration configuration)
        {
      _logger = logger;
 _connectionString = configuration.GetConnectionString("DefaultConnection") 
        ?? throw new InvalidOperationException("Connection string not found");
_reservaDAO = new ReservaDAO(_connectionString);
     _promocionDAO = new PromocionDAO(_connectionString);
        }

        // ============================================================
        // ?? LISTAR CARRITO DE RESERVAS DEL USUARIO CON PROMOCIONES
        // ============================================================
      [HttpGet("usuario/{idUsuario}")]
     public IActionResult ListarCarritoUsuario(int idUsuario, [FromQuery] int? promocionId = null)
     {
      try
{
        if (idUsuario <= 0)
      return BadRequest(new { success = false, message = "ID de usuario no válido" });

       DataSet resultado = _reservaDAO.ListarCarritoReservas(idUsuario, promocionId);

     if (resultado == null || resultado.Tables.Count == 0)
      return BadRequest(new { success = false, message = "No se pudo obtener el carrito" });

    // Verificar si hay error en la respuesta del SP
           if (resultado.Tables[0].Columns.Contains("Estado") &&
   resultado.Tables[0].Rows.Count > 0 &&
      resultado.Tables[0].Rows[0]["Estado"].ToString() == "ERROR")
    {
   return BadRequest(new { success = false, message = resultado.Tables[0].Rows[0]["Mensaje"].ToString() });
   }

   // Convertir DataTable a lista de objetos anónimos
            var reservasList = new List<object>();
            foreach (System.Data.DataRow row in resultado.Tables[0].Rows)
       {
                reservasList.Add(new
       {
     IdReserva = Convert.ToInt32(row["IdReserva"]),
        IdUsuario = Convert.ToInt32(row["IdUsuario"]),
          IdMesa = Convert.ToInt32(row["IdMesa"]),
      NumeroMesa = row["NumeroMesa"]?.ToString(),
CapacidadMesa = row["CapacidadMesa"] != DBNull.Value ? Convert.ToInt32(row["CapacidadMesa"]) : 0,
            PrecioMesa = row["PrecioMesa"] != DBNull.Value ? Convert.ToDecimal(row["PrecioMesa"]) : 0,
               Fecha = row["Fecha"] != DBNull.Value ? Convert.ToDateTime(row["Fecha"]).ToString("yyyy-MM-dd") : null,
         Hora = row["Hora"]?.ToString(),
      NumeroPersonas = Convert.ToInt32(row["NumeroPersonas"]),
  Estado = row["Estado"]?.ToString(),
           Observaciones = row["Observaciones"]?.ToString(),
   MetodoPago = row["MetodoPago"]?.ToString(),
 MontoDescuento = row["MontoDescuento"] != DBNull.Value ? Convert.ToDecimal(row["MontoDescuento"]) : 0,
             Total = row["Total"] != DBNull.Value ? Convert.ToDecimal(row["Total"]) : 0,
             Subtotal = row["Subtotal"] != DBNull.Value ? Convert.ToDecimal(row["Subtotal"]) : 0,
         PromocionSeleccionada = row["PromocionSeleccionada"] != DBNull.Value ? Convert.ToInt32(row["PromocionSeleccionada"]) : (int?)null,
         PorcentajeDescuento = row["PorcentajeDescuento"] != DBNull.Value ? Convert.ToDecimal(row["PorcentajeDescuento"]) : 0,
  MontoDescuentoCalculado = row["MontoDescuentoCalculado"] != DBNull.Value ? Convert.ToDecimal(row["MontoDescuentoCalculado"]) : 0,
  SubtotalConDescuento = row["SubtotalConDescuento"] != DBNull.Value ? Convert.ToDecimal(row["SubtotalConDescuento"]) : 0,
             IVA = row["IVA"] != DBNull.Value ? Convert.ToDecimal(row["IVA"]) : 0,
          TotalFinal = row["TotalFinal"] != DBNull.Value ? Convert.ToDecimal(row["TotalFinal"]) : 0
             });
 }

      // Convertir resumen (segunda tabla) a objeto
            object resumen = null;
            if (resultado.Tables.Count > 1 && resultado.Tables[1].Rows.Count > 0)
   {
       var resumenRow = resultado.Tables[1].Rows[0];
          resumen = new
        {
      TotalReservas = Convert.ToInt32(resumenRow["TotalReservas"]),
Subtotal = resumenRow["Subtotal"] != DBNull.Value ? Convert.ToDecimal(resumenRow["Subtotal"]) : 0,
           PromocionAplicada = resumenRow["PromocionAplicada"] != DBNull.Value ? Convert.ToInt32(resumenRow["PromocionAplicada"]) : (int?)null,
 PorcentajeDescuento = resumenRow["PorcentajeDescuento"] != DBNull.Value ? Convert.ToDecimal(resumenRow["PorcentajeDescuento"]) : 0,
          MontoDescuentoTotal = resumenRow["MontoDescuentoTotal"] != DBNull.Value ? Convert.ToDecimal(resumenRow["MontoDescuentoTotal"]) : 0,
   SubtotalConDescuento = resumenRow["SubtotalConDescuento"] != DBNull.Value ? Convert.ToDecimal(resumenRow["SubtotalConDescuento"]) : 0,
       IVA = resumenRow["IVA"] != DBNull.Value ? Convert.ToDecimal(resumenRow["IVA"]) : 0,
        TotalCarrito = resumenRow["TotalCarrito"] != DBNull.Value ? Convert.ToDecimal(resumenRow["TotalCarrito"]) : 0
  };
   }

      var response = new
       {
        success = true,
      reservas = reservasList,
   resumen = resumen
    };

   return Ok(response);
        }
     catch (Exception ex)
          {
          _logger.LogError($"Error al obtener carrito: {ex.Message}");
     return StatusCode(500, new { success = false, message = $"Error al obtener carrito: {ex.Message}" });
    }
        }

        // ============================================================
        // ?? LISTAR PROMOCIONES ACTIVAS
        // ============================================================
        [HttpGet("promociones")]
        public IActionResult ListarPromocionesActivas()
        {
        try
  {
 var promociones = _promocionDAO.ListarPromocionesActivas();

              if (promociones == null)
         return BadRequest(new { success = false, message = "No se pudieron obtener las promociones" });

         return Ok(new
        {
      success = true,
         promociones = promociones
    });
            }
   catch (Exception ex)
            {
     _logger.LogError($"Error al obtener promociones: {ex.Message}");
    return StatusCode(500, new { success = false, message = $"Error al obtener promociones: {ex.Message}" });
   }
        }

        // ============================================================
      // ? ELIMINAR RESERVA DEL CARRITO
        // ============================================================
      [HttpDelete("eliminar/{idUsuario}/{idReserva}")]
        public IActionResult EliminarReservaCarrito(int idUsuario, int idReserva)
        {
            try
 {
          if (idUsuario <= 0 || idReserva <= 0)
          return BadRequest(new { success = false, message = "ID de usuario y reserva son requeridos" });

      DataTable resultado = _reservaDAO.EliminarReservaCarrito(idUsuario, idReserva);

      if (resultado == null || resultado.Rows.Count == 0)
          return BadRequest(new { success = false, message = "No se pudo procesar la eliminación" });

    // Verificar si hay error en la respuesta del SP
         if (resultado.Columns.Contains("Estado") &&
   resultado.Rows[0]["Estado"].ToString() == "ERROR")
      {
   return BadRequest(new { success = false, message = resultado.Rows[0]["Mensaje"].ToString() });
        }

       return Ok(new
      {
      success = true,
         message = resultado.Rows[0]["Mensaje"].ToString(),
        reservaEliminada = resultado.Rows[0]["ReservaEliminada"]
        });
  }
         catch (Exception ex)
            {
                _logger.LogError($"Error eliminando reserva: {ex.Message}");
      return StatusCode(500, new { success = false, message = $"Error eliminando reserva: {ex.Message}" });
            }
        }

        // ============================================================
        // ? CONFIRMAR RESERVAS SELECTIVAS CON PROMOCIONES
        // ============================================================
     [HttpPost("confirmar")]
      public IActionResult ConfirmarReservasSelectivas([FromBody] dynamic data)
        {
   try
       {
                // Validar que data no sea null
        if (data == null)
        return BadRequest(new { success = false, message = "Datos requeridos" });

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
       return BadRequest(new { success = false, message = "ID de usuario no válido" });

        if (string.IsNullOrEmpty(reservasIds))
        return BadRequest(new { success = false, message = "Debe especificar las reservas a confirmar" });

        if (string.IsNullOrEmpty(metodoPago))
         return BadRequest(new { success = false, message = "Método de pago es requerido" });

      DataTable resultado = _reservaDAO.ConfirmarReservasSelectivas(idUsuario, reservasIds, metodoPago, promocionId);

          if (resultado == null || resultado.Rows.Count == 0)
          return BadRequest(new { success = false, message = "No se pudo procesar la confirmación" });

     // Verificar si hay error en la respuesta del SP
        if (resultado.Columns.Contains("Estado") &&
          resultado.Rows[0]["Estado"].ToString() == "ERROR")
     {
     return BadRequest(new { success = false, message = resultado.Rows[0]["Mensaje"].ToString() });
    }

           return Ok(new
    {
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
 _logger.LogError($"Error confirmando reservas: {ex.Message}");
  return StatusCode(500, new { success = false, message = $"Error confirmando reservas: {ex.Message}" });
 }
        }

      // ============================================================
        // ?? LISTAR PROMOCIONES VÁLIDAS PARA FECHAS DEL CARRITO DE UN USUARIO
 // ============================================================
        [HttpGet("promociones-validas/{idUsuario}")]
        public IActionResult ListarPromocionesValidasParaCarrito(int idUsuario)
        {
  try
          {
        if (idUsuario <= 0)
    return BadRequest(new { success = false, message = "ID de usuario no válido" });

    // Nota: Este método necesita estar implementado en PromocionDAO
// Si no existe, se puede usar ListarPromocionesActivas como alternativa
            var promocionesValidas = _promocionDAO.ListarPromocionesActivas();

       if (promocionesValidas == null)
        return BadRequest(new { success = false, message = "No se pudieron obtener las promociones válidas" });

      return Ok(new
        {
        success = true,
         promociones = promocionesValidas
    });
     }
   catch (Exception ex)
  {
              _logger.LogError($"Error al obtener promociones válidas: {ex.Message}");
       return StatusCode(500, new { success = false, message = $"Error al obtener promociones válidas: {ex.Message}" });
            }
        }
    }
}
