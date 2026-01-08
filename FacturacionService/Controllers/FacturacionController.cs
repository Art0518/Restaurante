using Microsoft.AspNetCore.Mvc;
using FacturacionService.Models;
using FacturacionService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace FacturacionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturacionController : ControllerBase
    {
        private readonly ILogger<FacturacionController> _logger;
        private readonly FacturaDAO _facturaDAO;
 private readonly DetalleFacturaDAO _detalleFacturaDAO;

        public FacturacionController(ILogger<FacturacionController> logger, IConfiguration configuration)
        {
            _logger = logger;
         var connectionString = configuration.GetConnectionString("DefaultConnection");
            _facturaDAO = new FacturaDAO(connectionString);
            _detalleFacturaDAO = new DetalleFacturaDAO(connectionString);
   }

        // ============================================================
        // ? GENERAR FACTURA BÁSICA (Legacy - usando SP)
    // ============================================================
        [HttpPost("generar")]
        public ActionResult<ApiResponse<Factura>> GenerarFactura([FromBody] GenerarFacturaDto request)
        {
        try
     {
   _logger.LogInformation($"REST: Generando factura para usuario {request.IdUsuario}");
   if (request.IdUsuario <= 0)
                 return BadRequest(new ApiResponse<Factura> { Success = false, Mensaje = "ID de usuario no válido" });
     if (request.IdReserva <= 0)
return BadRequest(new ApiResponse<Factura> { Success = false, Mensaje = "ID de reserva no válido" });

    var factura = new Factura
                {
IdUsuario = request.IdUsuario,
            IdReserva = request.IdReserva,
      FechaEmision = DateTime.Now,
             Subtotal = request.Subtotal,
          IVA = request.Subtotal * 0.115m, // IVA 11.5%
  Total = request.Subtotal * 1.115m,
  Estado = "Emitida",
     NumeroFactura = $"FAC-{DateTime.Now:yyyyMMddHHmmss}",
  MetodoPago = request.MetodoPago
      };

      factura = _facturaDAO.CrearFactura(factura);
  return Ok(new ApiResponse<Factura> { Success = true, Mensaje = "Factura generada correctamente", Data = factura });
        }
   catch (Exception ex)
            {
          _logger.LogError($"Error al generar factura: {ex.Message}");
        return StatusCode(500, new ApiResponse<Factura> { Success = false, Mensaje = $"Error: {ex.Message}" });
            }
        }

     // ============================================================
        // ?? GENERAR FACTURA DESDE CARRITO (Nuevo - Query SQL Directa)
        // ============================================================
        [HttpPost("generar-carrito")]
        public ActionResult<ApiResponse<object>> GenerarFacturaCarrito([FromBody] GenerarFacturaCarritoDto request)
        {
            try
   {
                _logger.LogInformation($"REST: Generando factura desde carrito para usuario {request.IdUsuario}");

      DataTable resultado = _facturaDAO.GenerarFacturaCarrito(
 request.IdUsuario,
       request.ReservasIds,
         request.PromocionId,
      request.MetodoPago
             );

         if (resultado == null || resultado.Rows.Count == 0)
     {
           return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "No se pudo generar la factura" });
        }

    var row = resultado.Rows[0];
     string estado = row["Estado"].ToString();

    if (estado == "ERROR")
       {
           return BadRequest(new ApiResponse<object> 
        { 
               Success = false, 
              Mensaje = row["Mensaje"].ToString() 
      });
          }

         var response = new
            {
          Estado = estado,
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
           };

                return Ok(new ApiResponse<object> 
    { 
        Success = true, 
        Mensaje = "Factura generada correctamente", 
        Data = response 
     });
            }
    catch (Exception ex)
            {
          _logger.LogError($"Error al generar factura desde carrito: {ex.Message}");
      return StatusCode(500, new ApiResponse<object> { Success = false, Mensaje = $"Error: {ex.Message}" });
            }
}

        // ============================================================
        // ?? OBTENER FACTURA DETALLADA
        // ============================================================
    [HttpGet("{idFactura}/detallada")]
        public ActionResult<ApiResponse<object>> ObtenerFacturaDetallada(int idFactura)
     {
            try
      {
              _logger.LogInformation($"REST: Obteniendo factura detallada {idFactura}");
   if (idFactura <= 0)
 return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "ID de factura no válido" });

     DataSet dataSet = _facturaDAO.ObtenerFacturaDetallada(idFactura);

   if (dataSet == null || dataSet.Tables.Count < 2)
     return NotFound(new ApiResponse<object> { Success = false, Mensaje = "Factura no encontrada" });

                var response = new
                {
      Factura = dataSet.Tables[0],
   Detalles = dataSet.Tables[1]
                };

     return Ok(new ApiResponse<object> 
                { 
      Success = true, 
               Mensaje = "Factura detallada obtenida correctamente", 
        Data = response 
      });
    }
          catch (Exception ex)
            {
 _logger.LogError($"Error al obtener factura detallada: {ex.Message}");
      return StatusCode(500, new ApiResponse<object> { Success = false, Mensaje = $"Error: {ex.Message}" });
         }
        }

      // ============================================================
        // ?? MARCAR FACTURA COMO PAGADA
        // ============================================================
        [HttpPost("marcar-pagada")]
        public ActionResult<ApiResponse<object>> MarcarFacturaPagada([FromBody] MarcarPagadaDto request)
        {
   try
       {
  _logger.LogInformation($"REST: Marcando factura {request.IdFactura} como pagada");

    DataTable resultado = _facturaDAO.MarcarFacturaPagada(request.IdFactura, request.MetodoPago);

   if (resultado == null || resultado.Rows.Count == 0)
       {
   return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "No se pudo marcar la factura como pagada" });
 }

      var row = resultado.Rows[0];
 string estado = row["Estado"].ToString();

         if (estado == "ERROR")
       {
      return BadRequest(new ApiResponse<object> 
           { 
                 Success = false, 
  Mensaje = row["Mensaje"].ToString() 
        });
              }

    var response = new
{
          Estado = estado,
          Mensaje = row["Mensaje"].ToString(),
          IdFactura = Convert.ToInt32(row["IdFactura"])
     };

          return Ok(new ApiResponse<object> 
  { 
    Success = true, 
     Mensaje = "Factura marcada como pagada correctamente", 
   Data = response 
          });
   }
   catch (Exception ex)
     {
         _logger.LogError($"Error al marcar factura como pagada: {ex.Message}");
         return StatusCode(500, new ApiResponse<object> { Success = false, Mensaje = $"Error: {ex.Message}" });
    }
     }

        // ============================================================
      // ?? LISTAR FACTURAS DE USUARIO
        // ============================================================
        [HttpGet("usuario/{idUsuario}")]
        public ActionResult<ApiResponse<DataTable>> ListarFacturasUsuario(int idUsuario)
        {
     try
    {
                _logger.LogInformation($"REST: Listando facturas del usuario {idUsuario}");
        if (idUsuario <= 0)
     return BadRequest(new ApiResponse<DataTable> { Success = false, Mensaje = "ID de usuario no válido" });

              DataTable facturas = _facturaDAO.ListarFacturasUsuario(idUsuario);
              return Ok(new ApiResponse<DataTable> 
     { 
                    Success = true, 
     Mensaje = "Facturas del usuario listadas correctamente", 
    Data = facturas 
         });
            }
      catch (Exception ex)
   {
                _logger.LogError($"Error al listar facturas del usuario: {ex.Message}");
   return StatusCode(500, new ApiResponse<DataTable> { Success = false, Mensaje = $"Error: {ex.Message}" });
            }
        }

        // ============================================================
        // ?? GENERAR FACTURA PARA RESERVAS CONFIRMADAS
   // ============================================================
        [HttpPost("generar-confirmadas")]
  public ActionResult<ApiResponse<object>> GenerarFacturaReservasConfirmadas([FromBody] GenerarFacturaConfirmadasDto request)
        {
            try
          {
     _logger.LogInformation($"REST: Generando factura de reservas confirmadas para usuario {request.IdUsuario}");

       DataTable resultado = _facturaDAO.GenerarFacturaReservasConfirmadas(
            request.IdUsuario,
              request.ReservasIds,
       request.TipoFactura ?? "CONFIRMADA"
     );

         if (resultado == null || resultado.Rows.Count == 0)
      {
 return BadRequest(new ApiResponse<object> { Success = false, Mensaje = "No se pudo generar la factura" });
          }

         var row = resultado.Rows[0];
    string estado = row["Estado"].ToString();

           if (estado == "ERROR")
    {
            return BadRequest(new ApiResponse<object> 
       { 
      Success = false, 
  Mensaje = row["Mensaje"].ToString() 
         });
   }

  var response = new
  {
     Estado = estado,
           Mensaje = row["Mensaje"].ToString(),
        IdFactura = Convert.ToInt32(row["IdFactura"]),
      SubtotalBruto = Convert.ToDecimal(row["SubtotalBruto"]),
          Subtotal = Convert.ToDecimal(row["Subtotal"]),
  IVA = Convert.ToDecimal(row["IVA"]),
          Total = Convert.ToDecimal(row["Total"]),
       CantidadReservas = Convert.ToInt32(row["CantidadReservas"]),
        MetodoPago = row["MetodoPago"].ToString()
       };

  return Ok(new ApiResponse<object> 
  { 
   Success = true, 
       Mensaje = "Factura de reservas confirmadas generada correctamente", 
   Data = response 
      });
       }
      catch (Exception ex)
       {
     _logger.LogError($"Error al generar factura de confirmadas: {ex.Message}");
       return StatusCode(500, new ApiResponse<object> { Success = false, Mensaje = $"Error: {ex.Message}" });
      }
        }

        // ============================================================
        // ? LISTAR TODAS LAS FACTURAS
        // ============================================================
        [HttpGet("listar")]
        public ActionResult<ApiResponse<List<Factura>>> ListarFacturas()
    {
            try
       {
  _logger.LogInformation("REST: Listando todas las facturas");
   var facturas = _facturaDAO.ListarTodasFacturas();
        return Ok(new ApiResponse<List<Factura>> { Success = true, Mensaje = "Facturas listadas correctamente", Data = facturas });
         }
            catch (Exception ex)
            {
   _logger.LogError($"Error al listar facturas: {ex.Message}");
                return StatusCode(500, new ApiResponse<List<Factura>> { Success = false, Mensaje = $"Error: {ex.Message}" });
       }
        }

     // ============================================================
 // ? OBTENER FACTURA POR ID
        // ============================================================
        [HttpGet("{idFactura}")]
