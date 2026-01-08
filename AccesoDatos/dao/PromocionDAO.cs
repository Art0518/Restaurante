using System;
using System.Data;
using System.Data.SqlClient;
using AccesoDatos.Conexion;

namespace AccesoDatos.DAO
{
    public class PromocionDAO
    {
        private readonly ConexionSQL conexion = new ConexionSQL();

        // ============================================================
        // 📋 LISTAR TODAS LAS PROMOCIONES
        // ============================================================
        public DataTable ListarPromociones()
        {
            try
            {
                using (SqlConnection cn = conexion.CrearConexion())
                {
                    cn.Open(); // ⭐ ABRIR CONEXIÓN
                    SqlCommand cmd = new SqlCommand("sp_listar_promociones", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar promociones: " + ex.Message);
            }
        }

        // ============================================================
        // 🎯 LISTAR PROMOCIONES ACTIVAS
        // ============================================================
        public DataTable ListarPromocionesActivas(DateTime? fechaConsulta = null)
        {
            try
            {
                using (SqlConnection cn = conexion.CrearConexion())
                {
                    cn.Open(); // ⭐ ABRIR CONEXIÓN
                    SqlCommand cmd = new SqlCommand("sp_listar_promociones_activas", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (fechaConsulta.HasValue)
                        cmd.Parameters.AddWithValue("@FechaConsulta", fechaConsulta.Value.Date);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar promociones activas: " + ex.Message);
            }
        }

        // ============================================================
        // 🎯 LISTAR PROMOCIONES VÁLIDAS PARA CARRITO DE UN USUARIO
        // ============================================================
        public DataTable ListarPromocionesValidasParaCarrito(int idUsuario)
        {
            try
            {
                using (SqlConnection cn = conexion.CrearConexion())
                {
                    cn.Open(); // ⭐ ABRIR CONEXIÓN
                    SqlCommand cmd = new SqlCommand("sp_listar_promociones_validas_carrito", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar promociones válidas para carrito: " + ex.Message);
            }
        }

        // ============================================================
        // ✅ GESTIONAR PROMOCIÓN (CREAR/ACTUALIZAR)
        // ============================================================
        public string GestionarPromocion(int idPromocion, string nombre, decimal descuento,
        DateTime fechaInicio, DateTime fechaFin, string estado)
        {
            try
            {
                using (SqlConnection cn = conexion.CrearConexion())
                {
                    cn.Open(); // ⭐ ABRIR CONEXIÓN
                    SqlCommand cmd = new SqlCommand("sp_gestionar_promocion", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdPromocion", idPromocion);
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@Descuento", descuento);
                    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin);
                    cmd.Parameters.AddWithValue("@Estado", estado);

                    // Ejecutar y obtener resultado
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        var row = dt.Rows[0];
                        if (row["Estado"].ToString() == "ERROR")
                        {
                            throw new Exception(row["Mensaje"].ToString());
                        }
                        return row["Mensaje"].ToString();
                    }

                    return "Operación completada exitosamente";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al gestionar promoción: " + ex.Message);
            }
        }

        // ============================================================
        // 🗑️ ELIMINAR PROMOCIÓN
        // ============================================================
        public string EliminarPromocion(int idPromocion)
        {
            try
            {
                using (SqlConnection cn = conexion.CrearConexion())
                {
                    cn.Open(); // ⭐ ABRIR CONEXIÓN
                    SqlCommand cmd = new SqlCommand("sp_eliminar_promocion", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdPromocion", idPromocion);

                    // Ejecutar y obtener resultado
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        var row = dt.Rows[0];
                        if (row["Estado"].ToString() == "ERROR")
                        {
                            throw new Exception(row["Mensaje"].ToString());
                        }
                        return row["Mensaje"].ToString();
                    }

                    return "Promoción eliminada exitosamente";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar promoción: " + ex.Message);
            }
        }
    }
}
