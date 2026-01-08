using System;
using System.Data;
using System.Web.Services;
using Logica.Servicios;

namespace WS_GestionBusSOAP
{
    [WebService(Namespace = "http://cafesanjuansoap.runasp.net/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class WS_Busqueda : WebService
    {
        private readonly MesaLogica mesaLogica = new MesaLogica();

        [WebMethod(Description = "Buscar mesas registradas (equivalente al endpoint REST /api/integracion/restaurantes/search)")]
        public DataSet BuscarMesas()
        {
            try
            {
                DataTable resultado = mesaLogica.ListarMesas();
                DataSet ds = new DataSet("ResultadoMesas");

                if (resultado == null || resultado.Rows.Count == 0)
                {
                    DataTable vacio = new DataTable("Mesas");
                    vacio.Columns.Add("Mensaje");
                    vacio.Columns.Add("Total");
                    vacio.Rows.Add("No se encontraron mesas registradas.", 0);
                    ds.Tables.Add(vacio);
                }
                else
                {
                    resultado.TableName = "Mesas";
                    ds.Tables.Add(resultado);
                }

                return ds;
            }
            catch (Exception ex)
            {
                DataTable error = new DataTable("Error");
                error.Columns.Add("Mensaje");
                error.Rows.Add("Error al buscar mesas: " + ex.Message);
                DataSet dsError = new DataSet("ErrorSOAP");
                dsError.Tables.Add(error);
                return dsError;
            }
        }
    }
}
