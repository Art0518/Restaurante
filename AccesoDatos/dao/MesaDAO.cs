using System;
using System.Data;
using System.Data.SqlClient;
using AccesoDatos.Conexion;

namespace AccesoDatos.DAO
{
    public class MesaDAO
    {
        private readonly ConexionSQL conexion = new ConexionSQL();

        // =========================================================
        // LISTAR TODAS LAS MESAS (usa SP existente)
        // =========================================================
        public DataTable ListarMesas()
        {
            using (SqlConnection cn = conexion.CrearConexion())
            {
                SqlCommand cmd = new SqlCommand("sp_listar_mesas", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }

        // =========================================================
        // NUEVO: MESAS DISPONIBLES PARA RESERVAS.JS
        // =========================================================
        public DataTable MesasDisponibles(string zona, int personas)
        {
            using (SqlConnection cn = conexion.CrearConexion())
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT TOP 1 *
                    FROM reservas.Mesa
                    WHERE Estado = 'DISPONIBLE'
                    AND TipoMesa = @zona
                    AND Capacidad >= @personas
                    ORDER BY Capacidad ASC", cn);

                cmd.Parameters.AddWithValue("@zona", zona);
                cmd.Parameters.AddWithValue("@personas", personas);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }

        public DataTable EjecutarConsulta(string sql)
        {
            using (SqlConnection cn = conexion.CrearConexion())
            {
                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(sql, cn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }




        // =========================================================
        // ACTUALIZAR ESTADO (usa tu SP existente)
        // =========================================================
        public void ActualizarEstado(int idMesa, string estado)
        {
            using (SqlConnection cn = conexion.CrearConexion())
            {
                SqlCommand cmd = new SqlCommand("sp_actualizar_estado_mesa", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdMesa", idMesa);
                cmd.Parameters.AddWithValue("@NuevoEstado", estado);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // =========================================================
        // REGISTRAR / MODIFICAR MESA (usa tu SP existente)
        // =========================================================
        public void GestionarMesa(string operacion, int? idMesa, int idRestaurante, int numeroMesa, string tipoMesa, int capacidad, decimal? precio, string imagenURL, string estado)
        {
            using (SqlConnection cn = conexion.CrearConexion())
            {
                SqlCommand cmd = new SqlCommand("sp_gestionar_mesa", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Operacion", operacion);
                
                // Asegurar que cuando es INSERT, se pase NULL
                if (operacion.Equals("INSERT", StringComparison.OrdinalIgnoreCase) || !idMesa.HasValue || idMesa.Value == 0)
                {
                    cmd.Parameters.AddWithValue("@IdMesa", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IdMesa", idMesa.Value);
                }
                
                cmd.Parameters.AddWithValue("@IdRestaurante", idRestaurante);
                cmd.Parameters.AddWithValue("@NumeroMesa", numeroMesa);
                cmd.Parameters.AddWithValue("@TipoMesa", tipoMesa);
                cmd.Parameters.AddWithValue("@Capacidad", capacidad);
                cmd.Parameters.AddWithValue("@Precio", (object)precio ?? DBNull.Value); ;
                cmd.Parameters.AddWithValue("@ImagenURL", (object)imagenURL ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", (object)estado ?? DBNull.Value);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
