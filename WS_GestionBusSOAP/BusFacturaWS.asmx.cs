using System;
using System.Data;
using System.Web.Services;
using Logica.Servicios;

namespace WS_GestionBusSOAP
{
    [WebService(Namespace = "http://cafesanjuansoap.runasp.net/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class WS_Factura : WebService
    {
        private readonly FacturaLogica facturaLogica = new FacturaLogica();

        [WebMethod(Description = "Emitir una factura (equivalente a /api/integracion/restaurantes/invoices)")]
        public DataSet EmitirFactura(int idUsuario, int idReserva, decimal subtotal, decimal iva, decimal total)
        {
            DataSet ds = new DataSet("ResultadoFactura");

            try
            {
                facturaLogica.GenerarFactura(idUsuario, idReserva);

                DataTable dt = new DataTable("FacturaEmitida");
                dt.Columns.Add("Mensaje");
                dt.Columns.Add("Usuario");
                dt.Columns.Add("Reserva");
                dt.Columns.Add("Subtotal");
                dt.Columns.Add("IVA");
                dt.Columns.Add("Total");
                dt.Columns.Add("Fecha");
                dt.Columns.Add("Estado");

                dt.Rows.Add(
                    "Factura emitida correctamente.",
                    idUsuario,
                    idReserva,
                    subtotal,
                    iva,
                    total,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    "ACT"
                );

                ds.Tables.Add(dt);
                return ds;
            }
            catch (Exception ex)
            {
                DataTable error = new DataTable("Error");
                error.Columns.Add("Mensaje");
                error.Rows.Add("Error al emitir factura: " + ex.Message);
                ds.Tables.Add(error);
                return ds;
            }
        }
    }
}
