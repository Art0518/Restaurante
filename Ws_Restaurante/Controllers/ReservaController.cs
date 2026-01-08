using System;
using System.Data;
using System.Web.Http;
using Logica.Servicios;
using GDatos.Entidades;
using System.Net;

namespace Ws_Restaurante.Controllers
{
    [RoutePrefix("api/reservas")]
    public class ReservaController : ApiController
    {
        private readonly ReservaLogica reservaLogica = new ReservaLogica();

        // LISTAR TODAS LAS RESERVAS
        [HttpGet]
        [Route("")]
        public IHttpActionResult ListarReservas()
        {
            try
            {
                DataTable reservas = reservaLogica.ListarReservas();
                return Ok(reservas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ============================================================
        // 🔵 CREAR RESERVA
        // ============================================================
        [HttpPost]
        [Route("crear")]
        public IHttpActionResult CrearReserva([FromBody] Reserva reserva)
        {
            try
            {
                if (reserva == null)
                    return BadRequest("Los datos de la reserva son requeridos");

                // 🔥 UNIFICAR FECHA + HORA EN FORMATO DATETIME
                if (reserva.Fecha != null && !string.IsNullOrEmpty(reserva.Hora))
                {
                    // ======================================================
                    // ✔ CORRECCIÓN DE ZONA HORARIA (EVITA QUE SE SUME 1 DÍA)
                    // ======================================================
                    DateTime fechaLocal = DateTime.SpecifyKind(reserva.Fecha, DateTimeKind.Unspecified);
                    fechaLocal = fechaLocal.AddHours(5); // Ecuador UTC-5

                    reserva.Fecha = fechaLocal.Date;

                    // Convertir hora AM/PM a formato TIME
                    DateTime horaBase = DateTime.Parse(
                        reserva.Hora,
                        null,
                        System.Globalization.DateTimeStyles.AllowInnerWhite
                    );

                    reserva.Hora = horaBase.ToString("HH:mm:ss"); // TIME SQL
                }

                DataTable resultado = reservaLogica.CrearReserva(reserva);

                if (resultado.Rows.Count > 0 && resultado.Columns.Contains("Resultado"))
                {
                    string mensaje = resultado.Rows[0]["Resultado"].ToString();

                    if (mensaje.Contains("ya está reservada"))
                        return Content(HttpStatusCode.Conflict, mensaje);

                    return Ok(new { mensaje = mensaje, datos = resultado });
                }

                return Ok(new { mensaje = "Reserva creada correctamente", datos = resultado });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ============================================================
        // CAMBIAR ESTADO
        // ============================================================

        // ============================================================
        // CAMBIAR ESTADO DE RESERVA + MÉTODO DE PAGO
        // ============================================================
        [HttpPut]
        [Route("estado")]
        public IHttpActionResult ActualizarEstado([FromBody] ActualizarEstadoRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Los datos son requeridos");

                if (string.IsNullOrEmpty(request.MetodoPago))
                    return BadRequest("Debe enviar un método de pago: EFECTIVO, TRANSFERENCIA o TARJETA.");

                reservaLogica.ActualizarEstado(
                    request.IdReserva,
                    request.Estado,
                    request.MetodoPago
                );

                return Ok(new { mensaje = "Estado actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // ============================================================
        // 🔵 EDITAR RESERVA COMPLETA
        // ============================================================
        [HttpPut]
        [Route("editar")]
        public IHttpActionResult EditarReserva([FromBody] Reserva reserva)
        {
            try
            {
                if (reserva == null)
                    return BadRequest("Datos inválidos");

                // ======================================================
                // ✔ CORRECCIÓN DE ZONA HORARIA (EVITA EL DESFASE)
                // ======================================================
                DateTime fechaLocal = DateTime.SpecifyKind(reserva.Fecha, DateTimeKind.Unspecified);
                fechaLocal = fechaLocal.AddHours(5); // Ecuador UTC-5

                reserva.Fecha = fechaLocal.Date;

                // ✔ Convertir hora AM/PM → TIME
                DateTime horaBase = DateTime.Parse(
                    reserva.Hora,
                    null,
                    System.Globalization.DateTimeStyles.AllowInnerWhite
                );

                reserva.Hora = horaBase.ToString("HH:mm:ss");

                string resultado = reservaLogica.EditarReserva(reserva);

                return Ok(new { mensaje = "Reserva actualizada correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ============================================================
        // OBTENER RESERVA POR ID
        // ============================================================
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult ObtenerReservaPorId(int id)
        {
            try
            {
                DataTable reservas = reservaLogica.ListarReservas();
                DataRow[] fila = reservas.Select($"IdReserva = {id}");

                if (fila.Length == 0)
                    return NotFound();

                var r = fila[0];

                return Ok(new
                {
                    IdReserva = r["IdReserva"],
                    IdUsuario = r["IdUsuario"],
                    IdMesa = r["IdMesa"],
                    Fecha = r["Fecha"],
                    Hora = r["Hora"],
                    NumeroPersonas = r["NumeroPersonas"],
                    Estado = r["Estado"],
                    Observaciones = r["Observaciones"]
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ============================================================
        // FILTRAR RESERVAS
        // ============================================================
        [HttpGet]
        [Route("filtrar")]
        public IHttpActionResult FiltrarReservas(int? idUsuario = null, string estado = null, DateTime? fecha = null)
        {
            try
            {
                DataTable reservas = reservaLogica.ListarReservas();
                string filtro = "";

                if (idUsuario.HasValue)
                    filtro += $"IdUsuario = {idUsuario.Value}";

                if (!string.IsNullOrEmpty(estado))
                    filtro += (filtro != "" ? " AND " : "") + $"Estado = '{estado}'";

                if (fecha.HasValue)
                    filtro += (filtro != "" ? " AND " : "") + $"Fecha = '{fecha.Value:yyyy-MM-dd}'";

                DataRow[] filasFiltradas = string.IsNullOrEmpty(filtro)
                    ? reservas.Select()
                    : reservas.Select(filtro);

                DataTable resultado = reservas.Clone();

                foreach (DataRow row in filasFiltradas)
                    resultado.ImportRow(row);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ============================================================
        // NUEVO: LISTAR RESERVAS CONFIRMADAS DE UN USUARIO
        // ============================================================
        [HttpGet]
        [Route("confirmadas/{idUsuario}")]
        public IHttpActionResult ListarReservasConfirmadas(int idUsuario)
        {
            try
            {
                if (idUsuario <= 0)
                    return BadRequest("ID de usuario no válido");

                DataTable reservasConfirmadas = reservaLogica.ListarReservasConfirmadas(idUsuario);

                return Ok(new
                {
                    success = true,
                    reservas = reservasConfirmadas,
                    message = "Reservas confirmadas obtenidas exitosamente"
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Error al obtener reservas confirmadas: {ex.Message}"));
            }
        }
        
        // ============================================================
        // NUEVO: LISTAR TODAS LAS RESERVAS PARA ADMINISTRADOR
        // ============================================================
        [HttpGet]
        [Route("admin/todas")]
        public IHttpActionResult ListarTodasReservasAdmin()
        {
            try
            {
                DataTable todasReservas = reservaLogica.ListarTodasReservasAdmin();

                return Ok(new
                {
                    success = true,
                    reservas = todasReservas,
                    message = "Todas las reservas obtenidas exitosamente"
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Error al obtener todas las reservas: {ex.Message}"));
            }
        }
        
        // ============================================================
        // NUEVO: GENERAR FACTURA DESDE ADMIN PARA CUALQUIER RESERVA
        // ============================================================
        [HttpPost]
        [Route("admin/generar-factura")]
        public IHttpActionResult GenerarFacturaDesdeAdmin([FromBody] dynamic body)
        {
            try
            {
                if (body == null)
                    return BadRequest("Datos requeridos para generar factura");

                int idReserva = Convert.ToInt32(body.IdReserva);
                string metodoPago = body.MetodoPago?.ToString();
                string tipoFactura = body.TipoFactura?.ToString() ?? "ADMIN";

                if (idReserva <= 0)
                    return BadRequest("ID de reserva no válido");

                if (string.IsNullOrWhiteSpace(metodoPago))
                    return BadRequest("Método de pago es requerido");

                DataTable resultado = reservaLogica.GenerarFacturaDesdeAdmin(idReserva, metodoPago, tipoFactura);

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
                            Total = Convert.ToDecimal(row["Total"])
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
                return InternalServerError(new Exception($"Error generando factura desde admin: {ex.Message}"));
            }
        }
    }

    public class ActualizarEstadoRequest
    {
        public int IdReserva { get; set; }
        public string Estado { get; set; }

        public string MetodoPago { get; set; }
    }
}
