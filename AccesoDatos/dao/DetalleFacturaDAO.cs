using System;
using System.Data;
using System.Data.SqlClient;
using AccesoDatos.Conexion;

namespace AccesoDatos.DAO
{
    public class DetalleFacturaDAO
    {
        private readonly ConexionSQL conexion = new ConexionSQL();

        // ✅ Listar detalles de una factura usando query SQL directa
        public DataTable ListarDetallesPorFactura(int idFactura)
        {
            using (SqlConnection cn = conexion.CrearConexion())
            {
                string query = @"
                    SELECT 
df.IdDetalle,
df.IdFactura,
     df.IdReserva,
        df.Descripcion,
   df.Cantidad,
      df.PrecioUnitario,
            df.Subtotal,
             r.Fecha AS FechaReserva,
             r.Hora AS HoraReserva,
  m.NumeroMesa,
      r.NumeroPersonas,
    r.MetodoPago AS MetodoPagoReserva,
   r.Total AS TotalReserva
  FROM DetalleFactura df
    LEFT JOIN Reserva r ON df.IdReserva = r.IdReserva
     LEFT JOIN Mesa m ON r.IdMesa = m.IdMesa
        WHERE df.IdFactura = @IdFactura
       ORDER BY df.IdDetalle";

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@IdFactura", idFactura);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // ✅ Insertar detalle de factura usando query SQL directa
        public void InsertarDetalle(int idFactura, int idReserva, string descripcion, int cantidad, decimal precioUnitario)
        {
            using (SqlConnection cn = conexion.CrearConexion())
            {
                string query = @"
         INSERT INTO DetalleFactura (IdFactura, IdReserva, Descripcion, Cantidad, PrecioUnitario, Subtotal)
         VALUES (@IdFactura, @IdReserva, @Descripcion, @Cantidad, @PrecioUnitario, @Subtotal)";

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@IdFactura", idFactura);
                cmd.Parameters.AddWithValue("@IdReserva", idReserva);
                cmd.Parameters.AddWithValue("@Descripcion", descripcion ?? "");
                cmd.Parameters.AddWithValue("@Cantidad", cantidad);
                cmd.Parameters.AddWithValue("@PrecioUnitario", precioUnitario);
                cmd.Parameters.AddWithValue("@Subtotal", cantidad * precioUnitario);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ✅ NUEVO: Actualizar detalle de factura
        public void ActualizarDetalle(int idDetalle, string descripcion, int cantidad, decimal precioUnitario)
        {
            using (SqlConnection cn = conexion.CrearConexion())
            {
                string query = @"
         UPDATE DetalleFactura 
    SET Descripcion = @Descripcion,
            Cantidad = @Cantidad,
    PrecioUnitario = @PrecioUnitario,
     Subtotal = @Cantidad * @PrecioUnitario
WHERE IdDetalle = @IdDetalle";

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@IdDetalle", idDetalle);
                cmd.Parameters.AddWithValue("@Descripcion", descripcion ?? "");
                cmd.Parameters.AddWithValue("@Cantidad", cantidad);
                cmd.Parameters.AddWithValue("@PrecioUnitario", precioUnitario);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ✅ NUEVO: Eliminar detalle de factura
        public void EliminarDetalle(int idDetalle)
        {
            using (SqlConnection cn = conexion.CrearConexion())
            {
                string query = "DELETE FROM DetalleFactura WHERE IdDetalle = @IdDetalle";

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@IdDetalle", idDetalle);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ✅ NUEVO: Obtener detalle específico por ID
        public DataTable ObtenerDetallePorId(int idDetalle)
        {
            using (SqlConnection cn = conexion.CrearConexion())
            {
                string query = @"
        SELECT 
               df.IdDetalle,
                 df.IdFactura,
           df.IdReserva,
               df.Descripcion,
             df.Cantidad,
     df.PrecioUnitario,
    df.Subtotal,
     r.Fecha AS FechaReserva,
 r.Hora AS HoraReserva,
      m.NumeroMesa,
   r.NumeroPersonas
      FROM DetalleFactura df
       LEFT JOIN Reserva r ON df.IdReserva = r.IdReserva
     LEFT JOIN Mesa m ON r.IdMesa = m.IdMesa
     WHERE df.IdDetalle = @IdDetalle";

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@IdDetalle", idDetalle);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // ✅ NUEVO: Calcular subtotal de una factura desde sus detalles
        public decimal CalcularSubtotalFactura(int idFactura)
        {
            using (SqlConnection cn = conexion.CrearConexion())
            {
                string query = @"
      SELECT ISNULL(SUM(Subtotal), 0) 
          FROM DetalleFactura 
WHERE IdFactura = @IdFactura";

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@IdFactura", idFactura);
                cn.Open();

                object result = cmd.ExecuteScalar();
                return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
            }
        }

        // ✅ NUEVO: Contar detalles de una factura
        public int ContarDetallesFactura(int idFactura)
        {
            using (SqlConnection cn = conexion.CrearConexion())
            {
                string query = "SELECT COUNT(*) FROM DetalleFactura WHERE IdFactura = @IdFactura";

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@IdFactura", idFactura);
                cn.Open();

                return (int)cmd.ExecuteScalar();
            }
        }
    }
}
