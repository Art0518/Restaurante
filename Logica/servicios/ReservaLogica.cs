using System;
using System.Data;
using AccesoDatos.DAO;
using GDatos.Entidades;
using Logica.Validaciones;

namespace Logica.Servicios
{
    public class ReservaLogica
    {
        private readonly ReservaDAO dao = new ReservaDAO();

        // ============================================================
        // ✅ CREAR RESERVA (HOLD por defecto + MétodoPago)
        // ============================================================
        public DataTable CrearReserva(Reserva r)
        {
            // ✔ Validaciones básicas
            if (r.IdUsuario <= 0)
                throw new Exception("Debe indicar un usuario válido.");

            if (r.IdMesa <= 0)
                throw new Exception("Debe seleccionar una mesa válida.");

            if (!ValidacionReserva.FechaValida(r.Fecha))
                throw new Exception("La fecha de la reserva no es válida.");

            if (string.IsNullOrEmpty(r.Hora))
                throw new Exception("Debe especificar la hora de la reserva.");

            if (r.NumeroPersonas <= 0)
                throw new Exception("Debe indicar la cantidad de personas.");

            // ⭐ Si no viene estado → HOLD
            if (string.IsNullOrEmpty(r.Estado))
                r.Estado = "HOLD";

            // ⭐ Si no viene método de pago → vacío (solo se pedirá al confirmar)
            if (string.IsNullOrEmpty(r.MetodoPago))
                r.MetodoPago = "";

            // 📌 Llamada a DAO con Método de Pago
            return dao.CrearReserva(
                r.IdUsuario,
                r.IdMesa,
                r.Fecha,
                r.Hora,
                r.NumeroPersonas,
                r.Observaciones,
                r.Estado,
                r.MetodoPago     // ⬅ NUEVO
            );
        }

        // ============================================================
        // LISTAR TODAS
        // ============================================================
        public DataTable ListarReservas()
        {
            return dao.ListarReservas();
        }


        // ============================================================
        // CAMBIAR ESTADO + METODO DE PAGO (CONFIRMAR RESERVA)
        // ============================================================
        public void ActualizarEstado(int idReserva, string estado, string metodoPago)
        {
            if (idReserva <= 0)
                throw new Exception("El ID de la reserva no es válido.");

            if (string.IsNullOrEmpty(estado))
                throw new Exception("Debe especificar un estado.");

            if (string.IsNullOrEmpty(metodoPago))
                throw new Exception("Debe especificar un método de pago.");

            dao.ActualizarEstado(idReserva, estado, metodoPago);
        }



        // ============================================================
        // BUSCAR POR ID
        // ============================================================
        public DataTable BuscarReservaPorId(int idReserva)
        {
            try
            {
                return dao.BuscarReservaPorId(idReserva);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar reserva por ID: " + ex.Message);
            }
        }


        // ============================================================
        // BUSCAR DATOS COMPLETOS (JOIN)
        // ============================================================
        public DataSet BuscarDatosReserva(int idReserva)
        {
            try
            {
                if (idReserva <= 0)
                    throw new Exception("ID inválido.");

                DataSet ds = dao.BuscarDatosReserva(idReserva);

                if (!ds.Tables[0].Columns.Contains("TipoMesa"))
                    throw new Exception("La consulta no devuelve TipoMesa.");

                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar: " + ex.Message);
            }
        }


        // ============================================================
        // EDITAR RESERVA COMPLETA (fecha, hora, personas…)
        // ============================================================
        public string EditarReserva(Reserva r)
        {
            if (r.IdReserva <= 0)
                throw new Exception("ID inválido.");

            if (!ValidacionReserva.FechaValida(r.Fecha))
                throw new Exception("Fecha inválida.");

            if (string.IsNullOrEmpty(r.Hora))
                throw new Exception("Debe especificar una hora.");

            if (r.NumeroPersonas <= 0)
                throw new Exception("Número de personas inválido.");

            return dao.EditarReserva(r);
        }

        // ============================================================
        // 🛒 LISTAR CARRITO DE RESERVAS DEL USUARIO CON PROMOCIONES
        // ============================================================
        public DataSet ListarCarritoReservas(int idUsuario, int? promocionId = null)
        {
            if (idUsuario <= 0)
                throw new Exception("ID de usuario no válido.");

            return dao.ListarCarritoReservas(idUsuario, promocionId);
        }

        // ============================================================
        // ❌ ELIMINAR RESERVA DEL CARRITO
        // ============================================================
        public DataTable EliminarReservaCarrito(int idUsuario, int idReserva)
        {
            if (idUsuario <= 0)
                throw new Exception("ID de usuario no válido.");

            if (idReserva <= 0)
                throw new Exception("ID de reserva no válido.");

            return dao.EliminarReservaCarrito(idUsuario, idReserva);
        }

        // ============================================================
        // ✅ CONFIRMAR RESERVAS SELECTIVAS CON PROMOCIONES
        // ============================================================
        public DataTable ConfirmarReservasSelectivas(int idUsuario, string reservasIds, string metodoPago, int? promocionId = null)
        {
            if (idUsuario <= 0)
                throw new Exception("ID de usuario no válido.");

            if (string.IsNullOrEmpty(reservasIds))
                throw new Exception("Debe especificar las reservas a confirmar.");

            if (string.IsNullOrEmpty(metodoPago))
                throw new Exception("Método de pago es requerido.");

            return dao.ConfirmarReservasSelectivas(idUsuario, reservasIds, metodoPago, promocionId);
        }
        
        // ============================================================
        // ✅ NUEVO: LISTAR RESERVAS CONFIRMADAS DE UN USUARIO
        // ============================================================
        public DataTable ListarReservasConfirmadas(int idUsuario)
        {
            if (idUsuario <= 0)
                throw new Exception("ID de usuario no válido.");

            return dao.ListarReservasConfirmadas(idUsuario);
        }
        
        // ============================================================
        // ✅ NUEVO: LISTAR TODAS LAS RESERVAS PARA ADMINISTRADOR
        // ============================================================
        public DataTable ListarTodasReservasAdmin()
        {
         return dao.ListarTodasReservasAdmin();
        }
        
        // ============================================================
        // ✅ NUEVO: GENERAR FACTURA DESDE ADMINISTRADOR
        // ============================================================
        public DataTable GenerarFacturaDesdeAdmin(int idReserva, string metodoPago, string tipoFactura = "ADMIN")
 {
      if (idReserva <= 0)
     throw new Exception("ID de reserva no válido.");

        if (string.IsNullOrWhiteSpace(metodoPago))
       throw new Exception("Método de pago es requerido.");

   return dao.GenerarFacturaDesdeAdmin(idReserva, metodoPago, tipoFactura);
   }
    }
}
