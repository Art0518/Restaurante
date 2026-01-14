namespace FacturacionService.Models
{
 /// <summary>
    /// DTO para generar factura desde el carrito
    /// </summary>
 public class GenerarFacturaCarritoDto
    {
        public int IdUsuario { get; set; }
        public string ReservasIds { get; set; } = string.Empty;
        public int? PromocionId { get; set; }
        public string? MetodoPago { get; set; }
    }

    /// <summary>
    /// DTO para generar factura de reservas confirmadas
    /// </summary>
    public class GenerarFacturaConfirmadasDto
    {
public int IdUsuario { get; set; }
  public string ReservasIds { get; set; } = string.Empty;
      public string? TipoFactura { get; set; }
    }

    /// <summary>
    /// DTO para marcar factura como pagada
  /// </summary>
 public class MarcarFacturaPagadaDto
    {
        public int IdFactura { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
    }
}
