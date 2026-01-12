using GDatos.Entidades;
using Logica.Servicios;
using System;
using System.Data;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Ws_Restaurante.Controllers
{
    [RoutePrefix("api/usuarios")]
    public class UsuarioController : ApiController
    {
        private readonly UsuarioLogica usuarioLogica = new UsuarioLogica();

        // ============================================================
        //  HANDLERS PARA OPTIONS (Preflight CORS) — REQUERIDOS EN MONSTER
        // ============================================================

        // OPTIONS: /api/usuarios
        [HttpOptions]
        [Route("")]
        public IHttpActionResult OptionsRoot()
        {
            return Ok();
        }

        // OPTIONS: /api/usuarios/login
        [HttpOptions]
        [Route("login")]
        public IHttpActionResult OptionsLogin()
        {
            return Ok();
        }

        // OPTIONS: /api/usuarios/registrar
        [HttpOptions]
        [Route("registrar")]
        public IHttpActionResult OptionsRegistrar()
        {
            return Ok();
        }

        // OPTIONS: /api/usuarios/{id}/estado
        [HttpOptions]
        [Route("{id}/estado")]
        public IHttpActionResult OptionsEstado()
        {
            return Ok();
        }

        // OPTIONS: /api/usuarios/{id}/actualizar
        [HttpOptions]
        [Route("{id}/actualizar")]
        public IHttpActionResult OptionsActualizar()
        {
            return Ok();
        }


        // ============================================================
        //  MÉTODOS REALES DE LA API
        // ============================================================

        // POST: /api/usuarios/login
        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login([FromBody] Usuario datos)
        {
            try
            {
                DataTable result = usuarioLogica.Login(datos.Email, datos.Contrasena);

                if (result.Rows.Count == 0)
                    return Content(System.Net.HttpStatusCode.Unauthorized, new { mensaje = "Credenciales inválidas" });

                var usuario = result.Rows[0];
                return Ok(new
                {
                    mensaje = "Login exitoso",
                    usuario = new
                    {
                        IdUsuario = usuario["IdUsuario"],
                        Nombre = usuario["Nombre"],
                        Email = usuario["Email"],
                        Cedula = usuario["Cedula"],  // 🔥 INCLUIR CÉDULA
                        Rol = usuario["Rol"],
                        Estado = usuario["Estado"],
                        Telefono = usuario["Telefono"],
                        Direccion = usuario["Direccion"]
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: /api/usuarios/registrar
        [HttpPost]
        [Route("registrar")]
        public IHttpActionResult Registrar([FromBody] Usuario nuevo)
        {
            try
            {
                usuarioLogica.Registrar(nuevo);
                return Ok(new { mensaje = "Usuario registrado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: /api/usuarios
        [HttpGet]
        [Route("")]
        public IHttpActionResult Listar(string rol = null, string estado = null)
        {
            try
            {
                DataTable dt = usuarioLogica.Listar(rol, estado);
                dt.TableName = "Usuarios";

                return Ok(dt);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: /api/usuarios/{id}/estado
        [HttpPut]
        [Route("{id}/estado")]
        public IHttpActionResult CambiarEstado(int id, [FromBody] Usuario dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Estado))
                    return BadRequest("Debe especificar un estado.");

                usuarioLogica.CambiarEstado(id, dto.Estado);
                return Ok(new { mensaje = "Estado actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: /api/usuarios/{id}/actualizar
        [HttpPut]
        [Route("edit/{id}")]
        public IHttpActionResult Actualizar(int id, [FromBody] Usuario u)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("ID de usuario no válido.");

                u.IdUsuario = id;
                usuarioLogica.Actualizar(u);

                return Ok(new { mensaje = "Usuario actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
