using Grpc.Net.Client;
using SeguridadService.Protos;
using System;
using System.Threading.Tasks;

namespace GrpcClients.Clients
{
    public class SeguridadGrpcClient
    {
 private readonly GrpcChannel _channel;
 private readonly SeguridadGrpc.SeguridadGrpcClient _client;

      public SeguridadGrpcClient(string serviceUrl)
        {
         // Permitir llamadas HTTP/2 sin encriptación (para desarrollo)
 AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
             
     _channel = GrpcChannel.ForAddress(serviceUrl);
     _client = new SeguridadGrpc.SeguridadGrpcClient(_channel);
        }

        // Registrar usuario
      public async Task<RegistrarUsuarioResponse> RegistrarUsuarioAsync(string nombre, string apellido, string email, string telefono, string contrasena, string rol = "Usuario")
  {
 try
      {
 var request = new RegistrarUsuarioRequest
    {
Nombre = nombre,
Apellido = apellido,
           Email = email,
       Telefono = telefono,
            Contrasena = contrasena,
          Rol = rol
     };

   return await _client.RegistrarUsuarioAsync(request);
   }
      catch (Exception ex)
   {
        throw new Exception($"Error al registrar usuario: {ex.Message}", ex);
       }
  }

      // Login
      public async Task<LoginResponse> LoginAsync(string email, string contrasena)
     {
try
      {
   var request = new LoginRequest
       {
      Email = email,
Contrasena = contrasena
   };

   return await _client.LoginAsync(request);
 }
   catch (Exception ex)
       {
  throw new Exception($"Error en login: {ex.Message}", ex);
     }
        }

        // Obtener usuario
   public async Task<ObtenerUsuarioResponse> ObtenerUsuarioAsync(int idUsuario)
        {
try
            {
var request = new ObtenerUsuarioRequest { IdUsuario = idUsuario };
      return await _client.ObtenerUsuarioAsync(request);
      }
    catch (Exception ex)
        {
        throw new Exception($"Error al obtener usuario: {ex.Message}", ex);
  }
        }

  // Actualizar usuario
      public async Task<ActualizarUsuarioResponse> ActualizarUsuarioAsync(int idUsuario, string nombre, string apellido, string email, string telefono, string rol)
        {
try
    {
var request = new ActualizarUsuarioRequest
           {
  IdUsuario = idUsuario,
           Nombre = nombre,
         Apellido = apellido,
Email = email,
 Telefono = telefono,
     Rol = rol
       };

   return await _client.ActualizarUsuarioAsync(request);
   }
     catch (Exception ex)
           {
throw new Exception($"Error al actualizar usuario: {ex.Message}", ex);
    }
        }

 // Cambiar contraseña
      public async Task<CambiarContrasenaResponse> CambiarContrasenaAsync(int idUsuario, string contrasenaActual, string nuevaContrasena)
      {
try
     {
   var request = new CambiarContrasenaRequest
      {
IdUsuario = idUsuario,
       ContrasenaActual = contrasenaActual,
      NuevaContrasena = nuevaContrasena
  };

    return await _client.CambiarContrasenaAsync(request);
    }
  catch (Exception ex)
    {
   throw new Exception($"Error al cambiar contraseña: {ex.Message}", ex);
        }
}

// Validar token
 public async Task<ValidarTokenResponse> ValidarTokenAsync(string token)
        {
try
            {
  var request = new ValidarTokenRequest { Token = token };
          return await _client.ValidarTokenAsync(request);
           }
  catch (Exception ex)
 {
throw new Exception($"Error al validar token: {ex.Message}", ex);
          }
        }

       // Eliminar usuario
    public async Task<EliminarUsuarioResponse> EliminarUsuarioAsync(int idUsuario)
        {
try
  {
var request = new EliminarUsuarioRequest { IdUsuario = idUsuario };
      return await _client.EliminarUsuarioAsync(request);
 }
    catch (Exception ex)
  {
  throw new Exception($"Error al eliminar usuario: {ex.Message}", ex);
   }
 }

   // Listar usuarios
 public async Task<ListarUsuariosResponse> ListarUsuariosAsync(string rol = "")
        {
try
     {
   var request = new ListarUsuariosRequest { Rol = rol };
     return await _client.ListarUsuariosAsync(request);
 }
    catch (Exception ex)
      {
  throw new Exception($"Error al listar usuarios: {ex.Message}", ex);
  }
      }

  public void Dispose()
        {
 _channel?.Dispose();
      }
    }
}
