using System;

namespace GDatos.Entidades
{
    public class Pago
    {
        public int IdPago { get; set; }
        public int IdFactura { get; set; }
        public string MetodoPago { get; set; }
        public decimal Monto { get; set; }
        public string TransaccionCodigo { get; set; }
        public DateTime FechaPago { get; set; }
        public string Estado { get; set; }
    }
}
