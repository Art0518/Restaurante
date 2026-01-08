using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ws_GIntegracionBus.DTOS
{
    public class HoldResponse
    {
        public string Mensaje { get; set; }
        public string HoldId { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public int Personas { get; set; }
        public int DuracionSegundos { get; set; }
    }
}
