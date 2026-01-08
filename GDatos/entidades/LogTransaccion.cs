using System;

namespace GDatos.Entidades
{
    public class LogTransaccion
    {
        public int IdLog { get; set; }
        public int IdUsuario { get; set; }
        public string Accion { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaHora { get; set; }
    }
}
