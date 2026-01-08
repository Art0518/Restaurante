using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ws_GIntegracionBus.DTOS
{
    public class ReservaResponse
    {
        public string Mensaje { get; set; }
        public int IdReserva { get; set; }
        public int IdMesa { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public string Estado { get; set; }
    }
}
