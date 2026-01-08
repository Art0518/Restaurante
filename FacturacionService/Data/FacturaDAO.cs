using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using FacturacionService.Models;

namespace FacturacionService.Data
{
    public class FacturaDAO
    {
        private readonly string _connectionString;

        public FacturaDAO(string connectionString)
        {
            _connectionString = connectionString;
        }

        // ? Generar factura básica usando queries SQL directas (NO SP)
        public DataTable GenerarFactura(int idUsuario, int idReserva)
        {
      DataTable resultado = new DataTable();
  resultado.Columns.Add("IdFactura", typeof(int));
            resultado.Columns.Add("Mensaje", typeof(string));

      using (SqlConnection cn = new SqlConnection(_connectionString))
{
        cn.Open();
    SqlTransaction transaction = cn.BeginTransaction();

          try
           {
     // Crear factura simple
string insertFacturaSql = @"
INSERT INTO facturacion.Factura (IdUsuario, FechaHora, Subtotal, IVA, Total, Estado, IdReserva) 
VALUES (@IdUsuario, GETDATE(), 0, 0, 0, 'Emitida', @IdReserva);
SELECT SCOPE_IDENTITY();";

  SqlCommand insertFacturaCmd = new SqlCommand(insertFacturaSql, cn, transaction);
            insertFacturaCmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
           insertFacturaCmd.Parameters.AddWithValue("@IdReserva", idReserva);

        int idFactura = Convert.ToInt32(insertFacturaCmd.ExecuteScalar());

       transaction.Commit();

    resultado.Rows.Add(idFactura, "Factura generada correctamente");
        return resultado;
            }
   catch (Exception ex)
{
           transaction.Rollback();
       resultado.Rows.Add(0, "Error: " + ex.Message);
            return resultado;
    }
          }
  }

        // ? Listar facturas usando queries SQL directas (NO SP)
        public DataTable ListarFacturas()
  {
      using (SqlConnection cn = new SqlConnection(_connectionString))
{
            string query = "SELECT * FROM facturacion.Factura ORDER BY FechaHora DESC";
SqlCommand cmd = new SqlCommand(query, cn);
      SqlDataAdapter da = new SqlDataAdapter(cmd);
          DataTable dt = new DataTable();
              da.Fill(dt);
    return dt;
   }
        }

   // ? Detalle de factura usando queries SQL directas (NO SP)
     public DataTable DetalleFactura(int idFactura)
        {
         using (SqlConnection cn = new SqlConnection(_connectionString))
          {
                string query = @"
SELECT f.*, u.Nombre, u.Email
FROM facturacion.Factura f
INNER JOIN Usuario u ON f.IdUsuario = u.IdUsuario
WHERE f.IdFactura = @IdFactura";

    SqlCommand cmd = new SqlCommand(query, cn);
        cmd.Parameters.AddWithValue("@IdFactura", idFactura);
  SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
  da.Fill(dt);
              return dt;
            }
  }

// ? Anular factura usando queries SQL directas (NO SP)
        public void AnularFactura(int idFactura)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
        string query = "UPDATE facturacion.Factura SET Estado = 'Anulada' WHERE IdFactura = @IdFactura";
    SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@IdFactura", idFactura);
    cn.Open();
      cmd.ExecuteNonQuery();
            }
        }

        // ? NUEVO: Generar factura desde carrito usando queries SQL directas
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

            using (SqlConnection cn = new SqlConnection(_connectionString))
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

        // Verificar si ya existen facturas emitidas
       string checkFacturasExistentesSql = $@"
SELECT f.IdFactura, STRING_AGG(CAST(df.IdReserva AS VARCHAR), ',') as ReservasExistentes
FROM facturacion.Factura f
INNER JOIN facturacion.DetalleFactura df ON f.IdFactura = df.IdFactura
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
      idFacturaExistente = Convert.ToInt32(facturasExistentes.Rows[0]["IdFactura"]);
               modificarFacturaExistente = true;
           }

   string idsParametros = string.Join(",", idsReservas);

    // Verificar que las reservas pertenecen al usuario
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
        resultado.Rows.Add("ERROR", "Algunas reservas no pertenecen al usuario o están en estado incorrecto", 0, 0, 0, 0, 0, 0, "");
            return resultado;
         }

    // Asignar precios automáticamente
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

          // Calcular subtotal bruto
 string subtotalSql = $@"
