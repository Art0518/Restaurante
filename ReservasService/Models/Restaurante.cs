namespace ReservasService.Models
{
  public class Restaurante
    {
   public int IdRestaurante { get; set; }
  public string Nombre { get; set; }
    public string Direccion { get; set; }
      public string Telefono { get; set; }
    public string HorarioApertura { get; set; }
        public string HorarioCierre { get; set; }
 public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
