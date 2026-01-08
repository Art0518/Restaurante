namespace SeguridadService.Models
{
    public class Usuario
    {
      public int IdUsuario { get; set; }
    public string Nombre { get; set; } = "";
      public string Email { get; set; } = "";
        public string Contrasena { get; set; } = "";
        public string Rol { get; set; } = "Usuario";
        public string Estado { get; set; } = "ACTIVO";
        public string Telefono { get; set; } = "";
        public string Direccion { get; set; } = "";
        public string Cedula { get; set; } = "";
    public DateTime? FechaCreacion { get; set; }
        public DateTime? UltimaConexion { get; set; }
    }
}
