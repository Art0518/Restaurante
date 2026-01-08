using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Ws_GIntegracionBus.DTOS
{
    public class ReservaSwaggerResponse
    {
        public ReservaSwaggerDetalle reserva { get; set; }
        public object _links { get; set; }
    }

    public class ReservaSwaggerDetalle
    {
        public int IdReserva { get; set; }
        public int IdUsuario { get; set; }
        public int IdMesa { get; set; }
        public string Fecha { get; set; }
        public string TipoMesa { get; set; }
        public string Hora { get; set; }
        public int NumeroPersonas { get; set; }
        public string Estado { get; set; }
    }
}

