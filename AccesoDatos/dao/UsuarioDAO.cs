using System;
using System.Data;
using System.Data.SqlClient;
using AccesoDatos.Conexion;
using GDatos.Entidades;

namespace AccesoDatos.DAO
{
    public class UsuarioDAO
    {
        private readonly Conexion.ConexionSQL conexion = new Conexion.ConexionSQL();

        public DataTable Login(string email, string contrasena)
        {
     using (SqlConnection cn = conexion.CrearConexion())
       {
            SqlCommand cmd = new SqlCommand("sp_login_usuario", cn);
            cmd.CommandType = CommandType.StoredProcedure;
         cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Contrasena", contrasena);
    SqlDataAdapter da = new SqlDataAdapter(cmd);
           DataTable dt = new DataTable();
         da.Fill(dt);
      return dt;
          }
        }

        public void Registrar(Usuario u)
        {
  using (SqlConnection cn = conexion.CrearConexion())
            {
    SqlCommand cmd = new SqlCommand("sp_registrar_usuario_2", cn);
cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Nombre", u.Nombre);
                cmd.Parameters.AddWithValue("@Email", u.Email);
       cmd.Parameters.AddWithValue("@Contrasena", u.Contrasena);
           cmd.Parameters.AddWithValue("@Telefono", u.Telefono);
        cmd.Parameters.AddWithValue("@Direccion", u.Direccion);
        cmd.Parameters.AddWithValue("@Cedula", u.Cedula);  // ?? NUEVO PARÁMETRO
            cmd.Parameters.AddWithValue("@Rol", u.Rol);
                cmd.Parameters.AddWithValue("@Estado", u.Estado);

        // ?? CAPTURAR MENSAJES DEL STORED PROCEDURE
        string mensajes = "";
    cn.InfoMessage += (sender, e) => { mensajes += e.Message; };
    
         cn.Open();
       
        try
       {
          cmd.ExecuteNonQuery();
         
           // Si hay mensajes del SP y contiene errores, lanzar excepción
      if (!string.IsNullOrEmpty(mensajes))
    {
         if (mensajes.Contains("Ya existe un usuario con este correo"))
 {
 throw new Exception("Ya existe un usuario con este correo electrónico.");
}
              else if (mensajes.Contains("Ya existe un usuario con esta cédula"))
    {
          throw new Exception("Ya existe un usuario con esta cédula.");
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
               throw new Exception("Ya existe un usuario con este correo electrónico.");
 }
            else if (ex.Message.ToLower().Contains("cedula"))
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

        public DataTable Listar(string rol = null, string estado = null)
        {
            using (SqlConnection cn = conexion.CrearConexion())
         {
       SqlCommand cmd = new SqlCommand("sp_listar_usuarios", cn);
    cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Rol", (object)rol ?? DBNull.Value);
       cmd.Parameters.AddWithValue("@Estado", (object)estado ?? DBNull.Value);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
         DataTable dt = new DataTable();
            da.Fill(dt);
    return dt;
            }
        }

   public void Actualizar(Usuario u)
      {
            using (SqlConnection cn = conexion.CrearConexion())
    {
         SqlCommand cmd = new SqlCommand("sp_actualizar_usuario", cn);
             cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@IdUsuario", u.IdUsuario);
     cmd.Parameters.AddWithValue("@Nombre", (object)u.Nombre ?? DBNull.Value);
          cmd.Parameters.AddWithValue("@Email", (object)u.Email ?? DBNull.Value);
       cmd.Parameters.AddWithValue("@Telefono", (object)u.Telefono ?? DBNull.Value);
         cmd.Parameters.AddWithValue("@Direccion", (object)u.Direccion ?? DBNull.Value);
           cmd.Parameters.AddWithValue("@Cedula", (object)u.Cedula ?? DBNull.Value);  // ?? NUEVO
                cmd.Parameters.AddWithValue("@Rol", (object)u.Rol ?? DBNull.Value);
         cmd.Parameters.AddWithValue("@Estado", (object)u.Estado ?? DBNull.Value);
              cmd.Parameters.AddWithValue("@Contrasena", (object)u.Contrasena ?? DBNull.Value);
       
            // ?? CAPTURAR MENSAJES DEL STORED PROCEDURE
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

        public void CambiarEstado(int idUsuario, string nuevoEstado)
        {
      using (SqlConnection cn = conexion.CrearConexion())
            {
                SqlCommand cmd = new SqlCommand("sp_cambiar_estado_usuario", cn);
       cmd.CommandType = CommandType.StoredProcedure;
       cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                cmd.Parameters.AddWithValue("@NuevoEstado", nuevoEstado);
    cn.Open();
 cmd.ExecuteNonQuery();
            }
        }
  }
}