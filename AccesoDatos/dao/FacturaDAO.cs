using System;
using System.Data;
using System.Data.SqlClient;
using AccesoDatos.Conexion;
using System.Collections.Generic;

namespace AccesoDatos.DAO
{
    public class FacturaDAO
    {
    private readonly ConexionSQL conexion = new ConexionSQL();

        public DataTable GenerarFactura(int idUsuario, int idReserva)
   {
            using (SqlConnection cn = conexion.CrearConexion())
      {
    SqlCommand cmd = new SqlCommand("sp_generar_factura", cn);
       cmd.CommandType = CommandType.StoredProcedure;
      cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
  cmd.Parameters.AddWithValue("@IdReserva", idReserva);
    SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
  da.Fill(dt);
        return dt;
   }
        }

        public DataTable ListarFacturas()
        {
     using (SqlConnection cn = conexion.CrearConexion())
{
     SqlCommand cmd = new SqlCommand("sp_listar_facturas", cn);
         cmd.CommandType = CommandType.StoredProcedure;
        SqlDataAdapter da = new SqlDataAdapter(cmd);
     DataTable dt = new DataTable();
    da.Fill(dt);
                return dt;
         }
   }

      public DataTable DetalleFactura(int idFactura)
     {
 using (SqlConnection cn = conexion.CrearConexion())
          {
                SqlCommand cmd = new SqlCommand("sp_detalle_factura", cn);
 cmd.CommandType = CommandType.StoredProcedure;
       cmd.Parameters.AddWithValue("@IdFactura", idFactura);
             SqlDataAdapter da = new SqlDataAdapter(cmd);
         DataTable dt = new DataTable();
        da.Fill(dt);
       return dt;
            }
  }

        public void AnularFactura(int idFactura)
        {
         using (SqlConnection cn = conexion.CrearConexion())
 {
        SqlCommand cmd = new SqlCommand("sp_anular_factura", cn);
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.Parameters.AddWithValue("@IdFactura", idFactura);
           cn.Open();
         cmd.ExecuteNonQuery();
    }
        }

