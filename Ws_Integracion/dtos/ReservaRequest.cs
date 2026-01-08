using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ws_GIntegracionBus.DTOS
{
    public class ReservaRequest
    {
        public int IdUsuario { get; set; }
        public int IdMesa { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public int Personas { get; set; }
        public string Estado { get; set; }
    }
}
