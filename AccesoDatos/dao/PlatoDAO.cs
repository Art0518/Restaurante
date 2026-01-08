using System;
using System.Data;
using System.Data.SqlClient;
using AccesoDatos.Conexion;

namespace AccesoDatos.DAO
{
    public class PlatoDAO
    {
        private readonly ConexionSQL conexion = new ConexionSQL();

        public DataTable ListarPlatos()
        {
            using (SqlConnection cn = conexion.CrearConexion())
            {
                SqlCommand cmd = new SqlCommand("sp_listar_platos", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                
                // Parámetros opcionales - pasar NULL para obtener todos los platos
                cmd.Parameters.AddWithValue("@IdRestaurante", DBNull.Value);
                cmd.Parameters.AddWithValue("@Categoria", DBNull.Value);
                cmd.Parameters.AddWithValue("@TipoComida", DBNull.Value);
                    
                DataTable dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
                return dt;
            }
        }

        public void GestionarPlato(string operacion, int idPlato, int idRestaurante, string nombre, string categoria,
                             string tipoComida, decimal precio, string descripcion, string imagenUrl)
        {
            using (SqlConnection cn = conexion.CrearConexion())
            {
                SqlCommand cmd = new SqlCommand("sp_gestionar_plato", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Operacion", operacion);
                cmd.Parameters.AddWithValue("@IdPlato", idPlato);
                cmd.Parameters.AddWithValue("@IdRestaurante", idRestaurante);
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@Categoria", categoria);
                cmd.Parameters.AddWithValue("@TipoComida", tipoComida);
                cmd.Parameters.AddWithValue("@Precio", precio);
                cmd.Parameters.AddWithValue("@Descripcion", descripcion);
                cmd.Parameters.AddWithValue("@ImagenURL", imagenUrl);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