      // ✅ NUEVO: Generar factura desde carrito usando queries SQL directas
        public DataTable GenerarFacturaCarrito(int idUsuario, string reservasIds, int? promocionId, string metodoPago)
   {
       DataTable resultado = new DataTable();
 resultado.Columns.Add("Estado", typeof(string));
 resultado.Columns.Add("Mensaje", typeof(string));
        resultado.Columns.Add("IdFactura", typeof(int));
        resultado.Columns.Add("SubtotalBruto", typeof(decimal));
  resultado.Columns.Add("Descuento", typeof(decimal));
    resultado.Columns.Add("Subtotal", typeof(decimal));
    resultado.Columns.Add("IVA", typeof(decimal));
        resultado.Columns.Add("Total", typeof(decimal));
          resultado.Columns.Add("PorcentajeDescuento", typeof(decimal));
resultado.Columns.Add("CantidadReservas", typeof(int));
     resultado.Columns.Add("MetodoPago", typeof(string));

     using (SqlConnection cn = conexion.CrearConexion())
      {
         cn.Open();
         SqlTransaction transaction = cn.BeginTransaction();

     try
       {
       // Validaciones
        if (idUsuario <= 0)
     {
   resultado.Rows.Add("ERROR", "Usuario no válido", 0, 0, 0, 0, 0, 0, "");
   return resultado;
  }

      if (string.IsNullOrWhiteSpace(reservasIds))
        {
        resultado.Rows.Add("ERROR", "Debe seleccionar al menos una reserva", 0, 0, 0, 0, 0, 0, "");
 return resultado;
     }

     // Verificar que el usuario existe
   string checkUserSql = "SELECT COUNT(*) FROM Usuario WHERE IdUsuario = @IdUsuario";
     SqlCommand checkUserCmd = new SqlCommand(checkUserSql, cn, transaction);
       checkUserCmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
 int userExists = (int)checkUserCmd.ExecuteScalar();

   if (userExists == 0)
 {
 resultado.Rows.Add("ERROR", "Usuario no encontrado", 0, 0, 0, 0, 0, 0, "");
       return resultado;
  }

     // Convertir IDs de reservas a lista
                List<int> idsReservas = new List<int>();
           string[] reservasArray = reservasIds.Split(',');
     foreach (string id in reservasArray)
          {
     if (int.TryParse(id.Trim(), out int reservaId))
    {
   idsReservas.Add(reservaId);
         }
         }

      if (idsReservas.Count == 0)
    {
  resultado.Rows.Add("ERROR", "No se encontraron reservas válidas", 0, 0, 0, 0, 0, 0, "");
     return resultado;
}

    // ✅ NUEVO: Verificar si ya existen facturas emitidas (no pagadas) para estas reservas
       string checkFacturasExistentesSql = $@"
            SELECT f.IdFactura, STRING_AGG(CAST(df.IdReserva AS VARCHAR), ',') as ReservasExistentes
      FROM Factura f
       INNER JOIN DetalleFactura df ON f.IdFactura = df.IdFactura
 WHERE df.IdReserva IN ({string.Join(",", idsReservas)})
 AND f.Estado = 'Emitida'
     AND f.IdUsuario = @IdUsuario
GROUP BY f.IdFactura";

 SqlCommand checkFacturasCmd = new SqlCommand(checkFacturasExistentesSql, cn, transaction);
  checkFacturasCmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
  
        DataTable facturasExistentes = new DataTable();
        SqlDataAdapter adapter = new SqlDataAdapter(checkFacturasCmd);
   adapter.Fill(facturasExistentes);

        int idFacturaExistente = 0;
      bool modificarFacturaExistente = false;
      
if (facturasExistentes.Rows.Count > 0)
 {
         // Existe una factura emitida, la vamos a modificar
            idFacturaExistente = Convert.ToInt32(facturasExistentes.Rows[0]["IdFactura"]);
   modificarFacturaExistente = true;
        }

        // Definir parámetros para consultas posteriores
        string idsParametros = string.Join(",", idsReservas);

        // Verificar que las reservas pertenecen al usuario y tienen precios válidos
        string checkReservasSql = $@"
  SELECT COUNT(*) 
 FROM Reserva 
 WHERE IdReserva IN ({idsParametros}) 
 AND IdUsuario = @IdUsuario 
    AND Estado IN ('CONFIRMADA', 'HOLD')";

   SqlCommand checkReservasCmd = new SqlCommand(checkReservasSql, cn, transaction);
     checkReservasCmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
  int reservasValidas = (int)checkReservasCmd.ExecuteScalar();

    if (reservasValidas != idsReservas.Count)
     {
 resultado.Rows.Add("ERROR", "Algunas reservas no pertenecen al usuario o están en estado incorrecto", 0, 0, 0, 0, 0, 0, 0, 0, "");
 return resultado;
    }

 // Asignar precios automáticamente a reservas sin precio
 string updatePreciosSql = $@"
UPDATE r 
   SET r.Total = CASE 
   WHEN r.NumeroPersonas <= 2 THEN 25.00
 WHEN r.NumeroPersonas <= 4 THEN 35.00
    WHEN r.NumeroPersonas <= 6 THEN 45.00
      ELSE 55.00
     END
  FROM Reserva r
      WHERE r.IdReserva IN ({idsParametros})
          AND (r.Total IS NULL OR r.Total = 0)";

        SqlCommand updatePreciosCmd = new SqlCommand(updatePreciosSql, cn, transaction);
 updatePreciosCmd.ExecuteNonQuery();

   // Calcular subtotal bruto (después de asignar precios)
string subtotalSql = $@"
     SELECT SUM(r.Total)
FROM Reserva r 
   WHERE r.IdReserva IN ({idsParametros})";

      SqlCommand subtotalCmd = new SqlCommand(subtotalSql, cn, transaction);
      object subtotalResult = subtotalCmd.ExecuteScalar();
      decimal subtotalBruto = subtotalResult != null && subtotalResult != DBNull.Value 
 ? Convert.ToDecimal(subtotalResult) 
    : 0m;

   // Verificar que el subtotal sea válido
   if (subtotalBruto <= 0)
    {
   resultado.Rows.Add("ERROR", "No se pudieron calcular los totales. Las reservas no tienen precios válidos.", 0, 0, 0, 0, 0, 0, 0, 0, "");
                return resultado;
          }

      // Aplicar promoción si se especifica
     decimal descuento = 0;
     decimal porcentajeDescuento = 0;

   if (promocionId.HasValue)
        {
    string promocionSql = @"
      SELECT Descuento 
       FROM Promocion 
   WHERE IdPromocion = @PromocionId 
AND Estado = 'ACTIVA' 
   AND FechaInicio <= GETDATE() 
     AND FechaFin >= GETDATE()";

     SqlCommand promocionCmd = new SqlCommand(promocionSql, cn, transaction);
            promocionCmd.Parameters.AddWithValue("@PromocionId", promocionId.Value);
            object promocionResult = promocionCmd.ExecuteScalar();

  if (promocionResult != null && promocionResult != DBNull.Value)
      {
   porcentajeDescuento = Convert.ToDecimal(promocionResult);
  descuento = subtotalBruto * (porcentajeDescuento / 100.0m);
      }
       }

         // Calcular totales
      decimal subtotal = subtotalBruto - descuento;
      decimal iva = subtotal * 0.115m; // IVA del 7% - CAMBIADO DE 0.12m
      decimal total = subtotal + iva;

      // Insertar o actualizar factura
     int idFactura;
    
      if (modificarFacturaExistente)
      {
  // Actualizar factura existente
 string updateFacturaSql = @"
     UPDATE Factura 
      SET Subtotal = @Subtotal, IVA = @IVA, Total = @Total, FechaHora = GETDATE(), IdReserva = @IdReserva
    WHERE IdFactura = @IdFactura";
    
   SqlCommand updateFacturaCmd = new SqlCommand(updateFacturaSql, cn, transaction);
     updateFacturaCmd.Parameters.AddWithValue("@IdFactura", idFacturaExistente);
        updateFacturaCmd.Parameters.AddWithValue("@Subtotal", subtotal);
updateFacturaCmd.Parameters.AddWithValue("@IVA", iva);
      updateFacturaCmd.Parameters.AddWithValue("@Total", total);
        updateFacturaCmd.Parameters.AddWithValue("@IdReserva", idsReservas[0]); // Usar la primera reserva como referencia
   updateFacturaCmd.ExecuteNonQuery();
  
     idFactura = idFacturaExistente;
       
 // Eliminar detalles existentes para recalcular
string deleteDetallesSql = "DELETE FROM DetalleFactura WHERE IdFactura = @IdFactura";
SqlCommand deleteDetallesCmd = new SqlCommand(deleteDetallesSql, cn, transaction);
   deleteDetallesCmd.Parameters.AddWithValue("@IdFactura", idFactura);
  deleteDetallesCmd.ExecuteNonQuery();
      }
      else
  {
        // Crear nueva factura
 string insertFacturaSql = @"
   INSERT INTO Factura (IdUsuario, FechaHora, Subtotal, IVA, Total, Estado, IdReserva) 
     VALUES (@IdUsuario, GETDATE(), @Subtotal, @IVA, @Total, 'Emitida', @IdReserva);
    SELECT SCOPE_IDENTITY();";

 SqlCommand insertFacturaCmd = new SqlCommand(insertFacturaSql, cn, transaction);
     insertFacturaCmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
 insertFacturaCmd.Parameters.AddWithValue("@Subtotal", subtotal);
    insertFacturaCmd.Parameters.AddWithValue("@IVA", iva);
     insertFacturaCmd.Parameters.AddWithValue("@Total", total);
        insertFacturaCmd.Parameters.AddWithValue("@IdReserva", idsReservas[0]); // Usar la primera reserva como referencia
   
           idFactura = Convert.ToInt32(insertFacturaCmd.ExecuteScalar());
 }

         // Insertar detalles de factura
               foreach (int idReserva in idsReservas)
       {
           // Calcular precio con descuento para este detalle
           decimal precioSinDescuento = 0;
           decimal precioConDescuento = 0;

           // Obtener el precio original de la reserva (ya asignado automáticamente)
         string getPrecioSql = "SELECT Total FROM Reserva WHERE IdReserva = @IdReserva";
   SqlCommand getPrecioCmd = new SqlCommand(getPrecioSql, cn, transaction);
      getPrecioCmd.Parameters.AddWithValue("@IdReserva", idReserva);
     object precioResult = getPrecioCmd.ExecuteScalar();
      
     if (precioResult != null && precioResult != DBNull.Value)
 {
 precioSinDescuento = Convert.ToDecimal(precioResult);
        }
      else
 {
      // Precio por defecto basado en número de personas (debe coincidir con la lógica anterior)
     string getPrecioPersonasSql = "SELECT NumeroPersonas FROM Reserva WHERE IdReserva = @IdReserva";
     SqlCommand getPersonasCmd = new SqlCommand(getPrecioPersonasSql, cn, transaction);
      getPersonasCmd.Parameters.AddWithValue("@IdReserva", idReserva);
      object personasResult = getPersonasCmd.ExecuteScalar();
      
      if (personasResult != null && personasResult != DBNull.Value)
      {
   int numPersonas = Convert.ToInt32(personasResult);
          if (numPersonas <= 2) precioSinDescuento = 25.00m;
         else if (numPersonas <= 4) precioSinDescuento = 35.00m;
    else if (numPersonas <= 6) precioSinDescuento = 45.00m;
         else precioSinDescuento = 55.00m;
      }
        else
      {
      precioSinDescuento = 30.00m; // Precio por defecto absoluto
  }
      }

      // Aplicar descuento proporcional si hay promoción
   if (porcentajeDescuento > 0)
       {
     decimal descuentoDetalle = precioSinDescuento * (porcentajeDescuento / 100.0m);
        precioConDescuento = precioSinDescuento - descuentoDetalle;
 }
            else
    {
  precioConDescuento = precioSinDescuento;
        }

string insertDetalleSql = @"
INSERT INTO DetalleFactura (IdFactura, IdReserva, Descripcion, Cantidad, PrecioUnitario, Subtotal)
       SELECT @IdFactura, r.IdReserva, 
  CONCAT('Reserva Mesa ', m.NumeroMesa, ' - ', r.NumeroPersonas, ' personas - ', 
   CONVERT(VARCHAR, r.Fecha, 103), ' ', r.Hora), 
   1, @PrecioUnitario, @SubtotalConDescuento
    FROM Reserva r
    INNER JOIN Mesa m ON r.IdMesa = m.IdMesa
 WHERE r.IdReserva = @IdReserva";

          SqlCommand insertDetalleCmd = new SqlCommand(insertDetalleSql, cn, transaction);
    insertDetalleCmd.Parameters.AddWithValue("@IdFactura", idFactura);
    insertDetalleCmd.Parameters.AddWithValue("@IdReserva", idReserva);
 insertDetalleCmd.Parameters.AddWithValue("@PrecioUnitario", precioSinDescuento); // Precio sin descuento
      insertDetalleCmd.Parameters.AddWithValue("@SubtotalConDescuento", precioConDescuento); // Subtotal con descuento
          insertDetalleCmd.ExecuteNonQuery();
  }

         // ✅ CAMBIO IMPORTANTE: NO actualizar estado de reservas a FACTURADA
 // Las reservas se mantienen en HOLD hasta que la factura se marque como pagada
         // string updateReservasSql = $@"
         // UPDATE Reserva 
  // SET Estado = 'FACTURADA'
         // WHERE IdReserva IN ({idsParametros})";

         // SqlCommand updateReservasCmd = new SqlCommand(updateReservasSql, cn, transaction);
         // updateReservasCmd.ExecuteNonQuery();

    // Obtener método de pago desde reservas
            string metodoPagoSql = $@"
  SELECT TOP 1 r.MetodoPago 
      FROM Reserva r
         WHERE r.IdReserva IN ({idsParametros})
   AND r.MetodoPago IS NOT NULL AND r.MetodoPago != ''";

         SqlCommand metodoPagoCmd = new SqlCommand(metodoPagoSql, cn, transaction);
  object metodoPagoResult = metodoPagoCmd.ExecuteScalar();
           string metodoPagoFinal = metodoPagoResult?.ToString() ?? "Todavia no realiza el pago";

 transaction.Commit();

  // Retornar resultado exitoso con información de si fue creada o modificada
        string mensajeAccion = modificarFacturaExistente ? "Factura actualizada correctamente" : "Factura generada correctamente";
 resultado.Rows.Add("SUCCESS", mensajeAccion, idFactura, 
   subtotalBruto, descuento, subtotal, iva, total, 
      porcentajeDescuento, idsReservas.Count, metodoPagoFinal);

    return resultado;
      }
     catch (Exception ex)
       {
   transaction.Rollback();
         resultado.Rows.Add("ERROR", "Error generando la factura: " + ex.Message, 0, 0, 0, 0, 0, 0, 0, 0, "");
      return resultado;
       }
 }
        }

