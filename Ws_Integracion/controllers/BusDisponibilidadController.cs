using System;
using System.Data;
using System.Web.Http;
using System.Web.Http.Description;
using Logica.Servicios;
using Ws_GIntegracionBus.DTOS;

namespace Ws_GIntegracionBus.Controllers.V1
{
    [RoutePrefix("api/v1/integracion/restaurantes")]
    public class BusDisponibilidadController : ApiController
    {
        private readonly MesaLogica mesaLogica = new MesaLogica();

        /// <summary>
        /// Valida la disponibilidad de mesas para una fecha, hora y número de personas.
        /// </summary>
        /// <param name="body">Objeto JSON con fecha, hora, numeroPersonas y ciudad.</param>
        /// <returns>Resultado de disponibilidad con enlaces HATEOAS.</returns>
        [HttpPost]
        [Route("availability")]
        [ResponseType(typeof(DisponibilidadResponse))]
        public IHttpActionResult ValidarDisponibilidad([FromBody] DisponibilidadRequest body)
        {
            try
            {
                // ⚠️ TU LÓGICA ORIGINAL — SIN CAMBIAR NADA
                DateTime fecha = Convert.ToDateTime(body.fecha);
                string hora = body.hora;
                int numeroPersonas = body.numeroPersonas;
                string ciudad = body.ciudad;

                DataTable mesas = mesaLogica.ListarMesas();

                bool disponible = false;

                foreach (DataRow row in mesas.Rows)
                {
                    int capacidad = Convert.ToInt32(row["Capacidad"]);
                    string estado = row["Estado"].ToString().ToUpper();

                    string ubicacion = row.Table.Columns.Contains("Ciudad")
                        ? row["Ciudad"].ToString()
                        : "N/A";

                    if (capacidad >= numeroPersonas &&
                        estado == "DISPONIBLE" &&
                        (ciudad == null || ubicacion.Equals(ciudad, StringComparison.OrdinalIgnoreCase)))
                    {
                        disponible = true;
                        break;
                    }
                }

                string baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);

                // ⚠️ RETORNO ORIGINAL — SIN TOCARLO
                return Ok(new
                {
                    mensaje = "Validación completada correctamente.",
                    fecha = fecha.ToString("yyyy-MM-dd"),
                    hora,
                    numeroPersonas,
                    ciudad,
                    disponible,
                    _links = new
                    {
                        self = new
                        {
                            href = Request.RequestUri.AbsoluteUri,
                            method = "POST"
                        },
                        hold = new
                        {
                            href = $"{baseUrl}/api/v1/integracion/restaurantes/hold",
                            method = "POST"
                        },
                        reservar = new
                        {
                            href = $"{baseUrl}/api/v1/integracion/restaurantes/reservar",
                            method = "POST"
                        },
                        search = new
                        {
                            href = $"{baseUrl}/api/v1/integracion/restaurantes/search",
                            method = "GET"
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Error al validar disponibilidad: " + ex.Message);
            }
        }
    }
}

