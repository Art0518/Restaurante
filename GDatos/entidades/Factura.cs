using System;

namespace GDatos.Entidades
{
    public class Factura
    {
        public int IdFactura { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaHora { get; set; }
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
    }
}
