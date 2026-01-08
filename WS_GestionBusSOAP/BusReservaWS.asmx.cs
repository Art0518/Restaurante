using System;
using System.Data;
using System.Web.Services;
using Logica.Servicios;
using GDatos.Entidades;

namespace WS_GestionBusSOAP
{
    [WebService(Namespace = "http://cafesanjuansoap.runasp.net/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class WS_Reserva : WebService
    {
        private readonly ReservaLogica reservaLogica = new ReservaLogica();

        // ✅ 1️⃣ Crear pre-reserva (HOLD)
        [WebMethod(Description = "Crear una pre-reserva temporal (equivalente a /api/integracion/restaurantes/hold)")]
        public DataSet CrearPreReserva(DateTime fecha, string hora, int personas, int idUsuario, int idMesa, int duracionHoldSegundos)
        {
            DataSet ds = new DataSet("ResultadoPreReserva");

            try
            {
                if (idUsuario <= 0 || idMesa <= 0)
                    throw new Exception("Debe indicar un usuario y una mesa válidos.");

                // Crear objeto Reserva
                var reserva = new Reserva
                {
                    IdUsuario = idUsuario,
                    IdMesa = idMesa,
                    Fecha = fecha,
                    Hora = hora,
                    NumeroPersonas = personas,
                    Estado = "HOLD"
                };

                // Insertar en base de datos
                reservaLogica.CrearReserva(reserva);

                // Crear tabla de respuesta
                DataTable dt = new DataTable("PreReserva");
                dt.Columns.Add("Mensaje");
                dt.Columns.Add("HoldId");
                dt.Columns.Add("Fecha");
                dt.Columns.Add("Hora");
                dt.Columns.Add("Personas");
                dt.Columns.Add("DuracionSegundos");

                dt.Rows.Add(
                    "Pre-reserva creada correctamente.",
                    Guid.NewGuid().ToString(),
                    fecha.ToString("yyyy-MM-dd"),
                    hora,
                    personas,
                    duracionHoldSegundos
                );

                ds.Tables.Add(dt);
                return ds;
            }
            catch (Exception ex)
            {
                DataTable error = new DataTable("Error");
                error.Columns.Add("Mensaje");
                error.Rows.Add("Error al crear pre-reserva: " + ex.Message);
                ds.Tables.Add(error);
                return ds;
            }
        }

        // ✅ 2️⃣ Confirmar reserva (BOOK)
        [WebMethod(Description = "Confirmar una reserva definitiva (equivalente a /api/integracion/restaurantes/book)")]
        public DataSet CrearReserva(DateTime fecha, string hora, int idUsuario, int idMesa, int personas, string metodoPago)
        {
            DataSet ds = new DataSet("ResultadoReserva");

            try
            {
                if (idUsuario <= 0 || idMesa <= 0)
                    throw new Exception("Debe indicar un usuario y una mesa válidos.");

                if (string.IsNullOrEmpty(metodoPago))
                    metodoPago = "EFECTIVO";

                // Crear objeto reserva confirmada
                var reserva = new Reserva
                {
                    IdUsuario = idUsuario,
                    IdMesa = idMesa,
                    Fecha = fecha,
                    Hora = hora,
                    NumeroPersonas = personas,
                    Estado = "CONFIRMADA"
                };

                // Insertar en base de datos
                reservaLogica.CrearReserva(reserva);

                // Crear tabla de respuesta
                DataTable dt = new DataTable("ReservaConfirmada");
                dt.Columns.Add("Mensaje");
                dt.Columns.Add("Fecha");
                dt.Columns.Add("Hora");
                dt.Columns.Add("MetodoPago");
                dt.Columns.Add("Estado");

                dt.Rows.Add(
                    "Reserva confirmada correctamente.",
                    fecha.ToString("yyyy-MM-dd"),
                    hora,
                    metodoPago,
                    "CONFIRMADA"
                );

                ds.Tables.Add(dt);
                return ds;
            }
            catch (Exception ex)
            {
                DataTable error = new DataTable("Error");
                error.Columns.Add("Mensaje");
                error.Rows.Add("Error al crear reserva: " + ex.Message);
                ds.Tables.Add(error);
                return ds;
            }
        }

        // ✅ 3️⃣ Consultar reserva por ID (GET)
        [WebMethod(Description = "Consultar los detalles de una reserva por su ID (equivalente a /api/integracion/restaurantes/reservas/{idReserva})")]
        public DataSet BuscarDatosReserva(int idReserva)
        {
            DataSet ds = new DataSet("DetalleReserva");

            try
            {
                if (idReserva <= 0)
                    throw new Exception("Debe enviar un ID de reserva válido.");

                // Usar el método correcto que obtiene la unión completa con Mesa y Usuario
                DataSet detalle = reservaLogica.BuscarDatosReserva(idReserva);

                if (detalle == null || detalle.Tables.Count == 0 || detalle.Tables[0].Rows.Count == 0)
                    throw new Exception("No se encontró ninguna reserva con ese ID.");

                // Renombrar la tabla principal para devolverla como resultado SOAP
                detalle.Tables[0].TableName = "Reserva";

                return detalle;
            }
            catch (Exception ex)
            {
                DataTable error = new DataTable("Error");
                error.Columns.Add("Mensaje");
                error.Rows.Add("Error al consultar reserva: " + ex.Message);
                ds.Tables.Add(error);
                return ds;
            }
        }
    }
}
