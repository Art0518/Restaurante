using GDatos.Entidades;
using Logica.Servicios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;

namespace Ws_Restaurante.Controllers
{
    [RoutePrefix("api/platos")]
    public class PlatoController : ApiController
    {
        private readonly PlatoLogica platoLogica = new PlatoLogica();

        // ================================
        // GET: /api/platos
        // ================================
        [HttpGet]
        [Route("")]
        public IHttpActionResult ListarPlatos()
        {
            try
            {
                DataTable dt = platoLogica.ListarPlatos();
                
                // Convertir DataTable a lista de objetos para mejor serialización JSON
                var platos = new List<object>();
                
                foreach (DataRow row in dt.Rows)
                {
                    platos.Add(new
                    {
                        IdPlato = Convert.ToInt32(row["IdPlato"]),
                        IdRestaurante = Convert.ToInt32(row["IdRestaurante"]),
                        Restaurante = row["Restaurante"].ToString(), // Nombre del restaurante
                        Nombre = row["Plato"].ToString(), // Nombre del plato desde el SP
                        Categoria = row["Categoria"].ToString(),
                        TipoComida = row["TipoComida"].ToString(),
                        Precio = Convert.ToDecimal(row["Precio"]),
                        Descripcion = row["Descripcion"].ToString(),
                        ImagenURL = row["ImagenURL"].ToString()
                    });
                }
           
                return Ok(platos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ================================
        // POST: /api/platos/gestionar
        // ================================
        [HttpPost]
        [Route("gestionar")]
        public IHttpActionResult GestionarPlato([FromBody] Plato p)
        {
            try
            {
                platoLogica.GestionarPlato(p);
                string mensaje = p.IdPlato > 0
                    ? "Plato actualizado correctamente"
                    : "Plato registrado correctamente";

                return Ok(new { mensaje });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ================================
        // GET: /api/platos/{id}
        // ================================
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult ObtenerPlatoPorId(int id)
        {
            try
            {
                DataTable platos = platoLogica.ListarPlatos();
                DataRow[] fila = platos.Select($"IdPlato = {id}");

                if (fila.Length == 0)
                    return NotFound();

                var plato = fila[0];

                return Ok(new
                {
                    IdPlato = plato["IdPlato"],
                    Nombre = plato["Nombre"],         // <- CORREGIDO
                    Precio = plato["Precio"],
                    Categoria = plato["Categoria"],
                    TipoComida = plato["TipoComida"],
                    Descripcion = plato["Descripcion"],
                    ImagenURL = plato["ImagenURL"]
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ================================
        // DELETE: /api/platos/{id}
        // ================================
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult EliminarPlato(int id)
        {
            try
            {
                Plato p = new Plato
                {
                    IdPlato = id,
                    Operacion = "DELETE"
                };

                platoLogica.GestionarPlato(p);

                return Ok(new { mensaje = "Plato eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
