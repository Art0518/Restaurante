using System;
using System.Data;
using System.Data.SqlClient;
using SeguridadService.Models;

namespace SeguridadService.Data
{
    public class UsuarioDAO
    {
        private readonly string _connectionString;

        public UsuarioDAO(string connectionString)
        {
  _connectionString = connectionString;
        }

        // Login de usuario
        public DataTable Login(string email, string contrasena)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
  SqlCommand cmd = new SqlCommand("seguridad.sp_login_usuario", cn);
   cmd.CommandType = CommandType.StoredProcedure;
   cmd.Parameters.AddWithValue("@Email", email);
 cmd.Parameters.AddWithValue("@Contrasena", contrasena);
    
    SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
da.Fill(dt);
        return dt;
   }
        }

        // Registrar nuevo usuario
        public void Registrar(Usuario u, string contrasena)
  {
      using (SqlConnection cn = new SqlConnection(_connectionString))
        {
  string query = @"
    INSERT INTO seguridad.Usuario 
       (Nombre, Email, Contrasena, Telefono, Cedula, Direccion, Rol, Estado)
    VALUES (@Nombre, @Email, @Contrasena, @Telefono, @Cedula, @Direccion, @Rol, @Estado)";

      SqlCommand cmd = new SqlCommand(query, cn);
          cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 30; // 30 segundos de timeout
      cmd.Parameters.AddWithValue("@Nombre", u.Nombre ?? "");
        cmd.Parameters.AddWithValue("@Email", u.Email);
            cmd.Parameters.AddWithValue("@Contrasena", contrasena);
      cmd.Parameters.AddWithValue("@Telefono", u.Telefono ?? "");
             cmd.Parameters.AddWithValue("@Cedula", u.Cedula ?? "");
                cmd.Parameters.AddWithValue("@Direccion", u.Direccion ?? "");
    cmd.Parameters.AddWithValue("@Rol", u.Rol ?? "Usuario");
     cmd.Parameters.AddWithValue("@Estado", "ACTIVO");

         try
       {
           cn.Open();
   cmd.ExecuteNonQuery();
           }
         catch (SqlException ex)
      {
      // Manejar errores específicos de SQL
          if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate") || ex.Number == 2627 || ex.Number == 2601)
     {
        if (ex.Message.ToLower().Contains("email") || ex.Message.Contains("UQ_Usuario_Email"))
        {
            throw new Exception("Ya existe un usuario con este correo electrónico.");
            }
    else if (ex.Message.ToLower().Contains("cedula") || ex.Message.Contains("UQ_Usuario_Cedula"))
 {
         throw new Exception("Ya existe un usuario con esta cédula.");
  }
     else
 {
     throw new Exception("Ya existe un usuario con estos datos.");
           }
}
         throw; // Re-lanzar otras excepciones SQL
}
            }
        }

        // Listar usuarios
        public DataTable Listar(string rol = null, string estado = null)
      {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
SqlCommand cmd = new SqlCommand("seguridad.sp_listar_usuarios", cn);
                cmd.CommandType = CommandType.StoredProcedure;
           cmd.Parameters.AddWithValue("@Rol", (object)rol ?? DBNull.Value);
     cmd.Parameters.AddWithValue("@Estado", (object)estado ?? DBNull.Value);
  
       SqlDataAdapter da = new SqlDataAdapter(cmd);
      DataTable dt = new DataTable();
     da.Fill(dt);
           return dt;
            }
     }

        // Listar usuarios con paginación
  public (DataTable datos, int totalRegistros) Listar(string rol = null, string estado = null, int pagina = 1, int tamanoPagina = 50)
        {
       using (SqlConnection cn = new SqlConnection(_connectionString))
    {
            SqlCommand cmd = new SqlCommand("seguridad.sp_listar_usuarios", cn);
     cmd.CommandType = CommandType.StoredProcedure;
      cmd.Parameters.AddWithValue("@Rol", (object)rol ?? DBNull.Value);
    cmd.Parameters.AddWithValue("@Estado", (object)estado ?? DBNull.Value);
    cmd.Parameters.AddWithValue("@Pagina", pagina);
    cmd.Parameters.AddWithValue("@TamanoPagina", tamanoPagina);
  
                SqlDataAdapter da = new SqlDataAdapter(cmd);
          DataSet ds = new DataSet();
      da.Fill(ds);
       
          // El primer resultado es el total de registros
      int totalRegistros = 0;
    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
      {
          totalRegistros = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalRegistros"]);
                }
       
    // El segundo resultado son los datos paginados
 DataTable dt = ds.Tables.Count > 1 ? ds.Tables[1] : new DataTable();
            
                return (dt, totalRegistros);
   }
      }

        // Obtener usuario por ID
     public DataTable ObtenerPorId(int idUsuario)
    {
 using (SqlConnection cn = new SqlConnection(_connectionString))
            {
SqlCommand cmd = new SqlCommand("SELECT IdUsuario, Nombre, Email, Cedula, Rol, Estado, Telefono, Direccion FROM seguridad.Usuario WHERE IdUsuario = @IdUsuario", cn);
         cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
      
    SqlDataAdapter da = new SqlDataAdapter(cmd);
  DataTable dt = new DataTable();
    da.Fill(dt);
return dt;
       }
     }

        // Actualizar usuario
        public void Actualizar(Usuario u)
  {
     using (SqlConnection cn = new SqlConnection(_connectionString))
            {
      SqlCommand cmd = new SqlCommand("seguridad.sp_actualizar_usuario", cn);
     cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@IdUsuario", u.IdUsuario);
       cmd.Parameters.AddWithValue("@Nombre", (object)u.Nombre ?? DBNull.Value);
      cmd.Parameters.AddWithValue("@Email", (object)u.Email ?? DBNull.Value);
    cmd.Parameters.AddWithValue("@Telefono", (object)u.Telefono ?? DBNull.Value);
      cmd.Parameters.AddWithValue("@Direccion", (object)u.Direccion ?? DBNull.Value);
     cmd.Parameters.AddWithValue("@Cedula", (object)u.Cedula ?? DBNull.Value);
             cmd.Parameters.AddWithValue("@Rol", (object)u.Rol ?? DBNull.Value);
      cmd.Parameters.AddWithValue("@Estado", (object)u.Estado ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Contrasena", (object)u.Contrasena ?? DBNull.Value);

      // Capturar mensajes del stored procedure
         string mensajes = "";
            cn.InfoMessage += (sender, e) => { mensajes += e.Message; };

 cn.Open();

          try
         {
      cmd.ExecuteNonQuery();

        // Si hay mensajes del SP y contiene errores, lanzar excepción
        if (!string.IsNullOrEmpty(mensajes))
     {
       if (mensajes.Contains("correo electrónico ya está en uso"))
         {
    throw new Exception("Ya existe otro usuario con este correo electrónico.");
              }
 else if (mensajes.Contains("cédula ya está en uso"))
          {
       throw new Exception("Ya existe otro usuario con esta cédula.");
              }
      }
           }
    catch (SqlException ex)
     {
          // Manejar errores específicos de SQL
      if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate"))
             {
if (ex.Message.ToLower().Contains("email"))
   {
       throw new Exception("Ya existe otro usuario con este correo electrónico.");
          }
      else if (ex.Message.ToLower().Contains("cedula"))
         {
      throw new Exception("Ya existe otro usuario con esta cédula.");
       }
        else
    {
             throw new Exception("Ya existe otro usuario con estos datos.");
 }
         }
                 throw; // Re-lanzar otras excepciones SQL
        }
         }
        }

        // Cambiar estado del usuario
        public void CambiarEstado(int idUsuario, string nuevoEstado)
        {
        using (SqlConnection cn = new SqlConnection(_connectionString))
 {
         SqlCommand cmd = new SqlCommand("seguridad.sp_cambiar_estado_usuario", cn);
        cmd.CommandType = CommandType.StoredProcedure;
              cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
          cmd.Parameters.AddWithValue("@NuevoEstado", nuevoEstado);
                cn.Open();
              cmd.ExecuteNonQuery();
            }
        }

   // Cambiar contraseña
  public void CambiarContrasena(int idUsuario, string nuevaContrasena)
        {
     using (SqlConnection cn = new SqlConnection(_connectionString))
          {
        SqlCommand cmd = new SqlCommand("UPDATE seguridad.Usuario SET Contrasena = @Contrasena WHERE IdUsuario = @IdUsuario", cn);
    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
           cmd.Parameters.AddWithValue("@Contrasena", nuevaContrasena);
      cn.Open();
   cmd.ExecuteNonQuery();
   }
        }

        // Actualizar última conexión (no disponible en Monster)
        public void ActualizarUltimaConexion(int idUsuario)
        {
 // La columna UltimaConexion no existe en la base de datos de Monster
    // Este método se mantiene vacío para compatibilidad
    }

        // Eliminar usuario
 public void Eliminar(int idUsuario)
        {
     using (SqlConnection cn = new SqlConnection(_connectionString))
  {
         SqlCommand cmd = new SqlCommand("DELETE FROM seguridad.Usuario WHERE IdUsuario = @IdUsuario", cn);
       cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
       cn.Open();
     cmd.ExecuteNonQuery();
        }
        }
    }
}
