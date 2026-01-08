namespace FacturacionService.Models
{
    public class Factura
    {
        public int IdFactura { get; set; }
        public int IdUsuario { get; set; }
public int? IdReserva { get; set; }
  public DateTime FechaEmision { get; set; }
        public decimal Subtotal { get; set; }
 public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } // Emitida, Pagada, Anulada
        public string NumeroFactura { get; set; }
        public string MetodoPago { get; set; }
        public DateTime? FechaPago { get; set; }
        public List<DetalleFactura> Detalles { get; set; } = new List<DetalleFactura>();
  }
}
