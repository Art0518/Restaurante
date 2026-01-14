using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using ReservasService.Data;
using ReservasService.Models;
using System;
using System.Data;

namespace ReservasService.Controllers
{
    [ApiController]
  [Route("api/carrito")]
    [EnableCors("AllowAll")]
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
 DataTable dt = _promocionDAO.ListarPromocionesActivas();

      if (dt == null || dt.Rows.Count == 0)
         return Ok(new { success = true, promociones = new List<object>() });

         // Convertir DataTable a lista de objetos
   var promociones = new List<object>();
         foreach (DataRow row in dt.Rows)
     {
      promociones.Add(new
 {
        IdPromocion = Convert.ToInt32(row["IdPromocion"]),
    IdRestaurante = row["IdRestaurante"] != DBNull.Value ? Convert.ToInt32(row["IdRestaurante"]) : (int?)null,
          Nombre = row["Nombre"]?.ToString(),
        Descuento = row["Descuento"] != DBNull.Value ? Convert.ToDecimal(row["Descuento"]) : 0,
          FechaInicio = row["FechaInicio"] != DBNull.Value ? Convert.ToDateTime(row["FechaInicio"]).ToString("yyyy-MM-dd") : null,
         FechaFin = row["FechaFin"] != DBNull.Value ? Convert.ToDateTime(row["FechaFin"]).ToString("yyyy-MM-dd") : null,
    Estado = row["Estado"]?.ToString()
        });
         }

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
        // ? CONFIRMAR RESERVAS SELECTIVAS - RECIBIR MONTO DE DESCUENTO
        // ============================================================
     [HttpPost("confirmar")]
      public IActionResult ConfirmarReservasSelectivas([FromBody] ConfirmarReservasRequest request)
   {
   try
     {
       // Validar que request no sea null
        if (request == null)
     return BadRequest(new { success = false, message = "Datos requeridos" });

       // Validaciones
  if (request.IdUsuario <= 0)
       return BadRequest(new { success = false, message = "ID de usuario no válido" });

        if (string.IsNullOrEmpty(request.ReservasIds))
        return BadRequest(new { success = false, message = "Debe especificar las reservas a confirmar" });

 if (string.IsNullOrEmpty(request.MetodoPago))
   return BadRequest(new { success = false, message = "Método de pago es requerido" });

      if (request.Total <= 0)
      return BadRequest(new { success = false, message = "El total debe ser mayor a 0" });

      DataTable resultado = _reservaDAO.ConfirmarReservasSelectivas(
     request.IdUsuario, 
            request.ReservasIds, 
            request.MetodoPago, 
            request.MontoDescuento, 
            request.Total);

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
       reservasConfirmadas = resultado.Rows[0]["ReservasConfirmadas"]
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
     DataTable dt = _promocionDAO.ListarPromocionesActivas();

   if (dt == null || dt.Rows.Count == 0)
        return Ok(new { success = true, promociones = new List<object>() });

     // Convertir DataTable a lista de objetos
      var promociones = new List<object>();
      foreach (DataRow row in dt.Rows)
 {
   promociones.Add(new
 {
      IdPromocion = Convert.ToInt32(row["IdPromocion"]),
   IdRestaurante = row["IdRestaurante"] != DBNull.Value ? Convert.ToInt32(row["IdRestaurante"]) : (int?)null,
        Nombre = row["Nombre"]?.ToString(),
    Descuento = row["Descuento"] != DBNull.Value ? Convert.ToDecimal(row["Descuento"]) : 0,
       FechaInicio = row["FechaInicio"] != DBNull.Value ? Convert.ToDateTime(row["FechaInicio"]).ToString("yyyy-MM-dd") : null,
  FechaFin = row["FechaFin"] != DBNull.Value ? Convert.ToDateTime(row["FechaFin"]).ToString("yyyy-MM-dd") : null,
  Estado = row["Estado"]?.ToString()
   });
}

      return Ok(new
     {
    success = true,
   promociones = promociones
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
