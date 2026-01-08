using System;

namespace GDatos.Entidades
{
    public class Reserva
    {
        public int IdReserva { get; set; }
        public int IdUsuario { get; set; }
        public int IdMesa { get; set; }
        public DateTime Fecha { get; set; }
        public string TipoMesa { get; set; }

        public string Hora { get; set; }
        public int NumeroPersonas { get; set; }

        // ⭐ Campo adicional que SÍ necesitas
        public string Observaciones { get; set; }

        // ⭐ Estado de la reserva (PENDIENTE / CONFIRMADA / CANCELADA)
        public string Estado { get; set; }

        public string MetodoPago { get; set; }

    }
}
