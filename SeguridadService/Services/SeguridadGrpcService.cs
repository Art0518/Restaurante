using Grpc.Core;
using SeguridadService.Models;
using SeguridadService.Data;
using SeguridadService.Protos;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SeguridadService.Services
{
    public class SeguridadGrpcService : SeguridadGrpc.SeguridadGrpcBase
    {
      private readonly ILogger<SeguridadGrpcService> _logger;
        private readonly UsuarioDAO _usuarioDAO;
 private readonly string _jwtSecret = "tu_clave_secreta_muy_larga_minimo_32_caracteres_aqui";

        public SeguridadGrpcService(ILogger<SeguridadGrpcService> logger, string connectionString)
 {
     _logger = logger;
  _usuarioDAO = new UsuarioDAO(connectionString);
        }

        // RPC: Registrar usuario
        public override Task<RegistrarUsuarioResponse> RegistrarUsuario(
  RegistrarUsuarioRequest request,
  ServerCallContext context)
 {
     try
    {
      _logger.LogInformation($"gRPC: Registrando usuario: {request.Email}");

if (string.IsNullOrWhiteSpace(request.Nombre) || string.IsNullOrWhiteSpace(request.Email))
return Task.FromResult(new RegistrarUsuarioResponse
      {
Success = false,
    Mensaje = "Nombre y Email son requeridos"
        });

  var usuario = new Models.Usuario
     {
          Nombre = $"{request.Nombre} {request.Apellido}",
  Email = request.Email,
 Telefono = request.Telefono ?? "",
         Cedula = request.Cedula ?? "",
 Direccion = request.Direccion ?? "",
     Rol = request.Rol ?? "Usuario",
   Estado = "ACTIVO"
      };

      _usuarioDAO.Registrar(usuario, request.Contrasena);

  // Obtener el usuario recién creado para generar token
     DataTable dt = _usuarioDAO.Login(request.Email, request.Contrasena);
 if (dt.Rows.Count == 0)
      return Task.FromResult(new RegistrarUsuarioResponse { Success = false, Mensaje = "Error al crear usuario" });

var row = dt.Rows[0];
     var usuarioCreado = MapearDataRowAUsuario(row);
     string token = GenerarToken(usuarioCreado);

   return Task.FromResult(new RegistrarUsuarioResponse
   {
       Success = true,
          Mensaje = "Usuario registrado correctamente",
   IdUsuario = usuarioCreado.IdUsuario,
      Usuario = MapearUsuarioProto(usuarioCreado),
    Token = token
});
        }
       catch (Exception ex)
      {
        _logger.LogError($"Error al registrar usuario: {ex.Message}");
      return Task.FromResult(new RegistrarUsuarioResponse
    {
   Success = false,
 Mensaje = $"Error: {ex.Message}"
           });
          }
}

  // RPC: Login
   public override Task<LoginResponse> Login(
   LoginRequest request,
 ServerCallContext context)
{
       try
   {
       _logger.LogInformation($"gRPC: Login intento para: {request.Email}");

       if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Contrasena))
   return Task.FromResult(new LoginResponse
     {
Success = false,
   Mensaje = "Email y contraseña son requeridos",
  CodigoError = 1
    });

        DataTable result = _usuarioDAO.Login(request.Email, request.Contrasena);

        if (result.Rows.Count == 0)
    return Task.FromResult(new LoginResponse
  {
       Success = false,
          Mensaje = "Credenciales inválidas",
          CodigoError = 2
        });

      var row = result.Rows[0];
 var usuario = MapearDataRowAUsuario(row);

       _usuarioDAO.ActualizarUltimaConexion(usuario.IdUsuario);
     string token = GenerarToken(usuario);

       return Task.FromResult(new LoginResponse
     {
Success = true,
        Mensaje = "Login exitoso",
     Usuario = MapearUsuarioProto(usuario),
   Token = token,
      CodigoError = 0
       });
   }
            catch (Exception ex)
   {
 _logger.LogError($"Error en login: {ex.Message}");
              return Task.FromResult(new LoginResponse
  {
        Success = false,
    Mensaje = $"Error: {ex.Message}",
  CodigoError = 99
    });
   }
        }

        // RPC: Obtener usuario
        public override Task<ObtenerUsuarioResponse> ObtenerUsuario(
 ObtenerUsuarioRequest request,
      ServerCallContext context)
        {
   try
   {
          _logger.LogInformation($"gRPC: Obteniendo usuario {request.IdUsuario}");

      if (request.IdUsuario <= 0)
   return Task.FromResult(new ObtenerUsuarioResponse
     {
     Success = false,
  Mensaje = "ID de usuario no válido"
       });

    DataTable dt = _usuarioDAO.ObtenerPorId(request.IdUsuario);

      if (dt.Rows.Count == 0)
         return Task.FromResult(new ObtenerUsuarioResponse
        {
     Success = false,
Mensaje = "Usuario no encontrado"
          });

      var usuario = MapearDataRowAUsuario(dt.Rows[0]);

        return Task.FromResult(new ObtenerUsuarioResponse
 {
   Success = true,
Mensaje = "Usuario obtenido correctamente",
Usuario = MapearUsuarioProto(usuario)
           });
    }
catch (Exception ex)
        {
        _logger.LogError($"Error al obtener usuario: {ex.Message}");
        return Task.FromResult(new ObtenerUsuarioResponse
     {
Success = false,
   Mensaje = $"Error: {ex.Message}"
         });
     }
  }

        // RPC: Actualizar usuario
  public override Task<ActualizarUsuarioResponse> ActualizarUsuario(
     ActualizarUsuarioRequest request,
            ServerCallContext context)
        {
  try
         {
       _logger.LogInformation($"gRPC: Actualizando usuario {request.IdUsuario}");

     if (request.IdUsuario <= 0)
     return Task.FromResult(new ActualizarUsuarioResponse
  {
Success = false,
      Mensaje = "ID de usuario no válido"
});

    var usuario = new Models.Usuario
    {
 IdUsuario = request.IdUsuario,
     Nombre = $"{request.Nombre} {request.Apellido}",
      Email = request.Email,
     Telefono = request.Telefono,
 Cedula = request.Cedula,
 Direccion = request.Direccion,
  Rol = request.Rol
           };

      _usuarioDAO.Actualizar(usuario);

            return Task.FromResult(new ActualizarUsuarioResponse
    {
       Success = true,
            Mensaje = "Usuario actualizado correctamente"
   });
    }
            catch (Exception ex)
  {
       _logger.LogError($"Error al actualizar usuario: {ex.Message}");
  return Task.FromResult(new ActualizarUsuarioResponse
       {
    Success = false,
       Mensaje = $"Error: {ex.Message}"
    });
     }
  }

     // RPC: Cambiar contraseña
     public override Task<CambiarContrasenaResponse> CambiarContrasena(
   CambiarContrasenaRequest request,
ServerCallContext context)
        {
   try
   {
      _logger.LogInformation($"gRPC: Cambiando contraseña para usuario {request.IdUsuario}");

     if (request.IdUsuario <= 0)
         return Task.FromResult(new CambiarContrasenaResponse
   {
     Success = false,
     Mensaje = "ID de usuario no válido"
       });

           DataTable dt = _usuarioDAO.ObtenerPorId(request.IdUsuario);

      if (dt.Rows.Count == 0)
       return Task.FromResult(new CambiarContrasenaResponse
    {
 Success = false,
        Mensaje = "Usuario no encontrado"
    });

     var row = dt.Rows[0];
         string email = row["Email"]?.ToString() ?? "";

// Verificar contraseña actual
        DataTable loginResult = _usuarioDAO.Login(email, request.ContrasenaActual);

      if (loginResult.Rows.Count == 0)
   return Task.FromResult(new CambiarContrasenaResponse
              {
     Success = false,
     Mensaje = "Contraseña actual incorrecta"
       });

     _usuarioDAO.CambiarContrasena(request.IdUsuario, request.NuevaContrasena);

   return Task.FromResult(new CambiarContrasenaResponse
        {
  Success = true,
       Mensaje = "Contraseña cambiada correctamente"
           });
        }
            catch (Exception ex)
         {
   _logger.LogError($"Error al cambiar contraseña: {ex.Message}");
 return Task.FromResult(new CambiarContrasenaResponse
    {
     Success = false,
    Mensaje = $"Error: {ex.Message}"
      });
      }
        }

        // RPC: Validar token
      public override Task<ValidarTokenResponse> ValidarToken(
         ValidarTokenRequest request,
       ServerCallContext context)
    {
     try
    {
        _logger.LogInformation("gRPC: Validando token");

          if (string.IsNullOrWhiteSpace(request.Token))
   return Task.FromResult(new ValidarTokenResponse
      {
    Valido = false,
       Mensaje = "Token no proporcionado"
   });

            var principal = ValidarTokenJWT(request.Token);

   if (principal == null)
         return Task.FromResult(new ValidarTokenResponse
  {
  Valido = false,
        Mensaje = "Token inválido o expirado"
       });

         var emailClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
    var rolClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
         var idClaim = principal.Claims.FirstOrDefault(c => c.Type == "IdUsuario")?.Value;

   return Task.FromResult(new ValidarTokenResponse
     {
         Valido = true,
  IdUsuario = int.TryParse(idClaim, out int id) ? id : 0,
            Email = emailClaim ?? string.Empty,
         Rol = rolClaim ?? string.Empty,
  Mensaje = "Token válido"
       });
    }
  catch (Exception ex)
  {
    _logger.LogError($"Error al validar token: {ex.Message}");
           return Task.FromResult(new ValidarTokenResponse
    {
      Valido = false,
                 Mensaje = $"Error: {ex.Message}"
              });
 }
        }

        // RPC: Eliminar usuario
    public override Task<EliminarUsuarioResponse> EliminarUsuario(
      EliminarUsuarioRequest request,
 ServerCallContext context)
   {
      try
  {
  _logger.LogInformation($"gRPC: Eliminando usuario {request.IdUsuario}");

     if (request.IdUsuario <= 0)
        return Task.FromResult(new EliminarUsuarioResponse
      {
     Success = false,
       Mensaje = "ID de usuario no válido"
   });

     _usuarioDAO.Eliminar(request.IdUsuario);

   return Task.FromResult(new EliminarUsuarioResponse
   {
  Success = true,
    Mensaje = "Usuario eliminado correctamente"
      });
     }
       catch (Exception ex)
    {
    _logger.LogError($"Error al eliminar usuario: {ex.Message}");
return Task.FromResult(new EliminarUsuarioResponse
    {
        Success = false,
        Mensaje = $"Error: {ex.Message}"
     });
    }
   }

        // RPC: Listar usuarios
        public override Task<ListarUsuariosResponse> ListarUsuarios(
    ListarUsuariosRequest request,
  ServerCallContext context)
     {
   try
  {
    _logger.LogInformation($"gRPC: Listando usuarios con rol: {request.Rol}");

   DataTable dt = _usuarioDAO.Listar(string.IsNullOrWhiteSpace(request.Rol) ? null : request.Rol, null);

      var usuariosProto = new Google.Protobuf.Collections.RepeatedField<Protos.Usuario>();

 foreach (DataRow row in dt.Rows)
   {
     var usuario = MapearDataRowAUsuario(row);
        usuariosProto.Add(MapearUsuarioProto(usuario));
 }

    return Task.FromResult(new ListarUsuariosResponse
    {
     Success = true,
  Mensaje = "Usuarios listados correctamente",
         Usuarios = { usuariosProto }
             });
 }
    catch (Exception ex)
     {
     _logger.LogError($"Error al listar usuarios: {ex.Message}");
      return Task.FromResult(new ListarUsuariosResponse
           {
    Success = false,
         Mensaje = $"Error: {ex.Message}"
       });
 }
  }

        // Métodos auxiliares
        private string GenerarToken(Models.Usuario usuario)
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

        private Models.Usuario MapearDataRowAUsuario(DataRow row)
      {
     return new Models.Usuario
    {
    IdUsuario = Convert.ToInt32(row["IdUsuario"]),
      Nombre = row["Nombre"]?.ToString() ?? "",
       Email = row["Email"]?.ToString() ?? "",
   Rol = row["Rol"]?.ToString() ?? "Usuario",
    Estado = row["Estado"]?.ToString() ?? "ACTIVO",
   Cedula = row["Cedula"]?.ToString() ?? "",
  Telefono = row["Telefono"]?.ToString() ?? "",
    Direccion = row["Direccion"]?.ToString() ?? "",
           FechaCreacion = row["FechaCreacion"] != DBNull.Value ? Convert.ToDateTime(row["FechaCreacion"]) : null,
   UltimaConexion = row["UltimaConexion"] != DBNull.Value ? Convert.ToDateTime(row["UltimaConexion"]) : null
            };
        }

        private Protos.Usuario MapearUsuarioProto(Models.Usuario usuario)
{
 // Separar nombre y apellido si es posible
          string nombre = usuario.Nombre;
        string apellido = "";
         
        if (!string.IsNullOrEmpty(usuario.Nombre))
     {
     var partes = usuario.Nombre.Split(' ', 2);
    nombre = partes[0];
    apellido = partes.Length > 1 ? partes[1] : "";
   }

  return new Protos.Usuario
            {
        IdUsuario = usuario.IdUsuario,
    Nombre = nombre,
Apellido = apellido,
    Email = usuario.Email,
     Cedula = usuario.Cedula,
       Telefono = usuario.Telefono,
       Direccion = usuario.Direccion ?? "",
       Rol = usuario.Rol,
    Activo = usuario.Estado == "ACTIVO",
        FechaCreacion = usuario.FechaCreacion?.ToString("yyyy-MM-dd") ?? "",
  UltimaConexion = usuario.UltimaConexion?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""
  };
        }
    }
}
