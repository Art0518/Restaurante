using System;
using System.Data;
using System.Data.SqlClient;
using AccesoDatos.Conexion;
using GDatos.Entidades;

namespace AccesoDatos.DAO
{
    public class ReservaDAO
    {
        private readonly ConexionSQL conexion = new ConexionSQL();

        // ============================================================
        // ✅ Crear nueva reserva (HOLD o CONFIRMADA)
        // ============================================================
        public DataTable CrearReserva(int idUsuario, int idMesa, DateTime fecha, string hora,
                                      int personas, string observaciones, string estado,
                                      string metodoPago)
        {
            using (SqlConnection cn = conexion.CrearConexion())
            {
                SqlCommand cmd = new SqlCommand("sp_crear_reserva", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                cmd.Parameters.AddWithValue("@IdMesa", idMesa);
                cmd.Parameters.AddWithValue("@Fecha", fecha);

                // 🔥 Hora en formato TIME --> HH:mm:ss
                TimeSpan horaTime = TimeSpan.Parse(hora);
                cmd.Parameters.AddWithValue("@Hora", horaTime);

                cmd.Parameters.AddWithValue("@NumeroPersonas", personas);
                cmd.Parameters.AddWithValue("@Observaciones", string.IsNullOrEmpty(observaciones) ? "" : observaciones);
                cmd.Parameters.AddWithValue("@Estado", estado);

                // ⭐ Método de pago, vacío cuando está en HOLD
                cmd.Parameters.AddWithValue("@MetodoPago", metodoPago ?? "");

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
        }

        // ============================================================
        // 🔍 Buscar una reserva por ID
        // ============================================================
        public DataTable BuscarReservaPorId(int idReserva)
        {
            try
            {
                using (SqlConnection cn = conexion.CrearConexion())
                {
                    SqlCommand cmd = new SqlCommand("sp_buscar_reserva_por_id", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdReserva", idReserva);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar la reserva por ID: " + ex.Message);
            }
        }

        // ============================================================
        // 📋 Listar todas las reservas
        // ============================================================
        public DataTable ListarReservas()
        {
            using (SqlConnection cn = conexion.CrearConexion())
            {
                SqlCommand cmd = new SqlCommand("sp_listar_reservas", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // ============================================================
        // 🔵 Actualizar estado de la reserva + método de pago
        // ============================================================
        public DataTable ActualizarEstado(int idReserva, string nuevoEstado, string metodoPago)
        {
            using (SqlConnection cn = conexion.CrearConexion())
            {
                SqlCommand cmd = new SqlCommand("sp_actualizar_estado_reserva", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdReserva", idReserva);
                cmd.Parameters.AddWithValue("@NuevoEstado", nuevoEstado);
                cmd.Parameters.AddWithValue("@MetodoPago", metodoPago);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // ============================================================
        // ✏ Editar reserva completa (sin estado)
        // ============================================================
        public string EditarReserva(Reserva r)
        {
            using (SqlConnection cn = conexion.CrearConexion())
            {
                SqlCommand cmd = new SqlCommand("sp_editar_reserva", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdReserva", r.IdReserva);
                cmd.Parameters.AddWithValue("@Fecha", r.Fecha);

                TimeSpan horaSolo = TimeSpan.Parse(r.Hora);
                cmd.Parameters.AddWithValue("@Hora", horaSolo);

                cmd.Parameters.AddWithValue("@NumeroPersonas", r.NumeroPersonas);
                cmd.Parameters.AddWithValue("@Observaciones", r.Observaciones ?? "");

                cn.Open();
                string resp = cmd.ExecuteScalar().ToString();
                cn.Close();

                return resp;
            }
        }

        // ============================================================
        // 📌 Información detallada de reserva
        // ============================================================
        public DataSet BuscarDatosReserva(int idReserva)
        {
            DataSet ds = new DataSet("DetalleReserva");

            using (SqlConnection cn = conexion.CrearConexion())
            {
                SqlCommand cmd = new SqlCommand("sp_detalle_reserva", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idReserva", idReserva);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds, "DetalleReserva");
            }

            return ds;
        }

        // ============================================================
        // 🛒 LISTAR CARRITO DE RESERVAS DEL USUARIO CON PROMOCIONES
        // ============================================================
        public DataSet ListarCarritoReservas(int idUsuario, int? promocionId = null)
        {
            try
            {
                using (SqlConnection cn = conexion.CrearConexion())
                {
                    SqlCommand cmd = new SqlCommand("sp_listar_carrito_reservas", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    if (promocionId.HasValue)
                        cmd.Parameters.AddWithValue("@PromocionId", promocionId.Value);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    return ds;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar carrito de reservas: " + ex.Message);
            }
        }

        // ============================================================
        // ❌ ELIMINAR RESERVA DEL CARRITO (DEFINITIVAMENTE)
        // ============================================================
        public DataTable EliminarReservaCarrito(int idUsuario, int idReserva)
        {
            try
            {
                using (SqlConnection cn = conexion.CrearConexion())
                {
                    SqlCommand cmd = new SqlCommand("sp_eliminar_reserva_carrito", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    cmd.Parameters.AddWithValue("@IdReserva", idReserva);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error eliminando reserva del carrito: " + ex.Message);
            }
        }

        // ============================================================
        // ✅ CONFIRMAR RESERVAS SELECTIVAS CON PROMOCIONES - SIN SP
      // ============================================================
  public DataTable ConfirmarReservasSelectivas(int idUsuario, string reservasIds, string metodoPago, int? promocionId = null)
        {
DataTable resultado = new DataTable();
   resultado.Columns.Add("Estado", typeof(string));
         resultado.Columns.Add("Mensaje", typeof(string));
   resultado.Columns.Add("ReservasConfirmadas", typeof(int));
   resultado.Columns.Add("IdFacturaAfectada", typeof(int));
         resultado.Columns.Add("MontoDescuento", typeof(decimal));  // ✅ CAMBIADO: MontoDescuento en lugar de PromocionAplicada
   resultado.Columns.Add("PorcentajeDescuento", typeof(decimal));  // ✅ AGREGADO: PorcentajeDescuento

    try
    {
          using (SqlConnection cn = conexion.CrearConexion())
       {
        cn.Open();
      SqlTransaction transaction = cn.BeginTransaction();

      try
       {
   // Obtener porcentaje de descuento si hay promoción
 decimal porcentajeDescuento = 0;
       if (promocionId.HasValue && promocionId.Value > 0)
           {
   string queryPromocion = "SELECT Descuento FROM menu.Promocion WHERE IdPromocion = @PromocionId";
      SqlCommand cmdPromo = new SqlCommand(queryPromocion, cn, transaction);
      cmdPromo.Parameters.AddWithValue("@PromocionId", promocionId.Value);
   object result = cmdPromo.ExecuteScalar();
       if (result != null && result != DBNull.Value)
   {
     porcentajeDescuento = Convert.ToDecimal(result);
    }
     }

          // Actualizar cada reserva
         string updateQuery = @"
       UPDATE reservas.Reserva
 SET Estado = 'CONFIRMADA',
          MetodoPago = @MetodoPago,
   MontoDescuento = CASE 
       WHEN @PorcentajeDescuento > 0 THEN (Total * @PorcentajeDescuento / 100)
ELSE 0
      END,
    Total = CASE
         WHEN @PorcentajeDescuento > 0 THEN Total - (Total * @PorcentajeDescuento / 100)
  ELSE Total
         END
  WHERE IdReserva IN (" + reservasIds + @")
     AND IdUsuario = @IdUsuario
         AND Estado = 'HOLD'";

        SqlCommand cmdUpdate = new SqlCommand(updateQuery, cn, transaction);
        cmdUpdate.Parameters.AddWithValue("@MetodoPago", metodoPago);
        cmdUpdate.Parameters.AddWithValue("@IdUsuario", idUsuario);
    cmdUpdate.Parameters.AddWithValue("@PorcentajeDescuento", porcentajeDescuento);

       int filasActualizadas = cmdUpdate.ExecuteNonQuery();

if (filasActualizadas == 0)
         {
      transaction.Rollback();
     resultado.Rows.Add("ERROR", "No se encontraron reservas para confirmar", 0, DBNull.Value, 0, 0);
     return resultado;
    }

        // Calcular monto de descuento total
      decimal montoDescuentoTotal = 0;
  if (porcentajeDescuento > 0)
 {
   string queryDescuento = "SELECT SUM(MontoDescuento) FROM reservas.Reserva WHERE IdReserva IN (" + reservasIds + ")";
           SqlCommand cmdDescuento = new SqlCommand(queryDescuento, cn, transaction);
object resultDesc = cmdDescuento.ExecuteScalar();
  if (resultDesc != null && resultDesc != DBNull.Value)
     {
 montoDescuentoTotal = Convert.ToDecimal(resultDesc);
  }
 }

        transaction.Commit();

    resultado.Rows.Add(
         "SUCCESS",
   $"Se confirmaron {filasActualizadas} reserva(s) correctamente",
filasActualizadas,
  DBNull.Value,
   montoDescuentoTotal,     // ✅ CAMBIADO: Devuelve el monto del descuento
      porcentajeDescuento      // ✅ AGREGADO: Devuelve el porcentaje
        );
    }
     catch (Exception ex)
       {
  transaction.Rollback();
  resultado.Rows.Add("ERROR", "Error al confirmar reservas: " + ex.Message, 0, DBNull.Value, 0, 0);
  }
     }
        }
          catch (Exception ex)
   {
      resultado.Rows.Add("ERROR", "Error de conexión: " + ex.Message, 0, DBNull.Value, 0, 0);
  }

     return resultado;
     }

        // ============================================================
        // ✅ NUEVO: LISTAR RESERVAS CONFIRMADAS DE UN USUARIO
        // ============================================================
        public DataTable ListarReservasConfirmadas(int idUsuario)
     {
        try
 {
        using (SqlConnection cn = conexion.CrearConexion())
    {
     string query = @"
    SELECT 
     r.IdReserva,
      r.IdUsuario,
    r.IdMesa,
  m.NumeroMesa,
 r.Fecha,
         r.Hora,
        r.NumeroPersonas,
    r.Total,
    r.MetodoPago,
  r.Estado,
  r.Observaciones,
     m.TipoMesa
   FROM reservas.Reserva r
        INNER JOIN reservas.Mesa m ON r.IdMesa = m.IdMesa
  WHERE r.IdUsuario = @IdUsuario 
   AND r.Estado = 'CONFIRMADA'
      AND r.MetodoPago IS NOT NULL 
     AND r.MetodoPago != ''
 ORDER BY r.Fecha DESC, r.Hora DESC";

      SqlCommand cmd = new SqlCommand(query, cn);
   cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

 SqlDataAdapter da = new SqlDataAdapter(cmd);
   DataTable dt = new DataTable();
  da.Fill(dt);
        return dt;
 }
    }
  catch (Exception ex)
      {
      throw new Exception("Error obteniendo reservas confirmadas: " + ex.Message);
}
    }
    
    
   // ✅ NUEVO: LISTAR TODAS LAS RESERVAS PARA ADMINISTRADOR
    public DataTable ListarTodasReservasAdmin()
     {
       try
      {
  using (SqlConnection cn = conexion.CrearConexion())
  {
    string query = @"
    SELECT 
     r.IdReserva,
      r.IdUsuario,
u.Nombre AS NombreUsuario,
u.Email AS EmailUsuario,
      r.IdMesa,
     m.NumeroMesa,
 r.Fecha,
   r.Hora,
 r.NumeroPersonas,
    r.Total,
   r.MetodoPago,
r.Estado,
  r.Observaciones,
     m.TipoMesa
   FROM reservas.Reserva r
INNER JOIN reservas.Mesa m ON r.IdMesa = m.IdMesa
     INNER JOIN seguridad.Usuario u ON r.IdUsuario = u.IdUsuario
 WHERE r.Estado IN ('HOLD', 'CONFIRMADA')
     ORDER BY r.Fecha DESC, r.Hora DESC, r.Estado DESC";

   SqlCommand cmd = new SqlCommand(query, cn);
     SqlDataAdapter da = new SqlDataAdapter(cmd);
   DataTable dt = new DataTable();
  da.Fill(dt);
        return dt;
 }
    }
  catch (Exception ex)
      {
    throw new Exception("Error obteniendo todas las reservas para admin: " + ex.Message);
}
    }
    
 // ✅ NUEVO: GENERAR FACTURA DESDE ADMINISTRADOR
      public DataTable GenerarFacturaDesdeAdmin(int idReserva, string metodoPago, string tipoFactura = "ADMIN")
 {
        DataTable resultado = new DataTable();
       resultado.Columns.Add("Estado", typeof(string));
    resultado.Columns.Add("Mensaje", typeof(string));
     resultado.Columns.Add("IdFactura", typeof(int));
  resultado.Columns.Add("Total", typeof(decimal));

  using (SqlConnection cn = conexion.CrearConexion())
    {
  cn.Open();
    SqlTransaction transaction = cn.BeginTransaction();

     try
    {
       // Validaciones
      if (idReserva <= 0)
     {
   resultado.Rows.Add("ERROR", "ID de reserva no válido", 0, 0);
   return resultado;
  }

   if (string.IsNullOrWhiteSpace(metodoPago))
 {
     resultado.Rows.Add("ERROR", "Método de pago es requerido", 0, 0);
     return resultado;
   }

    // Verificar que la reserva existe
   string checkReservaSql = "SELECT IdUsuario, Estado, Total, NumeroPersonas FROM reservas.Reserva WHERE IdReserva = @IdReserva";
     SqlCommand checkReservaCmd = new SqlCommand(checkReservaSql, cn, transaction);
     checkReservaCmd.Parameters.AddWithValue("@IdReserva", idReserva);
    
    int idUsuario = 0;
 string estadoReserva = "";
  decimal totalReserva = 0;
    int numeroPersonas = 0;

    using (SqlDataReader reader = checkReservaCmd.ExecuteReader())
   {
       if (reader.Read())
   {
    idUsuario = Convert.ToInt32(reader["IdUsuario"]);
   estadoReserva = reader["Estado"].ToString();
    totalReserva = reader["Total"] != DBNull.Value ? Convert.ToDecimal(reader["Total"]) : 0;
    numeroPersonas = Convert.ToInt32(reader["NumeroPersonas"]);
        }
        else
     {
   resultado.Rows.Add("ERROR", "Reserva no encontrada", 0, 0);
   return resultado;
   }
   }

  // Asignar precio si no tiene
   if (totalReserva == 0)
    {
  if (numeroPersonas <= 2) totalReserva = 25.00m;
         else if (numeroPersonas <= 4) totalReserva = 35.00m;
    else if (numeroPersonas <= 6) totalReserva = 45.00m;
 else totalReserva = 55.00m;
     
         // Actualizar el precio en la reserva
  string updatePrecioSql = "UPDATE reservas.Reserva SET Total = @Total WHERE IdReserva = @IdReserva";
SqlCommand updatePrecioCmd = new SqlCommand(updatePrecioSql, cn, transaction);
   updatePrecioCmd.Parameters.AddWithValue("@Total", totalReserva);
 updatePrecioCmd.Parameters.AddWithValue("@IdReserva", idReserva);
 updatePrecioCmd.ExecuteNonQuery();
    }

      // Calcular totales con IVA del 7%
      decimal subtotal = totalReserva;
  decimal iva = subtotal * 0.07m;
  decimal total = subtotal + iva;

      // Determinar estado de factura basado en estado de reserva
    string estadoFactura = estadoReserva == "CONFIRMADA" ? "Pagada" : "Emitida";
    
      // Insertar factura
     string insertFacturaSql = @"
   INSERT INTO facturacion.Factura (IdUsuario, FechaHora, Subtotal, IVA, Total, Estado, IdReserva) 
VALUES (@IdUsuario, GETDATE(), @Subtotal, @IVA, @Total, @Estado, @IdReserva);
    SELECT SCOPE_IDENTITY();";

 SqlCommand insertFacturaCmd = new SqlCommand(insertFacturaSql, cn, transaction);
 insertFacturaCmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
 insertFacturaCmd.Parameters.AddWithValue("@Subtotal", subtotal);
    insertFacturaCmd.Parameters.AddWithValue("@IVA", iva);
 insertFacturaCmd.Parameters.AddWithValue("@Total", total);
        insertFacturaCmd.Parameters.AddWithValue("@Estado", estadoFactura);
  insertFacturaCmd.Parameters.AddWithValue("@IdReserva", idReserva);
   
int idFactura = Convert.ToInt32(insertFacturaCmd.ExecuteScalar());

        // Insertar detalle de factura
string insertDetalleSql = @"
INSERT INTO facturacion.DetalleFactura (IdFactura, IdReserva, Descripcion, Cantidad, PrecioUnitario, Subtotal)
       SELECT @IdFactura, r.IdReserva, 
  CONCAT('Reserva Admin Mesa ', m.NumeroMesa, ' - ', r.NumeroPersonas, ' personas - ', 
   CONVERT(VARCHAR, r.Fecha, 103), ' ', r.Hora, CASE WHEN @MetodoPago IS NOT NULL THEN CONCAT(' - ', @MetodoPago) ELSE '' END), 
   1, @PrecioUnitario, @PrecioUnitario
    FROM reservas.Reserva r
    INNER JOIN reservas.Mesa m ON r.IdMesa = m.IdMesa
 WHERE r.IdReserva = @IdReserva";

 SqlCommand insertDetalleCmd = new SqlCommand(insertDetalleSql, cn, transaction);
    insertDetalleCmd.Parameters.AddWithValue("@IdFactura", idFactura);
    insertDetalleCmd.Parameters.AddWithValue("@IdReserva", idReserva);
 insertDetalleCmd.Parameters.AddWithValue("@PrecioUnitario", totalReserva);
 insertDetalleCmd.Parameters.AddWithValue("@MetodoPago", metodoPago);
          insertDetalleCmd.ExecuteNonQuery();

   // Actualizar estado de reserva y método de pago
  string updateReservaSql = @"
    UPDATE reservas.Reserva 
        SET Estado = CASE WHEN Estado = 'HOLD' THEN 'CONFIRMADA' ELSE Estado END,
   MetodoPago = @MetodoPago
   WHERE IdReserva = @IdReserva";

   SqlCommand updateReservaCmd = new SqlCommand(updateReservaSql, cn, transaction);
      updateReservaCmd.Parameters.AddWithValue("@MetodoPago", metodoPago);
      updateReservaCmd.Parameters.AddWithValue("@IdReserva", idReserva);
   updateReservaCmd.ExecuteNonQuery();

   transaction.Commit();

  resultado.Rows.Add("SUCCESS", "Factura generada correctamente desde administración", idFactura, total);
 return resultado;
 }
  catch (Exception ex)
     {
   transaction.Rollback();
   resultado.Rows.Add("ERROR", "Error generando factura desde admin: " + ex.Message, 0, 0);
      return resultado;
    }
 }
  }
    }
}
