using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ws_GIntegracionBus.DTOS
{
    public class EventoDTO
    {
        public string Accion { get; set; }
        public string Descripcion { get; set; }
        public int IdUsuario { get; set; }
        public string Fecha { get; set; }
    }
}