    // ✅ NUEVO: Obtener factura detallada usando queries SQL directas
  public DataSet ObtenerFacturaDetallada(int idFactura)
    {
            DataSet dataSet = new DataSet();

            using (SqlConnection cn = conexion.CrearConexion())
            {
        cn.Open();

   // Tabla 1: Información de la factura (CON columna IdReserva)
              string facturaQuery = @"
    SELECT 
        f.IdFactura,
    f.IdUsuario,
     f.IdReserva, -- NUEVA COLUMNA
        u.Nombre,
    '' AS Apellido,
  u.Telefono,
   u.Email,
     f.FechaHora,
    f.Subtotal,
  f.IVA,
   f.Total,
     f.Estado,
      ISNULL((
        SELECT TOP 1 r.MetodoPago 
   FROM DetalleFactura df 
   INNER JOIN Reserva r ON df.IdReserva = r.IdReserva
     WHERE df.IdFactura = f.IdFactura 
        AND r.MetodoPago IS NOT NULL 
AND r.MetodoPago != ''
   ), 'Todavia no realiza el pago') AS MetodoPago,
  0.00 AS Descuento -- Descuento calculado desde promociones, no almacenado
  FROM Factura f
  INNER JOIN Usuario u ON f.IdUsuario = u.IdUsuario
   WHERE f.IdFactura = @IdFactura";

         SqlCommand facturaCmd = new SqlCommand(facturaQuery, cn);
      facturaCmd.Parameters.AddWithValue("@IdFactura", idFactura);
     SqlDataAdapter facturaAdapter = new SqlDataAdapter(facturaCmd);
 DataTable facturaTable = new DataTable("Factura");
     facturaAdapter.Fill(facturaTable);
  dataSet.Tables.Add(facturaTable);

                // Tabla 2: Detalles de la factura
                string detallesQuery = @"
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

     SqlCommand detallesCmd = new SqlCommand(detallesQuery, cn);
      detallesCmd.Parameters.AddWithValue("@IdFactura", idFactura);
         SqlDataAdapter detallesAdapter = new SqlDataAdapter(detallesCmd);
      DataTable detallesTable = new DataTable("Detalles");
       detallesAdapter.Fill(detallesTable);
     dataSet.Tables.Add(detallesTable);
            }

    return dataSet;
        }

