namespace MenuService.Models
{
    public class Promocion
    {
        public int IdPromocion { get; set; }
        public int IdRestaurante { get; set; }
        public string Nombre { get; set; }
        public decimal Descuento { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; } // Activa, Inactiva, etc.
    }
}
