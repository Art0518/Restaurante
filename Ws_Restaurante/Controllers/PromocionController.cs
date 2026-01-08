using GDatos.Entidades;
using Logica.Servicios;
using System;
using System.Data;
using System.Web.Http;

namespace Ws_Restaurante.Controllers
{
    [RoutePrefix("api/promociones")]
    public class PromocionController : ApiController
    {
        private readonly PromocionLogica promocionLogica = new PromocionLogica();

        // ✅ GET: /api/promociones
        [HttpGet]
        [Route("")]
        public IHttpActionResult Listar()
        {
            try
            {
                DataTable dt = promocionLogica.ListarPromociones();

                if (dt == null || dt.Rows.Count == 0)
                    return Ok(new {
                        mensaje = "No existen promociones registradas.",
                        promociones = new object[] { }
                    });

                return Ok(new
                {
                    mensaje = "Promociones obtenidas correctamente.",
                    promociones = dt
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Error al listar promociones: " + ex.Message);
            }
        }

        // ✅ POST: /api/promociones/gestionar
        [HttpPost]
        [Route("gestionar")]
        public IHttpActionResult Gestionar([FromBody] Promocion p)
        {
            try
            {
                if (p == null)
                    return BadRequest("Debe enviar los datos de la promoción.");

                // ⭐ Si no viene IdRestaurante, asignar el valor por defecto (ID 2)
                if (p.IdRestaurante == 0)
                    p.IdRestaurante = 2; // ⭐ CORREGIDO: IdRestaurante = 2

                string resultado = promocionLogica.GestionarPromocion(p);

                return Ok(new { mensaje = resultado });
            }
            catch (Exception ex)
            {
                return BadRequest("Error al gestionar la promoción: " + ex.Message);
            }
        }

        // ✅ DELETE: /api/promociones/eliminar/{id}
        [HttpDelete]
        [Route("eliminar/{id:int}")]
        public IHttpActionResult Eliminar(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("ID de promoción inválido.");

                string resultado = promocionLogica.EliminarPromocion(id);

                return Ok(new { mensaje = resultado });
            }
            catch (Exception ex)
            {
                return BadRequest("Error al eliminar la promoción: " + ex.Message);
            }
        }
    }
}