   // ✅ NUEVO: Marcar factura como pagada usando queries SQL directas
        public DataTable MarcarFacturaPagada(int idFactura, string metodoPago)
        {
            DataTable resultado = new DataTable();
            resultado.Columns.Add("Estado", typeof(string));
      resultado.Columns.Add("Mensaje", typeof(string));
         resultado.Columns.Add("IdFactura", typeof(int));

      using (SqlConnection cn = conexion.CrearConexion())
            {
        cn.Open();
                SqlTransaction transaction = cn.BeginTransaction();

             try
  {
     // Validaciones
        if (idFactura <= 0)
 {
            resultado.Rows.Add("ERROR", "ID de factura no válido", 0);
     return resultado;
   }

    if (string.IsNullOrWhiteSpace(metodoPago))
          {
        resultado.Rows.Add("ERROR", "Debe especificar un método de pago", 0);
     return resultado;
        }

            // Verificar que la factura existe
string checkFacturaSql = "SELECT COUNT(*) FROM Factura WHERE IdFactura = @IdFactura";
    SqlCommand checkFacturaCmd = new SqlCommand(checkFacturaSql, cn, transaction);
               checkFacturaCmd.Parameters.AddWithValue("@IdFactura", idFactura);
      int facturaExists = (int)checkFacturaCmd.ExecuteScalar();

       if (facturaExists == 0)
                {
            resultado.Rows.Add("ERROR", "Factura no encontrada", 0);
         return resultado;
               }

         // Verificar que la factura no esté anulada
        string checkAnuladaSql = "SELECT COUNT(*) FROM Factura WHERE IdFactura = @IdFactura AND Estado = 'Anulada'";
       SqlCommand checkAnuladaCmd = new SqlCommand(checkAnuladaSql, cn, transaction);
      checkAnuladaCmd.Parameters.AddWithValue("@IdFactura", idFactura);
            int facturaAnulada = (int)checkAnuladaCmd.ExecuteScalar();

        if (facturaAnulada > 0)
          {
  resultado.Rows.Add("ERROR", "No se puede marcar como pagada una factura anulada", 0);
          return resultado;
       }

    // Actualizar estado de factura
         string updateFacturaSql = "UPDATE Factura SET Estado = 'Pagada' WHERE IdFactura = @IdFactura";
           SqlCommand updateFacturaCmd = new SqlCommand(updateFacturaSql, cn, transaction);
    updateFacturaCmd.Parameters.AddWithValue("@IdFactura", idFactura);
        updateFacturaCmd.ExecuteNonQuery();

      // Actualizar método de pago en las reservas relacionadas
string updateReservasSql = @"
 UPDATE Reserva 
 SET MetodoPago = @MetodoPago
  WHERE IdReserva IN (
            SELECT df.IdReserva 
       FROM DetalleFactura df 
   WHERE df.IdFactura = @IdFactura
         )";

        SqlCommand updateReservasCmd = new SqlCommand(updateReservasSql, cn, transaction);
     updateReservasCmd.Parameters.AddWithValue("@MetodoPago", metodoPago);
 updateReservasCmd.Parameters.AddWithValue("@IdFactura", idFactura);
    updateReservasCmd.ExecuteNonQuery();

    // ✅ NUEVO: Actualizar estado de reservas a FACTURADA cuando se paga
        string updateEstadoReservasSql = @"
    UPDATE Reserva 
        SET Estado = 'CONFIRMADA'
        WHERE IdReserva IN (
            SELECT df.IdReserva 
    FROM DetalleFactura df 
   WHERE df.IdFactura = @IdFactura
 )";

     SqlCommand updateEstadoReservasCmd = new SqlCommand(updateEstadoReservasSql, cn, transaction);
        updateEstadoReservasCmd.Parameters.AddWithValue("@IdFactura", idFactura);
      updateEstadoReservasCmd.ExecuteNonQuery();

   transaction.Commit();

     resultado.Rows.Add("SUCCESS", "Factura marcada como pagada correctamente", idFactura);
     return resultado;
    }
        catch (Exception ex)
     {
   transaction.Rollback();
resultado.Rows.Add("ERROR", "Error marcando la factura como pagada: " + ex.Message, 0);
    return resultado;
 }
      }
        }

