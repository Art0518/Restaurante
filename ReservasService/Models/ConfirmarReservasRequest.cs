namespace ReservasService.Models
{
    public class ConfirmarReservasRequest
    {
        public int IdUsuario { get; set; }
  public string ReservasIds { get; set; } = string.Empty;
        public string MetodoPago { get; set; } = string.Empty;
    public decimal MontoDescuento { get; set; }
   public decimal Total { get; set; }
    }
}
