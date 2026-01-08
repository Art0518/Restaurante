using System;
using System.Data;
using System.Web.Services;
using Logica.Servicios;

namespace WS_GestionBusSOAP
{
    [WebService(Namespace = "http://cafesanjuansoap.runasp.net/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class WS_Disponibilidad : WebService
    {
        private readonly MesaLogica mesaLogica = new MesaLogica();

        [WebMethod(Description = "Validar disponibilidad de mesas (equivalente a /api/integracion/restaurantes/availability)")]
        public DataSet ValidarDisponibilidad(DateTime fecha, string hora, int numeroPersonas, string ciudad)
        {
            DataSet ds = new DataSet("ResultadoDisponibilidad");

            try
            {
                DataTable mesas = mesaLogica.ListarMesas();
                DataTable resultado = new DataTable("Disponibilidad");
                resultado.Columns.Add("Mensaje");
                resultado.Columns.Add("Fecha");
                resultado.Columns.Add("Hora");
                resultado.Columns.Add("NumeroPersonas");
                resultado.Columns.Add("Ciudad");
                resultado.Columns.Add("Disponible");

                bool disponible = false;

                foreach (DataRow row in mesas.Rows)
                {
                    int capacidad = Convert.ToInt32(row["Capacidad"]);
                    string estado = row["Estado"].ToString().ToUpper();

                    string ubicacion = row.Table.Columns.Contains("Ciudad")
                        ? row["Ciudad"].ToString()
                        : "N/A";

                    if (capacidad >= numeroPersonas &&
                        estado == "DISPONIBLE" &&
                        (string.IsNullOrEmpty(ciudad) || ubicacion.Equals(ciudad, StringComparison.OrdinalIgnoreCase)))
                    {
                        disponible = true;
                        break;
                    }
                }

                resultado.Rows.Add(
                    "Validación completada correctamente.",
                    fecha.ToString("yyyy-MM-dd"),
                    hora,
                    numeroPersonas,
                    ciudad,
                    disponible ? "true" : "false"
                );

                ds.Tables.Add(resultado);
                return ds;
            }
            catch (Exception ex)
            {
                DataTable error = new DataTable("Error");
                error.Columns.Add("Mensaje");
                error.Rows.Add("Error al validar disponibilidad: " + ex.Message);
                ds.Tables.Add(error);
                return ds;
            }
        }
    }
}