SELECT SUM(r.Total)
FROM Reserva r 
WHERE r.IdReserva IN ({idsParametros})";

    SqlCommand subtotalCmd = new SqlCommand(subtotalSql, cn, transaction);
        object subtotalResult = subtotalCmd.ExecuteScalar();
  decimal subtotalBruto = subtotalResult != null && subtotalResult != DBNull.Value 
      ? Convert.ToDecimal(subtotalResult) 
            : 0m;

     if (subtotalBruto <= 0)
       {
         resultado.Rows.Add("ERROR", "No se pudieron calcular los totales. Las reservas no tienen precios válidos.", 0, 0, 0, 0, 0, 0, "");
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
        decimal iva = subtotal * 0.115m; // IVA del 11.5%
                    decimal total = subtotal + iva;

  // Insertar o actualizar factura
         int idFactura;

        if (modificarFacturaExistente)
    {
          string updateFacturaSql = @"
UPDATE facturacion.Factura 
SET Subtotal = @Subtotal, IVA = @IVA, Total = @Total, FechaHora = GETDATE(), IdReserva = @IdReserva
WHERE IdFactura = @IdFactura";

          SqlCommand updateFacturaCmd = new SqlCommand(updateFacturaSql, cn, transaction);
     updateFacturaCmd.Parameters.AddWithValue("@IdFactura", idFacturaExistente);
         updateFacturaCmd.Parameters.AddWithValue("@Subtotal", subtotal);
            updateFacturaCmd.Parameters.AddWithValue("@IVA", iva);
    updateFacturaCmd.Parameters.AddWithValue("@Total", total);
        updateFacturaCmd.Parameters.AddWithValue("@IdReserva", idsReservas[0]);
      updateFacturaCmd.ExecuteNonQuery();

      idFactura = idFacturaExistente;

         string deleteDetallesSql = "DELETE FROM facturacion.DetalleFactura WHERE IdFactura = @IdFactura";
         SqlCommand deleteDetallesCmd = new SqlCommand(deleteDetallesSql, cn, transaction);
   deleteDetallesCmd.Parameters.AddWithValue("@IdFactura", idFactura);
     deleteDetallesCmd.ExecuteNonQuery();
        }
              else
          {
 string insertFacturaSql = @"
INSERT INTO facturacion.Factura (IdUsuario, FechaHora, Subtotal, IVA, Total, Estado, IdReserva) 
VALUES (@IdUsuario, GETDATE(), @Subtotal, @IVA, @Total, 'Emitida', @IdReserva);
SELECT SCOPE_IDENTITY();";

       SqlCommand insertFacturaCmd = new SqlCommand(insertFacturaSql, cn, transaction);
       insertFacturaCmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
            insertFacturaCmd.Parameters.AddWithValue("@Subtotal", subtotal);
              insertFacturaCmd.Parameters.AddWithValue("@IVA", iva);
            insertFacturaCmd.Parameters.AddWithValue("@Total", total);
          insertFacturaCmd.Parameters.AddWithValue("@IdReserva", idsReservas[0]);

    idFactura = Convert.ToInt32(insertFacturaCmd.ExecuteScalar());
   }

               // Insertar detalles de factura
            foreach (int idReserva in idsReservas)
         {
            decimal precioSinDescuento = 0;
     decimal precioConDescuento = 0;

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
          precioSinDescuento = 30.00m;
             }
         }

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
INSERT INTO facturacion.DetalleFactura (IdFactura, IdReserva, Descripcion, Cantidad, PrecioUnitario, Subtotal)
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
                 insertDetalleCmd.Parameters.AddWithValue("@PrecioUnitario", precioSinDescuento);
       insertDetalleCmd.Parameters.AddWithValue("@SubtotalConDescuento", precioConDescuento);
       insertDetalleCmd.ExecuteNonQuery();
           }

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

// ? NUEVO: Obtener factura detallada usando queries SQL directas
        public DataSet ObtenerFacturaDetallada(int idFactura)
        {
        DataSet dataSet = new DataSet();

     using (SqlConnection cn = new SqlConnection(_connectionString))
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
     FROM facturacion.DetalleFactura df 
    INNER JOIN Reserva r ON df.IdReserva = r.IdReserva
    WHERE df.IdFactura = f.IdFactura 
        AND r.MetodoPago IS NOT NULL 
  AND r.MetodoPago != ''
   ), 'Todavia no realiza el pago') AS MetodoPago,
