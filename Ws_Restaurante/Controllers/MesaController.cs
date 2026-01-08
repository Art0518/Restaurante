using GDatos.Entidades;
using Logica.Servicios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Http;

namespace Ws_GestionInterna.Controllers
{
    [RoutePrefix("api/mesas")]
    public class MesaController : ApiController
    {
        private readonly MesaLogica mesaLogica = new MesaLogica();

        // ============================================================
        // GET: /api/mesas
        // ============================================================
        [HttpGet]
        [Route("")]
        public IHttpActionResult Listar()
        {
            try
            {
                var dt = mesaLogica.ListarMesas();
                return Ok(dt);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al listar mesas: " + ex.Message);
            }
        }


        // ✅ OBTENER MESAS DISPONIBLES
        // GET /api/mesas/disponibles?zona=Interior&personas=4
        [HttpGet]
        [Route("disponibles")]
        public IHttpActionResult ObtenerDisponibles(string zona, int personas)
        {
            try
            {
                var dt = mesaLogica.MesasDisponibles(zona, personas);
                return Ok(dt);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ============================================================
        // GET: /api/mesas/{id}
        // ============================================================
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult ObtenerMesa(int id)
        {
            try
            {
                DataTable todas = mesaLogica.ListarMesas();
                DataRow[] filtro = todas.Select($"IdMesa = {id}");

                if (filtro.Length == 0)
                    return NotFound();

                var mesa = filtro[0];

                return Ok(new
                {
                    IdMesa = mesa["IdMesa"],
                    NumeroMesa = mesa["NumeroMesa"],
                    TipoMesa = mesa["TipoMesa"],
                    Capacidad = mesa["Capacidad"],
                    Precio = mesa["Precio"],
                    ImagenURL = mesa["ImagenURL"],
                    Estado = mesa["Estado"]
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Error al obtener mesa: " + ex.Message);
            }
        }

        // ============================================================
        // POST: /api/mesas/gestionar
        // ============================================================
        [HttpPost]
        [Route("gestionar")]
        public IHttpActionResult GestionarMesa([FromBody] Mesa m)
        {
            try
            {
                mesaLogica.GestionarMesa(m);
                string mensaje = (m.IdMesa > 0) ? "Mesa actualizada correctamente" : "Mesa registrada correctamente";
                return Ok(new { mensaje });
            }
            catch (Exception ex)
            {
                return BadRequest("Error al gestionar mesa: " + ex.Message);
            }
        }

        // ============================================================
        // PUT: /api/mesas/{id}/estado
        // ============================================================
        [HttpPut]
        [Route("{id}/estado")]
        public IHttpActionResult ActualizarEstado(int id, [FromBody] Mesa dto)
        {
            try
            {
                mesaLogica.ActualizarEstado(id, dto.Estado);
                return Ok(new { mensaje = "Estado actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest("Error al actualizar estado: " + ex.Message);
            }
        }

        // ============================================================
        // DELETE: /api/mesas/{id}
        // ============================================================
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult EliminarMesa(int id)
        {
            try
            {
                mesaLogica.ActualizarEstado(id, "INACTIVA");
                return Ok(new { mensaje = "Mesa inactivada correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest("Error al eliminar mesa: " + ex.Message);
            }
        }

        // ============================================================
        // GET: /api/mesas/{idMesa}/disponibilidad?fecha=2025-11-20
        // ============================================================
        [HttpGet]
        [Route("{idMesa}/disponibilidad")]
        public IHttpActionResult ObtenerDisponibilidadMesa(int idMesa, DateTime fecha)
        {
            try
            {
                var dt = mesaLogica.ObtenerDisponibilidad(idMesa, fecha);

                var listaHoras = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    string valor = row["Hora"].ToString().Trim();

                    string horaSolo = valor;

                    // Si por alguna razón el SP devuelve DATETIME, se corrige aquí
                    if (valor.Contains(" "))
                    {
                        horaSolo = valor.Split(' ')[1]; // extrae solo HH:mm:ss
                    }

                    listaHoras.Add(new { Hora = horaSolo });
                }

                return Ok(listaHoras);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al obtener disponibilidad: " + ex.Message);
            }
        }





    }
}
