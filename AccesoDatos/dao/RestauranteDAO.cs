using System;
using System.Data;
using System.Data.SqlClient;
using AccesoDatos.Conexion;

namespace AccesoDatos.DAO
{
    public class RestauranteDAO
    {
        private readonly ConexionSQL conexion = new ConexionSQL();

        // ✅ Listar todos los restaurantes registrados
        public DataTable ListarRestaurantes()
        {
            using (SqlConnection cn = conexion.CrearConexion())
            {
                SqlCommand cmd = new SqlCommand("sp_listar_restaurantes", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // ✅ Obtener detalle de un restaurante por ID
        public DataTable DetalleRestaurante(int idRestaurante)
        {
            using (SqlConnection cn = conexion.CrearConexion())
            {
                SqlCommand cmd = new SqlCommand("sp_detalle_restaurante", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdRestaurante", idRestaurante);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // ✅ Registrar o actualizar restaurante
        public void GestionarRestaurante(int idRestaurante, string nombre, string ciudad, string direccion, string horario, string descripcion)
        {
            using (SqlConnection cn = conexion.CrearConexion())
            {
                SqlCommand cmd = new SqlCommand("sp_gestionar_restaurante", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdRestaurante", idRestaurante);
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@Ciudad", ciudad);
                cmd.Parameters.AddWithValue("@Direccion", direccion);
                cmd.Parameters.AddWithValue("@Horario", horario);
                cmd.Parameters.AddWithValue("@Descripcion", descripcion);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ✅ Eliminar restaurante (opcional si tu SP lo tiene)
        public void EliminarRestaurante(int idRestaurante)
        {
            using (SqlConnection cn = conexion.CrearConexion())
            {
                SqlCommand cmd = new SqlCommand("sp_eliminar_restaurante", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdRestaurante", idRestaurante);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
