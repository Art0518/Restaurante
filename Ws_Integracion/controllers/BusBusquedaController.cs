using System;
using System.Data;
using System.Web.Http;
using System.Web.Http.Description;
using Logica.Servicios;
using Ws_GIntegracionBus.DTOS;
using Ws_Integracion.dtos;

namespace Ws_GIntegracionBus.Controllers.V1
{
    [RoutePrefix("api/v1/integracion/restaurantes")]
    public class BusBusquedaController : ApiController
    {
        private readonly MesaLogica mesaLogica = new MesaLogica();

        /// <summary>
        /// Obtiene la lista de mesas. Los parámetros son únicamente para documentación del Swagger.
        /// </summary>
        /// <param name="capacidad">Capacidad mínima (solo documentado, no usado).</param>
        /// <param name="tipoMesa">Tipo de mesa (solo documentado, no usado).</param>
        /// <param name="estado">Estado de la mesa (solo documentado, no usado).</param>
        /// <returns>Listado completo de mesas.</returns>
        [HttpGet]
        [Route("search")]
        [ResponseType(typeof(BusquedaMesasSwaggerResponse))]
        public IHttpActionResult BuscarMesas(
            [FromUri] int? capacidad = null,
            [FromUri] string tipoMesa = null,
            [FromUri] string estado = null)
        {
            try
            {
                // 👇 TU LÓGICA ORIGINAL — NO TOCO NADA
                DataTable resultado = mesaLogica.ListarMesas();

                if (resultado == null || resultado.Rows.Count == 0)
                {
                    return Ok(new
                    {
                        mensaje = "No se encontraron mesas registradas.",
                        total = 0,
                        _links = new
                        {
                            self = new
                            {
                                href = Request.RequestUri.AbsoluteUri
                            },
                            createHold = new
                            {
                                href = $"{Request.RequestUri.GetLeftPart(UriPartial.Authority)}/api/v1/integracion/restaurantes/hold",
                                method = "POST"
                            }
                        }
                    });
                }

                return Ok(new
                {
                    mensaje = "Consulta de mesas realizada con éxito.",
                    total = resultado.Rows.Count,
                    mesas = resultado,
                    _links = new
                    {
                        self = new
                        {
                            href = Request.RequestUri.AbsoluteUri
                        },
                        createHold = new
                        {
                            href = $"{Request.RequestUri.GetLeftPart(UriPartial.Authority)}/api/v1/integracion/restaurantes/hold",
                            method = "POST"
                        },
                        reservar = new
                        {
                            href = $"{Request.RequestUri.GetLeftPart(UriPartial.Authority)}/api/v1/integracion/restaurantes/reservar",
                            method = "POST"
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Error al buscar mesas: " + ex.Message);
            }
        }
    }
}

