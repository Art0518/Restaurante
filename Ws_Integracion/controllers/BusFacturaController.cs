using System;
using System.Web.Http;
using System.Web.Http.Description;
using Logica.Servicios;
using Ws_GIntegracionBus.DTOS;

namespace Ws_GIntegracionBus.Controllers.V1
{
    [RoutePrefix("api/v1/integracion/restaurantes")]
    public class BusFacturaController : ApiController
    {
        private readonly FacturaLogica facturaLogica = new FacturaLogica();

        /// <summary>
        /// Genera una factura para una reserva específica.
        /// </summary>
        /// <param name="body">Objeto JSON con IdUsuario, IdReserva, Subtotal, IVA y Total.</param>
        /// <returns>Detalle de la factura emitida.</returns>
        [HttpPost]
        [Route("invoices")]
        [ResponseType(typeof(FacturaResponse))]
        public IHttpActionResult EmitirFactura([FromBody] FacturaRequest body)
        {
            try
            {
                // ⚠ LÓGICA ORIGINAL — NO SE TOCA
                int idUsuario = body.IdUsuario;
                int idReserva = body.IdReserva;

                decimal subtotal = body.Subtotal;
                decimal iva = body.IVA;
                decimal total = body.Total;

                // Llamada real a tu capa lógica
                var resultado = facturaLogica.GenerarFactura(idUsuario, idReserva);

                string baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);

                // RETORNO ORIGINAL — SOLO DOCUMENTACIÓN SE ARREGLA
                return Ok(new
                {
                    Mensaje = "Factura emitida correctamente.",
                    IdUsuario = idUsuario,
                    IdReserva = idReserva,
                    Subtotal = subtotal,
                    IVA = iva,
                    Total = total,
                    Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Estado = "ACT",

                    _links = new
                    {
                        self = new
                        {
                            href = Request.RequestUri.AbsoluteUri,
                            method = "POST"
                        },
                        verFactura = new
                        {
                            href = $"{baseUrl}/api/v1/integracion/restaurantes/invoices/{idReserva}",
                            method = "GET"
                        },
                        reservas = new
                        {
                            href = $"{baseUrl}/api/v1/integracion/restaurantes/reservar",
                            method = "POST"
                        },
                        searchMesas = new
                        {
                            href = $"{baseUrl}/api/v1/integracion/restaurantes/search",
                            method = "GET"
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Error al emitir factura: " + ex.Message);
            }
        }
    }
}

