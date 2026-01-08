using SeguridadService.Data;
using SeguridadService.Models;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;

namespace SeguridadService.GraphQL
{
    public class SeguridadMutation
    {
 private readonly string _connectionString;

   public SeguridadMutation(IConfiguration configuration)
      {
     _connectionString = configuration.GetConnectionString("DefaultConnection");
 }

        public RegistroResponse RegistrarUsuario(RegistroUsuarioInput input)
     {
    try
      {
    var usuarioDAO = new UsuarioDAO(_connectionString);
       var usuario = new Usuario
       {
     Nombre = input.Nombre,
      Email = input.Email,
     Telefono = input.Telefono,
   Cedula = input.Cedula,
   Direccion = input.Direccion,
    Rol = input.Rol ?? "Cliente",
   Estado = "ACTIVO"
   };

    // Hashear la contraseña
     string contrasenaHash = BCrypt.Net.BCrypt.HashPassword(input.Contrasena);
      
  usuarioDAO.Registrar(usuario, contrasenaHash);
 
 return new RegistroResponse
  {
         Success = true,
     Message = "Usuario registrado exitosamente"
      };
 }
          catch (Exception ex)
{
 return new RegistroResponse
  {
          Success = false,
 Message = ex.Message
            };
   }
 }

        public LoginResponse Login(LoginInput input)
  {
   try
     {
      var usuarioDAO = new UsuarioDAO(_connectionString);
     var dt = usuarioDAO.Login(input.Email, input.Contrasena);
  
       if (dt.Rows.Count == 0)
     {
    return new LoginResponse
{
    Success = false,
          Message = "Credenciales inválidas"
      };
            }

        var row = dt.Rows[0];
     var success = row["Estado"].ToString() == "SUCCESS" || row["Estado"].ToString() == "EXITO";
  
    if (!success)
  {
      return new LoginResponse
   {
     Success = false,
   Message = row["Mensaje"].ToString()
         };
     }

  return new LoginResponse
    {
  Success = true,
     Message = "Login exitoso",
  Token = row["Token"]?.ToString() ?? "",
      Usuario = new Usuario
   {
   IdUsuario = Convert.ToInt32(row["IdUsuario"]),
    Nombre = row["Nombre"].ToString(),
      Email = row["Email"].ToString(),
    Rol = row["Rol"].ToString()
     }
   };
  }
      catch (Exception ex)
     {
 return new LoginResponse
    {
  Success = false,
   Message = "Error en el login: " + ex.Message
  };
  }
  }

public bool ActualizarUsuario(int id, ActualizarUsuarioInput input)
{
            try
 {
  var usuarioDAO = new UsuarioDAO(_connectionString);
       var usuario = new Usuario
  {
  IdUsuario = id,
 Nombre = input.Nombre,
     Email = input.Email,
  Telefono = input.Telefono,
   Rol = input.Rol,
     Estado = input.Estado ?? "ACTIVO"
   };
            
    usuarioDAO.Actualizar(usuario);
 return true;
 }
          catch
  {
   return false;
 }
}

      public bool CambiarEstadoUsuario(int id, string estado)
        {
   try
  {
   var usuarioDAO = new UsuarioDAO(_connectionString);
 usuarioDAO.CambiarEstado(id, estado);
     return true;
     }
      catch
     {
    return false;
  }
  }

  public bool CambiarContrasena(CambiarContrasenaInput input)
        {
try
  {
  var usuarioDAO = new UsuarioDAO(_connectionString);
        
        // Verificar contraseña actual con login
        var dtLogin = usuarioDAO.ObtenerPorId(input.IdUsuario);
     if (dtLogin.Rows.Count == 0)
     return false;

     // Hashear nueva contraseña
string nuevaContrasenaHash = BCrypt.Net.BCrypt.HashPassword(input.NuevaContrasena);
            usuarioDAO.CambiarContrasena(input.IdUsuario, nuevaContrasenaHash);
   return true;
        }
    catch
  {
  return false;
        }
  }
    }

    public class RegistroUsuarioInput
    {
        public string Nombre { get; set; }
  public string Email { get; set; }
 public string Contrasena { get; set; }
        public string Telefono { get; set; }
   public string Cedula { get; set; }
  public string Direccion { get; set; }
  public string Rol { get; set; }
  }

    public class LoginInput
    {
   public string Email { get; set; }
   public string Contrasena { get; set; }
    }

    public class ActualizarUsuarioInput
    {
public string Nombre { get; set; }
        public string Email { get; set; }
     public string Telefono { get; set; }
 public string Rol { get; set; }
        public string Estado { get; set; }
    }

    public class CambiarContrasenaInput
    {
        public int IdUsuario { get; set; }
 public string ContrasenaActual { get; set; }
     public string NuevaContrasena { get; set; }
 }

    public class RegistroResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class LoginResponse
{
    public bool Success { get; set; }
 public string Message { get; set; }
        public string Token { get; set; }
        public Usuario Usuario { get; set; }
    }
}