        // ✅ NUEVO: Listar facturas de usuario usando queries SQL directas
        public DataTable ListarFacturasUsuario(int idUsuario)
{
 using (SqlConnection cn = conexion.CrearConexion())
            {
     string query = @"
         SELECT 
       f.IdFactura,
 ISNULL(f.IdReserva, 0) AS IdReserva,
 f.FechaHora,
      ISNULL(f.Subtotal, 0) AS Subtotal,
  ISNULL(f.IVA, 0) AS IVA,
 ISNULL(f.Total, 0) AS Total,
    ISNULL(f.Estado, 'Emitida') AS Estado,
  ISNULL((
     SELECT TOP 1 r.MetodoPago 
 FROM DetalleFactura df 
        INNER JOIN Reserva r ON df.IdReserva = r.IdReserva
WHERE df.IdFactura = f.IdFactura 
  AND r.MetodoPago IS NOT NULL 
       AND r.MetodoPago != ''
    ), 'Todavia no realiza el pago') AS MetodoPago,
    COUNT(df.IdDetalle) AS CantidadItems,
       STRING_AGG(CAST(ISNULL(df.IdReserva, 0) AS VARCHAR), ', ') AS ReservasIds
   FROM Factura f
   LEFT JOIN DetalleFactura df ON f.IdFactura = df.IdFactura
  WHERE f.IdUsuario = @IdUsuario
     GROUP BY f.IdFactura, f.IdReserva, f.FechaHora, f.Subtotal, f.IVA, f.Total, f.Estado
    ORDER BY f.FechaHora DESC";

          SqlCommand cmd = new SqlCommand(query, cn);
          cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
     SqlDataAdapter da = new SqlDataAdapter(cmd);
     DataTable dt = new DataTable();
    da.Fill(dt);
    return dt;
            }
     }

