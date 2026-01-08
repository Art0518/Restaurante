using System;
using System.Data;
using AccesoDatos.DAO;
using GDatos.Entidades;

namespace Logica.Servicios
{
    public class FacturaLogica
    {
        private readonly FacturaDAO dao = new FacturaDAO();
        private readonly DetalleFacturaDAO daoDetalle = new DetalleFacturaDAO();

        // ✅ Generar una nueva factura
        public DataTable GenerarFactura(int idUsuario, int idReserva)
        {
         try
      {
         if (idUsuario <= 0)
              throw new ArgumentException("ID de usuario no válido");
       
         if (idReserva <= 0)
             throw new ArgumentException("ID de reserva no válido");

       return dao.GenerarFactura(idUsuario, idReserva);
         }
  catch (Exception ex)
 {
         throw new Exception("Error en lógica al generar factura: " + ex.Message);
    }
        }

        // ✅ Listar facturas existentes
        public DataTable ListarFacturas()
        {
            try
    {
    return dao.ListarFacturas();
 }
            catch (Exception ex)
      {
      throw new Exception("Error en lógica al listar facturas: " + ex.Message);
            }
        }

        // ✅ Obtener el detalle de una factura
  public DataTable DetalleFactura(int idFactura)
        {
            try
       {
 if (idFactura <= 0)
       throw new ArgumentException("ID de factura no válido");

 return dao.DetalleFactura(idFactura);
          }
   catch (Exception ex)
            {
        throw new Exception("Error en lógica al obtener detalle de factura: " + ex.Message);
       }
        }

        // ✅ Calcular totales (subtotal, IVA y total)
        public Factura CalcularTotales(Factura f, decimal porcentajeIVA = 0.07m) // CAMBIADO DE 0.12m
  {
            try
     {
    if (f == null)
          throw new ArgumentNullException("Factura no puede ser nula");

    if (f.Subtotal < 0)
   throw new ArgumentException("El subtotal no puede ser negativo");

 f.IVA = f.Subtotal * porcentajeIVA;
         f.Total = f.Subtotal + f.IVA;

      return f;
 }
            catch (Exception ex)
    {
   throw new Exception("Error en lógica al calcular totales: " + ex.Message);
   }
        }

     // ✅ Anular una factura
        public void AnularFactura(int idFactura)
        {
try
  {
        if (idFactura <= 0)
       throw new ArgumentException("ID de factura no válido");

         dao.AnularFactura(idFactura);
            }
        catch (Exception ex)
     {
                throw new Exception("Error en lógica al anular factura: " + ex.Message);
  }
        }

        // ✅ NUEVO: Generar factura desde carrito de reservas
        public DataTable GenerarFacturaCarrito(int idUsuario, string reservasIds, int? promocionId, string metodoPago)
        {
   try
        {
      // Validaciones de negocio
           if (idUsuario <= 0)
{
          DataTable errorResult = new DataTable();
     errorResult.Columns.Add("Estado", typeof(string));
     errorResult.Columns.Add("Mensaje", typeof(string));
        errorResult.Rows.Add("ERROR", "ID de usuario no válido");
         return errorResult;
 }

 if (string.IsNullOrWhiteSpace(reservasIds))
  {
               DataTable errorResult = new DataTable();
    errorResult.Columns.Add("Estado", typeof(string));
                    errorResult.Columns.Add("Mensaje", typeof(string));
 errorResult.Rows.Add("ERROR", "Debe seleccionar al menos una reserva");
       return errorResult;
  }

                // Validar que las reservas no estén vacías después del split
           string[] reservasArray = reservasIds.Split(',');
        if (reservasArray.Length == 0)
  {
          DataTable errorResult = new DataTable();
      errorResult.Columns.Add("Estado", typeof(string));
              errorResult.Columns.Add("Mensaje", typeof(string));
     errorResult.Rows.Add("ERROR", "No hay reservas válidas seleccionadas");
       return errorResult;
      }

                // Validar promoción si se especifica
                if (promocionId.HasValue && promocionId.Value <= 0)
       {
          DataTable errorResult = new DataTable();
                    errorResult.Columns.Add("Estado", typeof(string));
       errorResult.Columns.Add("Mensaje", typeof(string));
          errorResult.Rows.Add("ERROR", "ID de promoción no válido");
   return errorResult;
    }

     return dao.GenerarFacturaCarrito(idUsuario, reservasIds, promocionId, metodoPago);
     }
      catch (Exception ex)
  {
        DataTable errorResult = new DataTable();
             errorResult.Columns.Add("Estado", typeof(string));
           errorResult.Columns.Add("Mensaje", typeof(string));
           errorResult.Rows.Add("ERROR", "Error en lógica al generar factura desde carrito: " + ex.Message);
     return errorResult;
         }
        }

        // ✅ NUEVO: Obtener factura detallada con información completa
        public DataSet ObtenerFacturaDetallada(int idFactura)
        {
        try
  {
            if (idFactura <= 0)
       throw new ArgumentException("ID de factura no válido");

  return dao.ObtenerFacturaDetallada(idFactura);
          }
 catch (Exception ex)
            {
   throw new Exception("Error en lógica al obtener factura detallada: " + ex.Message);
     }
        }

