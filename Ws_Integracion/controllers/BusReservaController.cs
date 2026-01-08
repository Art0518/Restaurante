using System;
using System.Web.Http;
using Logica.Servicios;
using GDatos.Entidades;
using Ws_GIntegracionBus.Dtos;
using Ws_GIntegracionBus.DTOS;
using System.Data;
using System.Web.Http.Description;

namespace Ws_GIntegracionBus.Controllers.V1
{
    [RoutePrefix("api/v1/integracion/restaurantes")]
    public class BusReservaController : ApiController
    {
        private readonly ReservaLogica reservaLogica = new ReservaLogica();

        // ================================================================
        // 🟦 1. PRE-RESERVA (HOLD)
        // ================================================================
        [HttpPost]
        [Route("hold")]
        [ResponseType(typeof(HoldResponse))]
        public IHttpActionResult CrearPreReserva([FromBody] PreReservaDTO body)
        {
            try
            {
                if (body == null)
                    return BadRequest("El cuerpo de la solicitud está vacío o mal formateado.");

                DateTime fecha = body.fecha;
                string hora = body.hora;
                int personas = body.personas;
                int idUsuario = body.bookingUserId;
                int idMesa = body.idMesa;
                int duracionHold = body.duracionHoldSegundos ?? 300;

                var reserva = new Reserva
                {
                    IdUsuario = idUsuario,
                    IdMesa = idMesa,
                    Fecha = fecha,
                    Hora = hora,
                    NumeroPersonas = personas,
                    Estado = "HOLD"
                };

                reservaLogica.CrearReserva(reserva);

                string baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);

                return Ok(new
                {
                    Mensaje = "Pre-reserva creada correctamente.",
                    HoldId = Guid.NewGuid().ToString(),
                    Fecha = fecha.ToString("yyyy-MM-dd"),
                    Hora = hora,
                    Personas = personas,
                    DuracionSegundos = duracionHold,

                    _links = new
                    {
                        self = new
                        {
                            href = Request.RequestUri.AbsoluteUri,
                            method = "POST"
                        },
                        confirmarReserva = new
                        {
                            href = $"{baseUrl}/api/v1/integracion/restaurantes/book",
                            method = "POST"
                        },
                        detalleReserva = new
                        {
                            href = $"{baseUrl}/api/v1/integracion/restaurantes/reservas/{{idReserva}}",
                            method = "GET"
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Error al crear pre-reserva: " + ex.Message);
            }
        }

        // ================================================================
        // 🟦 2. CONFIRMACIÓN DE RESERVA (BOOK)
        // ================================================================
        [HttpPost]
        [Route("book")]
        [ResponseType(typeof(ReservaResponse))]
        public IHttpActionResult CrearReserva([FromBody] ConfirmarReservaDTO body)
        {
            try
            {
                if (body == null)
                    return BadRequest("El cuerpo de la solicitud está vacío o mal formateado.");

                DateTime fecha = body.fecha;
                string hora = body.hora;
                int idUsuario = body.bookingUserId;
                int idMesa = body.idMesa;
                int personas = body.personas;
                string metodoPago = body.metodoPago ?? "EFECTIVO";

                var reserva = new Reserva
                {
                    IdUsuario = idUsuario,
                    IdMesa = idMesa,
                    Fecha = fecha,
                    Hora = hora,
                    NumeroPersonas = personas,
                    Estado = "CONFIRMADA"
                };

                reservaLogica.CrearReserva(reserva);

                string baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);

                return Ok(new
                {
                    Mensaje = "Reserva confirmada correctamente.",
                    Fecha = fecha.ToString("yyyy-MM-dd"),
                    Hora = hora,
                    MetodoPago = metodoPago,
                    Estado = "CONFIRMADA",

                    _links = new
                    {
                        self = new
                        {
                            href = Request.RequestUri.AbsoluteUri,
                            method = "POST"
                        },
                        detalle = new
                        {
                            href = $"{baseUrl}/api/v1/integracion/restaurantes/reservas/{{idReserva}}",
                            method = "GET"
                        },
                        facturar = new
                        {
                            href = $"{baseUrl}/api/v1/integracion/restaurantes/invoices",
                            method = "POST"
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Error al crear reserva: " + ex.Message);
            }
        }

        // ================================================================
        // 🟦 3. CONSULTAR DETALLE DE RESERVA
        // ================================================================
        [HttpGet]
        [Route("reservas/{idReserva:int}")]
        [ResponseType(typeof(ReservaSwaggerResponse))]
        public IHttpActionResult BuscarDatosReserva(int idReserva)
        {
            try
            {
                if (idReserva <= 0)
                    return BadRequest("Debe enviar un ID de reserva válido.");

                DataSet ds = reservaLogica.BuscarDatosReserva(idReserva);

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                    return NotFound();

                var fila = ds.Tables[0].Rows[0];

                var reserva = new Reserva
                {
                    IdReserva = Convert.ToInt32(fila["IdReserva"]),
                    IdUsuario = Convert.ToInt32(fila["IdUsuario"]),
                    IdMesa = Convert.ToInt32(fila["IdMesa"]),
                    Fecha = Convert.ToDateTime(fila["Fecha"]),
                    TipoMesa = fila["TipoMesa"].ToString(),
                    Hora = fila["Hora"].ToString(),
                    NumeroPersonas = Convert.ToInt32(fila["NumeroPersonas"]),
                    Estado = fila["Estado"].ToString()
                };

                string baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);

                return Ok(new
                {
                    reserva,

                    _links = new
                    {
                        self = new
                        {
                            href = Request.RequestUri.AbsoluteUri,
                            method = "GET"
                        },
                        facturar = new
                        {
                            href = $"{baseUrl}/api/v1/integracion/restaurantes/invoices",
                            method = "POST"
                        },
                        buscarMesas = new
                        {
                            href = $"{baseUrl}/api/v1/integracion/restaurantes/search",
                            method = "GET"
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.BadRequest, new
                {
                    Message = "Error al buscar datos de reserva: " + ex.Message
                });
            }
        }
    }
}
