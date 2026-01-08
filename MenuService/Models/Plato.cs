namespace MenuService.Models
{
    public class Plato
    {
        public int IdPlato { get; set; }
        public int IdRestaurante { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string Categoria { get; set; }
        public string TipoComida { get; set; }
        public string ImagenURL { get; set; }
        public int Stock { get; set; }
        public string Estado { get; set; }  // ACTIVO, INACTIVO, etc.
    }
}
