using Microsoft.AspNetCore.Mvc;
using FacturacionService.Data;
using System;
using System.Data;

namespace FacturacionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarritoController : ControllerBase
    {
        private readonly ILogger<CarritoController> _logger;
        // Nota: En el microservicio, el carrito se maneja en ReservasService
  // Este controlador delega las operaciones de facturación al FacturacionController

        public CarritoController(ILogger<CarritoController> logger)
        {
    _logger = logger;
        }

 // ============================================================
   // NOTA: Los endpoints de carrito están en ReservasService
        // Este controlador solo maneja la facturación del carrito
        // ============================================================

        [HttpGet("info")]
        public ActionResult<object> GetCarritoInfo()
        {
            return Ok(new
  {
      success = true,
    mensaje = "Los endpoints de carrito están en ReservasService. Este servicio solo maneja facturación.",
                endpoints = new
     {
         listarCarrito = "GET /api/reservas/carrito/{idUsuario} - En ReservasService",
       generarFactura = "POST /api/facturacion/generar-carrito - En FacturacionService",
         marcarPagada = "POST /api/facturacion/marcar-pagada - En FacturacionService"
       }
    });
        }
    }
}