0.00 AS Descuento -- Descuento calculado desde promociones, no almacenado
      FROM facturacion.Factura f
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
            FROM facturacion.DetalleFactura df
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

        // ? NUEVO: Marcar factura como pagada usando queries SQL directas
        public DataTable MarcarFacturaPagada(int idFactura, string metodoPago)
        {
        DataTable resultado = new DataTable();
   resultado.Columns.Add("Estado", typeof(string));
        resultado.Columns.Add("Mensaje", typeof(string));
         resultado.Columns.Add("IdFactura", typeof(int));

   using (SqlConnection cn = new SqlConnection(_connectionString))
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
   string checkFacturaSql = "SELECT COUNT(*) FROM facturacion.Factura WHERE IdFactura = @IdFactura";
   SqlCommand checkFacturaCmd = new SqlCommand(checkFacturaSql, cn, transaction);
   checkFacturaCmd.Parameters.AddWithValue("@IdFactura", idFactura);
       int facturaExists = (int)checkFacturaCmd.ExecuteScalar();

              if (facturaExists == 0)
         {
            resultado.Rows.Add("ERROR", "Factura no encontrada", 0);
         return resultado;
         }

   // Verificar que la factura no esté anulada
     string checkAnuladaSql = "SELECT COUNT(*) FROM facturacion.Factura WHERE IdFactura = @IdFactura AND Estado = 'Anulada'";
 SqlCommand checkAnuladaCmd = new SqlCommand(checkAnuladaSql, cn, transaction);
      checkAnuladaCmd.Parameters.AddWithValue("@IdFactura", idFactura);
        int facturaAnulada = (int)checkAnuladaCmd.ExecuteScalar();

       if (facturaAnulada > 0)
    {
  resultado.Rows.Add("ERROR", "No se puede marcar como pagada una factura anulada", 0);
                 return resultado;
            }

    // Actualizar estado de factura
              string updateFacturaSql = "UPDATE facturacion.Factura SET Estado = 'Pagada' WHERE IdFactura = @IdFactura";
          SqlCommand updateFacturaCmd = new SqlCommand(updateFacturaSql, cn, transaction);
          updateFacturaCmd.Parameters.AddWithValue("@IdFactura", idFactura);
 updateFacturaCmd.ExecuteNonQuery();

        // Actualizar método de pago en las reservas relacionadas
       string updateReservasSql = @"
                UPDATE Reserva 
     SET MetodoPago = @MetodoPago
            WHERE IdReserva IN (
     SELECT df.IdReserva 
                   FROM facturacion.DetalleFactura df 
       WHERE df.IdFactura = @IdFactura
      )";

       SqlCommand updateReservasCmd = new SqlCommand(updateReservasSql, cn, transaction);
     updateReservasCmd.Parameters.AddWithValue("@MetodoPago", metodoPago);
       updateReservasCmd.Parameters.AddWithValue("@IdFactura", idFactura);
         updateReservasCmd.ExecuteNonQuery();

      // ? NUEVO: Actualizar estado de reservas a CONFIRMADA cuando se paga
    string updateEstadoReservasSql = @"
         UPDATE Reserva 
           SET Estado = 'CONFIRMADA'
   WHERE IdReserva IN (
            SELECT df.IdReserva 
         FROM facturacion.DetalleFactura df 
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

        // ? NUEVO: Listar facturas de usuario usando queries SQL directas
        public DataTable ListarFacturasUsuario(int idUsuario)
        {
   using (SqlConnection cn = new SqlConnection(_connectionString))
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
        FROM facturacion.DetalleFactura df 
          INNER JOIN Reserva r ON df.IdReserva = r.IdReserva
          WHERE df.IdFactura = f.IdFactura 
      AND r.MetodoPago IS NOT NULL 
          AND r.MetodoPago != ''
      ), 'Todavia no realiza el pago') AS MetodoPago,
       COUNT(df.IdDetalle) AS CantidadItems,
     STRING_AGG(CAST(ISNULL(df.IdReserva, 0) AS VARCHAR), ', ') AS ReservasIds
        FROM facturacion.Factura f
      LEFT JOIN facturacion.DetalleFactura df ON f.IdFactura = df.IdFactura
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

      // ? NUEVO: Generar factura específicamente para reservas confirmadas
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

       using (SqlConnection cn = new SqlConnection(_connectionString))
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

   // ? CORRECCIÓN: El Total de la reserva YA incluye IVA 11.5%
               // Necesitamos separar el subtotal del IVA
             decimal subtotal = totalConIVA / 1.115m; // Extraer subtotal sin IVA
             decimal iva = totalConIVA - subtotal; // IVA que ya estaba incluido (11.5%)
               decimal total = totalConIVA; // El total es el mismo que vino de las reservas

         // Insertar factura con estado "Confirmada"
string insertFacturaSql = @"
           INSERT INTO facturacion.Factura (IdUsuario, FechaHora, Subtotal, IVA, Total, Estado, IdReserva) 
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
  INSERT INTO facturacion.DetalleFactura (IdFactura, IdReserva, Descripcion, Cantidad, PrecioUnitario, Subtotal)
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

        // ? Métodos legacy para compatibilidad con Entity Framework

 // Obtener factura por ID
 public Factura ObtenerFacturaById(int idFactura)
    {
   using (SqlConnection conn = new SqlConnection(_connectionString))
  {
 conn.Open();
       string query = "SELECT * FROM facturacion.Factura WHERE IdFactura = @IdFactura";

       using (SqlCommand cmd = new SqlCommand(query, conn))
        {
      cmd.Parameters.AddWithValue("@IdFactura", idFactura);
       using (SqlDataReader reader = cmd.ExecuteReader())
    {
     if (reader.Read())
         {
 return MapearFactura(reader);
              }
      }
         }
     }
        return null;
      }

     // Listar todas las facturas
        public List<Factura> ListarTodasFacturas()
        {
 List<Factura> facturas = new List<Factura>();
        using (SqlConnection conn = new SqlConnection(_connectionString))
{
        conn.Open();
     string query = "SELECT * FROM facturacion.Factura ORDER BY FechaHora DESC";

       using (SqlCommand cmd = new SqlCommand(query, conn))
    {
       using (SqlDataReader reader = cmd.ExecuteReader())
          {
      while (reader.Read())
      {
       facturas.Add(MapearFactura(reader));
     }
     }
      }
            }
   return facturas;
        }

        // Marcar como pagada (método legacy)
   public bool MarcarComoPageda(int idFactura, string metodoPago)
        {
    using (SqlConnection conn = new SqlConnection(_connectionString))
    {
   conn.Open();
      string query = "UPDATE facturacion.Factura SET Estado = @Estado WHERE IdFactura = @IdFactura";

   using (SqlCommand cmd = new SqlCommand(query, conn))
 {
   cmd.Parameters.AddWithValue("@Estado", "Pagada");
     cmd.Parameters.AddWithValue("@IdFactura", idFactura);
     return cmd.ExecuteNonQuery() > 0;
}
         }
        }

   // Crear factura (método legacy)
        public Factura CrearFactura(Factura factura)
        {
  using (SqlConnection conn = new SqlConnection(_connectionString))
            {
    conn.Open();
      string query = @"
   INSERT INTO facturacion.Factura 
       (IdUsuario, IdReserva, FechaHora, Subtotal, IVA, Total, Estado)
      VALUES (@IdUsuario, @IdReserva, @FechaHora, @Subtotal, @IVA, @Total, @Estado);
         SELECT SCOPE_IDENTITY();";

        using (SqlCommand cmd = new SqlCommand(query, conn))
    {
    cmd.Parameters.AddWithValue("@IdUsuario", factura.IdUsuario);
  cmd.Parameters.AddWithValue("@IdReserva", factura.IdReserva ?? (object)DBNull.Value);
    cmd.Parameters.AddWithValue("@FechaHora", factura.FechaEmision);
  cmd.Parameters.AddWithValue("@Subtotal", factura.Subtotal);
        cmd.Parameters.AddWithValue("@IVA", factura.IVA);
 cmd.Parameters.AddWithValue("@Total", factura.Total);
    cmd.Parameters.AddWithValue("@Estado", factura.Estado);

          factura.IdFactura = (int)(decimal)cmd.ExecuteScalar();
        }
}
   return factura;
        }

        private Factura MapearFactura(SqlDataReader reader)
      {
  return new Factura
  {
   IdFactura = reader["IdFactura"] != DBNull.Value ? (int)reader["IdFactura"] : 0,
IdUsuario = reader["IdUsuario"] != DBNull.Value ? (int)reader["IdUsuario"] : 0,
IdReserva = reader["IdReserva"] != DBNull.Value ? (int)reader["IdReserva"] : (int?)null,
    FechaEmision = reader["FechaHora"] != DBNull.Value ? (DateTime)reader["FechaHora"] : DateTime.Now,
Subtotal = reader["Subtotal"] != DBNull.Value ? (decimal)reader["Subtotal"] : 0m,
 IVA = reader["IVA"] != DBNull.Value ? (decimal)reader["IVA"] : 0m,
   Total = reader["Total"] != DBNull.Value ? (decimal)reader["Total"] : 0m,
    Estado = reader["Estado"] != DBNull.Value ? (string)reader["Estado"] : "Emitida",
 NumeroFactura = null, // Columna no existe en BD
 MetodoPago = null, // Se obtiene desde las reservas relacionadas
         FechaPago = null // Columna no existe en BD
    };
      }
    }
}
