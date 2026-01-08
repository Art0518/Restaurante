namespace ReservasService.Models
{
    public class Mesa
    {
        public int IdMesa { get; set; }
        public int IdRestaurante { get; set; }
        public int NumeroMesa { get; set; }
        public string TipoMesa { get; set; } = "";
        public int Capacidad { get; set; }
        public decimal Precio { get; set; }
        public string ImagenURL { get; set; }
        public string Estado { get; set; } = "DISPONIBLE"; // DISPONIBLE, OCUPADA, RESERVADA, INACTIVA
        
        // Propiedades de compatibilidad
        public string Ubicacion { get; set; } = "";
        public bool Disponible 
        {
            get => Estado == "DISPONIBLE";
            set => Estado = value ? "DISPONIBLE" : "OCUPADA";
        }
        
        public DateTime FechaCreacion { get; set; }
    }
}
