#nullable enable

using Microsoft.AspNetCore.Mvc;
using SeguridadService.Models;
using SeguridadService.Data;
using System;
using System.Data;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SeguridadService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly ILogger<UsuariosController> _logger;
        private readonly UsuarioDAO _usuarioDAO;
        private readonly string _jwtSecret = "tu_clave_secreta_muy_larga_minimo_32_caracteres_aqui";

   public UsuariosController(ILogger<UsuariosController> logger, IConfiguration configuration)
        {
         _logger = logger;
        var connectionString = configuration.GetConnectionString("DefaultConnection");
    _usuarioDAO = new UsuarioDAO(connectionString ?? "");
     }

        // ============================================================
        //  GET: /api/usuarios - Verificar servicio
     // ============================================================
        [HttpGet]
        public IActionResult VerificarServicio()
        {
  return Ok(new { mensaje = "Servicio de Seguridad activo", version = "1.0" });
        }

        // ============================================================
        //  GET: /api/usuarios/listar
        // ============================================================
        [HttpGet("listar")]
  public IActionResult ListarUsuarios(
        [FromQuery] string? rol = null,
  [FromQuery] string? estado = null,
     [FromQuery] int pagina = 1,
    [FromQuery] int tamanoPagina = 50)
   {
  try
      {
   // Validar parámetros de paginación
 if (pagina < 1) pagina = 1;
 if (tamanoPagina < 1) tamanoPagina = 50;
         if (tamanoPagina > 500) tamanoPagina = 500; // Límite máximo para evitar sobrecarga
 
    _logger.LogInformation($"REST: Listando usuarios - Página: {pagina}, Tamaño: {tamanoPagina}, Rol: {rol}, Estado: {estado}");

      var (dt, totalRegistros) = _usuarioDAO.Listar(rol, estado, pagina, tamanoPagina);
  
      // Convertir DataTable a lista de objetos para serialización
    var usuarios = new List<object>();
      foreach (DataRow row in dt.Rows)
        {
      usuarios.Add(new
   {
           IdUsuario = Convert.ToInt32(row["IdUsuario"]),
      Nombre = row["Nombre"]?.ToString() ?? "",
  Email = row["Email"]?.ToString() ?? "",
    Cedula = row["Cedula"]?.ToString() ?? "",
Rol = row["Rol"]?.ToString() ?? "",
      Estado = row["Estado"]?.ToString() ?? "",
 Telefono = row["Telefono"]?.ToString() ?? "",
      Direccion = row["Direccion"]?.ToString() ?? ""
  });
        }

      // Calcular información de paginación
      int totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamanoPagina);

   return Ok(new 
 { 
          total = totalRegistros,
   pagina = pagina,
tamanoPagina = tamanoPagina,
        totalPaginas = totalPaginas,
 usuarios = usuarios
  });
    }
 catch (Exception ex)
  {
      _logger.LogError($"Error al listar usuarios: {ex.Message}");
    return StatusCode(500, new { mensaje = ex.Message });
    }
    }

        // ============================================================
   //  GET: /api/usuarios/{id}
        // ============================================================
        [HttpGet("{idUsuario}")]
      public IActionResult ObtenerUsuario(int idUsuario)
 {
    try
  {
      _logger.LogInformation($"REST: Obteniendo usuario {idUsuario}");

  if (idUsuario <= 0)
  return BadRequest(new { mensaje = "ID de usuario no válido" });

 DataTable dt = _usuarioDAO.ObtenerPorId(idUsuario);

 if (dt.Rows.Count == 0)
        return NotFound(new { mensaje = "Usuario no encontrado" });

   var row = dt.Rows[0];

   return Ok(new
           {
    IdUsuario = Convert.ToInt32(row["IdUsuario"]),
     Nombre = row["Nombre"]?.ToString() ?? "",
   Email = row["Email"]?.ToString() ?? "",
    Cedula = row["Cedula"]?.ToString() ?? "",
      Telefono = row["Telefono"]?.ToString() ?? "",
        Direccion = row["Direccion"]?.ToString() ?? "",
        Rol = row["Rol"]?.ToString() ?? "",
  Estado = row["Estado"]?.ToString() ?? ""
        });
 }
            catch (Exception ex)
  {
    _logger.LogError($"Error al obtener usuario: {ex.Message}");
      return StatusCode(500, new { mensaje = ex.Message });
    }
     }

        // ============================================================
        //  POST: /api/usuarios/registrar
        // ============================================================
 [HttpPost("registrar")]
     public IActionResult Registrar([FromBody] RegistrarDto request)
     {
            try
            {
        _logger.LogInformation($"REST: Registrando usuario: {request.Email}");

     if (string.IsNullOrWhiteSpace(request.Nombre))
          return BadRequest(new { mensaje = "El nombre es obligatorio" });

        if (string.IsNullOrWhiteSpace(request.Email))
     return BadRequest(new { mensaje = "El email es obligatorio" });

     if (string.IsNullOrWhiteSpace(request.Contrasena))
return BadRequest(new { mensaje = "La contraseña es obligatoria" });

        // Concatenar nombre y apellido si se proporciona apellido
  string nombreCompleto = request.Nombre;
        if (!string.IsNullOrWhiteSpace(request.Apellido))
  {
   nombreCompleto = $"{request.Nombre} {request.Apellido}";
        }

         // Crear el objeto usuario
             var usuario = new Usuario
     {
     Nombre = nombreCompleto,
 Email = request.Email,
    Telefono = request.Telefono ?? "",
         Cedula = request.Cedula ?? "",
        Direccion = request.Direccion ?? "",
     Rol = request.Rol ?? "Usuario",
 Estado = "ACTIVO"
     };

      // La contraseña se pasa sin encriptar
         _usuarioDAO.Registrar(usuario, request.Contrasena);

      return Ok(new
   {
      mensaje = "Usuario registrado correctamente"
 });
      }
        catch (Exception ex)
  {
    _logger.LogError($"Error al registrar usuario: {ex.Message}");
        return BadRequest(new { mensaje = ex.Message });
        }
        }

        // ============================================================
   //  POST: /api/usuarios/login
 // ============================================================
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto request)
        {
  try
      {
    _logger.LogInformation($"REST: Login intento para: {request.Email}");

        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Contrasena))
           return BadRequest(new { mensaje = "Email y contraseña son requeridos" });

        DataTable result = _usuarioDAO.Login(request.Email, request.Contrasena);

                if (result.Rows.Count == 0)
                return Unauthorized(new { mensaje = "Credenciales inválidas" });

   var row = result.Rows[0];

         // Actualizar última conexión
         _usuarioDAO.ActualizarUltimaConexion(Convert.ToInt32(row["IdUsuario"]));

   // Generar token
            var usuario = new Usuario
    {
         IdUsuario = Convert.ToInt32(row["IdUsuario"]),
 Nombre = row["Nombre"].ToString() ?? "",
             Email = row["Email"].ToString() ?? "",
    Rol = row["Rol"].ToString() ?? "Usuario",
       Cedula = row["Cedula"]?.ToString() ?? "",
              Telefono = row["Telefono"]?.ToString() ?? "",
   Direccion = row["Direccion"]?.ToString() ?? ""
       };

        string token = GenerarToken(usuario);

                return Ok(new
          {
 mensaje = "Login exitoso",
     token = token,
    usuario = new
       {
          IdUsuario = usuario.IdUsuario,
 Nombre = usuario.Nombre,
      Email = usuario.Email,
     Cedula = usuario.Cedula,
              Rol = usuario.Rol,
            Estado = row["Estado"]?.ToString() ?? "",
Telefono = usuario.Telefono,
         Direccion = usuario.Direccion
         }
    });
        }
         catch (Exception ex)
  {
   _logger.LogError($"Error en login: {ex.Message}");
    return BadRequest(new { mensaje = ex.Message });
   }
        }

        // ============================================================
   //  PUT: /api/usuarios/{id}
        // ============================================================
  [HttpPut("{idUsuario}")]
  public IActionResult ActualizarUsuario(int idUsuario, [FromBody] ActualizarUsuarioDto request)
        {
  try
 {
                _logger.LogInformation($"REST: Actualizando usuario {idUsuario}");

                if (idUsuario <= 0)
   return BadRequest(new { mensaje = "ID de usuario no válido" });

   var usuario = new Usuario
          {
    IdUsuario = idUsuario,
            Nombre = request.Nombre,
          Email = request.Email,
         Telefono = request.Telefono,
           Cedula = request.Cedula,
       Direccion = request.Direccion,
 Rol = request.Rol
         };

      _usuarioDAO.Actualizar(usuario);

                return Ok(new { mensaje = "Usuario actualizado correctamente" });
          }
            catch (Exception ex)
            {
     _logger.LogError($"Error al actualizar usuario: {ex.Message}");
  return BadRequest(new { mensaje = ex.Message });
            }
        }

        // ============================================================
     //  POST: /api/usuarios/{id}/cambiar-contrasena
        // ============================================================
        [HttpPost("{idUsuario}/cambiar-contrasena")]
        public IActionResult CambiarContrasena(int idUsuario, [FromBody] CambiarContrasenaDto request)
        {
 try
            {
                _logger.LogInformation($"REST: Cambiando contraseña para usuario {idUsuario}");

     if (idUsuario <= 0)
  return BadRequest(new { mensaje = "ID de usuario no válido" });

      // Verificar que la contraseña actual sea correcta
          DataTable dt = _usuarioDAO.ObtenerPorId(idUsuario);

   if (dt.Rows.Count == 0)
    return NotFound(new { mensaje = "Usuario no encontrado" });

 var row = dt.Rows[0];
      string email = row["Email"]?.ToString() ?? "";

 // Intentar login con contraseña actual
   DataTable loginResult = _usuarioDAO.Login(email, request.ContrasenaActual);

         if (loginResult.Rows.Count == 0)
          return Unauthorized(new { mensaje = "Contraseña actual incorrecta" });

                // Cambiar contraseña (sin encriptar)
        _usuarioDAO.CambiarContrasena(idUsuario, request.NuevaContrasena);

 return Ok(new { mensaje = "Contraseña cambiada correctamente" });
 }
 catch (Exception ex)
          {
              _logger.LogError($"Error al cambiar contraseña: {ex.Message}");
return StatusCode(500, new { mensaje = ex.Message });
    }
 }

        // ============================================================
        //  POST: /api/usuarios/validar-token
 // ============================================================
    [HttpPost("validar-token")]
