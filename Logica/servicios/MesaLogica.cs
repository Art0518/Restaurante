using AccesoDatos.DAO;
using GDatos.Entidades;
using Logica.Validaciones;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Logica.Servicios
{
    public class MesaLogica
    {
        private readonly MesaDAO dao = new MesaDAO();

        // ✅ Listar todas las mesas
        public DataTable ListarMesas()
        {
            return dao.ListarMesas();
        }

        public DataTable MesasDisponibles(string zona, int personas)
        {
            return dao.MesasDisponibles(zona, personas);
        }

        // ✅ Actualizar estado de una mesa
        public void ActualizarEstado(int idMesa, string estado)
        {
            if (idMesa <= 0)
                throw new Exception("Debe indicar un ID de mesa válido.");

            if (string.IsNullOrEmpty(estado))
                throw new Exception("Debe especificar el estado de la mesa.");

            dao.ActualizarEstado(idMesa, estado);
        }

        // ✅ Buscar mesas por filtros (opcional)
        public DataTable BuscarMesas(string tipo = null, int capacidad = 0, string estado = null)
        {
            DataTable mesas = dao.ListarMesas();

            // 🔍 Filtros en memoria (si no existe un SP específico)
            if (!string.IsNullOrEmpty(tipo))
                mesas = mesas.Select($"TipoMesa = '{tipo}'").CopyToDataTable();

            if (capacidad > 0)
                mesas = mesas.Select($"Capacidad >= {capacidad}").CopyToDataTable();

            if (!string.IsNullOrEmpty(estado))
                mesas = mesas.Select($"Estado = '{estado}'").CopyToDataTable();

            return mesas;
        }
        // ✅ Crear o actualizar mesa
        public void GestionarMesa(Mesa m)
        {
            // 🔍 Validaciones antes de guardar
            if (m.IdRestaurante <= 0)
                throw new Exception("Debe seleccionar un restaurante válido.");

            if (m.NumeroMesa <= 0)
                throw new Exception("Debe indicar el número de la mesa.");

            if (string.IsNullOrEmpty(m.TipoMesa))
                throw new Exception("Debe especificar el tipo de mesa.");

            if (m.Capacidad <= 0)
                throw new Exception("La capacidad debe ser mayor a 0.");

            if (m.Precio == null || m.Precio <= 0)
                m.Precio = null;   // permitir mesa sin precio

            if (string.IsNullOrEmpty(m.Estado))
                throw new Exception("Debe especificar el estado de la mesa.");

            // ✅ Determinar la operación: INSERT o UPDATE
            // Si IdMesa es 0 o null, es INSERT
            string operacion = (m.IdMesa == 0) ? "INSERT" : "UPDATE";
            
            // Para INSERT, pasar null en lugar de 0
            int? idMesaParam = (operacion == "INSERT") ? (int?)null : m.IdMesa;

            // ✅ Llamar al DAO (usa el SP sp_gestionar_mesa)
            dao.GestionarMesa(
                operacion,
                idMesaParam,  // Ahora pasa null cuando es INSERT
                m.IdRestaurante,
                m.NumeroMesa,
                m.TipoMesa,
                m.Capacidad,
                m.Precio,
                m.ImagenURL,
                m.Estado
            );
        }

        public DataTable ObtenerDisponibilidad(int idMesa, DateTime fecha)
        {
            string estados = "'PENDIENTE', 'CONFIRMADA', 'COMPLETADA', 'HOLD'";

            string sql = $@"
                SELECT CONVERT(VARCHAR(8), Hora, 108) AS Hora
                FROM reservas.Reserva
                WHERE IdMesa = {idMesa}
                AND Fecha = '{fecha:yyyy-MM-dd}'
                AND Estado IN ({estados})
            ";

            return dao.EjecutarConsulta(sql);
        }




    }
}