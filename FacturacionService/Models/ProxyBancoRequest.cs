namespace FacturacionService.Models
{
 public class ProxyBancoRequest
 {
 public string cuenta_origen { get; set; }
 public string cuenta_destino { get; set; }
 public decimal monto { get; set; }
 }
}