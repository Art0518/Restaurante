namespace GDatos.Entidades
{
    public class Mesa
    {
        public int IdMesa { get; set; }
        public int IdRestaurante { get; set; }
        public int NumeroMesa { get; set; }
        public string TipoMesa { get; set; }
        public int Capacidad { get; set; }
        public decimal? Precio { get; set; }
        public string ImagenURL { get; set; }
        public string Estado { get; set; }
    }
}