public ActionResult<ApiResponse<FacturaDetalladaDto>> ObtenerFactura(int idFactura)
{
            try
            {
_logger.LogInformation($"REST: Obteniendo factura {idFactura}");
           if (idFactura <= 0)
          return BadRequest(new ApiResponse<FacturaDetalladaDto> { Success = false, Mensaje = "ID de factura no válido" });

                var factura = _facturaDAO.ObtenerFacturaById(idFactura);
      if (factura == null)
    return NotFound(new ApiResponse<FacturaDetalladaDto> { Success = false, Mensaje = "Factura no encontrada" });

       var detalles = _detalleFacturaDAO.ObtenerDetallesFactura(idFactura);
       var result = new FacturaDetalladaDto { Factura = factura, Detalles = detalles };
     return Ok(new ApiResponse<FacturaDetalladaDto> { Success = true, Mensaje = "Factura obtenida correctamente", Data = result });
}
            catch (Exception ex)
  {
              _logger.LogError($"Error al obtener factura: {ex.Message}");
  return StatusCode(500, new ApiResponse<FacturaDetalladaDto> { Success = false, Mensaje = $"Error: {ex.Message}" });
       }
        }

 // ============================================================
        // ? MARCAR FACTURA COMO PAGADA (Legacy)
        // ============================================================
   [HttpPut("{idFactura}/marcar-pagada")]
    public ActionResult<ApiResponse<string>> MarcarFacturaPagadaLegacy(int idFactura, [FromBody] MarcarPagadaDto request)
        {
        try
            {
          _logger.LogInformation($"REST: Marcando factura {idFactura} como pagada (legacy)");
 if (idFactura <= 0)
       return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "ID de factura no válido" });

           var resultado = _facturaDAO.MarcarComoPageda(idFactura, request.MetodoPago);
      if (!resultado)
       return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "No se pudo marcar la factura como pagada" });

       return Ok(new ApiResponse<string> { Success = true, Mensaje = "Factura marcada como pagada", Data = "Pagada" });
            }
            catch (Exception ex)
            {
        _logger.LogError($"Error al marcar como pagada: {ex.Message}");
                return StatusCode(500, new ApiResponse<string> { Success = false, Mensaje = $"Error: {ex.Message}" });
      }
     }

        // ============================================================
        // ? ANULAR FACTURA
        // ============================================================
     [HttpPut("{idFactura}/anular")]
        public ActionResult<ApiResponse<string>> AnularFactura(int idFactura, [FromBody] AnularFacturaDto request)
        {
       try
      {
                _logger.LogInformation($"REST: Anulando factura {idFactura}");
           if (idFactura <= 0)
     return BadRequest(new ApiResponse<string> { Success = false, Mensaje = "ID de factura no válido" });

        _facturaDAO.AnularFactura(idFactura);
          return Ok(new ApiResponse<string> { Success = true, Mensaje = "Factura anulada correctamente", Data = "Anulada" });
            }
            catch (Exception ex)
          {
    _logger.LogError($"Error al anular factura: {ex.Message}");
 return StatusCode(500, new ApiResponse<string> { Success = false, Mensaje = $"Error: {ex.Message}" });
}
        }

        // ============================================================
   // ? CALCULAR TOTALES
        // ============================================================
        [HttpPost("calcular-totales")]
        public ActionResult<ApiResponse<Factura>> CalcularTotales([FromBody] CalcularTotalesDto request)
      {
  try
            {
 _logger.LogInformation("REST: Calculando totales de factura");
   if (request.Factura == null)
   return BadRequest(new ApiResponse<Factura> { Success = false, Mensaje = "Factura no proporcionada" });

    var factura = request.Factura;
       float porcentajeIVA = request.PorcentajeIva > 0 ? request.PorcentajeIva : 0.115f; // IVA 11.5%
      factura.IVA = factura.Subtotal * (decimal)porcentajeIVA;
              factura.Total = factura.Subtotal + factura.IVA;

           return Ok(new ApiResponse<Factura> { Success = true, Mensaje = "Totales calculados correctamente", Data = factura });
        }
            catch (Exception ex)
        {
      _logger.LogError($"Error al calcular totales: {ex.Message}");
     return StatusCode(500, new ApiResponse<Factura> { Success = false, Mensaje = $"Error: {ex.Message}" });
            }
        }
    }

    // ============================================================
    // DTOs
    // ============================================================
    public class GenerarFacturaDto
    {
    public int IdUsuario { get; set; }
        public int IdReserva { get; set; }
        public decimal Subtotal { get; set; }
 public string MetodoPago { get; set; } = "";
    }

    public class GenerarFacturaCarritoDto
    {
  public int IdUsuario { get; set; }
        public string ReservasIds { get; set; } = "";
        public int? PromocionId { get; set; }
        public string MetodoPago { get; set; } = "";
    }

    public class GenerarFacturaConfirmadasDto
    {
        public int IdUsuario { get; set; }
        public string ReservasIds { get; set; } = "";
        public string? TipoFactura { get; set; }
    }

    public class MarcarPagadaDto
    {
        public int IdFactura { get; set; }
        public string MetodoPago { get; set; } = "";
        public string FechaPago { get; set; } = "";
    }

    public class AnularFacturaDto
    {
  public string Motivo { get; set; } = "";
    }

    public class CalcularTotalesDto
    {
        public Factura? Factura { get; set; }
     public float PorcentajeIva { get; set; }
    }

    public class FacturaDetalladaDto
    {
        public Factura? Factura { get; set; }
public List<DetalleFactura>? Detalles { get; set; }
    }
}