public IActionResult ValidarToken([FromBody] ValidarTokenDto request)
        {
    try
{
        _logger.LogInformation("REST: Validando token");

          if (string.IsNullOrWhiteSpace(request.Token))
 return BadRequest(new { mensaje = "Token no proporcionado" });

    var principal = ValidarTokenJWT(request.Token);
        if (principal == null)
          return Unauthorized(new { mensaje = "Token inválido o expirado" });

         var emailClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
  var rolClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
    var idClaim = principal.Claims.FirstOrDefault(c => c.Type == "IdUsuario")?.Value;

     return Ok(new
         {
      mensaje = "Token válido",
              valido = true,
           idUsuario = int.TryParse(idClaim, out int id) ? id : 0,
         email = emailClaim ?? string.Empty,
     rol = rolClaim ?? string.Empty
       });
            }
            catch (Exception ex)
            {
       _logger.LogError($"Error al validar token: {ex.Message}");
   return StatusCode(500, new { mensaje = ex.Message });
       }
 }

        // ============================================================
        //  DELETE: /api/usuarios/{id}
  // ============================================================
        [HttpDelete("{idUsuario}")]
        public IActionResult EliminarUsuario(int idUsuario)
   {
       try
    {
        _logger.LogInformation($"REST: Eliminando usuario {idUsuario}");

           if (idUsuario <= 0)
  return BadRequest(new { mensaje = "ID de usuario no válido" });

          _usuarioDAO.Eliminar(idUsuario);

return Ok(new { mensaje = "Usuario eliminado correctamente" });
     }
 catch (Exception ex)
   {
    _logger.LogError($"Error al eliminar usuario: {ex.Message}");
    return StatusCode(500, new { mensaje = ex.Message });
    }
        }

        // ============================================================
        //  MÉTODOS PRIVADOS - JWT
     // ============================================================
      private string GenerarToken(Usuario usuario)
        {
    var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
          {
     Subject = new ClaimsIdentity(new[]
        {
     new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
 new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Name, usuario.Nombre),
           new Claim(ClaimTypes.Role, usuario.Rol),
   new Claim("IdUsuario", usuario.IdUsuario.ToString())
          }),
  Expires = DateTime.UtcNow.AddHours(24),
  SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
  return tokenHandler.WriteToken(token);
        }

        private ClaimsPrincipal? ValidarTokenJWT(string token)
        {
   try
          {
      var tokenHandler = new JwtSecurityTokenHandler();
           var key = Encoding.ASCII.GetBytes(_jwtSecret);

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
     {
     ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = false,
 ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
           }, out SecurityToken validatedToken);

    return principal;
            }
 catch
            {
 return null;
            }
   }
 }

    // ============================================================
    //  DTOs
    // ============================================================
    public class RegistrarDto
    {
        public string Nombre { get; set; } = "";
        public string Apellido { get; set; } = "";
        public string Email { get; set; } = "";
        public string Contrasena { get; set; } = "";
        public string Telefono { get; set; } = "";
        public string Cedula { get; set; } = "";
      public string Direccion { get; set; } = "";
  public string Rol { get; set; } = "";
    }

    public class LoginDto
    {
  public string Email { get; set; } = "";
        public string Contrasena { get; set; } = "";
    }

    public class ActualizarUsuarioDto
    {
        public string Nombre { get; set; } = "";
        public string Email { get; set; } = "";
    public string Telefono { get; set; } = "";
        public string Cedula { get; set; } = "";
        public string Direccion { get; set; } = "";
        public string Rol { get; set; } = "";
    }

    public class CambiarContrasenaDto
    {
        public string ContrasenaActual { get; set; } = "";
        public string NuevaContrasena { get; set; } = "";
    }

    public class ValidarTokenDto
    {
        public string Token { get; set; } = "";
    }
}
