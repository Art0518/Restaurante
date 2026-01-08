using System;
using System.Web.Http;
using Logica.Servicios;
using GDatos.Entidades;
using System.Web.Http.Description;
using Ws_GIntegracionBus.DTOS;

namespace Ws_GIntegracionBus.Controllers.V1
{
    [RoutePrefix("api/v1/integracion/restaurantes")]
    public class BusUsuarioController : ApiController
    {
        private readonly UsuarioLogica usuarioLogica = new UsuarioLogica();

        // 🟦 POST /api/v1/integracion/restaurantes/usuarios
        [HttpPost]
        [Route("usuarios")]
        [ResponseType(typeof(UsuarioResponseDTO))]
        public IHttpActionResult CrearUsuario([FromBody] UsuarioRequestDTO body)
        {
            try
            {
                // ⚠ TU LÓGICA NO SE MODIFICA
                string nombre = body.nombre;
                string email = body.email;
                string contrasena = body.contrasena;
                string rol = body.rol ?? "CLIENTE";
                string telefono = body.telefono ?? "";
                string direccion = body.direccion ?? "";

                var usuario = new Usuario
                {
                    Nombre = nombre,
                    Email = email,
                    Contrasena = contrasena,
                    Rol = rol,
                    Estado = "ACTIVO",
                    Telefono = telefono,
                    Direccion = direccion
                };

                usuarioLogica.Registrar(usuario);

                string baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);

                return Ok(new
                {
                    mensaje = "Usuario registrado correctamente.",
                    nombre,
                    email,
                    rol,
                    estado = "ACTIVO",
                    _links = new
                    {
                        self = new { href = Request.RequestUri.AbsoluteUri, method = "POST" },
                        listarUsuarios = new { href = $"{baseUrl}/api/v1/integracion/restaurantes/usuarios", method = "GET" },
                        crearReserva = new { href = $"{baseUrl}/api/v1/integracion/restaurantes/book", method = "POST" }
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Error al registrar usuario: " + ex.Message);
            }
        }

        // 🟦 GET /api/v1/integracion/restaurantes/usuarios
        [HttpGet]
        [Route("usuarios")]
        public IHttpActionResult ListarUsuarios()
        {
            try
            {
                var usuarios = usuarioLogica.Listar();

                // Base URL dinámica
                string baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);

                return Ok(new
                {
                    usuarios,

                    // 🟣 HATEOAS
                    _links = new
                    {
                        self = new
                        {
                            href = Request.RequestUri.AbsoluteUri,
                            method = "GET"
                        },
                        crearUsuario = new
                        {
                            href = $"{baseUrl}/api/v1/integracion/restaurantes/usuarios",
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
                return BadRequest("Error al listar usuarios: " + ex.Message);
            }
        }
    }
}