      // ✅ NUEVO: Marcar factura como pagada
        public DataTable MarcarFacturaPagada(int idFactura, string metodoPago)
        {
            try
            {
       // Validaciones de negocio
   if (idFactura <= 0)
         {
         DataTable errorResult = new DataTable();
               errorResult.Columns.Add("Estado", typeof(string));
      errorResult.Columns.Add("Mensaje", typeof(string));
       errorResult.Rows.Add("ERROR", "ID de factura no válido");
   return errorResult;
         }

      if (string.IsNullOrWhiteSpace(metodoPago))
   {
           DataTable errorResult = new DataTable();
           errorResult.Columns.Add("Estado", typeof(string));
   errorResult.Columns.Add("Mensaje", typeof(string));
     errorResult.Rows.Add("ERROR", "Debe especificar un método de pago");
      return errorResult;
         }

          // Validar que el método de pago sea válido
        string[] metodosValidos = { "Efectivo", "Tarjeta de Crédito", "Tarjeta de Débito", "Transferencia", "Sinpe Móvil" };
                bool metodoPagoValido = false;
     foreach (string metodo in metodosValidos)
              {
            if (string.Equals(metodoPago, metodo, StringComparison.OrdinalIgnoreCase))
          {
 metodoPagoValido = true;
       break;
      }
   }

  if (!metodoPagoValido)
   {
      DataTable errorResult = new DataTable();
   errorResult.Columns.Add("Estado", typeof(string));
     errorResult.Columns.Add("Mensaje", typeof(string));
   errorResult.Rows.Add("ERROR", "Método de pago no válido");
                    return errorResult;
        }

        return dao.MarcarFacturaPagada(idFactura, metodoPago);
   }
        catch (Exception ex)
          {
     DataTable errorResult = new DataTable();
        errorResult.Columns.Add("Estado", typeof(string));
                errorResult.Columns.Add("Mensaje", typeof(string));
      errorResult.Rows.Add("ERROR", "Error en lógica al marcar factura como pagada: " + ex.Message);
          return errorResult;
            }
        }

        // ✅ NUEVO: Listar facturas de un usuario específico
    public DataTable ListarFacturasUsuario(int idUsuario)
        {
       try
            {
                if (idUsuario <= 0)
               throw new ArgumentException("ID de usuario no válido");

     return dao.ListarFacturasUsuario(idUsuario);
     }
        catch (Exception ex)
   {
                throw new Exception("Error en lógica al listar facturas del usuario: " + ex.Message);
   }
        }

        // ✅ MÉTODO AUXILIAR: Validar estado de factura
        public bool ValidarEstadoFactura(string estado)
        {
     try
            {
  string[] estadosValidos = { "Emitida", "Pagada", "Anulada" };
         foreach (string estadoValido in estadosValidos)
       {
   if (string.Equals(estado, estadoValido, StringComparison.OrdinalIgnoreCase))
           {
                 return true;
         }
             }
                return false;
   }
 catch
   {
      return false;
            }
        }

  // ✅ MÉTODO AUXILIAR: Calcular descuento por promoción
        public decimal CalcularDescuentoPromocion(decimal subtotal, decimal porcentajeDescuento)
   {
      try
     {
      if (subtotal < 0)
throw new ArgumentException("El subtotal no puede ser negativo");

     if (porcentajeDescuento < 0 || porcentajeDescuento > 100)
    throw new ArgumentException("El porcentaje de descuento debe estar entre 0 y 100");

   return subtotal * (porcentajeDescuento / 100);
  }
  catch (Exception ex)
            {
throw new Exception("Error calculando descuento: " + ex.Message);
    }
      }

    // ✅ NUEVO: Generar factura para reservas confirmadas
     public DataTable GenerarFacturaReservasConfirmadas(int idUsuario, string reservasIds, string tipoFactura = "CONFIRMADA")
  {
      try
      {
        // Validaciones de negocio
       if (idUsuario <= 0)
   {
   DataTable errorResult = new DataTable();
       errorResult.Columns.Add("Estado", typeof(string));
     errorResult.Columns.Add("Mensaje", typeof(string));
  errorResult.Rows.Add("ERROR", "ID de usuario no válido");
    return errorResult;
       }

        if (string.IsNullOrWhiteSpace(reservasIds))
      {
  DataTable errorResult = new DataTable();
      errorResult.Columns.Add("Estado", typeof(string));
   errorResult.Columns.Add("Mensaje", typeof(string));
    errorResult.Rows.Add("ERROR", "Debe seleccionar al menos una reserva confirmada");
    return errorResult;
 }

        // Validar que las reservas no estén vacías después del split
        string[] reservasArray = reservasIds.Split(',');
  if (reservasArray.Length == 0)
  {
         DataTable errorResult = new DataTable();
    errorResult.Columns.Add("Estado", typeof(string));
        errorResult.Columns.Add("Mensaje", typeof(string));
      errorResult.Rows.Add("ERROR", "No hay reservas confirmadas válidas seleccionadas");
        return errorResult;
  }

return dao.GenerarFacturaReservasConfirmadas(idUsuario, reservasIds, tipoFactura);
    }
        catch (Exception ex)
         {
    DataTable errorResult = new DataTable();
     errorResult.Columns.Add("Estado", typeof(string));
 errorResult.Columns.Add("Mensaje", typeof(string));
      errorResult.Rows.Add("ERROR", "Error en lógica al generar factura de confirmadas: " + ex.Message);
            return errorResult;
      }
        }
    }
}
