using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using FacturacionService.Models;

namespace FacturacionService.Data
{
    public class DetalleFacturaDAO
    {
      private readonly string _connectionString;

        public DetalleFacturaDAO(string connectionString)
        {
            _connectionString = connectionString;
        }

    // ? Listar detalles de una factura usando query SQL directa
        public DataTable ListarDetallesPorFactura(int idFactura)
        {
     using (SqlConnection cn = new SqlConnection(_connectionString))
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
        FROM facturacion.DetalleFactura df
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

    // ? Insertar detalle de factura usando query SQL directa
        public void InsertarDetalle(int idFactura, int idReserva, string descripcion, int cantidad, decimal precioUnitario)
{
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                string query = @"
         INSERT INTO facturacion.DetalleFactura (IdFactura, IdReserva, Descripcion, Cantidad, PrecioUnitario, Subtotal)
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

        // ? NUEVO: Actualizar detalle de factura
        public void ActualizarDetalle(int idDetalle, string descripcion, int cantidad, decimal precioUnitario)
        {
    using (SqlConnection cn = new SqlConnection(_connectionString))
      {
            string query = @"
         UPDATE facturacion.DetalleFactura 
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

        // ? NUEVO: Eliminar detalle de factura
        public void EliminarDetalle(int idDetalle)
        {
        using (SqlConnection cn = new SqlConnection(_connectionString))
      {
      string query = "DELETE FROM facturacion.DetalleFactura WHERE IdDetalle = @IdDetalle";

        SqlCommand cmd = new SqlCommand(query, cn);
    cmd.Parameters.AddWithValue("@IdDetalle", idDetalle);

       cn.Open();
           cmd.ExecuteNonQuery();
            }
        }

        // ? NUEVO: Obtener detalle específico por ID
        public DataTable ObtenerDetallePorId(int idDetalle)
        {
        using (SqlConnection cn = new SqlConnection(_connectionString))
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
         FROM facturacion.DetalleFactura df
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

        // ? NUEVO: Calcular subtotal de una factura desde sus detalles
        public decimal CalcularSubtotalFactura(int idFactura)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
    {
         string query = @"
 SELECT ISNULL(SUM(Subtotal), 0) 
        FROM facturacion.DetalleFactura 
       WHERE IdFactura = @IdFactura";

         SqlCommand cmd = new SqlCommand(query, cn);
       cmd.Parameters.AddWithValue("@IdFactura", idFactura);
     cn.Open();

        object result = cmd.ExecuteScalar();
    return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
            }
        }

        // ? NUEVO: Contar detalles de una factura
        public int ContarDetallesFactura(int idFactura)
 {
        using (SqlConnection cn = new SqlConnection(_connectionString))
      {
        string query = "SELECT COUNT(*) FROM facturacion.DetalleFactura WHERE IdFactura = @IdFactura";

        SqlCommand cmd = new SqlCommand(query, cn);
     cmd.Parameters.AddWithValue("@IdFactura", idFactura);
       cn.Open();

                return (int)cmd.ExecuteScalar();
 }
        }

     // ? Métodos legacy para compatibilidad con Entity Framework

    // Crear detalle de factura
        public DetalleFactura CrearDetalleFactura(DetalleFactura detalle)
        {
using (SqlConnection conn = new SqlConnection(_connectionString))
            {
     conn.Open();
      string query = @"
        INSERT INTO facturacion.DetalleFactura 
          (IdFactura, IdReserva, Descripcion, Cantidad, PrecioUnitario, Subtotal)
   VALUES (@IdFactura, @IdReserva, @Descripcion, @Cantidad, @PrecioUnitario, @Subtotal);
            SELECT SCOPE_IDENTITY();";

      using (SqlCommand cmd = new SqlCommand(query, conn))
            {
     cmd.Parameters.AddWithValue("@IdFactura", detalle.IdFactura);
     cmd.Parameters.AddWithValue("@IdReserva", detalle.IdReserva ?? (object)DBNull.Value);
   cmd.Parameters.AddWithValue("@Descripcion", detalle.Descripcion ?? (object)DBNull.Value);
       cmd.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
            cmd.Parameters.AddWithValue("@PrecioUnitario", detalle.PrecioUnitario);
   cmd.Parameters.AddWithValue("@Subtotal", detalle.Subtotal);

             detalle.IdDetalleFactura = (int)(decimal)cmd.ExecuteScalar();
             }
            }
         return detalle;
        }

        // Obtener detalles de una factura (retorna lista de entidades)
        public List<DetalleFactura> ObtenerDetallesFactura(int idFactura)
        {
  List<DetalleFactura> detalles = new List<DetalleFactura>();
  using (SqlConnection conn = new SqlConnection(_connectionString))
     {
        conn.Open();
                string query = "SELECT * FROM facturacion.DetalleFactura WHERE IdFactura = @IdFactura";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
cmd.Parameters.AddWithValue("@IdFactura", idFactura);
         using (SqlDataReader reader = cmd.ExecuteReader())
  {
       while (reader.Read())
           {
     detalles.Add(MapearDetalleFactura(reader));
      }
           }
}
   }
   return detalles;
        }

// Eliminar detalle de factura
public bool EliminarDetalleFactura(int idDetalleFactura)
        {
        using (SqlConnection conn = new SqlConnection(_connectionString))
            {
      conn.Open();
 string query = "DELETE FROM facturacion.DetalleFactura WHERE IdDetalle = @IdDetalle";

 using (SqlCommand cmd = new SqlCommand(query, conn))
   {
        cmd.Parameters.AddWithValue("@IdDetalle", idDetalleFactura);
  return cmd.ExecuteNonQuery() > 0;
       }
     }
        }

        private DetalleFactura MapearDetalleFactura(SqlDataReader reader)
        {
   return new DetalleFactura
     {
     IdDetalleFactura = reader["IdDetalle"] != DBNull.Value ? (int)reader["IdDetalle"] : 0,
    IdFactura = reader["IdFactura"] != DBNull.Value ? (int)reader["IdFactura"] : 0,
 IdReserva = reader["IdReserva"] != DBNull.Value ? (int)reader["IdReserva"] : (int?)null,
Descripcion = reader["Descripcion"] != DBNull.Value ? (string)reader["Descripcion"] : null,
 Cantidad = reader["Cantidad"] != DBNull.Value ? (int)reader["Cantidad"] : 0,
   PrecioUnitario = reader["PrecioUnitario"] != DBNull.Value ? (decimal)reader["PrecioUnitario"] : 0m,
  Subtotal = reader["Subtotal"] != DBNull.Value ? (decimal)reader["Subtotal"] : 0m
};
        }
    }
}