        // ✅ NUEVO: Generar factura específicamente para reservas confirmadas
        public DataTable GenerarFacturaReservasConfirmadas(int idUsuario, string reservasIds, string tipoFactura = "CONFIRMADA")
        {
      DataTable resultado = new DataTable();
resultado.Columns.Add("Estado", typeof(string));
   resultado.Columns.Add("Mensaje", typeof(string));
  resultado.Columns.Add("IdFactura", typeof(int));
     resultado.Columns.Add("SubtotalBruto", typeof(decimal));
    resultado.Columns.Add("Subtotal", typeof(decimal));
    resultado.Columns.Add("IVA", typeof(decimal));
        resultado.Columns.Add("Total", typeof(decimal));
        resultado.Columns.Add("CantidadReservas", typeof(int));
        resultado.Columns.Add("MetodoPago", typeof(string));

  using (SqlConnection cn = conexion.CrearConexion())
{
         cn.Open();
    SqlTransaction transaction = cn.BeginTransaction();

     try
    {
       // Validaciones
      if (idUsuario <= 0)
     {
         resultado.Rows.Add("ERROR", "Usuario no válido", 0, 0, 0, 0, 0, 0, "");
     return resultado;
  }

   if (string.IsNullOrWhiteSpace(reservasIds))
        {
        resultado.Rows.Add("ERROR", "Debe seleccionar al menos una reserva confirmada", 0, 0, 0, 0, 0, 0, "");
     return resultado;
     }

    // Verificar que el usuario existe
   string checkUserSql = "SELECT COUNT(*) FROM Usuario WHERE IdUsuario = @IdUsuario";
     SqlCommand checkUserCmd = new SqlCommand(checkUserSql, cn, transaction);
     checkUserCmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
    int userExists = (int)checkUserCmd.ExecuteScalar();

      if (userExists == 0)
     {
 resultado.Rows.Add("ERROR", "Usuario no encontrado", 0, 0, 0, 0, 0, 0, "");
   return resultado;
  }

       // Convertir IDs de reservas a lista
    List<int> idsReservas = new List<int>();
           string[] reservasArray = reservasIds.Split(',');
     foreach (string id in reservasArray)
        {
     if (int.TryParse(id.Trim(), out int reservaId))
    {
   idsReservas.Add(reservaId);
      }
         }

  if (idsReservas.Count == 0)
       {
  resultado.Rows.Add("ERROR", "No se encontraron reservas válidas", 0, 0, 0, 0, 0, 0, "");
     return resultado;
       }

        string idsParametros = string.Join(",", idsReservas);

       // Verificar que las reservas están confirmadas y pertenecen al usuario
     string checkReservasSql = $@"
    SELECT COUNT(*) 
   FROM Reserva 
      WHERE IdReserva IN ({idsParametros}) 
AND IdUsuario = @IdUsuario 
  AND Estado = 'CONFIRMADA'
     AND MetodoPago IS NOT NULL 
     AND MetodoPago != ''";

   SqlCommand checkReservasCmd = new SqlCommand(checkReservasSql, cn, transaction);
     checkReservasCmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
  int reservasValidas = (int)checkReservasCmd.ExecuteScalar();

    if (reservasValidas != idsReservas.Count)
     {
 resultado.Rows.Add("ERROR", "Algunas reservas no están confirmadas o no pertenecen al usuario", 0, 0, 0, 0, 0, 0, "");
 return resultado;
    }

        // Calcular subtotal de reservas confirmadas
string subtotalSql = $@"
 SELECT SUM(ISNULL(r.Total, 0))
FROM Reserva r 
   WHERE r.IdReserva IN ({idsParametros})";

      SqlCommand subtotalCmd = new SqlCommand(subtotalSql, cn, transaction);
        object subtotalResult = subtotalCmd.ExecuteScalar();
        decimal totalConIVA = subtotalResult != null && subtotalResult != DBNull.Value 
     ? Convert.ToDecimal(subtotalResult) 
            : 0m;

        if (totalConIVA <= 0)
        {
            resultado.Rows.Add("ERROR", "Las reservas confirmadas no tienen totales válidos", 0, 0, 0, 0, 0, 0, "");
return resultado;
        }

// ✅ CORRECCIÓN: El Total de la reserva YA incluye IVA 11.5%
      // Necesitamos separar el subtotal del IVA
        decimal subtotal = totalConIVA / 1.115m; // Extraer subtotal sin IVA
     decimal iva = totalConIVA - subtotal; // IVA que ya estaba incluido (11.5%)
        decimal total = totalConIVA; // El total es el mismo que vino de las reservas

// Insertar factura con estado "Confirmada"
        string insertFacturaSql = @"
INSERT INTO Factura (IdUsuario, FechaHora, Subtotal, IVA, Total, Estado, IdReserva) 
    VALUES (@IdUsuario, GETDATE(), @Subtotal, @IVA, @Total, 'Confirmada', @IdReserva);
         SELECT SCOPE_IDENTITY();";

    SqlCommand insertFacturaCmd = new SqlCommand(insertFacturaSql, cn, transaction);
insertFacturaCmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
        insertFacturaCmd.Parameters.AddWithValue("@Subtotal", subtotal);
        insertFacturaCmd.Parameters.AddWithValue("@IVA", iva);
  insertFacturaCmd.Parameters.AddWithValue("@Total", total);
        insertFacturaCmd.Parameters.AddWithValue("@IdReserva", idsReservas[0]); // Usar la primera reserva como referencia
   
 int idFactura = Convert.ToInt32(insertFacturaCmd.ExecuteScalar());

  // Insertar detalles de factura para cada reserva confirmada
        foreach (int idReserva in idsReservas)
       {
      decimal precioReserva = 0;

  // Obtener datos de la reserva confirmada
         string getDatosReservaSql = @"
      SELECT r.Total, r.MetodoPago, m.NumeroMesa, r.NumeroPersonas, r.Fecha, r.Hora 
   FROM Reserva r 
      INNER JOIN Mesa m ON r.IdMesa = m.IdMesa 
      WHERE r.IdReserva = @IdReserva";

   SqlCommand getDatosCmd = new SqlCommand(getDatosReservaSql, cn, transaction);
      getDatosCmd.Parameters.AddWithValue("@IdReserva", idReserva);
    
      using (SqlDataReader reader = getDatosCmd.ExecuteReader())
      {
       if (reader.Read())
   {
  precioReserva = reader["Total"] != DBNull.Value ? Convert.ToDecimal(reader["Total"]) : 0m;
        }
   }

string insertDetalleSql = @"
INSERT INTO DetalleFactura (IdFactura, IdReserva, Descripcion, Cantidad, PrecioUnitario, Subtotal)
       SELECT @IdFactura, r.IdReserva, 
  CONCAT('Reserva Confirmada Mesa ', m.NumeroMesa, ' - ', r.NumeroPersonas, ' personas - ', 
   CONVERT(VARCHAR, r.Fecha, 103), ' ', r.Hora, ' - ', r.MetodoPago), 
   1, @PrecioUnitario, @PrecioUnitario
    FROM Reserva r
    INNER JOIN Mesa m ON r.IdMesa = m.IdMesa
 WHERE r.IdReserva = @IdReserva";

     SqlCommand insertDetalleCmd = new SqlCommand(insertDetalleSql, cn, transaction);
    insertDetalleCmd.Parameters.AddWithValue("@IdFactura", idFactura);
    insertDetalleCmd.Parameters.AddWithValue("@IdReserva", idReserva);
 insertDetalleCmd.Parameters.AddWithValue("@PrecioUnitario", precioReserva);
          insertDetalleCmd.ExecuteNonQuery();
  }

    // Obtener métodos de pago de las reservas
     string metodoPagoSql = $@"
  SELECT DISTINCT r.MetodoPago 
      FROM Reserva r
         WHERE r.IdReserva IN ({idsParametros})
   AND r.MetodoPago IS NOT NULL AND r.MetodoPago != ''";

         SqlCommand metodoPagoCmd = new SqlCommand(metodoPagoSql, cn, transaction);
 List<string> metodosPago = new List<string>();
 
      using (SqlDataReader reader = metodoPagoCmd.ExecuteReader())
    {
       while (reader.Read())
       {
 metodosPago.Add(reader["MetodoPago"].ToString());
   }
        }

        string metodoPagoFinal = metodosPago.Count == 1 ? metodosPago[0] : $"Múltiples ({string.Join(", ", metodosPago)})";

        transaction.Commit();

  // Retornar resultado exitoso
    // ✅ SubtotalBruto ya no es necesario, usamos subtotal (sin IVA)
        resultado.Rows.Add("SUCCESS", "Factura de reservas confirmadas generada correctamente", idFactura, 
       subtotal, subtotal, iva, total, 
          idsReservas.Count, metodoPagoFinal);

    return resultado;
   }
catch (Exception ex)
        {
    transaction.Rollback();
  resultado.Rows.Add("ERROR", "Error generando la factura de confirmadas: " + ex.Message, 0, 0, 0, 0, 0, 0, "");
     return resultado;
   }
            }
  }
  }
}
