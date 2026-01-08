using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ws_GIntegracionBus.DTOS
{
    public class FacturaResponse
    {
        public string Mensaje { get; set; }
        public int IdUsuario { get; set; }
        public int IdReserva { get; set; }
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public string Fecha { get; set; }
        public string Estado { get; set; }
    }
}
