namespace ReservasService.Models
{
    public class Reserva
    {
        public int IdReserva { get; set; }
        public int IdUsuario { get; set; }
        public int IdMesa { get; set; }
        public int IdRestaurante { get; set; }
        
        // Propiedades con nombres consistentes con el código existente
        public DateTime Fecha { get; set; }
        public DateTime FechaReserva
        {
            get => Fecha;
            set => Fecha = value;
        }

        public string Hora { get; set; } = "";
        public string HoraReserva
        {
            get => Hora;
            set => Hora = value;
        }

        public int NumeroPersonas { get; set; }
        public string Estado { get; set; } = ""; // Confirmada, Cancelada, Completada, HOLD
        
        public string Observaciones { get; set; } = "";
        public string Notas
        {
            get => Observaciones;
            set => Observaciones = value;
        }
        
        public decimal Total { get; set; }
        public string MetodoPago { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
