using System;
using System.Data;
using AccesoDatos.DAO;
using GDatos.Entidades;

namespace Logica.Servicios
{
    public class LogTransaccionLogica
    {
        private readonly LogDAO dao = new LogDAO();

        // ✅ Registrar una acción en el log
        public void RegistrarLog(string accion, string descripcion, int idUsuario)
        {
            if (string.IsNullOrEmpty(accion))
                throw new Exception("Debe especificar la acción realizada.");

            if (string.IsNullOrEmpty(descripcion))
                descripcion = "Sin descripción";

            if (idUsuario <= 0)
                throw new Exception("Debe indicar un usuario válido.");

            dao.RegistrarLog(accion, descripcion, idUsuario);
        }

        // ✅ Listar todos los registros de log
        public DataTable ListarLogs()
        {
            return dao.ListarLogs();
        }
    }
}
