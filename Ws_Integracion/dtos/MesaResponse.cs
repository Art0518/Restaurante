using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ws_GIntegracionBus.DTOS
{
    public class MesaResponse
    {
        public int IdMesa { get; set; }
        public int IdRestaurante { get; set; }
        public int NumeroMesa { get; set; }
        public string TipoMesa { get; set; }
        public int Capacidad { get; set; }
        public string Estado { get; set; }
        public decimal Precio { get; set; }
    }
}
